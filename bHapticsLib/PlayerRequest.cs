using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bHapticsLib
{
    interface IParsable
    {
        JSONObject ToJsonObject();
    }

    public class PlayerRequest: IParsable
    {

        public List<RegisterRequest> Register;
        public List<SubmitRequest> Submit;

        public static PlayerRequest Create()
        {
            return new PlayerRequest
            {
                Register = new List<RegisterRequest>(),
                Submit = new List<SubmitRequest>()
            };
        }

        public JSONObject ToJsonObject()
        {
            var requestArray = new JSONArray();

            foreach (var registerRequest in Register)
            {
                var obj = new JSONObject();
                obj["Key"]= registerRequest.Key;
                obj["Project"] = registerRequest.Project.ToJsonObject();
                requestArray.Add(obj);
            }
            var array = new JSONArray();
            foreach (var submitRequest in Submit)
            {
                array.Add(submitRequest.ToJsonObject());
            }

            var jsonObject = new JSONObject();
            jsonObject["Register"] = requestArray;
            jsonObject["Submit"]=  array;

            return jsonObject;
        }
    }

    public class RegisterRequest
    {
        public string Key { get; set; }
        public Project Project { get; set; }
    }

    public class SubmitRequest : IParsable
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public Dictionary<string, object> Parameters { get; set; } // durationRatio
        public Frame Frame { get; set; }

        public JSONObject ToJsonObject()
        {
            var jsonObject = new JSONObject();
            jsonObject["type"] = Type;
            jsonObject["key"] = Key;
            if (Parameters != null)
            {
                var paramsValue = new JSONObject();
                foreach (var parameter in Parameters)
                {
                    try
                    {
                        var parameterKey = parameter.Key;
                        var value = parameter.Value;
                        var par = value as IParsable;

                        if (par != null)
                        {
                            paramsValue[parameterKey] = par.ToJsonObject();
                        }
                        else
                        {
                            try
                            {
                                var str = (string)value;
                                paramsValue[parameterKey] = str;
                            }
                            catch
                            {
                                var floatVal = (float)value;
                                paramsValue[parameterKey] = floatVal;
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Params" + e.Message);
                    }

                }
                jsonObject["Parameters"] = paramsValue;
            }


            if (Frame != null)
            {
                jsonObject["Frame"] = Frame.ToJsonObject();
            }


            return jsonObject;
        }
    }

    public class RotationOption: IParsable
    {
        public float OffsetAngleX { get; set; }
        public float OffsetY { get; set; }

        public RotationOption(float offsetAngleX, float offsetY)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }

        public JSONObject ToJsonObject()
        {
            var jsonObject = new JSONObject();
            jsonObject["offsetAngleX"] = OffsetAngleX;
            jsonObject["offsetY"] = OffsetY;

            return jsonObject;
        }
    }

    public class ScaleOption : IParsable
    {
        public float Intensity { get; set; }
        public float Duration { get; set; }

        public ScaleOption(float intensity, float duration)
        {
            Intensity = intensity;
            Duration = duration;
        }

        public JSONObject ToJsonObject()
        {
            var jsonObject = new JSONObject();
            jsonObject["intensity"] = Intensity;
            jsonObject["duration"] = Duration;

            return jsonObject;
        }
    }

    public class PlayerResponse
    {
        public List<string> RegisteredKeys { get; set; }
        public List<string> ActiveKeys { get; set; }
        public int ConnectedDeviceCount { get; set; }
        public List<PositionType> ConnectedPositions { get; set; }
        public Dictionary<string, int[]> Status { get; set; }

        public static PlayerResponse ToObject(string jsonStr)
        {
            var jsonObject = JSON.Parse(jsonStr);
            var obj = new PlayerResponse();

            obj.ConnectedDeviceCount = (int)jsonObject["ConnectedDeviceCount"];


            obj.RegisteredKeys = new List<string>();
            foreach (var jsonValue in jsonObject["RegisteredKeys"].AsArray)
            {
                obj.RegisteredKeys.Add(jsonValue.Value);
            }

            obj.ActiveKeys = new List<string>();
            foreach (var jsonValue in jsonObject["ActiveKeys"].AsArray)
            {
                obj.ActiveKeys.Add(jsonValue.Value);
            }

            obj.ConnectedPositions = new List<PositionType>();
            foreach (var jsonValue in jsonObject["ConnectedPositions"].AsArray)
            {
                obj.ConnectedPositions.Add(EnumParser.ToPositionType(jsonValue.Value));
            }

            obj.Status = new Dictionary<string, int[]>();

            var status = jsonObject[("Status")];
            foreach (var statusKey in status.Keys)
            {
                var arr = status[statusKey];
                var item = new int[arr.Count];
                var i = 0;
                foreach (var jsonValue in arr)
                {
                    item[i] = jsonValue.Value.AsInt;
                    i++;
                }
                obj.Status[statusKey] = item;
            }

            return obj;
        }

    }

    public class Frame
    {
        public int DurationMillis { get; set; }
        public PositionType Position { get; set; }
        public List<PathPoint> PathPoints { get; set; }
        public List<DotPoint> DotPoints { get; set; }

        public JSONObject ToJsonObject()
        {
            var pathPointList = new JSONArray();
            foreach (PathPoint point in PathPoints)
            {
                pathPointList.Add(point.ToJsonObject());
            }

            var dotPointList = new JSONArray();
            foreach (DotPoint point in DotPoints)
            {
                dotPointList.Add(point.ToJsonObject());
            }

            var jsonObject = new JSONObject();
            jsonObject["durationMillis"] = DurationMillis; 
            jsonObject["position"] = Position.ToString(); 
            jsonObject["pathPoints"] = pathPointList; 
            jsonObject["dotPoints"] = dotPointList; 

            return jsonObject;
        }
    }
}
