using Network.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Network.Game
{
    public static class PersistantObjectParser
    {
        public static TableData Parse(PersistantObject obj, bool ignoreDefault = false)
        {
            var table = Attribute.GetCustomAttribute(obj.GetType(), typeof(Table));
            if (table == null)
                throw new Exception("This object is not declared as a database Table");
            string tableName = ((Table)table).Name;

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
                            if(!ignoreDefault || !data.Default)
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
