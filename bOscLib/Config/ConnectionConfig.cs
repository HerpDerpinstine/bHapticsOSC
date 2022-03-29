using bOscLib.Config.Interface;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class ConnectionConfig : ConfigFile
    {
        public ConfigCategory<Connection> connection;
        public ConfigCategory<Threading> threading;

        public ConnectionConfig(string filepath) : base(filepath)
        {
            Categories.Add(connection = new ConfigCategory<Connection>(nameof(Connection)));
            Categories.Add(threading = new ConfigCategory<Threading>(nameof(Threading)));
        }

        [TomlDoNotInlineObject]
        public class Connection
        {
            public int Port = 9001;
        }

        [TomlDoNotInlineObject]
        public class Threading
        {
            public int UpdateRate = 100;
        }
    }
}
