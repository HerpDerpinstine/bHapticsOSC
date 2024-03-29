﻿using System.Net;
using OscLib.Config;
using OscLib.Utils;
using Tomlet.Attributes;

namespace OscLib
{
    public class ConnectionConfig : ConfigFile
    {
        public ConfigCategory<Receiver> receiver;
        public ConfigCategory<Sender> sender;

        public ConnectionConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                receiver = new ConfigCategory<Receiver>("OscReceiver"),
                sender = new ConfigCategory<Sender>("OscSender")
            });
        }

        [TomlDoNotInlineObject]
        public class Receiver : ConfigCategoryValue
        {
            [TomlPrecedingComment("Port for the OSC Receiver.  (0 - 65535)")]
            public int Port = 9001;

            public override void Clamp()
                => Port = Port.Clamp(0, 65535);
        }

        [TomlDoNotInlineObject]
        public class Sender : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the OSC Sender is Enabled.")]
            public bool Enabled = false;

            [TomlPrecedingComment("IP Address for the OSC Sender.")]
            public string IP = "127.0.0.1";

            [TomlPrecedingComment("Port for the OSC Sender.  (0 - 65535)")]
            public int Port = 9000;

            [TomlPrecedingComment("If the OSC Sender should pipe ALL packets and not just the ones bHapticsOSC understands.")]
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
