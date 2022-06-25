using bHapticsLib;
using OscLib.Config;

namespace bHapticsOSC
{
    public abstract class DeviceCategoryBase : ConfigCategoryValue
    {
        public abstract bool GetEnabled();
        public abstract int GetIntensity();
    }

    public class DevicesConfigBase<T> : ConfigFile where T : DeviceCategoryBase
    {
        public ConfigCategory<T> Head;

        public ConfigCategory<T> Vest;

        public ConfigCategory<T> ArmLeft;
        public ConfigCategory<T> ArmRight;

        public ConfigCategory<T> HandLeft;
        public ConfigCategory<T> HandRight;

        //public ConfigCategory<T> GloveLeft;
        //public ConfigCategory<T> GloveRight;

        public ConfigCategory<T> FootLeft;
        public ConfigCategory<T> FootRight;

        public DevicesConfigBase(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]{
                Head = new ConfigCategory<T>(nameof(Head)),

                Vest = new ConfigCategory<T>(nameof(Vest)),

                ArmLeft = new ConfigCategory<T>(nameof(ArmLeft)),
                ArmRight = new ConfigCategory<T>(nameof(ArmRight)),

                HandLeft = new ConfigCategory<T>(nameof(HandLeft)),
                HandRight = new ConfigCategory<T>(nameof(HandRight)),

                //GloveLeft = new ConfigCategory<T>(nameof(GloveLeft)),
                //GloveRight = new ConfigCategory<T>(nameof(GloveRight)),

                FootLeft = new ConfigCategory<T>(nameof(FootLeft)),
                FootRight = new ConfigCategory<T>(nameof(FootRight))
            });
        }

        public bool PositionTypeToEnabled(PositionType positionType)
        {
            return (positionType) switch
            {
                PositionType.Head => Head.Value.GetEnabled(),

                PositionType.Vest => Vest.Value.GetEnabled(),
                PositionType.VestFront => Vest.Value.GetEnabled(),
                PositionType.VestBack => Vest.Value.GetEnabled(),

                PositionType.ForearmL => ArmLeft.Value.GetEnabled(),
                PositionType.ForearmR => ArmRight.Value.GetEnabled(),

                PositionType.HandL => HandLeft.Value.GetEnabled(),
                PositionType.HandR => HandRight.Value.GetEnabled(),

                //PositionType.GloveLeft => GloveLeft.Value.GetEnabled(),
                //PositionType.GloveRight => GloveRight.Value.GetEnabled(),

                PositionType.FootL => FootLeft.Value.GetEnabled(),
                PositionType.FootR => FootRight.Value.GetEnabled(),

                _ => true
            };
        }

        public int PositionTypeToIntensity(PositionType positionType)
        {
            return (positionType) switch
            {
                PositionType.Head => Head.Value.GetIntensity(),

                PositionType.Vest => Vest.Value.GetIntensity(),
                PositionType.VestFront => Vest.Value.GetIntensity(),
                PositionType.VestBack => Vest.Value.GetIntensity(),

                PositionType.ForearmL => ArmLeft.Value.GetIntensity(),
                PositionType.ForearmR => ArmRight.Value.GetIntensity(),

                PositionType.HandL => HandLeft.Value.GetIntensity(),
                PositionType.HandR => HandRight.Value.GetIntensity(),

                //PositionType.GloveLeft => GloveLeft.Value.GetIntensity(),
                //PositionType.GloveRight => GloveRight.Value.GetIntensity(),

                PositionType.FootL => FootLeft.Value.GetIntensity(),
                PositionType.FootR => FootRight.Value.GetIntensity(),

                _ => 100
            };
        }
    }
}
