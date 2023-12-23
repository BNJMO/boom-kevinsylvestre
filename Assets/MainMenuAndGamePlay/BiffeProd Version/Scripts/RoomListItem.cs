using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
	public string code;
	public RoomInfo info;
    public void SetUp(RoomInfo _info)
	{
		info = _info;
	}
}