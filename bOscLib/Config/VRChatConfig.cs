using bHapticsOSC.Config.Interface;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class VRChatConfig : ConfigFile
    {
        public ConfigCategory<VRChat> vrchat;

        public VRChatConfig(string filepath) : base(filepath)
        {
            Categories.Add(vrchat = new ConfigCategory<VRChat>(nameof(VRChat)));
        }

        [TomlDoNotInlineObject]
        public class VRChat : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Devices should React while in a Seat.")]
            public bool InStation = true;
        }
    }
}
