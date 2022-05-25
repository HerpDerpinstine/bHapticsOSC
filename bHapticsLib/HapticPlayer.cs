using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bHapticsLib
{
    public class HapticPlayer : IHapticPlayer
    {
        private readonly WebSocketSender _sender;
        private readonly List<string> _activeKeys = new List<string>();
        private readonly List<PositionType> _activePosition = new List<PositionType>();
        public event Action<PlayerResponse> StatusReceived;

        public HapticPlayer(string appId, string appName, bool tryReconnect = true) : this(appId, appName, null, tryReconnect) { }
        public HapticPlayer(string appId, string appName,  Action<bool> connectionChanged, bool tryReconnect = true)
        {
            _sender = new WebSocketSender(appId, appName);
            _sender.StatusReceived += feedback =>
            {
                StatusReceived?.Invoke(feedback);

                lock (_activeKeys)
                {
                    _activeKeys.Clear();
                    _activeKeys.AddRange(feedback.ActiveKeys);
                }

                lock (_activePosition)
                {
                    _activePosition.Clear();
                    _activePosition.AddRange(feedback.ConnectedPositions);
                }
            };
            _sender.ConnectionChanged += (isConn) =>
            {
                connectionChanged?.Invoke(isConn);
            };
            _sender.Initialize(tryReconnect);
        }

        public void Dispose()
        {
            _sender.TurnOff();
            _sender.Dispose();
        }

        public void Enable()
            => _sender.Enable();

        public void Disable()
            => _sender.Disable();

        public bool IsActive(PositionType type)
        {
            lock (_activePosition)
                return _activePosition.Contains(type);
        }

        public bool IsPlaying(string key)
        {
            lock (_activeKeys)
                return _activeKeys.Contains(key);
        }

        public bool IsPlaying()
            => _activeKeys.Count > 0;

        public void Register(string key, Project project)
            => _sender.Register(key, project);


        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            var file = CommonUtils.ConvertJsonStringToTactosyFile(tactFileStr);
            Register(key, file.Project);
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            // not supported
        }

        public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            var points = new List<DotPoint>();
            for (int i = 0; i < motorBytes.Length; i++)
                if (motorBytes[i] > 0)
                    points.Add(new DotPoint(i, motorBytes[i]));

            Submit(key, position, points, durationMillis);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            var frame = new Frame();
            frame.DurationMillis = durationMillis;
            frame.Position = position;
            frame.DotPoints = points;
            frame.PathPoints = new List<PathPoint>();
            _sender.Submit(key, frame);
        }

        public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
            => Submit(key, position, new List<DotPoint> { point }, durationMillis);

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            var frame = new Frame();
            frame.DurationMillis = durationMillis;
            frame.Position = position;
            frame.DotPoints = new List<DotPoint>();
            frame.PathPoints = points;
            _sender.Submit(key, frame);
        }

        public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
            => Submit(key, position, new List<PathPoint> { point }, durationMillis);

        public void SubmitRegistered(string key, ScaleOption option)
            => SubmitRegistered(key, key, option);

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            if (option == null)
                return;

            if (option.Duration < 0.01f || option.Duration > 100f)
            {
                Debug.WriteLine("not allowed duration " + option.Duration);
                return;
            }

            if (option.Intensity < 0.01f || option.Intensity > 100f)
            {
                Debug.WriteLine("not allowed intensity " + option.Intensity);
                return;
            }

            _sender.SubmitRegistered(key, altKey, option);
        }

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option)
            => SubmitRegisteredVestRotation(key, altKey, option, new ScaleOption(1f, 1f));

        public void SubmitRegisteredVestRotation(string key, RotationOption option)
            => SubmitRegisteredVestRotation(key, key, option);

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option, ScaleOption sOption)
            => _sender.SubmitRegistered(key, altKey, option, sOption);

        public void SubmitRegistered(string key)
            => _sender.SubmitRegistered(key);

        public void SubmitRegistered(string key, float ratio)
        {
            if (ratio < 0 || ratio > 1)
            {
                Debug.WriteLine("ratio should be between [0, 1]");
                return;
            }
            _sender.SubmitRegistered(key, ratio);
        }

        public void TurnOff(string key)
            => _sender.TurnOff(key);

        public void TurnOff()
            => _sender.TurnOff();
    }
}
