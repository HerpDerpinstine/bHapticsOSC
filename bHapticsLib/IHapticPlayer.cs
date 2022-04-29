using System;
using System.Collections.Generic;

namespace bHapticsLib
{
    public class FeedbackEvent
    {
        public delegate void StatusReceivedEvent(PlayerResponse feedback);
        public delegate void ConnectionEvent(bool isConnected);
    }
    
    public interface IHapticPlayer : IDisposable
    {
        void Enable();
        void Disable();
        
        bool IsActive(PositionType type);

        bool IsPlaying(string key);
        bool IsPlaying();
        
        void Register(string key, Project project );
        void RegisterTactFileStr(string key, string tactFileStr);
        void RegisterTactFileStrReflected(string key, string tactFileStr);
        
        void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis);
        void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis);
        void Submit(string key, PositionType position, DotPoint point, int durationMillis);
        void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis);
        void Submit(string key, PositionType position, PathPoint point, int durationMillis);

        void SubmitRegistered(string key, ScaleOption option);
        void SubmitRegistered(string key, string altKey, ScaleOption option);
        void SubmitRegisteredVestRotation(string key, RotationOption option);
        void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option);
        void SubmitRegisteredVestRotation(string key, string altKey, RotationOption rOption, ScaleOption sOption);

        void SubmitRegistered(string key);
        void SubmitRegistered(string key, float duration);

        void TurnOff(string key);
        void TurnOff();
        
        event Action<PlayerResponse> StatusReceived;
    }
}
