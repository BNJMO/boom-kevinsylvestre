using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class UImanager : MonoBehaviour
{
    public static UImanager instance;
    public Text blueTeamScoreDisplay;
    public Text redTeamScoreDisplay;
    [PunRPC]
    public int blueTeamScore = 0;
    [PunRPC]
    public int redTeamScore = 0;
    PhotonView PV;
    public int Score;


    private void Start()
    {
        instance = this;
        PV = GetComponent<PhotonView>();
    }


    public void updateBlueTeamScore()
    {
        scoreValue();
        PV.RPC("updateBlueTeamScoreRPC", RpcTarget.AllBuffered, Score);
    }
    public void updateRedTeamScore()
    {
        scoreValue();
        PV.RPC("updateRedTeamScoreRPC", RpcTarget.AllBuffered, Score);
    }

    [PunRPC]
    public void updateBlueTeamScoreRPC(int _Score)
    {
        blueTeamScore += _Score;
        blueTeamScoreDisplay.text = blueTeamScore.ToString();
    }
    [PunRPC]
    public void updateRedTeamScoreRPC(int _Score)
    {
        redTeamScore += _Score;
        redTeamScoreDisplay.text = redTeamScore.ToString();
    }

    void scoreValue()
    {
        if (Timer.isDouble)
        {
            Score = 2;
        }
        else
        {
            Score = 1;
        }
    }

    public string GetResult(string MyTeamColor)
    {
        string Winner = "";

        if (blueTeamScore == redTeamScore)
        {
            return "Equality";
        }
        else if (blueTeamScore > redTeamScore)
        {
            Winner = "Blue";
        }
        else if (blueTeamScore < redTeamScore)
        {
            Winner = "Red";
        }

        if (Winner == MyTeamColor)
        {
            return "Victory";
        }
        else
        {
            return "Defeat";
        }
    }
}
