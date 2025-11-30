using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class SpaceShipModeManager : MonoBehaviour
{
    public static SpaceShipModeManager Instance;

    public Transform[] targets;

    public GameObject explosionEffect;

    public TMP_Text timePassedText;
    public TMP_Text scoreboardText;

    [HideInInspector] public List<Transform> players = new List<Transform>();
    [HideInInspector] public List<float> playerScores = new List<float>();

    int number = 0;

    Transform tempPlayer;
    PhotonView PV;

    void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    public void AddScore(Transform player, float playerScore, string playerName)
    {
        tempPlayer = player;
        PV.RPC("RPC_AddScore", RpcTarget.All, playerScore, playerName);
    }

    [PunRPC]
    public void RPC_AddScore(float playerScore, string playerName)
    {
        players.Add(tempPlayer);
        playerScores.Add(playerScore);

        number++;

        scoreboardText.text += "\n" + number.ToString() + ") " + playerName + " (" + playerScore.ToString("F2") + ")";

        if (number == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            GameManager.Instance.gameFinished = true;
        }
    }
}
