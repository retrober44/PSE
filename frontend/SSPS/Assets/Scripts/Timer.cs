using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private int countDownStartValue = 60;
    public Text timerUI;

    public bool nextTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        CountDownTimer();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void CountDownTimer()
    {
        if (countDownStartValue > 0)
        {
            nextTurn = false;
            timerUI.text = "Timer: " + countDownStartValue;
            Debug.Log("Timer : " + countDownStartValue);
            countDownStartValue--;
            Invoke("CountDownTimer", 1.0f);
        }
        else
        {
            timerUI.text = "Gegner ist am Zug!";
            Debug.Log("Gegner ist am Zug!");
            nextTurn = true;
        }
    }
}