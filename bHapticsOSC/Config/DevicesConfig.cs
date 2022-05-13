using OscLib.Config;
using OscLib.Utils;
using Tomlet.Attributes;

namespace bHapticsOSC
{
    public class DevicesConfig : ConfigFile
    {
        public ConfigCategory<Device> Head;

        public ConfigCategory<Device> Vest;

        public ConfigCategory<Device> ArmLeft;
        public ConfigCategory<Device> ArmRight;

        public ConfigCategory<Device> HandLeft;
        public ConfigCategory<Device> HandRight;

        //public ConfigCategory<Device> GloveLeft;
        //public ConfigCategory<Device> GloveRight;

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

                //GloveLeft = new ConfigCategory<Device>(nameof(GloveLeft)),
                //GloveRight = new ConfigCategory<Device>(nameof(GloveRight)),

                FootLeft = new ConfigCategory<Device>(nameof(FootLeft)),
                FootRight = new ConfigCategory<Device>(nameof(FootRight))
            });
        }

        public bool PositionTypeToEnabled(bHapticsLib.PositionType positionType)
        {
            return (positionType) switch
            {
                bHapticsLib.PositionType.Head => Head.Value.Enabled,

                bHapticsLib.PositionType.VestFront => Vest.Value.Enabled,
                bHapticsLib.PositionType.VestBack => Vest.Value.Enabled,

                bHapticsLib.PositionType.ForearmL => ArmLeft.Value.Enabled,
                bHapticsLib.PositionType.ForearmR => ArmRight.Value.Enabled,

                bHapticsLib.PositionType.HandL => HandLeft.Value.Enabled,
                bHapticsLib.PositionType.HandR => HandRight.Value.Enabled,

                //bHaptics.PositionType.GloveLeft => GloveLeft.Value.Enabled,
                //bHaptics.PositionType.GloveRight => GloveRight.Value.Enabled,

                bHapticsLib.PositionType.FootL => FootLeft.Value.Enabled,
                bHapticsLib.PositionType.FootR => FootRight.Value.Enabled,

                _ => true
            };
        }

        public int PositionTypeToIntensity(bHapticsLib.PositionType positionType)
        {
            return (positionType) switch
            {
                bHapticsLib.PositionType.Head => Head.Value.Intensity,

                bHapticsLib.PositionType.VestFront => Vest.Value.Intensity,
                bHapticsLib.PositionType.VestBack => Vest.Value.Intensity,

                bHapticsLib.PositionType.ForearmL => ArmLeft.Value.Intensity,
                bHapticsLib.PositionType.ForearmR => ArmRight.Value.Intensity,

                bHapticsLib.PositionType.HandL => HandLeft.Value.Intensity,
                bHapticsLib.PositionType.HandR => HandRight.Value.Intensity,

                //bHaptics.PositionType.GloveLeft => GloveLeft.Value.Intensity,
                //bHaptics.PositionType.GloveRight => GloveRight.Value.Intensity,

                bHapticsLib.PositionType.FootL => FootLeft.Value.Intensity,
                bHapticsLib.PositionType.FootR => FootRight.Value.Intensity,

                _ => 100
            };
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
