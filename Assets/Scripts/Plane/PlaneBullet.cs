using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlaneBullet : MonoBehaviour
{
    float damage = 1f;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlaneController>().TakeDamage(damage);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
