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

        public bool PositionIDToEnabled(PositionID PositionID)
        {
            return (PositionID) switch
            {
                PositionID.Head => Head.Value.GetEnabled(),

                PositionID.Vest => Vest.Value.GetEnabled(),
                PositionID.VestFront => Vest.Value.GetEnabled(),
                PositionID.VestBack => Vest.Value.GetEnabled(),

                PositionID.ArmLeft => ArmLeft.Value.GetEnabled(),
                PositionID.ArmRight => ArmRight.Value.GetEnabled(),

                PositionID.HandLeft => HandLeft.Value.GetEnabled(),
                PositionID.HandRight => HandRight.Value.GetEnabled(),

                //PositionID.GloveLeft => GloveLeft.Value.GetEnabled(),
                //PositionID.GloveRight => GloveRight.Value.GetEnabled(),

                PositionID.FootLeft => FootLeft.Value.GetEnabled(),
                PositionID.FootRight => FootRight.Value.GetEnabled(),

                _ => true
            };
        }

        public int PositionIDToIntensity(PositionID PositionID)
        {
            return (PositionID) switch
            {
                PositionID.Head => Head.Value.GetIntensity(),

                PositionID.Vest => Vest.Value.GetIntensity(),
                PositionID.VestFront => Vest.Value.GetIntensity(),
                PositionID.VestBack => Vest.Value.GetIntensity(),

                PositionID.ArmLeft => ArmLeft.Value.GetIntensity(),
                PositionID.ArmRight => ArmRight.Value.GetIntensity(),

                PositionID.HandLeft => HandLeft.Value.GetIntensity(),
                PositionID.HandRight => HandRight.Value.GetIntensity(),

                //PositionID.GloveLeft => GloveLeft.Value.GetIntensity(),
                //PositionID.GloveRight => GloveRight.Value.GetIntensity(),

                PositionID.FootLeft => FootLeft.Value.GetIntensity(),
                PositionID.FootRight => FootRight.Value.GetIntensity(),

                _ => 100
            };
        }
    }
}
