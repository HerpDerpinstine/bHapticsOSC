using bHapticsOSC.Config.Interface;
using bHapticsOSC.Utils;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class DevicesConfig : ConfigFile
    {
        public ConfigCategory<Device> Head;

        public ConfigCategory<Device> Vest;

        public ConfigCategory<Device> ArmLeft;
        public ConfigCategory<Device> ArmRight;

        public ConfigCategory<Device> HandLeft;
        public ConfigCategory<Device> HandRight;

        public ConfigCategory<Device> FootLeft;
        public ConfigCategory<Device> FootRight;

        public DevicesConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]{
                Head = new ConfigCategory<Device>(nameof(Head)),

                Vest = new ConfigCategory<Device>(nameof(Vest)),

                ArmLeft = new ConfigCategory<Device>(nameof(ArmLeft)),
                ArmRight = new ConfigCategory<Device>(nameof(ArmRight)),

                HandLeft = new ConfigCategory<Device>(nameof(HandLeft)),
                HandRight = new ConfigCategory<Device>(nameof(HandRight)),

                FootLeft = new ConfigCategory<Device>(nameof(FootLeft)),
                FootRight = new ConfigCategory<Device>(nameof(FootRight))
            });
        }
        
        [TomlDoNotInlineObject]
        public class Device : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Device is Enabled.")]
            public bool Enabled = true;

            [TomlPrecedingComment("Percentage of Intensity for the Device.  (0 - 500)")]
            public int Intensity = 100;

            public override void Clamp()
                => Intensity = Intensity.Clamp(0, 500);
        }
    }
}
