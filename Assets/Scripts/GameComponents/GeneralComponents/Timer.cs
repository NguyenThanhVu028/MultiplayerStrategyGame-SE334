using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private TextMeshProUGUI displayer;
    [SerializeField] private int initialMinute;
    [SerializeField] private int initialSecond;
    [SerializeField] private bool isCountDown;
    [SerializeField] private bool isRunning = false; //Timer cannot send event if not running
    [Header("Events")]
    [SerializeField] UnityEvent targetReachedEvent;

    [SerializeField] float counter = 0;
    float targetTime = 0;
    
    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            if (isCountDown)
            {
                counter -= Time.deltaTime;
                if (counter <= targetTime)
                {
                    targetReachedEvent.Invoke(); isRunning = false;
                    counter = targetTime;
                }
            }
            else
            {
                counter += Time.deltaTime;
                if (counter >= targetTime)
                {
                    targetReachedEvent.Invoke(); isRunning = false;
                    counter = targetTime;
                }
            }
            if (counter < 0) counter = 0;
        }

        if (displayer == null) return;
        int second, minute;
        ConvertTimeFromFloat(counter, out minute, out second);
        displayer.text = string.Format("{0:00}:{1:00}", minute, second);
    }
    public bool IsRunning() { return isRunning; }
    public void StartTimer() { isRunning = true;/* isFreezing = false;*/ }
    public void StopTimer() { isRunning = false; }
    public void SetTimer(int min, int second)
    {
        counter = min * 60 + second;
    }
    public void SetTimer(float time)
    {
        counter = time;
    }
    public void GetTime(out int min, out int second)
    {
        ConvertTimeFromFloat(counter, out min,out second);
    }
    public float GetTime() {  return counter; }
    public void ResetTimer()
    {
        counter = initialMinute * 60 + initialSecond;
    }
    public bool IsTargetReached()
    {
        return (isCountDown)? (counter <= targetTime) : (counter >= targetTime);
    }

    void ConvertTimeFromFloat(float time, out int minute, out int second)
    {
        second = Mathf.FloorToInt(time % 60);
        minute = Mathf.FloorToInt(time / 60);
    }
}
