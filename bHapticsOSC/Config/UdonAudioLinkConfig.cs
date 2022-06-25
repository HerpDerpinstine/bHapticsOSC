using OscLib.Config;
using OscLib.Utils;
using Tomlet.Attributes;

namespace bHapticsOSC
{
    public class UdonAudioLinkConfig : DevicesConfigBase<UdonAudioLinkConfig.Device>
    {
        public ConfigCategory<UdonAudioLink> udonAudioLink;

        public UdonAudioLinkConfig(string filepath) : base(filepath)
            => Categories.Add(udonAudioLink = new ConfigCategory<UdonAudioLink>(nameof(UdonAudioLink)));

        [TomlDoNotInlineObject]
        public class UdonAudioLink : ConfigCategoryValue
        {
            [TomlPrecedingComment("If Udon AudioLink Extension support should be Enabled.")]
            public bool Enabled = true;
            [TomlPrecedingComment("If Touch Intensity should be overridden by Udon AudioLink Intensity instead of layered together.")]
            public bool OverrideTouch = false;
        }

        [TomlDoNotInlineObject]
        public class Device : DeviceCategoryBase
        {
            [TomlPrecedingComment("If the Device should react to Udon AudioLink Amplitude.")]
            public bool Enabled = true;

            [TomlPrecedingComment("Percentage of Intensity for the Device's Reaction to Udon AudioLink Amplitude.  (0 - 500)")]
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