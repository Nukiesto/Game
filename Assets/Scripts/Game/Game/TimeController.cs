using System;
using LeopotamGroup.Math;
using System.Collections;
using EasyButtons;
using Game.Lighting;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public enum Time
    {
        Night,
        Morning,
        Day,
        Evening
    }
    [SerializeField] private AmbientLightController ambientLightController;
    private float _currentTimeDay;
    private int _currentDay = 1;
    private int _globalTime;

    private const int TimeDay = 1440; //Минут в дне

    private void Awake()
    {
        _currentTimeDay = 14f * 60f;
        StartCoroutine(EmulateTime());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("CurrentTime: " + GetHoursString() + ":" + GetMinutesString() + " ;Day: " + GetCurrentDay());
        }
    }

    public string GetTime()
    {
        return "CurrentTime: " + GetHoursString() + ":" + GetMinutesString() + " ;Day: " + GetCurrentDay();
    }
    private IEnumerator EmulateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);//1 игровая минута = 1 реальная секунда
            _currentTimeDay++;
            _globalTime++;
            //Debug.Log(_currentTimeDay / 60);
            ambientLightController.SetTime(_currentTimeDay / 60);
            if (_currentTimeDay > TimeDay)
            {
                _currentDay++;
                _currentTimeDay = 0;
            }
            yield return EmulateTime();
        }
    }

    public void SetTime(float time)
    {
        _currentTimeDay = Mathf.Lerp(0, TimeDay, time);
        ambientLightController.SetTime(_currentTimeDay);
    }
    public void SetTime(Time time)
    {
        switch (time)    
        {
            case Time.Night:
                _currentTimeDay = 0;
                break;
            case Time.Morning:
                _currentTimeDay = 8f*60f;
                break;
            case Time.Day:
                _currentTimeDay = 14*60f;
                break;
            case Time.Evening:
                _currentTimeDay = 18*60;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(time), time, null);
        }
        ambientLightController.SetTime(_currentTimeDay / 60);
    }
    private string GetMinutesString()
    {
        return ReStringTime(GetMinutes());
    }

    private string GetHoursString()
    {
        return ReStringTime(GetHours());
    }

    private static string ReStringTime(int a)
    {
        if (a > 9)
            return a.ToString();
        return "0" + a;
    }

    private string GetCurrentDay()
    {
        return _currentDay.ToString();
    }

    private int GetMinutes()
    {
        var minutes = Mathf.CeilToInt(_currentTimeDay);
        if (minutes <= 60) return minutes;

        var hours = GetHours();
        minutes -= hours * 60;
        
        //Debug.Log(minutes);
        return minutes;
    }

    private int GetHours()
    {
        return Mathf.FloorToInt(f: _currentTimeDay / 60);
    }
}
