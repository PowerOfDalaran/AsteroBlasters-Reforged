using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGameSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MultiplayerGameManager.instance.StartTheGame();
    }
}
