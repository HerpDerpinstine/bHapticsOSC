using OscLib.Utils;
using Tomlet.Attributes;

namespace bHapticsOSC
{
    public class DevicesConfig : DevicesConfigBase<DevicesConfig.Device>
    {
        public DevicesConfig(string filepath) : base(filepath) { }

        [TomlDoNotInlineObject]
        public class Device : DeviceCategoryBase
        {
            [TomlPrecedingComment("If the Device is Enabled.")]
            public bool Enabled = true;

            [TomlPrecedingComment("Percentage of Intensity for the Device.  (0 - 500)")]
            public int Intensity = 100;

            public override void Clamp()
                => Intensity = Intensity.Clamp(0, 500);

            public override bool GetEnabled()
                => Enabled;
            public override int GetIntensity()
                => Intensity;
        }
    }
}
