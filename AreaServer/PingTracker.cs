using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PingTracker
{
    public static Dictionary<int, Pinger> trackers = new Dictionary<int, Pinger>();
    public static void NewPing(int _client)
    {
        if (!trackers.ContainsKey(_client))
        {
            trackers.Add(_client, new Pinger());
        }
    }
    public static void StartTime(int _client, float _time)
    {
        trackers[_client].startTime = _time;
    }

    public static void EndTime(int _client, float _endTime)
    {
        trackers[_client].endTime = _endTime;
    }

    public static float GetTrackerTime(int _client)
    {
        if (trackers.ContainsKey(_client))
        {
            return trackers[_client].endTime - trackers[_client].startTime;
        }
        else
        {
            return -1f;
        }
    }

    public static float GetAverage()
    {
        float _average = 0;
       foreach(var pinger in trackers)
        {
            Pinger p = pinger.Value;
            float _time = p.endTime - p.startTime;
            if(_time > 0)
            {
                _average += _time;
            }
        }
        return _average/trackers.Count;
    }

    


}

public class Pinger
{
    public float startTime;
    public float endTime;
    float ping;
    public Pinger(float _startTime)
    {
        startTime = _startTime;
    }
    public Pinger()
    {

    }
    
}

