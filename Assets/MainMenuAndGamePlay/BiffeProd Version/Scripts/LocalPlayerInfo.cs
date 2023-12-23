using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerInfo : MonoBehaviour
{
    public static LocalPlayerInfo instance;
    public static int GameCharacterCount = 15;

    public static string PlayerNickName { get; private set; }
    public static int Trophies { get; private set; }
    public static int PlayerMatchCount { get; private set; }
    public static int ChosenCharacter;

    private void Awake()
    {
        instance = this;
        UpdateValues();
    }

    public void UpdateValues()
    {
        PlayerNickName = PlayerPrefs.GetString("PlayerNickNameKey", "Player 0000");
        Trophies = PlayerPrefs.GetInt("TrophiesKey", 0);
        PlayerMatchCount = PlayerPrefs.GetInt("PlayerMatchCountKey", 0);
        ChosenCharacter = PlayerPrefs.GetInt("ChosenCharacterKey", 0);
    }

    public static void AddResult(int _Trophies)
    {
        Trophies = PlayerPrefs.GetInt("TrophiesKey", 0);
        PlayerMatchCount = PlayerPrefs.GetInt("PlayerMatchCountKey", 0);

        Trophies = Trophies + _Trophies;
        PlayerMatchCount++;

        PlayerPrefs.SetInt("TrophiesKey", Trophies);
        PlayerPrefs.SetInt("PlayerMatchCountKey", PlayerMatchCount);
    }
}
