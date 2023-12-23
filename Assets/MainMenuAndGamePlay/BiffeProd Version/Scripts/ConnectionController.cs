using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionController : MonoBehaviourPunCallbacks
{
    public static ConnectionController instance;

    [SerializeField] Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] InputField roomCodeDisplay;
    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    [HideInInspector] public bool IsRandom = false;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("Main");
        Camera.main.GetComponent<AudioSource>().mute = false;

        InitializeOrUpdatePlayerCustomProperties();
        Luncher.instance.AutoMatchPrepare();
    }
    public void InitializeOrUpdatePlayerCustomProperties()
    {
        LocalPlayerInfo.instance.UpdateValues();

        // Local Update
        RoomsManager.instance.UpdateLocalPlayerInfo();

        PhotonNetwork.NickName = LocalPlayerInfo.PlayerNickName;
        // Player Custom Properties (Public Update)
        ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
        playerCustomProperties["PlayerMatchCount"] = LocalPlayerInfo.PlayerMatchCount.ToString();
        playerCustomProperties["ChosenCharacter"] = LocalPlayerInfo.ChosenCharacter.ToString();
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
    }

    public void CreateRoom(bool IsRandomRoom)
    {
        IsRandom = IsRandomRoom;
        InitializeOrUpdatePlayerCustomProperties();
        
        string roomName = UnityEngine.Random.Range(0, 9999999).ToString("0000000");
        PhotonNetwork.CreateRoom(roomName, InitializeRoomOptions(IsRandomRoom));
        MenuManager.Instance.OpenMenu("Loading");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("Error");
    }

    RoomOptions InitializeRoomOptions(bool _IsRandom)
    {
        RoomOptions roomOptions = new RoomOptions() 
        { 
            IsVisible = true, 
            IsOpen = true, 
            BroadcastPropsChangeToAll = true, 
            CleanupCacheOnLeave = false 
        };
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

        if (_IsRandom)
        {
            // Mode 0 is Random (Solo) only
            customProperties["Mode"] = "0";

            float MaxPlayers = UnityEngine.Random.Range(4, 9);
            float half = MaxPlayers / 2;
            half = (UnityEngine.Random.Range(0, 2) == 0) ? Mathf.Ceil(half) : Mathf.Floor(half);

            roomOptions.MaxPlayers = (byte)MaxPlayers;
            customProperties["NumberOfPlayersTeam1"] = half.ToString();
        }
        else
        {
            // Mode 1 is Teamate with Random
            // Mode 2 is Teamate with other Teamate only
            int Mode = UnityEngine.Random.Range(1, 3);
            customProperties["Mode"] = Mode.ToString();

            roomOptions.MaxPlayers = 4;
            customProperties["NumberOfPlayersTeam1"] = "0"; // change later when raedy
        }

        customProperties["IsReady"] = "0";
        customProperties["IsAvailable"] = "";               // only for 2 Teamate matching
        customProperties["HighestMatchCount"] = "0";        // only for 2 Teamate matching
        customProperties["MatchCountPlayer0"] = "0";
        customProperties["MatchCountPlayer1"] = "0";
        customProperties["MatchCountPlayer2"] = "0";
        customProperties["MatchCountPlayer3"] = "0";
        customProperties["MatchCountPlayer4"] = "0";
        customProperties["MatchCountPlayer5"] = "0";
        customProperties["MatchCountPlayer6"] = "0";
        customProperties["Charchter0"] = "";
        customProperties["Charchter1"] = "";
        customProperties["Charchter2"] = "";
        customProperties["Charchter3"] = "";
        customProperties["Charchter4"] = "";
        customProperties["Charchter5"] = "";
        customProperties["Charchter6"] = "";

        string[] customPropertiesNames = new string[19]
        {
            "Mode",
            "IsReady",
            "NumberOfPlayersTeam1",
            "IsAvailable",
            "HighestMatchCount",
            "MatchCountPlayer0",
            "MatchCountPlayer1",
            "MatchCountPlayer2",
            "MatchCountPlayer3",
            "MatchCountPlayer4",
            "MatchCountPlayer5",
            "MatchCountPlayer6",
            "Charchter0",
            "Charchter1",
            "Charchter2",
            "Charchter3",
            "Charchter4",
            "Charchter5",
            "Charchter6"
        };

        roomOptions.CustomRoomProperties = customProperties;
        roomOptions.CustomRoomPropertiesForLobby = customPropertiesNames;

        return roomOptions;
    }
    public void UpdateRoomOptions()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount == 8)
            {
                return;
            }
            
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

            Transform par = playerListContent;
            if (par != null)
            {
                int HighestMatchCount = -1;
                for (int i = 0; i < par.childCount; i++)
                {
                    SinglePlayer singlePlayer = par.GetChild(i).GetComponent<SinglePlayer>();
                    customProperties["Charchter" + i] = singlePlayer.ChosenCharacter;
                    customProperties["MatchCountPlayer" + i] = singlePlayer.PlayerMatchCount;

                    if (HighestMatchCount < int.Parse(singlePlayer.PlayerMatchCount))
                    {
                        HighestMatchCount = int.Parse(singlePlayer.PlayerMatchCount);
                    }
                }
                customProperties["HighestMatchCount"] = HighestMatchCount;
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
            }
        }
    }
    public void UpdateRoomOptions(out int[] charchtersNumbers)
    {
        charchtersNumbers = new int[0];
        if (PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount == 8)
            {
                return;
            }
            
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

            Transform par = playerListContent;
            int[] CharchtersNumbers = new int[par.childCount];
            if (par != null)
            {
                int HighestMatchCount = -1;
                for (int i = 0; i < par.childCount; i++)
                {
                    SinglePlayer singlePlayer = par.GetChild(i).GetComponent<SinglePlayer>();
                    customProperties["Charchter" + i] = singlePlayer.ChosenCharacter;
                    customProperties["MatchCountPlayer" + i] = singlePlayer.PlayerMatchCount;

                    if (HighestMatchCount < int.Parse(singlePlayer.PlayerMatchCount))
                    {
                        HighestMatchCount = int.Parse(singlePlayer.PlayerMatchCount);
                    }

                    CharchtersNumbers[i] = int.Parse(singlePlayer.ChosenCharacter);
                }
                customProperties["HighestMatchCount"] = HighestMatchCount;
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

                charchtersNumbers = CharchtersNumbers;
            }
        }
    }

    public void JoinRoom(string name, bool IsRandomRoom)
    {
        IsRandom = IsRandomRoom;
        InitializeOrUpdatePlayerCustomProperties();

        PhotonNetwork.JoinRoom(name);
        MenuManager.Instance.OpenMenu("Loading");
    }
    public void JoinRandomOrCreateRoom(bool IsRandomRoom, string _roomName)
    {
        IsRandom = IsRandomRoom;
        InitializeOrUpdatePlayerCustomProperties();

        PhotonNetwork.JoinRandomOrCreateRoom(roomName: _roomName);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        if (IsRandom)
        {
            MenuManager.Instance.OpenMenu("Waiting");
            Camera.main.GetComponent<AudioSource>().mute = true;
        }
        else
        {
            MenuManager.Instance.OpenMenu("Room");
        }

        Player[] players = PhotonNetwork.PlayerList;
        roomCodeDisplay.text = PhotonNetwork.CurrentRoom.Name.Substring(0, 7);

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<SinglePlayer>().SetUp(players[i]);
        }

        ConnectionInfoToPlayer();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<SinglePlayer>().SetUp(newPlayer);
        ConnectionInfoToPlayer();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ConnectionInfoToPlayer();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        UpdateCachedRoomList(roomList);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            GameObject clone = Instantiate(roomListItemPrefab, roomListContent);
            clone.GetComponent<RoomListItem>().SetUp(roomList[i]);
            string Rcode = roomList[i].Name.Substring(0, 7);
            clone.GetComponent<RoomListItem>().code = Rcode;
            RoomsManager.instance.Newrooms.Add(clone);

        }

    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }
    
    public void StartGame()
    {
        ConnectionInfoToPlayer();
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    void ConnectionInfoToPlayer()
    {
        if (!PhotonNetwork.InRoom)
            return;

        Transform par = playerListContent;
        if (par != null)
        {
            for (int i = 0; i < par.childCount; i++)
            {
                if (PhotonNetwork.LocalPlayer == par.GetChild(i).GetComponent<SinglePlayer>().player)
                {
                    RoomsManager.instance.PlayerOrder = i;
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            int[] CharchtersNumbers;
            UpdateRoomOptions(out CharchtersNumbers);
            RoomsManager.instance.AllCharchtersNumbers = CharchtersNumbers;
        }
    }
}
