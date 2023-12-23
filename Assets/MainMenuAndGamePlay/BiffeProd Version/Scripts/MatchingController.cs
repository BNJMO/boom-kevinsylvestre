using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingController : MonoBehaviour
{
    public static MatchingController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;    
    }

    public void PrepareRoomMerge()
    {
        if (!RoomsManager.instance.IsNeedMerge || !PhotonNetwork.IsMasterClient)
            return;
        
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        bool pendingRooms = int.Parse(PhotonNetwork.CurrentRoom.Name) > 5000000;

        if ((string)customProperties["Mode"] == "2" && (string)customProperties["IsReady"] == "1")
        {
            foreach (GameObject room in RoomsManager.instance.Newrooms)
            {
                RoomListItem roomListItem = room.GetComponent<RoomListItem>();
                if (!pendingRooms)
                {
                    if (SpecialPendingRoomsSearch(roomListItem.info))
                    {
                        ExitGames.Client.Photon.Hashtable setCustomProperties = new ExitGames.Client.Photon.Hashtable();
                        setCustomProperties["IsAvailable"] = roomListItem.code;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(setCustomProperties);

                        RoomsManager.instance.IsNeedMerge = false;
                    }
                }
                else
                {
                    if ((string)roomListItem.info.CustomProperties["IsAvailable"] == PhotonNetwork.CurrentRoom.Name)
                    {
                        RoomsManager.instance.JoinRoomToOtherRoom(roomListItem.code);
                    }
                }
            }
        }
    }

    public bool IsSuitableRoom(RoomInfo roomInfo, bool IsRandom)
    {
        ExitGames.Client.Photon.Hashtable customProperties = roomInfo.CustomProperties;
        int PlayerCount = roomInfo.PlayerCount;
        bool IsReady = ((string)customProperties["IsReady"] == "1");
        int NumberOfPlayersTeam1 = int.Parse((string)customProperties["NumberOfPlayersTeam1"]);
        
        int PlayerMatchCount = LocalPlayerInfo.PlayerMatchCount;

        if (PlayerCount == roomInfo.MaxPlayers)
            return false;

        // Modes System
        if ((string)customProperties["Mode"] == "2")
        {
            // After Ready in Mode 2 -> Special Teamate Search
            // Mode 2 doesn't able to join random
            if (IsReady == true || IsRandom == true)
            {
                return false;
            }
        }
        else if ((string)customProperties["Mode"] == "1")
        {
            // only join Random after ready, but before ready only join with code
            // so if not ready not random, if ready yes random
            if (IsReady != IsRandom)
            {
                return false;
            }
        }
        else
        {
            // Mode 0 is only random
            if (!IsRandom)
                return false;
        }


        // Mathing System
        if (PlayerCount == NumberOfPlayersTeam1)
        {
            int PlayerMatchCountRange = PlayerMatchCount - int.Parse((string)customProperties["MatchCountPlayer0"]);
            if (Mathf.Abs(PlayerMatchCountRange) > 5)
            {
                return false;
            }
        }
        else
        {
            if (PlayerMatchCount >= int.Parse((string)customProperties["MatchCountPlayer" + (PlayerCount - 1)]))
            {
                return false;
            }

            if (PlayerCount > NumberOfPlayersTeam1)
            {
                if (PlayerMatchCount < int.Parse((string)customProperties["MatchCountPlayer" + (NumberOfPlayersTeam1 - 1).ToString()]) + roomInfo.MaxPlayers - PlayerCount - 1)
                {
                    return false;
                }
            }
        }

        // Charchter Check
        for (int i = 0; i < PlayerCount; i++)
        {
            if (int.Parse((string)customProperties["Charchter" + i]) == LocalPlayerInfo.ChosenCharacter)
            {
                return false;
            }
        }

        return true;
    }

    public bool SpecialPendingRoomsSearch(RoomInfo roomInfo)
    {
        ExitGames.Client.Photon.Hashtable OurCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        ExitGames.Client.Photon.Hashtable OtherCustomProperties = roomInfo.CustomProperties;
        
        if (roomInfo.PlayerCount == roomInfo.MaxPlayers)
            return false;

        if ((string)OtherCustomProperties["Mode"] != "2")
        {
            return false;
        }
        else
        {
            bool pendingRooms = (int.Parse(roomInfo.Name) > 5000000);

            if ((string)OtherCustomProperties["IsReady"] != "1" || !pendingRooms)
            {
                return false;
            }
            else
            {
                if ((string)OtherCustomProperties["IsAvailable"] != "")
                {
                    return false;
                }
            }
        }

        // Nubmer of Players Check
        if (PhotonNetwork.CurrentRoom.PlayerCount != (roomInfo.MaxPlayers - roomInfo.PlayerCount))
        {
            return false;
        }

        // Highest Player Check
        int OurHighestPlayer = int.Parse((string)OurCustomProperties["HighestMatchCount"]);
        int OtherHighestPlayer = int.Parse((string)OtherCustomProperties["HighestMatchCount"]);
        int PlayerMatchCountRange = OurHighestPlayer - OtherHighestPlayer;
        if (Mathf.Abs(PlayerMatchCountRange) > 5)
        {
            return false;
        }

        // Charchter Check
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            int OurPlayer = int.Parse((string)OurCustomProperties["Charchter" + i]);
            for (int j = 0; j < roomInfo.PlayerCount; j++)
            {
                int OtherPlayer = int.Parse((string)OtherCustomProperties["Charchter" + j]);
                if (OurPlayer == OtherPlayer)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
