using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class SinglePlayer : MonoBehaviourPunCallbacks
{
    public Text playerName;
    public Player player;
    public GameObject HoteText;

    public string ChosenCharacter;
    public string PlayerMatchCount;

    private void Start()
    {
        HoteText.SetActive(false);
        Transform par = this.gameObject.transform.parent;
        if(par.GetChild(0) == this.transform)
        {
            HoteText.SetActive(true);
        }
        ConnectionController.instance.UpdateRoomOptions();
    }
    
    public void SetUp(Player _player)
    {
        player = _player;
        playerName.text = _player.NickName;
        SettingPlayerCustomProperties(_player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            if(changedProps.ContainsKey("ChosenCharacter") || changedProps.ContainsKey("PlayerMatchCount"))
                SettingPlayerCustomProperties(targetPlayer);
        }
    }

    public void SettingPlayerCustomProperties(Player _player)
    {
        if (_player.CustomProperties.ContainsKey("ChosenCharacter"))
        {
            ChosenCharacter = (string)_player.CustomProperties["ChosenCharacter"];
        }
        if (_player.CustomProperties.ContainsKey("PlayerMatchCount"))
        {
            PlayerMatchCount = (string)_player.CustomProperties["PlayerMatchCount"];
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
        ConnectionController.instance.UpdateRoomOptions();
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        ConnectionController.instance.UpdateRoomOptions();
    }
}
