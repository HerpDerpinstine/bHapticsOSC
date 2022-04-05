﻿using bHapticsOSC.Config.Interface;
using bHapticsOSC.Utils;
using Tomlet.Attributes;

namespace bHapticsOSC.Config
{
    public class UdonAudioLinkConfig : ConfigFile
    {
        public ConfigCategory<UdonAudioLink> udonAudioLink;

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

        public UdonAudioLinkConfig(string filepath) : base(filepath)
        {
            Categories.AddRange(new ConfigCategory[]
            {
                udonAudioLink = new ConfigCategory<UdonAudioLink>(nameof(UdonAudioLink)),

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

        public bool PositionTypeToEnabled(bHaptics.PositionType positionType)
        {
            return (positionType) switch
            {
                bHaptics.PositionType.Head => Head.Value.Enabled,

                bHaptics.PositionType.VestFront => Vest.Value.Enabled,
                bHaptics.PositionType.VestBack => Vest.Value.Enabled,

                bHaptics.PositionType.ForearmL => ArmLeft.Value.Enabled,
                bHaptics.PositionType.ForearmR => ArmRight.Value.Enabled,

                bHaptics.PositionType.HandL => HandLeft.Value.Enabled,
                bHaptics.PositionType.HandR => HandRight.Value.Enabled,

                //bHaptics.PositionType.GloveLeft => GloveLeft.Value.Enabled,
                //bHaptics.PositionType.GloveRight => GloveRight.Value.Enabled,

                bHaptics.PositionType.FootL => FootLeft.Value.Enabled,
                bHaptics.PositionType.FootR => FootRight.Value.Enabled,

                _ => true
            };
        }

        public int PositionTypeToIntensity(bHaptics.PositionType positionType)
        {
            return (positionType) switch
            {
                bHaptics.PositionType.Head => Head.Value.Intensity,

                bHaptics.PositionType.VestFront => Vest.Value.Intensity,
                bHaptics.PositionType.VestBack => Vest.Value.Intensity,

                bHaptics.PositionType.ForearmL => ArmLeft.Value.Intensity,
                bHaptics.PositionType.ForearmR => ArmRight.Value.Intensity,

                bHaptics.PositionType.HandL => HandLeft.Value.Intensity,
                bHaptics.PositionType.HandR => HandRight.Value.Intensity,

                //bHaptics.PositionType.GloveLeft => GloveLeft.Value.Intensity,
                //bHaptics.PositionType.GloveRight => GloveRight.Value.Intensity,

                bHaptics.PositionType.FootL => FootLeft.Value.Intensity,
                bHaptics.PositionType.FootR => FootRight.Value.Intensity,

                _ => 100
            };
        }

        [TomlDoNotInlineObject]
        public class UdonAudioLink : ConfigCategoryValue
        {
            [TomlPrecedingComment("If Touch Intensity should be overridden by Udon AudioLink Intensity.")]
            public bool OverrideTouch = false;
        }

        [TomlDoNotInlineObject]
        public class Device : ConfigCategoryValue
        {
            [TomlPrecedingComment("If the Device should react to Udon AudioLink Amplitude.")]
            public bool Enabled = true;

            [TomlPrecedingComment("Percentage of Intensity for the Device's Reaction to Udon AudioLink Amplitude.  (0 - 500)")]
            public int Intensity = 100;

            public override void Clamp()
                => Intensity = Intensity.Clamp(0, 500);
        }
    }
}