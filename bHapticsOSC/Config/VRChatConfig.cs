using OscLib.Config;
using Tomlet.Attributes;

namespace bHapticsOSC
{
    public class VRChatConfig : ConfigFile
    {
        public ConfigCategory<Reactivity> reactivity;
        public ConfigCategory<AvatarOSCConfigReset> avatarOSCConfigReset;

        public VRChatConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                reactivity = new ConfigCategory<Reactivity>(nameof(Reactivity)),
                avatarOSCConfigReset = new ConfigCategory<AvatarOSCConfigReset>(nameof(avatarOSCConfigReset))
            });
        }

        [TomlDoNotInlineObject]
        public class Reactivity : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Devices should React while AFK.")]
            public bool AFK = true;
            [TomlPrecedingComment("If the Devices should React while in a Station.")]
            public bool InStation = true;
            [TomlPrecedingComment("If the Devices should React while Seated in a Station.")]
            public bool Seated = true;
        }

        [TomlDoNotInlineObject]
        public class AvatarOSCConfigReset : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Application should Automatically Reset the Avatar's OSC Config on Xhange.")]
            public bool Enabled = true;
        }
    }
}
