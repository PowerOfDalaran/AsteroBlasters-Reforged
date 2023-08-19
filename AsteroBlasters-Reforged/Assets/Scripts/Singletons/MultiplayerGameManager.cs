using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using DataStructure;
using PlayerFunctionality;
using SceneManagment;
using GameManager;

namespace NetworkFunctionality
{
    /// <summary>
    /// Singleton class responsible for managing the multiplayer game and lobby.
    /// </summary>
    public class MultiplayerGameManager : NetworkBehaviour
    {
        public static MultiplayerGameManager instance;
        public GameModeManager gameModeManager;

        [SerializeField] GameObject playerPrefab;
        [SerializeField] List<Color> playerColorList;

        public NetworkList<PlayerNetworkData> playerDataNetworkList;

        public event EventHandler OnPlayerDataNetworkListChanged;

        public bool gameActive = false;

        #region Build-in methods
        private void Awake()
        {
            // Checking if another instance of this class don't exist yet and deleting itself if that is the case
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            // Initializating the list of players data and adding an event
            playerDataNetworkList = new NetworkList<PlayerNetworkData>();
            playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        }
        #endregion

        #region NetworkList Events

        /// <summary>
        /// Event, which triggers every time the list of players data is changed
        /// </summary>
        /// <param name="changeEvent"></param>
        private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerNetworkData> changeEvent)
        {
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method, which adds new PlayerData to the list and assigns their data
        /// </summary>
        /// <param name="clientId">Id of the player</param>
        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                playerDataNetworkList.Add(new PlayerNetworkData
                {
                    clientId = clientId,
                    colorId = GetFirstUnusedColorId(),
                });

                // Triggering the player to make them assign their name
                TriggerSetNameClientRpc(clientId);
            }
        }

        /// <summary>
        /// Method, which activates for host and (disconnecting) client when player disconnects
        /// For host, it removes player with given id from the playerNetworkList.
        /// For client it moves him to the main menu
        /// </summary>
        /// <param name="clientId">Id of player you want to remove</param>
        private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
        {
            // WORK ON IT LATER - the conditions are probably set up wrong, because without the gameActive bool, the method is being run twice
            if (NetworkManager.Singleton.IsHost && clientId != GetCurrentPlayerData().clientId)
            {
                // Removing player from the list
                foreach (PlayerNetworkData playerData in playerDataNetworkList)
                {
                    if (playerData.clientId == clientId)
                    {
                        playerDataNetworkList.Remove(playerData);
                    }
                }
            }
            else if (gameActive)
            {
                // Switching the bool in order to assure, the player won't run the method twice
                gameActive = false;

                // Deleting network connections and moving players to main menu
                UtilitiesToolbox.DeleteNetworkConnections(false, true, false, true);
                LobbyManager.instance.LeaveLobby();
                LevelManager.instance.LoadScene("NetworkMenuScene");
            }

        }
        #endregion

        #region Start The Game
        /// <summary>
        /// Method handling creating the game as the host
        /// </summary>
        public void StartAsHost()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
            NetworkManager.StartHost();
            gameActive = true;
        }

        /// <summary>
        /// Method handling joining the game as client.
        /// </summary>
        public void StartAsClient()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
            NetworkManager.StartClient();
            gameActive = true;
        }

        /// <summary>
        /// Method spawning player character for each player in <c>playerDataNetworkList</c>
        /// </summary>
        public void StartTheGame()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                for (int i = 0; i < playerDataNetworkList.Count; i++)
                {
                    PlayerNetworkData playerData = playerDataNetworkList[i];
                    GameObject newPlayer = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

                    // Assigning proper player index to every play character ON HOST
                    newPlayer.GetComponent<NetworkPlayerController>().playerIndex = playerDataNetworkList.IndexOf(playerData);
                    newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerData.clientId, true);

                    // Assigning proper player index to every play character ON CLIENT
                    newPlayer.GetComponent<NetworkPlayerController>().SetMyIndexClientRpc(playerDataNetworkList.IndexOf(playerData));
                }
            }
        }
        #endregion

        #region Set Player Data
        /// <summary>
        /// Server Rpc method, which sets the player name (of the sending client) in the <c>playerDataNetworkList</c>.
        /// Does not require ownership.
        /// </summary>
        /// <param name="playerName">Name the player want to set</param>
        /// <param name="serverRpcParams">Default parameters of server rpc</param>
        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(FixedString64Bytes playerName, ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerNetworkData playerData = GetPlayerDataFromClientId(clientId);

            playerData.playerName = playerName;
            playerDataNetworkList[playerDataIndex] = playerData;
        }


        /// <summary>
        /// Client Rpc method, which serves as an activator to <c>SetPlayerNameServerRpc</c> method.
        /// Thanks to that, the server can force the player to set their name.
        /// </summary>
        /// <param name="playerId">Id of player you want to trigger</param>
        [ClientRpc]
        private void TriggerSetNameClientRpc(ulong playerId)
        {
            ulong myClientId = NetworkManager.Singleton.LocalClientId;
            if (myClientId == playerId)
            {
                FixedString64Bytes myName = LobbyManager.instance.playerName;
                SetPlayerNameServerRpc(myName);
            }
        }
        #endregion

        #region Get Player Data
        /// <summary>
        /// Method returning the index of the player in <c>playerDataNetworkList</c> array
        /// </summary>
        /// <param name="clientId">Id of the player, whose index you want to access</param>
        /// <returns>Index of player with given id</returns>
        public int GetPlayerIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < playerDataNetworkList.Count; i++)
            {
                if (playerDataNetworkList[i].clientId == clientId)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Method checking if player with given index is connected.
        /// </summary>
        /// <param name="playerIndex">Player index</param>
        /// <returns></returns>
        public bool IsPlayerIndexConnected(int playerIndex)
        {
            return playerIndex < playerDataNetworkList.Count;
        }

        /// <summary>
        /// Method returning the <c>PlayerData</c> object of player with given id
        /// </summary>
        /// <param name="clientId">Id of the player, whose data you want to get</param>
        /// <returns><c>PlayerData</c> object of player with given id</returns>
        public PlayerNetworkData GetPlayerDataFromClientId(ulong clientId)
        {
            foreach (PlayerNetworkData playerData in playerDataNetworkList)
            {
                if (playerData.clientId == clientId)
                {
                    return playerData;
                }
            }
            return default;
        }

        /// <summary>
        /// Method returning the <c>PlayerData</c> object of local player
        /// </summary>
        /// <returns><c>PlayerData</c> object of the local player</returns>
        public PlayerNetworkData GetCurrentPlayerData()
        {
            return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        }

        /// <summary>
        /// Method returning the <c>PlayerData</c> object of player with given index
        /// </summary>
        /// <param name="playerIndex">Index of player, whose data you want to access</param>
        /// <returns>Data of player with given id</returns>
        public PlayerNetworkData GetPlayerDataFromPlayerIndex(int playerIndex)
        {
            return playerDataNetworkList[playerIndex];
        }
        #endregion

        #region Player Color

        /// <summary>
        /// Method returning the color with given index from <c>playerColorList</c>
        /// </summary>
        /// <param name="colorId">Id of color you want to access</param>
        /// <returns>Color with given id</returns>
        public Color GetPlayerColor(int colorId)
        {
            return playerColorList[colorId];
        }

        /// <summary>
        /// Method iterating on every player in the lobby and checking if their color is equal to given one
        /// </summary>
        /// <param name="colorId">Id of color, which availability you want to check</param>
        /// <returns>True if color is available, false if its not</returns>
        private bool IsColorAvailable(int colorId)
        {
            foreach (PlayerNetworkData playerData in playerDataNetworkList)
            {
                if (playerData.colorId == colorId)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Method iterating on list of colors and checking on every one if its available
        /// </summary>
        /// <returns>Id of first available color, -1 if every color is occupied</returns>
        int GetFirstUnusedColorId()
        {
            for (int i = 0; i < playerColorList.Count; i++)
            {
                if (IsColorAvailable(i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Method calling the <c>ChangePlayerColorServerRpc</c> to change the color of player
        /// </summary>
        /// <param name="colorId">Color, which you want to apply to the player</param>
        public void ChangePlayerColor(int colorId)
        {
            ChangePlayerColorServerRpc(colorId);
        }

        /// <summary>
        /// Server Rpc method, which basing on <c>ServerRpcParams</c> defines, which players called the method, checks if given color is available and changes his colorId in <c>playerDataNetworkList</c>
        /// </summary>
        /// <param name="colorId">Id of color we want to change to</param>
        /// <param name="serverRpcParams">Default parameters of Server Rpc, which allows to decide which player want to change their colors</param>
        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
        {
            if (!IsColorAvailable(colorId))
            {
                return;
            }

            int playerDataIndex = GetPlayerIndexFromClientId(serverRpcParams.Receive.SenderClientId);

            PlayerNetworkData playerData = playerDataNetworkList[playerDataIndex];

            playerData.colorId = colorId;
            playerDataNetworkList[playerDataIndex] = playerData;
        }
        #endregion

        #region Remove Player

        /// <summary>
        /// Method called only by clients in order to make host throw them out of the lobby
        /// </summary>
        /// <param name="clientId"></param>
        [ServerRpc(RequireOwnership = false)]
        public void RemoveMeServerRpc(ulong clientId)
        {
            RemovePlayer(clientId);
        }

        /// <summary>
        /// Method called only by the host, to remove player from the lobby.
        /// </summary>
        /// <param name="clientId">Id of the player you want to remove</param>
        public void RemovePlayer(ulong clientId)
        {
            RemoveFromClientClientRpc(clientId);
            RemoveFromServer(clientId);
        }

        /// <summary>
        /// Method disconnecting client with given id from <c>NetworkManager</c> and activating <c>NetworkManager_OnClientDisconnectedCallback</c>
        /// </summary>
        /// <param name="clientId">Id of client you want to disconnect</param>
        public void RemoveFromServer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_OnClientDisconnectedCallback(clientId);
            gameModeManager.NetworkManager_OnClientDisconnectedCallback(clientId);
        }

        /// <summary>
        /// ClientRpc method, which is triggered by the host and activated on every client.
        /// Every client checks if his id is equal the given one, and if yes, then they close their network connections, and go back to the network menu.
        /// </summary>
        /// <param name="clientId">Id of player that have to leave the lobby</param>
        [ClientRpc]
        void RemoveFromClientClientRpc(ulong clientId)
        {
            ulong currentPlayerId = GetCurrentPlayerData().clientId;

            if (clientId == currentPlayerId)
            {
                // Activating NetworkManager_OnClientDisconnectedCallback to properly remove player from the lobby
                NetworkManager_OnClientDisconnectedCallback(clientId);
            }
        }
        #endregion
    }
}