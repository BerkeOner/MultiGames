using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class TankBullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            TankModeManager.Instance.UpdatePlayerCount(-1);

            PhotonNetwork.Instantiate(Path.Combine("Tank", "Destroyed"), other.transform.position, other.transform.rotation);
            PhotonNetwork.Destroy(other.gameObject);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
