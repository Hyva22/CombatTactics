using Network.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Network.Game
{
    public static class PersistantObjectParser
    {
        public static string GetTableName(Type type)
        {
            var table = type.GetCustomAttribute(typeof(Table));
            if (table == null)
                throw new Exception("This object is not declared as a database Table");
            string tableName = ((Table)table).Name;

            return tableName;
        }

        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// Checks if a field has an attribute
        /// </summary>
        /// <param name="fieldInfo">The field to search on</param>
        /// <param name="type">type of the searched attribute</param>
        /// <returns>true if attribute was found. false if not</returns>
        public static A GetAttribute<A>(FieldInfo fieldInfo) where A : Attribute
        {
            var attributes = fieldInfo.GetCustomAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(A))
                    return (A)attribute;
            }

            return null;
        }

        /// <summary>
        /// Fetches the fieldinfo of a type, of a class
        /// </summary>
        /// <typeparam name="C">Class to search on</typeparam>
        /// <typeparam name="A">Type of the field</typeparam>
        /// <returns>Found fields of type A on class C</returns>
        public static Dictionary<A, FieldInfo> GetAttributes<C, A>() where A : Attribute
        {
            Dictionary<A, FieldInfo> result = new();
            FieldInfo[] fieldInfos = typeof(C).GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                A attribute = GetAttribute<A>(fieldInfo);
                if (attribute != null)
                        result.Add(attribute, fieldInfo);
            }
            return result;
        }

        public static TableData Parse(PersistantObject obj, bool ignoreDefault = false)
        {
            string tableName = GetTableName(obj.GetType());

            Dictionary<string, object> columns = new();
            (string, object) primaryKey = default;

            FieldInfo[] fieldInfos = obj.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var attributes = fieldInfo.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType() == typeof(Column))
                    {
                        Column data = (Column)attribute;
                        string name = data.Name;
                        var value = fieldInfo.GetValue(obj);
                        if (data.PrimaryKey)
                        {
                            primaryKey = new(name, value);
                        }
                        else
                        {
                            if (!ignoreDefault || !data.Default || value != default)
                            {
                                columns.Add(name, value);
                            }
                        }
                    }
                }
            }

            if (primaryKey == default)
                throw new Exception("Could not find primary key.");

            TableData tabledata = new(tableName, primaryKey, columns);

            return tabledata;
        }
    }
}
