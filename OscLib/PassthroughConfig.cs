using System.Net;
using OscLib.Config;
using OscLib.Utils;
using Tomlet.Attributes;

namespace OscLib
{
    public class PassthroughConfig : ConfigFile
    {
        public ConfigCategory<Receiver> receiver;
        public ConfigCategory<Sender> sender;

        public PassthroughConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                receiver = new ConfigCategory<Receiver>("Receiver"),
                sender = new ConfigCategory<Sender>("Sender")
            });
        }

        [TomlDoNotInlineObject]
        public class Receiver : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Passthrough OSC Receiver is Enabled.")]
            public bool Enabled = false;

            [TomlPrecedingComment("Port for the Passthrough OSC Receiver.  (0 - 65535)")]
            public int Port = 1337;

            public override void Clamp()
                => Port = Port.Clamp(0, 65535);
        }

        [TomlDoNotInlineObject]
        public class Sender : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Passthrough OSC Sender is Enabled.")]
            public bool Enabled = false;

            [TomlPrecedingComment("IP Address for the Passthrough OSC Sender.")]
            public string IP = "127.0.0.1";

            [TomlPrecedingComment("Port for the Passthrough OSC Sender.  (0 - 65535)")]
            public int Port = 1338;

            [TomlPrecedingComment("If the Passthrough OSC Sender should pipe ALL packets and not just the ones bHapticsOSC understands.")]
            public bool PipeAllPackets = false;

            public override void Clamp()
            {
                Port = Port.Clamp(0, 65535);
                if (!IPAddress.TryParse(IP, out IPAddress address))
                    IP = "127.0.0.1";
            }
        }
    }
}
