using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdatePlayersInRoom : MonoBehaviour
{
    public Text playersCountDispaly;
    public Transform playersHolder;

    private void Update()
    {
        playersCountDispaly.text = playersHolder.transform.childCount.ToString() + "/4";
    }

}
