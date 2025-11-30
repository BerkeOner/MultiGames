#pragma warning disable 0618

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    void Awake() { Instance = this; }

    [SerializeField] Transform RoomListContent;
    [SerializeField] GameObject RoomListItemPrefab;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] TMP_Text usernameField;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Button startGameButton;
    [SerializeField] TMP_Text dropdownText1;
    [SerializeField] TMP_Text dropdownText2;
    [SerializeField] TMP_Text dropdownText3;
    [SerializeField] TMP_Text dropdownText4;
    [SerializeField] TMP_Text dropdownText5;

    public void _CONNECT()
    {
        StartCoroutine(CheckConnection());
    }

    IEnumerator CheckConnection()
    {
        MenuManager.Instance.OpenMenu("Loading");

        WWW request = new WWW("google.com");
        float _time = 0;

        while (!request.isDone)
        {
            _time += Time.deltaTime;
            if (_time >= 10 && request.progress <= 0.5f)
                break;
            yield return null;
        }

        if (!request.isDone || !string.IsNullOrEmpty(request.error))
        {
            MenuManager.Instance.OpenMenu("Connect");
            PopUp.Instance.NoConnection();
        }
        else
            PhotonNetwork.ConnectUsingSettings();
    }

    public void _SET_PLAYER_NAME()
    {
        PhotonNetwork.NickName = usernameField.text;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        PhotonNetwork.NickName = "Oyuncu " + Random.Range(100, 1000).ToString("000");
    }

    public void _CREATE_ROOM()
    {
        string _roomName = "Yeni Oda " + Random.Range(100, 1000).ToString("000");
        PhotonNetwork.CreateRoom(_roomName);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform obj in PlayerListContent)
        {
            Destroy(obj.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
    }

    public void _START_GAME()
    {
        GameManager.Instance.SceneLoop(new List<string> {dropdownText1.text, dropdownText2.text, dropdownText3.text, dropdownText4.text, dropdownText5.text});
    }

    public void _LEAVE_ROOM()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform obj in RoomListContent)
        {
            Destroy(obj.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(RoomListItemPrefab, RoomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void _OPEN_SCENE(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void _QUIT_GAME()
    {
        Application.Quit();
    }
}
