using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviourPunCallbacks
{
    public Text timerText;
    PhotonView photonView;
    [PunRPC]
    private float startTime;
    private bool finished = false;
    public static bool isDouble;

    public static bool IsFinished = false;

    private void Awake()
    {
        IsFinished = false;
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = Time.time;
        }
    }

    void Update()
    {
        IsFinished = finished;

        if (finished || !PhotonNetwork.IsMasterClient)
            return;

        // calculate the time that has passed since the timer started
        float t = Time.time - startTime;

        // calculate the minutes and seconds that have passed
        int minutes = ((int)t / 60);
        int seconds = ((int)t % 60);

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdateTimer", RpcTarget.AllBuffered, minutes, seconds);
        }
    }

    [PunRPC]
    void UpdateTimer(int minutes, int seconds)
    {
        timerText.text = string.Format("{0:00}:{1:00}", 3 - minutes, 60 - seconds);
        if (minutes >= 4)
        {
            finished = true;
        }
        if (3 - minutes == 2 && (60 - seconds) >= 0 && (60 - seconds) <= 30 ||
            3 - minutes == 0 && (60 - seconds) >= 0 && (60 - seconds) <= 30)
        {
            timerText.color = Color.red;
            isDouble = true;
        }
        else
        {
            isDouble = false;
            timerText.color = Color.black;
        }
    }
}
