using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    public Transform[] Spots = new Transform[8];
    Transform[] ReverseSpots = new Transform[8];
    private int CharacterCount;
    public int[] AllCharchtersNumbers;

    public int PlayerOrder = -1;
    public int ChosenCharacter = -1;
    public string PlayerColor = "";

    PhotonView PV;
    string firstOwnerID;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        firstOwnerID = PV.Owner.UserId;
        CharacterCount = LocalPlayerInfo.GameCharacterCount;

        for (int i = 0; i < Spots.Length; i++)
        {
            ReverseSpots[7 - i] = Spots[i];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        int NumberOfPlayersTeam1 = int.Parse((string)PhotonNetwork.CurrentRoom.CustomProperties["NumberOfPlayersTeam1"]);

        if (NumberOfPlayersTeam1 < 4)
        {
            if ((PlayerOrder + 1) > NumberOfPlayersTeam1)
            {
                PlayerOrder = PlayerOrder + (4 - NumberOfPlayersTeam1);
            }
        }

        // Random order based on room name value 
        if (int.Parse(PhotonNetwork.CurrentRoom.Name) % 2 == 0)
        {
            for (int i = 0; i < Spots.Length; i++)
            {
                Spots[i] = ReverseSpots[i];
            }

            SetPlayerColor(true);
        }
        else
        {
            SetPlayerColor(false);
        }

        

        if (PV.IsMine)
        {
            CreateCharacterController();

            if (PhotonNetwork.IsMasterClient)
                MakingRobots();
        }
    }

    private void Update()
    {
        // Ownership change so we destroy this manualy
        if (firstOwnerID != PV.Owner.UserId)
        {
            Destroy(this.gameObject);
        }
    }

    void SetPlayerColor(bool IsReverse)
    {
        if (PlayerOrder < 4)
        {
            PlayerColor = "Blue";

            if (IsReverse)
                PlayerColor = "Red";
        }
        else
        {
            PlayerColor = "Red";

            if (IsReverse)
                PlayerColor = "Blue";
        }
    }

    private void CreateCharacterController()
    {
        Vector3 Pos = Spots[PlayerOrder].position;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "character" + ChosenCharacter.ToString()), Pos, Quaternion.identity);
        SwipeController.CharacterNumber = ChosenCharacter;
    }

    void MakingRobots()
    {
        int NumberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int NumberOfPlayersTeam1 = int.Parse((string)PhotonNetwork.CurrentRoom.CustomProperties["NumberOfPlayersTeam1"]);
        int NumberOfPlayersTeam2 = NumberOfPlayers - NumberOfPlayersTeam1;
        int NumberOfRobotsTeam1 = 4 - NumberOfPlayersTeam1;
        int NumberOfRobotsTeam2 = 4 - NumberOfPlayersTeam2;
        int NumberOfRobots = NumberOfRobotsTeam1 + NumberOfRobotsTeam2;

        int[] CharactersChosenForRobots = new int[NumberOfRobots];
        bool[] Characters = new bool[CharacterCount];

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Characters[AllCharchtersNumbers[i]] = true;
        }

        for (int i = 0; i < CharactersChosenForRobots.Length; i++)
        {
            CharactersChosenForRobots[i] = Random.Range(0, Characters.Length);
            while (Characters[CharactersChosenForRobots[i]])
            {
                CharactersChosenForRobots[i] = Random.Range(0, Characters.Length);
            }
            Characters[CharactersChosenForRobots[i]] = true;
        }

        for (int i = 0; i < NumberOfRobotsTeam1; i++)
        {
            Vector3 Pos = Spots[NumberOfPlayersTeam1 + i].position;
            GameObject Robot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "character" + CharactersChosenForRobots[i]), Pos, Quaternion.identity);
            Robot.GetComponent<CharacterController>().IsRobot = true;
        }
        for (int i = 0; i < NumberOfRobotsTeam2; i++)
        {
            Vector3 Pos = Spots[7 - i].position;
            int k = NumberOfRobotsTeam1 + i;
            int j = CharactersChosenForRobots[k];
            GameObject Robot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "character" + j), Pos, Quaternion.identity);
            Robot.GetComponent<CharacterController>().IsRobot = true;
        }
    }
}
