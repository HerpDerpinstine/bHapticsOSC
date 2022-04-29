using bHapticsLib;
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

        public bool PositionTypeToEnabled(PositionType positionType)
        {
            return (positionType) switch
            {
                PositionType.Head => Head.Value.Enabled,

                PositionType.VestFront => Vest.Value.Enabled,
                PositionType.VestBack => Vest.Value.Enabled,

                PositionType.ForearmL => ArmLeft.Value.Enabled,
                PositionType.ForearmR => ArmRight.Value.Enabled,

                PositionType.HandL => HandLeft.Value.Enabled,
                PositionType.HandR => HandRight.Value.Enabled,

                //PositionType.GloveLeft => GloveLeft.Value.Enabled,
                //PositionType.GloveRight => GloveRight.Value.Enabled,

                PositionType.FootL => FootLeft.Value.Enabled,
                PositionType.FootR => FootRight.Value.Enabled,

                _ => true
            };
        }

        public int PositionTypeToIntensity(PositionType positionType)
        {
            return (positionType) switch
            {
                PositionType.Head => Head.Value.Intensity,

                PositionType.VestFront => Vest.Value.Intensity,
                PositionType.VestBack => Vest.Value.Intensity,

                PositionType.ForearmL => ArmLeft.Value.Intensity,
                PositionType.ForearmR => ArmRight.Value.Intensity,

                PositionType.HandL => HandLeft.Value.Intensity,
                PositionType.HandR => HandRight.Value.Intensity,

                //PositionType.GloveLeft => GloveLeft.Value.Intensity,
                //PositionType.GloveRight => GloveRight.Value.Intensity,

                PositionType.FootL => FootLeft.Value.Intensity,
                PositionType.FootR => FootRight.Value.Intensity,

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
