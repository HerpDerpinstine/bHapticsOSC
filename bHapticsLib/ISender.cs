using System;

namespace bHapticsLib
{
    public interface ISender
    {
        void TurnOff(string key);
        void TurnOff();
        void Register(string key, Project project);

        void SubmitRegistered(string key);
        void SubmitRegistered(string key, float ratio);
        void SubmitRegistered(string key, string altKey, ScaleOption option);
        void SubmitRegistered(string key, string altKey, RotationOption option, ScaleOption sOption);
        void Submit(string key, Frame req);

        void Dispose();
        void Enable();
        void Disable();

        event Action<PlayerResponse> StatusReceived;
        event Action<bool> ConnectionChanged;
        event Action<string> LogReceived;
    }
}
