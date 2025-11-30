using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlaneModeManager : MonoBehaviour
{
    public static PlaneModeManager Instance;

    PhotonView PV;

    bool gameStarted = false;

    [HideInInspector] public int playerCount;

    void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    public void UpdatePlayerCount(int count)
    {
        PV.RPC("RPC_UpdatePlayerCount", RpcTarget.All, count);
    }

    [PunRPC]
    public void RPC_UpdatePlayerCount(int count)
    {
        playerCount++;

        if (playerCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            gameStarted = true;
        }

        if (gameStarted && playerCount == 1)
        {
            GameManager.Instance.gameFinished = true;
        }
    }
}
