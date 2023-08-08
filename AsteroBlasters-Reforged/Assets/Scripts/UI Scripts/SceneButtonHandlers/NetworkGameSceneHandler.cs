using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGameSceneHandler : MonoBehaviour
{
    [SerializeField]
    Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        MultiplayerGameManager.instance.StartTheGame();
    }

    private void Update()
    {
        TimeSpan timerTimeSpan = TimeSpan.FromSeconds(DeathmatchGameManager.instance.timeLeft.Value);

        string minutesLeft = timerTimeSpan.Minutes < 10 ? "0" + timerTimeSpan.Minutes.ToString() : timerTimeSpan.Minutes.ToString();
        string secondsLeft = timerTimeSpan.Seconds < 10 ? "0" + timerTimeSpan.Seconds.ToString() : timerTimeSpan.Seconds.ToString();

        timerText.text = minutesLeft + ":" + secondsLeft;
    }
}
