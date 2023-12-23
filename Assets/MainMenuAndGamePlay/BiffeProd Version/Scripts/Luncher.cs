using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class Luncher : MonoBehaviourPunCallbacks
{
	public static Luncher instance;
	[SerializeField] GameObject readyBtn;
	public GameObject Creeuneequipe;
	public GameObject RoomPanal;
	string OtherRoomName;

	// After match data
	private bool PrevIsRandom;
	private int Trophies;
	private bool IsPlayAgain;
	private bool IsVirtualDisconnect;
	private string PlayAgainTeamCode;

	public GameObject DisconnectPanel;
	public GameObject OnlyOneRemainPanel;

	void Awake()
	{
		instance = this;
		OtherRoomName = "";
	}
    private void Update()
    {
        if (!readyBtn.activeInHierarchy)
        {
			if (!PhotonNetwork.InRoom)
				return;

			if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
				readyBtn.SetActive(PhotonNetwork.IsMasterClient);
		}
    }
    
	public void AutoMatchPrepare()
    {
		if (PlayerPrefs.HasKey("IsUpdatePostMatchActions"))
		{
			if (bool.Parse(PlayerPrefs.GetString("IsUpdatePostMatchActions")))
			{
				PostMatchActions();
				PlayerPrefs.SetString("IsUpdatePostMatchActions", false.ToString());
			}
		}

		// Room merging
		if (OtherRoomName != "")
		{
			ConnectionController.instance.JoinRoom(OtherRoomName, false);
			OtherRoomName = "";
		}
	}

	// Room merging
	public void JoinRoomToOtherRoom(string name)
	{
		OtherRoomName = name;
		ConnectionController.instance.LeaveRoom();
	}

	void PostMatchActions()
    {
		Creeuneequipe.SetActive(false);
		RoomPanal.SetActive(false);

		// Basic info
		PrevIsRandom = bool.Parse(PlayerPrefs.GetString("PreIsRandom"));
		Trophies = PlayerPrefs.GetInt("Trophies");

		// Disconnecting
		IsVirtualDisconnect = bool.Parse(PlayerPrefs.GetString("IsVirtualDisconnect"));

		// Play again
		IsPlayAgain = bool.Parse(PlayerPrefs.GetString("IsPlayAgain"));
		PlayAgainTeamCode = PlayerPrefs.GetString("PlayAgainTeamCode");

		if (IsVirtualDisconnect)
        {
			DisconnectPanel.SetActive(true);
			return;
		}

		if (Trophies > 0)
        {
			LocalPlayerInfo.AddResult((int)Trophies);
		}

        if (IsPlayAgain)
        {
			if (PrevIsRandom)
			{
				joinRandomRoomWithButton();

			}
			else
            {
				if (PlayAgainTeamCode == "")
                {
					OnlyOneRemainPanel.SetActive(true);
                }
                else
                {
					Creeuneequipe.SetActive(true);
					RoomPanal.SetActive(true);

					ConnectionController.instance.JoinRandomOrCreateRoom(false, PlayAgainTeamCode);
				}
			}
		}
    }

	public void CreateRoomButton()
    {
		ConnectionController.instance.CreateRoom(false);
    }
	public void joinRoomWithCodeButton()
    {
		RoomsManager.instance.joinRoomWithCode_Search(true, "get_code_from_looby");
    }
	public void LeaveRoomButton()
	{
		ConnectionController.instance.LeaveRoom();
	}
	public void joinRandomRoomWithButton()
    {
		// To make matching work for random teams
		Creeuneequipe.SetActive(true);
		RoomPanal.SetActive(true);

		RoomsManager.instance.joinRandomRoom_Matching();
    }
	public override void OnLeftRoom()
	{
        if (ConnectionController.instance.IsRandom)
        {
			Creeuneequipe.SetActive(false);
			RoomPanal.SetActive(false);
		}

		Camera.main.GetComponent<AudioSource>().mute = false;
	}
}