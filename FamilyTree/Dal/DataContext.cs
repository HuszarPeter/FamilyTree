using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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

        private List<T> ExecuteQuery<T>(string query, List<MySqlParameter> parameters = null) where T : new()
        {
            var result = new List<T>();
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = query;
            if (parameters != null)
                parameters.ForEach(p => cmd.Parameters.Add(p));
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
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            parameters.ForEach(p => cmd.Parameters.Add(p));
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            return (int) cmd.LastInsertedId;
        }

        private int ExecuteNonQuery(string query)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var cmd = _connection.CreateCommand();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            return (int) cmd.LastInsertedId;
        }

        private static T CreateObjectFromReader<T>(MySqlDataReader reader) where T : new()
        {
            var result = new T();
            var tt = typeof (T);

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
                throw new Exception(string.Format("DatabaseTableAttribute not found on {0}", typeof (T)));
            }

            var idProperty = typeof (T)
                .GetDatabaseFileds()
                .FirstOrDefault(p => p.DatabaseFieldAttribute.IsPrimaryKey);

            var nameValues = typeof (T)
                .GetDatabaseFileds()
                .Where(p => !p.DatabaseFieldAttribute.IsPrimaryKey)
                .Select(p => p.GetMysqlQueryParameter(entity));

            var insertQuery = string.Format("INSERT INTO {2} ({0}) VALUES ({1})"
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
            var dbTable = typeof (T)
                .GetCustomAttribute<DatabaseTableAttribute>();

            if (dbTable == null)
            {
                throw new Exception(string.Format("DatabaseTableAttribute not found on {0}", typeof (T)));
            }

            var dbFields = typeof (T)
                .GetDatabaseFileds();

            var idProp = dbFields.FirstOrDefault(p => p.DatabaseFieldAttribute.IsPrimaryKey);
            if (idProp == null)
            {
                throw new Exception(string.Format("No Primary Key found! {0}", typeof (T)));
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
            return ExecuteQuery<Person>("SELECT * FROM szemely ORDER BY keresztnev, vezeteknev");
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
            //ExecuteNonQuery(string.Format("DELETE FROM kapcsolat WHERE szemely_id1 = {0} OR szemely_id2 = {0}",
            //    person.Id));
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

        #region Events

        public List<Participation> GetEventParticipators(int evtId)
        {
            var qb = new StringBuilder();
            qb.AppendLine("select szemely.szemely_id, 1 as resztvesz from szemely, resztvevo");
            qb.AppendLine("where resztvevo.resztvevo_id = szemely.szemely_id");
            qb.AppendLine("and resztvevo.esemeny_id = ?p");
            qb.AppendLine("union");
            qb.AppendLine("select szemely.szemely_id, 0 as resztvesz from szemely");
            qb.AppendLine("where szemely.szemely_id not in (select resztvevo_id from resztvevo where esemeny_id = ?p)");

            var q = qb.ToString();
            var p = new MySqlParameter("?p", evtId);
            return ExecuteQuery<Participation>(q, new List<MySqlParameter>{p});
        }

        public void AddEvent(Event evt)
        {
            Insert(evt);
        }

        public void UpdateEvent(Event evt)
        {
            Update(evt);
        }

        public void DeleteEvent(Event evt)
        {
            var p = new MySqlParameter("?p", evt.Id);
            const string query2 = "DELETE FROM esemeny WHERE esemeny_id = ?p";
            ExecuteNonQuery(query2, new List<MySqlParameter> { p });
        } 
        #endregion

        public List<GenderStatistic> GetGenderStatistics()
        {
            var temp =
                ExecuteQuery<GenderStatistic>(
                    "SELECT ferfi, count(*) as cnt, (select count(*) from szemely) as osszes FROM szemely GROUP BY ferfi ORDER BY ferfi");
            return temp;
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

        // Select with inner select
        public List<Person> GetAllPersonsWithoutChilds()
        {
            var q = new StringBuilder();
            q.AppendLine(
                "select * from szemely WHERE NOT exists( select * from kapcsolat WHERE kapcsolat.szemely_id1 = szemely.szemely_id and kapcsolat.tipus = 1)");

            return ExecuteQuery<Person>(q.ToString());
        }

        // Join with group by 1
        public List<PersonWithCount> GetFeritlityList()
        {
            var q = new StringBuilder();
            q.AppendLine(
                "select sz.szemely_id, sz.keresztnev, sz.vezeteknev, sz.ferfi, sz.halalozas_ideje, sz.leanykori_keresztnev, sz.leanykori_vezeteknev, sz.portre, sz.szuletes_ideje, count(*) as count from szemely as sz, kapcsolat as k");
            q.AppendLine("where sz.szemely_id = k.szemely_id1 and tipus = 1");
            q.AppendLine(
                "group by sz.szemely_id, sz.keresztnev, sz.vezeteknev, sz.ferfi, sz.halalozas_ideje, sz.leanykori_keresztnev, sz.leanykori_vezeteknev, sz.portre, sz.szuletes_ideje");
            q.AppendLine("order by count(*) desc");

            return ExecuteQuery<PersonWithCount>(q.ToString());
        }

        public List<GeneratedEvent> GetCombinedEventList()
        {
            var q = new StringBuilder();
            q.AppendLine(
                "select esemeny.*, null as tipus from esemeny, szemely as sz");
            q.AppendLine("where esemeny.szemely_id = sz.szemely_id");
            q.AppendLine("union");
            q.AppendLine(
                "select null as esemeny_id, sz.szuletes_ideje as idopont, null as megnevezes, sz.szemely_id as szemely_id, 1 as tipus from szemely as sz");
            q.AppendLine("where sz.szuletes_ideje is not null");
            q.AppendLine("union");
            q.AppendLine(
                "select null as esemeny_id, sz.halalozas_ideje as idopont, null as megnevezes, sz.szemely_id as szemely_id, 2 as tipus from szemely as sz");
            q.AppendLine("where sz.halalozas_ideje is not null");
            q.AppendLine("order by idopont asc");

            return ExecuteQuery<GeneratedEvent>(q.ToString());
        }

        public List<Event> GetPersonEvents(Person p)
        {
            const string query =
                "select distinct e.* from esemeny as e left outer join resztvevo as r on r.esemeny_id = e.esemeny_id where e.szemely_id = ?p or r.resztvevo_id = ?p ORDER BY idopont asc";
            var param = new MySqlParameter("?p", p.Id);
            return ExecuteQuery<Event>(query, new List<MySqlParameter> {param});
        }

        public List<YearStatistics> GetEventStatsByYear()
        {
            const string query =
                "select year(idopont) as year, count(*) as count from esemeny as e left outer join resztvevo as r on r.esemeny_id = e.esemeny_id group by year(idopont) order by year(idopont) desc";
            return ExecuteQuery<YearStatistics>(query);
        }

        public List<PersonIdAndCounter> GetMostParticipation()
        {
            const string query =
                "select cast(szemely_id as signed) as szemely_id, count(*) as count from (select esemeny.szemely_id as szemely_id from esemeny union all select resztvevo.resztvevo_id as szemely_id from resztvevo) as t group by szemely_id order by count(*) desc";
            return ExecuteQuery<PersonIdAndCounter>(query);
        }


        public void UpdateEventParticipators(int eventId, List<int> participators)
        {
            var existing = GetEventParticipators(eventId)
                .Where(p => p.IsParticipating > 0)
                .Select(p => p.PersonId)
                .ToList();

            var add = participators.Except(existing).ToList();
            var del = existing.Except(participators).ToList();

            add.ForEach(a =>
            {
                var q = "INSERT INTO resztvevo (resztvevo_id, esemeny_id) VALUES (?p1, ?p2)";
                var prms = new List<MySqlParameter>
                {
                    new MySqlParameter("?p1", a),
                    new MySqlParameter("?p2", eventId)
                };
                ExecuteNonQuery(q, prms);
            });
            del.ForEach(d =>
            {
                var q = "DELETE FROM resztvevo WHERE resztvevo_id = ?p1 AND esemeny_id = ?p2";
                var prms = new List<MySqlParameter>
                {
                    new MySqlParameter("?p1", d),
                    new MySqlParameter("?p2", eventId)
                };
                ExecuteNonQuery(q, prms);
            });
        }
    }
}
