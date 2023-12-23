using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

public class GameEndController : MonoBehaviourPunCallbacks
{
    public static GameEndController Instance;
    public GameObject EndPanel;
    public GameObject ObjectsPanel;
    public GameObject BluePanel;
    public GameObject RedPanel;
    public GameObject Loading;
    public GameObject WinSgin;
    public GameObject EqualSgin;
    public GameObject LossSgin;
    public Text TrophiesText;
    public Text TimerText;

    public GameObject MainCam;
    public GameObject SecondCam;
    public GameObject TimerCanvas;
    public GameObject MusicSource;


    float StartTimer;
    bool IsExit;
    bool IsResultSet;
    bool IsPlayAgainTeamCountSet = false;
    int MyTeamCountToPlayAgian = 0;

    // Data to get
    bool IsRandom;
    bool IsVIP;
    [HideInInspector] public string TeamColor;

    // Data to send
    int Trophies;
    bool IsPlayAgain;
    public bool IsVirtualDisconnect;
    string PlayAgainTeamCode;

    PhotonView PV;

    private void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
        EndPanel.SetActive(false);
    }
    void Start()
    {
        IsRandom = RoomsManager.instance.IsRandom;
        IsVIP = RoomsManager.instance.IsVIP;

        Trophies = 0;
        IsPlayAgain = false;
        IsVirtualDisconnect = false;
        PlayAgainTeamCode = "";

        MusicSource.SetActive(true);
    }

    void Update()
    {
        if (IsVirtualDisconnect)
            ExitGame();

        if (Timer.IsFinished)
        {
            EndPanel.SetActive(true);
        }

        if (EndPanel.activeInHierarchy && !IsResultSet)
        {
            PostGameResult();

            StartTimer = Time.time;
            SecondCam.SetActive(true);
            MainCam.SetActive(false);
            TimerCanvas.SetActive(false);
            
            IsResultSet = true;
        }

        if (IsPlayAgain && PlayAgainTeamCode != "")
        {
            ExitGame();
        }

        if (StartTimer != 0)
            TimerAfterMatch();
    }

    void PostGameResult()
    {
        MusicSource.SetActive(false);
        ObjectsPanel.SetActive(true);

        if (TeamColor == "Blue")
            BluePanel.SetActive(true);
        else if (TeamColor == "Red")
            RedPanel.SetActive(true);

        string Result = UImanager.instance.GetResult(TeamColor);

        if (Result == "Victory")
        {
            Trophies = 10;
            WinSgin.SetActive(true);
        }
        else if (Result == "Equality")
        {
            Trophies = 6;
            EqualSgin.SetActive(true);
        }
        else if (Result == "Defeat")
        {
            Trophies = 3;
            LossSgin.SetActive(true);
        }

        if (IsVIP)
            Trophies = Trophies * 2;

        TrophiesText.text = "+" + Trophies;


        PlayerPrefs.SetString("IsUpdatePostMatchActions", true.ToString());
        PlayerPrefs.SetString("PreIsRandom", IsRandom.ToString());
        PlayerPrefs.SetInt("Trophies", Trophies);
    }

    void TimerAfterMatch()
    {
        if (Time.time > StartTimer + 10f)
        {
            if (!IsPlayAgainTeamCountSet)
            {
                ReadyTeamToPlayAgain();
                IsPlayAgainTeamCountSet = true;
            }
            PlayAgain();
        }
        else
        {
            TimerText.text = "la partie va commencer dans " + ((int)StartTimer + 10f - (int)Time.time).ToString() + "s";
        }
    }

    public void PlayAgain()
    {
        IsPlayAgain = true;
        if (IsRandom)
        {
            ExitGame();
        }
        else
        {
            if (PlayAgainTeamCode != "")
                return;

            if (MyTeamCountToPlayAgian > 1)
            {
                Loading.SetActive(true);
                SwipeController.Instance.Character.SetActive(false);

                if (PhotonNetwork.IsMasterClient && PlayAgainTeamCode == "")
                {
                    sendCodeToTeam();
                }
            }
            else
            {
                ExitGame();
            }
        }
    }

    void ReadyTeamToPlayAgain()
    {
        int _MyTeamCount = 0;

        GameObject[] PlayerManagers = GameObject.FindGameObjectsWithTag("PlayerManager");
        for (int i = 0; i < PlayerManagers.Length; i++)
        {
            if (PlayerManagers[i].GetComponent<PlayerManager>().PlayerColor == TeamColor)
            {
                _MyTeamCount++;
            }
        }

        MyTeamCountToPlayAgian = _MyTeamCount;
    }

    void sendCodeToTeam()
    {
        PlayAgainTeamCode = UnityEngine.Random.Range(0, 9999999).ToString("0000000");
        PV.RPC("sendCodeToTeamRPC", RpcTarget.All, PlayAgainTeamCode, TeamColor);
        ExitGame();
    }
    [PunRPC]
    void sendCodeToTeamRPC(string code,string _TeamColor)
    {
        if (TeamColor == _TeamColor)
            PlayAgainTeamCode = code;
    }

    public void ExitButton()
    {
        ExitGame();
    }

    public void ExitGame()
    {
        if (!IsExit)
        {
            PhotonNetwork.LeaveRoom();
            Loading.SetActive(true);
            if (!IsVirtualDisconnect)
                SwipeController.Instance.Character.SetActive(false);
            IsExit = true;
        }
    }

    void MoveDataThrowStorge()
    {
        PlayerPrefs.SetString("IsUpdatePostMatchActions", true.ToString());
        PlayerPrefs.SetString("IsPlayAgain", IsPlayAgain.ToString());
        PlayerPrefs.SetString("IsVirtualDisconnect", IsVirtualDisconnect.ToString());
        PlayerPrefs.SetString("PlayAgainTeamCode", PlayAgainTeamCode);
    }

    public override void OnLeftRoom()
    {
        MoveDataThrowStorge();
        Destroy(RoomsManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }
}