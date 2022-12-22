using System.Collections.Generic;

namespace Network.Game
{
    public class TableData
    {
        public string TableName;
        public (string, object) PrimaryKey;
        public ((string, object), (string, object)) SecondaryKeys;
        public Dictionary<string, object> Data;

        public TableData(string tableName, (string, object) primaryKey, Dictionary<string, object> data)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
            Data = data;
        }
    }
}
