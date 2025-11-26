using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI statusText;
    public static PhotonConnector Inst;
    private bool _isTryingToMatch;

    private void Awake() => Inst = this;

    internal void ConnectToPhoton()
    {
        statusText.text = "Starting Game";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected → Checking rooms...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Lobby joined → Fetching room list...";
    }

    public override void OnRoomListUpdate(System.Collections.Generic.List<RoomInfo> roomList)
    {
        if (_isTryingToMatch) return;
        _isTryingToMatch = true;

        statusText.text = "Room List received: " + roomList.Count;
        var availableRoom = roomList.FirstOrDefault(r => r.PlayerCount == 1 && !r.RemovedFromList);

        if (availableRoom != null)
        {
            statusText.text = "Found room with 1 player → Joining: " + availableRoom.Name;
            PhotonNetwork.JoinRoom(availableRoom.Name);
            return;
        }

        statusText.text = "No rooms available → Creating new room...";
        PhotonNetwork.CreateRoom("Room_" + Random.Range(1000, 9999), new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            statusText.text = "✔ Both players connected! Start game!";
            GameManager.Inst.noClickPanel.SetActive(true);
            Invoke(nameof(ShowGamePlay), 1f);
            return;
        }

        statusText.text = "Waiting for second player...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        statusText.text = "Another player joined: " + newPlayer.NickName;
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;
        statusText.text = "✔ Both players connected! Start game!";
        GameManager.Inst.noClickPanel.SetActive(true);
        Invoke(nameof(ShowGamePlay), 1f);
    }

    private void ShowGamePlay()
    {
        // Show gameplay UI
        GameManager.Inst.ShowScreen(GeneralDataManager.Screens.GamePlay);
        GameManager.Inst.noClickPanel.SetActive(false);

        // EACH PLAYER creates their OWN deck
        DeckManager.Inst.InitDeck();

        // Draw initial 3 cards for THIS device
        List<CardData> firstThree = new List<CardData>();
        for (int i = 0; i < 3; i++)
        {
            CardData c = DeckManager.Inst.DrawOne(); // CORRECT FUNCTION
            if (c != null) firstThree.Add(c);
        }

        // Fill this player's hand UI
        HandManager.Inst.InitializeInitialHand(firstThree);

        // MasterClient only starts the match (turnStart broadcast)
        if (PhotonNetwork.IsMasterClient)
        {
            TurnManager.Inst.StartMatch();
        }
    }
}