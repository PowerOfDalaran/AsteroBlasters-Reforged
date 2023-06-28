using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonScript : MonoBehaviour
{
    [SerializeField]
    int colorId;
    Image myImage;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerGameManager.instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        myImage = GetComponent<Image>();
        myImage.color = MultiplayerGameManager.instance.GetPlayerColor(colorId);
    }
}
