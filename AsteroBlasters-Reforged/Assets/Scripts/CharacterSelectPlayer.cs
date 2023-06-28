using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class respionsible for managing player images in lobby
/// </summary>
public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        MultiplayerGameManager.instance.OnPlayerDataNetworkListChanged += CharacterSelect_OnPlayerDataNetworkListChanged;
        UpdatePlayer();
    }

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetPlayerColor(Color color)
    {
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Method, which runs eevery time the player list is being changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CharacterSelect_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    /// <summary>
    /// Method checking if the player image should be visible
    /// </summary>
    private void UpdatePlayer()
    {
        if(MultiplayerGameManager.instance.IsPlayerIndexConnected(playerIndex)) 
        {
            Show();

            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            SetPlayerColor(MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// Method showing the player image
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Method hiding the player image
    /// </summary>
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
