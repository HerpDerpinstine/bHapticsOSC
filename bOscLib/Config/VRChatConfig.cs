using bHapticsOSC.Config.Interface;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class VRChatConfig : ConfigFile
    {
        public ConfigCategory<VRChat> vrchat;

        public VRChatConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                vrchat = new ConfigCategory<VRChat>(nameof(VRChat)),
            });
        }

        [TomlDoNotInlineObject]
        public class VRChat : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Devices should React while AFK.")]
            public bool AFK = true;
            [TomlPrecedingComment("If the Devices should React while in a Station.")]
            public bool InStation = true;
            [TomlPrecedingComment("If the Devices should React while Seated in a Station.")]
            public bool Seated = true;
        }
    }
}
