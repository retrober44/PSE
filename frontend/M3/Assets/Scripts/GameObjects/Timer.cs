using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text counter;
    private float _timer;
    private bool _timerOver;
    [SerializeField] float time;


    void Start()
    {
        time = 60;
        counter = GetComponentInChildren<Text>();
        _timerOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerOver) return;
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        if (_timer < 0)
        {
            _timer = 0;
        }
        if (_timer == 0 && !_timerOver)
        {
            _timerOver = true;
            EventManager.TimerOver.Invoke();
        }
        counter.text = Math.Floor(_timer).ToString();
    }

    public void StopTimer()
    {
        _timerOver = true;
        _timer = 0;
    }
    public void StartTimer()
    {
        _timerOver = false;
        _timer = time;
    }
    public float GetTime()
    {
        return _timer;
    }
}
