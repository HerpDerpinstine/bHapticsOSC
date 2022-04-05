using bHapticsOSC.Config.Interface;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class AudioLinkConfig : DevicesConfig
    {
        public ConfigCategory<AudioLink> audioLink;

        public AudioLinkConfig(string filepath) : base(filepath)
        {
            Categories.Add(audioLink = new ConfigCategory<AudioLink>(nameof(AudioLink)));
        }

        [TomlDoNotInlineObject]
        public class AudioLink : ConfigCategoryValue
        {
            [TomlPrecedingComment("If Touch Intensity should be overridden by AudioLink Intensity.")]
            public bool OverrideTouch = false;
        }
    }
}
