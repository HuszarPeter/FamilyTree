using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Resources;
using FamilyTree.Dal.Model;
using FamilyTree.Utils;
using MySql.Data.MySqlClient;

namespace FamilyTree.Dal
{
    public class DataContext : IDisposable
    {
        private MySqlConnection _connection = null;

        public DataContext()
            : this(ConfigurationManager.AppSettings["connection_string"])
        {
        }

        public DataContext(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        #region Helpers
        private List<T> ExecuteQuery<T>(string query) where T : new()
        {
            var result = new List<T>();
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = query;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(CreateObjectFromReader<T>(reader));
                }
            }

            return result;
        }

        private int ExecuteNonQuery(string query, List<MySqlParameter> parameters)
        {
            if(_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            parameters.ForEach(p => cmd.Parameters.Add(p));
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            return (int) cmd.LastInsertedId;
        }

        private int ExecuteNonQuery(string query)
        {
            if(_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        private static T CreateObjectFromReader<T>(MySqlDataReader reader) where T : new()
        {
            var result = new T();
            var tt = typeof(T);

            tt.GetProperties()
                .Select(pi => new
                {
                    Property = pi,
                    DatabaseField = pi.GetCustomAttribute<DatabaseFieldAttribute>()
                })
                .Where(x => x.DatabaseField != null)
                .ToList()
                .ForEach(x => SetValue(result, x.Property, x.DatabaseField.Name, reader));

            return result;
        }

        private static void SetValue(Object obj, PropertyInfo pi, string dbFieldName, IDataRecord reader)
        {
            var ordinal = reader.GetOrdinal(dbFieldName);
            var dbValue = reader.GetValue(ordinal);
            if (dbValue is DBNull)
                pi.SetValue(obj, null);
            else
                pi.SetValue(obj, reader.GetValue(ordinal));
        }

        private void Insert<T>(T entity)
        {
            var dbTable = typeof (T)
                .GetCustomAttribute<DatabaseTableAttribute>();

            if (dbTable == null)
            {
                throw new Exception(string.Format("DatabaseTableAttribute not found on {0}", typeof(T)));
            }

            var idProperty = typeof (T)
                .GetDatabaseFileds()
                .FirstOrDefault(p => p.DatabaseFieldAttribute.IsPrimaryKey);

            var nameValues = typeof (T)
                .GetDatabaseFileds()
                .Where(p => !p.DatabaseFieldAttribute.IsPrimaryKey)
                .Select(p => p.GetMysqlQueryParameter(entity));

            var insertQuery =  string.Format("INSERT INTO {2} ({0}) VALUES ({1})"
                , string.Join(",", nameValues.Select(p => p.SourceColumn))
                , string.Join(",", nameValues.Select(p => p.ParameterName)),
                dbTable.Name);

            var id = ExecuteNonQuery(insertQuery, nameValues.ToList());
            if (idProperty != null)
            {
                idProperty.PropertyInfo.SetValue(entity, id);
            }
        }

        private void Update<T>(T entity)
        {
            var dbTable = typeof(T)
                .GetCustomAttribute<DatabaseTableAttribute>();

            if (dbTable == null)
            {
                throw new Exception(string.Format("DatabaseTableAttribute not found on {0}", typeof(T)));
            }

            var dbFields = typeof(T)
                .GetDatabaseFileds();

            var idProp = dbFields.FirstOrDefault(p => p.DatabaseFieldAttribute.IsPrimaryKey);
            if (idProp == null)
            {
                throw new Exception(string.Format("No Primary Key found! {0}", typeof(T)));
            }

            var mqslFields = dbFields
                .Where(p => !p.DatabaseFieldAttribute.IsPrimaryKey)
                .Select(p => p.GetMysqlQueryParameter(entity))
                .ToList();

            var updateQuery = string.Format("UPDATE {3} SET {0} WHERE {1}={2}", 
                string.Join(",", mqslFields.Select(f => string.Format("{0}={1}", f.SourceColumn, f.ParameterName))), 
                idProp.DatabaseFieldAttribute.Name, 
                idProp.PropertyInfo.GetMySqlCompatiblePropertyValue(entity),
                dbTable.Name);

            ExecuteNonQuery(updateQuery, mqslFields);
        }

        #endregion

        #region Person
        public Person GetPerson(int id)
        {
            return ExecuteQuery<Person>(string.Format("SELECT * FROM szemely WHERE szemely_id = '{0}'", id))
                .FirstOrDefault();
        }

        public List<Person> GetAllPersons()
        {
            return ExecuteQuery<Person>("SELECT * FROM szemely");
        }

        public void AddPerson(Person person)
        {
            Insert(person);
        }

        public void UpdatePerson(Person person)
        {
            Update(person);
        }

        public void DeletePerson(Person person)
        {
            ExecuteNonQuery(string.Format("DELETE FROM kapcsolat WHERE szemely_id1 = {0} OR szemely_id2 = {0}",
                person.Id));
            ExecuteNonQuery(string.Format("DELETE FROM szemely WHERE szemely_id={0}", person.Id));
        }

        #endregion

        #region Relation
        public List<Relation> GetAllRelations()
        {
            return ExecuteQuery<Relation>("SELECT * FROM kapcsolat");
        }

        public void AddRelation(Relation relation)
        {
            Insert(relation);
        }

        public void UpdateRelation(Relation relation)
        {
            Update(relation);
        }

        public void DeleteRelation(Relation relation)
        {
            ExecuteNonQuery(string.Format("DELETE FROM kapcsolat WHERE kapcsolat_id = {0}", relation.RelationId));
        }
        #endregion

        public List<GenderStatistic> GetGenderStatistics()
        {
            var query = new StringBuilder();
            query.AppendLine("select true as ferfi, count(*) as cnt, (select count(*) from szemely) as osszes FROM szemely");
            query.AppendLine("where szemely.ferfi = 1");
            query.AppendLine("union");
            query.AppendLine("select false,count(*) as cnt, (select count(*) from szemely) as osszes FROM szemely");
            query.AppendLine("where szemely.ferfi = 0");

            var result = ExecuteQuery<GenderStatistic>(query.ToString());
            return result;
        }

        public List<AgeStatistic> GetAgeStatistics()
        {
            var query = new StringBuilder();
            query.AppendLine("SELECT * FROM (");
            query.AppendLine("select 2 as k, count(szemely.szemely_id) as num from szemely");
            query.AppendLine("where year(curdate()) - year(szemely.szuletes_ideje) > 18");
            query.AppendLine("union");
            query.AppendLine("select 0 as k, count(szemely.szemely_id) as num from szemely");
            query.AppendLine("where year(curdate()) - year(szemely.szuletes_ideje) < 12");
            query.AppendLine("union");
            query.AppendLine("select 1 as k, count(szemely.szemely_id) as num from szemely");
            query.AppendLine("where year(curdate()) - year(szemely.szuletes_ideje) between 12 and 18");
            query.AppendLine("union");
            query.AppendLine("select 3 as k, count(szemely.szemely_id) as num from szemely");
            query.AppendLine("where szemely.szuletes_ideje is null");
            query.AppendLine("union");
            query.AppendLine("select 100 as k, count(*) as num from szemely");
            query.AppendLine(") as t");
            query.AppendLine("ORDER BY k");

            return ExecuteQuery<AgeStatistic>(query.ToString());
        }


        public void Dispose()
        {
            if (_connection == null || _connection.State != ConnectionState.Open) return;

            _connection.Close();
            _connection = null;
        }
    }
}
