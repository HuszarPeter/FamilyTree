using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Dal;
using MySql.Data.MySqlClient;

namespace FamilyTree.Utils
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if(list == null || action == null) return;
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static List<PropertyMappedDatabaseField> GetDatabaseFileds(this Type entityType)
        {
            return entityType
                .GetProperties()
                .Select(p => new PropertyMappedDatabaseField
                {
                    PropertyInfo = p,
                    DatabaseFieldAttribute = p.GetCustomAttribute<DatabaseFieldAttribute>()
                })
                .Where(i => i.DatabaseFieldAttribute != null)
                .ToList();
        }

        public static MySqlParameter GetMysqlQueryParameter(this PropertyMappedDatabaseField pd, Object o)
        {
            PropertyInfo pi = pd.PropertyInfo;
            var result = new MySqlParameter
            {
                SourceColumn = pd.DatabaseFieldAttribute.Name,
                ParameterName = string.Format("?p{0}", pd.DatabaseFieldAttribute.Name)
            };

            if (pi.PropertyType == typeof (string))
            {
                result.MySqlDbType = MySqlDbType.String;
            } 
            else if (pi.PropertyType == typeof(int))
            {
                result.MySqlDbType = MySqlDbType.Int64;
            }
            else if (pi.PropertyType == typeof (byte[]))
            {
                result.MySqlDbType = MySqlDbType.LongBlob;
            }
            else if (pi.PropertyType == typeof (bool))
            {
                result.MySqlDbType = MySqlDbType.Int16;
            }
            result.Value = pi.GetValue(o);
            return result;
        }

        public static string GetMySqlCompatiblePropertyValue(this PropertyInfo pi, Object o)
        {
            var result = "NULL";
            var value = pi.GetValue(o);

            if (value != null)
            {
                if (pi.PropertyType == typeof (string))
                {
                    result = string.Format("\"{0}\"", value);
                }
                else if (pi.PropertyType == typeof (int))
                {
                    result = string.Format("{0}", value);
                }
                else if (pi.PropertyType == typeof (bool))
                {
                    result = (bool)value ? "1" : "0";
                }
            }

            return result;
        }
    }


}
