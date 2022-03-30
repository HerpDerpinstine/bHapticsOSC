using bHapticsOSC.Config.Interface;
using Tomlet.Attributes;
using bHapticsOSC.Utils;

namespace bHapticsOSC.Config
{
    public class ConnectionConfig : ConfigFile
    {
        public ConfigCategory<Receiver> receiver;

        public ConnectionConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                receiver = new ConfigCategory<Receiver>(nameof(Receiver)),
            });
        }

        [TomlDoNotInlineObject]
        public class Receiver : ConfigCategoryValue
        {
            [TomlPrecedingComment("Port for the Receiver.  (0 - 65535)")]
            public int Port = 9001;

            public override void Clamp()
                => Port = Port.Clamp(0, 65535);
        }
    }
}
