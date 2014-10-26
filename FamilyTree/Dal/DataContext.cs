using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal.Model;
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
        }

        private List<T> ExecuteQuery<T>(string query) where T : new()
        {
            var result = new List<T>();
            if (_connection.State == ConnectionState.Closed)
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

        public List<Person> GetAllPersons()
        {
            return ExecuteQuery<Person>("SELECT * FROM szemely");
        }

        public List<Relation> GetAllRelations()
        {
            return ExecuteQuery<Relation>("SELECT * FROM kapcsolat");
        }

        public void Dispose()
        {
            if (_connection == null || _connection.State != ConnectionState.Open) return;

            _connection.Close();
            _connection = null;
        }
    }
}
