using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using ExitGames.Client.Photon;
using System;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class RoomsManager : MonoBehaviourPunCallbacks
{
    public static RoomsManager instance;
    public List<GameObject> Newrooms;
    //public List<string> pendingRooms = new List<string>(2);
    public InputField roomCodeInput;
    public PhotonView PV;

    [HideInInspector] public bool IsNeedMerge = true;
    private bool SceneChange = false;

    [HideInInspector] public int PlayerOrder = -1;
    [HideInInspector] public int ChosenCharacter = -1;
    [HideInInspector] public float PlayerMatchCount = -1;
    [HideInInspector] public bool IsRandom;
    [HideInInspector] public bool IsVIP { get; private set; }

    public int[] AllCharchtersNumbers;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        PV = GetComponent<PhotonView>();
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }
    void OnSceneLoaded(Scene scene , LoadSceneMode loadscenemode)
    {
        if(scene.buildIndex == 1)
        {
            GameObject PM = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            PM.GetComponent<PlayerManager>().PlayerOrder = PlayerOrder;
            PM.GetComponent<PlayerManager>().ChosenCharacter = ChosenCharacter;
            if (PhotonNetwork.IsMasterClient)
            {
               var ball = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Ball"), new Vector3(0, 1, 0), Quaternion.identity);
               PM.GetComponent<PlayerManager>().AllCharchtersNumbers = AllCharchtersNumbers;
            }
        }
    }

    private void Update()
    {
        PrepareToStart();

        if (!SceneChange)
        {
            autoDestroyRoom();
        }
    }

    private void autoDestroyRoom()
    {
        foreach (GameObject room in Newrooms)
        {
            if (room == null)
            {
                Newrooms.Remove(room);
            }
        }
    }

    public void UpdateLocalPlayerInfo()
    {
        ChosenCharacter = LocalPlayerInfo.ChosenCharacter;
        PlayerMatchCount = LocalPlayerInfo.PlayerMatchCount;
        IsVIP = Samples.Purchasing.Core.BuyingSubscription.VIPSubscriptions.GetVIPState();
        IsRandom = ConnectionController.instance.IsRandom;
    }
    
    public void joinRoomWithCode_Search(bool IsAlsoMatching, string _roomCode)
    {
        string roomCode;
        if (_roomCode == "get_code_from_looby")
        {
            roomCode = roomCodeInput.text;
        }
        else
        {
            roomCode = _roomCode;
        }

        foreach (GameObject room in Newrooms)
        {
            RoomListItem roomListItem = room.GetComponent<RoomListItem>();
            if (roomListItem.code == roomCode)
            {
                if (IsAlsoMatching)
                {
                    if (MatchingController.instance.IsSuitableRoom(roomListItem.info, false))
                    {
                        ConnectionController.instance.JoinRoom(roomListItem.code, false);
                    }
                }
                else
                {
                    ConnectionController.instance.JoinRoom(roomListItem.code, false);
                }
            }
        }
    }
    public void joinRandomRoom_Matching()
    {
        foreach (GameObject room in Newrooms)
        {
            RoomListItem roomListItem = room.GetComponent<RoomListItem>();
            if (MatchingController.instance.IsSuitableRoom(roomListItem.info, true))
            {
                ConnectionController.instance.JoinRoom(roomListItem.code, true);
                return;
            }
        }
        ConnectionController.instance.CreateRoom(true);
    }

    public void JoinRoomToOtherRoom(string RoomName)
    {
        PV.RPC("JoinRoomToOtherRoomRPC", RpcTarget.All, RoomName);
    }
    [PunRPC]
    void JoinRoomToOtherRoomRPC(string RoomName)
    {
        Luncher.instance.JoinRoomToOtherRoom(RoomName);
    }

    // for teams only
    public void ReadyButton()
    {
        PV.RPC("openWaitingForTeam", RpcTarget.AllBuffered);

        int MaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount + UnityEngine.Random.Range(2, 5);
        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)MaxPlayers;
        
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["IsReady"] = "1";
        customProperties["NumberOfPlayersTeam1"] = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }
    [PunRPC]
    void openWaitingForTeam()
    {
        MenuManager.Instance.OpenMenu("Waiting");
        Camera.main.GetComponent<AudioSource>().mute = true;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!SceneChange)
        {
            if (ConnectionController.instance.IsRandom == false 
                && (string)PhotonNetwork.CurrentRoom.CustomProperties["IsReady"] == "0")
            {
                ConnectionController.instance.LeaveRoom();
            }
        }
    }

    void PrepareToStart()
    {
        if (PhotonNetwork.InRoom && !SceneChange)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MatchingController.instance.PrepareRoomMerge();

                if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    if ((string)PhotonNetwork.CurrentRoom.CustomProperties["Mode"] != "0")
                    {
                        if ((string)PhotonNetwork.CurrentRoom.CustomProperties["IsReady"] == "1")
                        {
                            StartGame();
                        }
                    }
                    else
                    {
                        StartGame();
                    }
                }
            }
        }
    }

    void StartGame()
    {
        ConnectionController.instance.StartGame();
        SceneChange = true;
    }
}
