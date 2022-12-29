using System;

namespace Network.Attributes
{
    public class Table : Attribute
    {
        public string Name;

        public Table()
        {
            Name = GetType().Name;
        }

        public Table(string name)
        {
            Name = name;
        }
    }
}
