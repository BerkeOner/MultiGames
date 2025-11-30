using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    PhotonView PV;

    [HideInInspector] public bool gameFinished = false;

    void Awake()
    {
        if (Instance != null) { return; } Instance = this;
        PV = GetComponent<PhotonView>();
    }

    public void SceneLoop(List<string> sceneOrder)
    {
        List<string> translatedList = new List<string>();

        for (int i = 0; i < sceneOrder.Count; i++)
        {
            if (sceneOrder[i] == "Uçak Savaşı")
                translatedList.Add("Plane");
            else if (sceneOrder[i] == "Uzayda Savaş")
                translatedList.Add("SpaceWar");
            else if (sceneOrder[i] == "Uzay Yarışı")
                translatedList.Add("SpaceShip");
            else if (sceneOrder[i] == "Yarış")
                translatedList.Add("RaceCar");
            else if (sceneOrder[i] == "Tank")
                translatedList.Add("Tank");
        }

        StartCoroutine(Test(translatedList));
    }

    IEnumerator Test(List<string> sceneOrder)
    {
        for (int i = 0; i < sceneOrder.Count; i++)
        {
            PhotonNetwork.LoadLevel(sceneOrder[i]);

            yield return new WaitUntil(() => gameFinished == true);

            gameFinished = false;

            yield return new WaitForSeconds(5);
        }

        PV.RPC("RPC_KickEveryone", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_KickEveryone()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
        Destroy(DontDestroy.Instance.gameObject);
    }
}
