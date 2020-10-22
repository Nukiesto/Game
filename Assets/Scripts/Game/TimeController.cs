using LeopotamGroup.Math;
using System.Collections;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private AmbientLightController _ambientLightController;
    private int _currentTimeDay;
    private int _currentDay = 1;
    private int _globalTime;
    
    private int timeDay = 1440;//Минут в дне

    private void Start()
    {
        StartCoroutine(EmulateTime());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("CurrentTime: " + GetHoursString() + ":" + GetMinutesString() + " ;Day: " + GetCurrentDay());
        }
    }

    private IEnumerator EmulateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);//1 игровая минута = 1 реальная секунда
            _currentTimeDay++;
            _globalTime++;
            if (_currentTimeDay > timeDay)
            {
                _currentDay++;
                _currentTimeDay = 0;
            }
            yield return EmulateTime();
        }
    }
    public string GetMinutesString()
    {
        return ReString(GetMinutes());
    }
    public string GetHoursString()
    {
        return ReString(GetHours());
    }

    private string ReString(int a)
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
        return (_currentTimeDay - Mathf.CeilToInt(_currentTimeDay / 60) * 60);
    }

    private int GetHours(string a = "")
    {
        return Mathf.CeilToInt(f: _currentTimeDay / 60);
    }
}
