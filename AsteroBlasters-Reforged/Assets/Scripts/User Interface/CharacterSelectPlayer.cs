using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DataStructure;
using NetworkFunctionality;

/// <summary>
/// Class respionsible for managing player images in lobby
/// </summary>
public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] Button kickButton;
    [SerializeField] TextMeshProUGUI playerNameText;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Adding the method to the event and updating the visual
        MultiplayerGameManager.instance.OnPlayerDataNetworkListChanged += CharacterSelect_OnPlayerDataNetworkListChanged;
        UpdatePlayer();
    }

    private void Awake()
    {
        // Assigning values to properties
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Adding functionality to buttons
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            MultiplayerGameManager.instance.KickPlayer(playerData.clientId);
        });
    }

    /// <summary>
    /// Method setting the player visual color to given one
    /// </summary>
    /// <param name="color">Color you want to swap to</param>
    public void SetPlayerColor(Color color)
    {
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Method, which runs every time the player list is being changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CharacterSelect_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    /// <summary>
    /// Method checking if the player image should be visible, changing the color to the proper one and setting Text UI object to player name
    /// </summary>
    private void UpdatePlayer()
    {
        if(MultiplayerGameManager.instance.IsPlayerIndexConnected(playerIndex)) 
        {
            Show();

            PlayerData playerData = MultiplayerGameManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            SetPlayerColor(MultiplayerGameManager.instance.GetPlayerColor(playerData.colorId));
            playerNameText.text = playerData.playerName.ToString();
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

    private void OnDestroy()
    {
        MultiplayerGameManager.instance.OnPlayerDataNetworkListChanged -= CharacterSelect_OnPlayerDataNetworkListChanged;
    }
}
