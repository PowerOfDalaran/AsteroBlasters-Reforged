using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeathmatchGameManager : NetworkBehaviour
{
    public static DeathmatchGameManager instance;
    public NetworkList<int> playersKillCount = new NetworkList<int>();
    public NetworkVariable<float> timeLeft = new NetworkVariable<float>();

    private void Start()
    {
        instance = this;
        if (IsHost)
        {
            foreach (PlayerData playerData in MultiplayerGameManager.instance.playerDataNetworkList) 
            {
                playersKillCount.Add(0);
            }

            timeLeft.Value = 100f;
        }
    }

    [ServerRpc]
    public void AddKillCountServerRpc(int playerIndex)
    {
        playersKillCount[playerIndex] += 1;

        if (playersKillCount[playerIndex] == 1)
        {
            EndGame();
        }
    }

    private void FixedUpdate()
    {
        if (IsHost)
        {
            timeLeft.Value -= Time.deltaTime;

            if (timeLeft.Value <= 0)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        Debug.Log("We are in engame now!");
    }
}
