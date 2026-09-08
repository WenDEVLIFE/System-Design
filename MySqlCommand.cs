using System;

namespace System_Design
{
    internal class MySqlCommand
    {
        public object Parameters { get; internal set; }

        internal MySqlDataReader ExecuteReader()
        {
            throw new NotImplementedException();
        }
    }
}