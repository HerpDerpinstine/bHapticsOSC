using bHapticsOSC.Config.Interface;
using Tomlet.Attributes;
using bHapticsOSC.Utils;

namespace bHapticsOSC.Config
{
    public class ConnectionConfig : ConfigFile
    {
        public ConfigCategory<Connection> connection;
        public ConfigCategory<Threading> threading;

        public ConnectionConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                connection = new ConfigCategory<Connection>(nameof(Connection)),
                threading = new ConfigCategory<Threading>(nameof(Threading))
            });
        }

        [TomlDoNotInlineObject]
        public class Connection : ConfigCategoryValue
        {
            [TomlPrecedingComment("Port for the Receiver.  (0 - 65535)")]
            public int Port = 9001;

            public override void Clamp()
                => Port = Port.Clamp(0, 65535);
        }

        [TomlDoNotInlineObject]
        public class Threading : ConfigCategoryValue
        {
            [TomlPrecedingComment("Update Rate of the Receiver Thread in Milliseconds.  (100 - 1000)")]
            public int UpdateRate = 100;

            public override void Clamp()
                => UpdateRate = UpdateRate.Clamp(100, 1000);
        }
    }
}
