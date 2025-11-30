using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class SpaceWarShipModeManager : MonoBehaviour
{
    public static SpaceWarShipModeManager Instance;

    public TMP_Text enemyCountText;

    [HideInInspector] public int enemyCount;

    PhotonView PV;
    float elapsedTime;

    void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void UpdateEnemyCount(int count)
    {
        PV.RPC("RPC_UpdateEnemyCount", RpcTarget.All, count);
    }

    [PunRPC]
    public void RPC_UpdateEnemyCount(int count)
    {
        enemyCount += count;
        enemyCountText.text = enemyCount.ToString();

        if (enemyCount <= 10 && elapsedTime >= 10f)
        {
            GameManager.Instance.gameFinished = true;
        }
    }
}
