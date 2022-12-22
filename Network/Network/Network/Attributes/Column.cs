using System;

namespace Network.Attributes
{
    public class Column : Attribute
    {
        public string Name;
        public bool PrimaryKey;
        public bool SecondaryKey;
        public bool NotNull;
        public bool Unique;
        public bool Default;

        public Column(bool primaryKey = false)
        {
            Name = GetType().Name;
            PrimaryKey = primaryKey;
        }

        public Column(string name, bool primaryKey = false, bool secondaryKey = false, bool notNull = false, bool unique = false, bool @default = false)
        {
            if (primaryKey && secondaryKey)
                throw new Exception("Column can not be primary key and secondary key!");
            if ((primaryKey || secondaryKey) && @default)
                throw new Exception("Primary or secondary key can't be dafault!");

            Name = name;
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            NotNull = notNull;
            Unique = unique;
            Default = @default;
        }
    }
}
