using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomWebSocketSharp;
using System.Timers;
using CustomWebSocketSharp.Net;

namespace bHapticsLib
{
    public class WebSocketSender : ISender
    {
        private WebSocket _webSocket;
        private Timer _timer;

        private void TimerOnElapsed(object o, ElapsedEventArgs args)
        {
            if (!_websocketConnected)
            {
                if (!_enable)
                    return;
                //Console.Write("RetryConnect()\n");
                _retryCount++;
                _webSocket.Connect();
                if (_retryCount >= MaxRetryCount)
                    _timer.Stop();
            }
        }

        private void MessageReceived(object o, MessageEventArgs args)
        {
            if (!_enable)
            {
                return;
            }
            var msg = args.Data;

            var response = PlayerResponse.ToObject(msg);
            StatusReceived?.Invoke(response);
        }


        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/v2/feedbacks";
        private string appId;
        private string appName;

        public event Action<PlayerResponse> StatusReceived;
        public event Action<bool> ConnectionChanged;
        public event Action<string> LogReceived;
        
        private int _retryCount = 0;
        private const int MaxRetryCount = 5;
        

        // We need copy when conncted
        private readonly List<RegisterRequest> _registered;
        private bool _enable;

        
        private PlayerRequest _activeRequest;

        public WebSocketSender(string appId, string appName)
        {
            this.appId = HttpUtility.UrlEncode(appId);
            this.appName = HttpUtility.UrlEncode(appName);
            _registered = new List<RegisterRequest>();
        }

        public void Initialize(bool tryReconnect)
        {
            if (_webSocket != null)
            {
                //Console.Write("Initialized\n");
                return;
            }

            if (tryReconnect)
            {
                _timer = new Timer(3 * 1000); // 3 sec
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
            }

            _webSocket = new WebSocket(WebsocketUrl + "?app_id=" + appId + "&app_name=" + appName);

            _webSocket.OnMessage += MessageReceived;
            _webSocket.OnOpen += OnConnected;
            _webSocket.OnError += (sender, args) =>
            {
                Console.Write($"OnError {args.Message }\n");
            };

            _webSocket.OnClose += (sender, args) =>
            {
                //Console.Write("Closed.\n");
                _websocketConnected = false;
                ConnectionChanged?.Invoke(_websocketConnected);
            };

            _webSocket.Connect();
            _enable = true;
        }

        public void Enable()
        {
            _enable = true;
            _timer.Start();
        }

        public void Disable()
        {
            _enable = false;
            _timer.Stop();
        }

        public void Dispose()
        {
            try
            {
                _webSocket.CloseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private PlayerRequest GetActiveRequest()
        {
            if (_activeRequest == null)
                _activeRequest = PlayerRequest.Create();

            return _activeRequest;
        }

        private void OnConnected(object o, EventArgs args)
        {
            _websocketConnected = true;

            AddRegister(_registered);
            ConnectionChanged?.Invoke(_websocketConnected);
        }

        public void TurnOff(string key)
        {
            if (!_enable)
                return;

            var req = new SubmitRequest
            {
                Key = key,
                Type = "turnOff"
            };
            AddSubmit(req);
        }

        public void TurnOff()
        {
            if (!_enable)
                return;

            var req = new SubmitRequest { Type = "turnOffAll" };
            AddSubmit(req);
        }

        public void Register(string key, Project project)
        {
            var req = new RegisterRequest
            {
                Key = key,
                Project = project
            };
            _registered.Add(req);

            AddRegister(req);
        }

        public void SubmitRegistered(string key)
        {
            if (!_enable)
                return;

            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key"
            };
            AddSubmit(submitRequest);
        }

        public void SubmitRegistered(string key, float ratio)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "ratio", ratio}
                }
            };

            AddSubmit(submitRequest);
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "scaleOption", option},
                    { "altKey", altKey}
                }
            };

            AddSubmit(submitRequest);
        }


        public void SubmitRegistered(string key, string altKey, RotationOption option, ScaleOption sOption)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "rotationOption", option},
                    { "scaleOption", sOption},
                    { "altKey", altKey}
                }
            };

            AddSubmit(submitRequest);
        }

        public void Submit(string key, Frame req)
        {
            if (!_enable)
                return;

            var submitRequest = new SubmitRequest
            {
                Frame = req,
                Key = key,
                Type = "frame"
            };
            AddSubmit(submitRequest);
        }

        private void AddSubmit(SubmitRequest submitRequest)
        {
            var request = GetActiveRequest();
            if (request == null)
                return;

            request.Submit.Add(submitRequest);

            Send();
        }

        private void AddRegister(RegisterRequest req)
            => AddRegister(new List<RegisterRequest> { req });

        private void AddRegister(List<RegisterRequest> req)
        {
            var request = GetActiveRequest();
            if (request == null)
                return;

            request.Register.AddRange(req);

            Send();
        }

        private void Send()
        {
            try
            {
                if (!_websocketConnected)
                {
                    //Console.Write("not connected.\n");
                    return;
                }

                var msg = GetActiveRequest().ToJsonObject().ToString();
                Debug.WriteLine("Send() String " + msg);
                _webSocket.SendAsync(msg, b => { _activeRequest = null; });
            }
            catch (Exception e)
            {
                Console.Write($"{e.Message} {e}\n");
            }
        }
    }
}
