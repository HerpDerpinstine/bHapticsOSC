#if UNITY_EDITOR
using System.Collections.Generic;
using VRC.Dynamics;

namespace bHapticsOSC.VRChat
{
    public static class bContacts
    {
        public static List<string> DefaultTags = new List<string>
        {
            "Head",
            "Torso",

            "Hand",
            "HandL",
            "HandR",

            "Foot",
            "FootL",
            "FootR",

            "Finger",
            "FingerL",
            "FingerR",

            "FingerIndex",
            "FingerIndexL",
            "FingerIndexR",

            "FingerMiddle",
            "FingerMiddleL",
            "FingerMiddleR",

            "FingerRing",
            "FingerRingL",
            "FingerRingR",

            "FingerLittle",
            "FingerLittleL",
            "FingerLittleR",
        };

        public static void ScanForExistingTags(bUserSettings settings)
        {
            if (settings.CurrentPrefab != null)
                foreach (ContactReceiver contactReceiver in settings.CurrentPrefab.GetComponentsInChildren<ContactReceiver>(true))
                    foreach (string tag in contactReceiver.collisionTags)
                        if (!DefaultTags.Contains(tag) && !settings.CustomContactTags.Contains(tag))
                            settings.CustomContactTags.Add(tag);
        }

        public static void ApplyNewTags(bHapticsOSCIntegration editorComp)
        {
            foreach (bUserSettings settings in editorComp.AllUserSettings.Values)
            {
                if (settings.CurrentPrefab == null)
                    continue;

                foreach (ContactReceiver contactReceiver in settings.CurrentPrefab.GetComponentsInChildren<ContactReceiver>(true))
                {
                    foreach (string tag in contactReceiver.collisionTags.ToArray())
                        if (!DefaultTags.Contains(tag))
                            contactReceiver.collisionTags.Remove(tag);

                    contactReceiver.collisionTags.AddRange(settings.CustomContactTags);
                }
            }
        }
    }
}
#endif