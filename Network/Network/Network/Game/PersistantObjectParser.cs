using Network.Game;
using Network.Attributes;
using System.Reflection;

namespace Network.Game
{
    public static class PersistantObjectParser
    {
        public static Dictionary<string, object> Parse(PersistantObject obj, out string tableName, bool includeKey = true)
        {
            var table = Attribute.GetCustomAttribute(obj.GetType(), typeof(Table));
            if (table == null)
                throw new Exception("This object is not declared as a database Table");
            tableName = ((Table)table).Name;

            Dictionary<string, object> result = new();

            FieldInfo[] fieldInfos = obj.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var attributes = fieldInfo.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute.GetType() == typeof(Column))
                    {
                        Column data = (Column)attribute;
                        if (includeKey == true || !data.PrimaryKey)
                        {
                            string name = data.Name;
                            var value = fieldInfo.GetValue(obj);
                            result.Add(name, value);
                        }
                    }
                }
            }

            return result;
        }
    }
}
