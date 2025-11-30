using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpaceWarShipBullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public bool ownerLaser = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemySpaceWarShip>().TakeDamage(damage);
        }
        else if (other.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<SpaceWarShipController>().TakeDamage(damage);
        }

        if (!ownerLaser)
            PhotonNetwork.Destroy(gameObject);
    }
}
