#if UNITY_EDITOR && VRC_SDK_VRCSDK3 && bHapticsOSC_HasAac
namespace bHapticsOSC.VRChat
{
    public enum bDeviceType
    {
        HEAD,

        VEST,
        VEST_FRONT,
        VEST_BACK,

        ARM_LEFT,
        ARM_RIGHT,

        HAND_LEFT,
        HAND_RIGHT,

        //GLOVE_LEFT,
        //GLOVE_RIGHT,

        FOOT_LEFT,
        FOOT_RIGHT,
    }
}
#endif