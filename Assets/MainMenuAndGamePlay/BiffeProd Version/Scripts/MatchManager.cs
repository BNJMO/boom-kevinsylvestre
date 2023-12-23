using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class MatchManager : MonoBehaviour
{
    public CharacterController[] myTeamPlayer;
    public CharacterController[] advTeamPlayer;

    private void Start()
    {
        foreach (CharacterController MTP in myTeamPlayer)
        {
            MTP.isMyTeamMate = true;
        }
        foreach (CharacterController ATP in advTeamPlayer)
        {
            ATP.isMyTeamMate = false;
        }
    }
}
