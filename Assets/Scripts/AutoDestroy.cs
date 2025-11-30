using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AutoDestroy : MonoBehaviour
{
    public float delay;
    public bool useNetwork = true;

    PhotonView PV;

    void Awake()
    {
        if (useNetwork)
            PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        StartCoroutine(LateDestroy());
    }

    IEnumerator LateDestroy()
    {
        yield return new WaitForSeconds(delay);

        if (useNetwork && PV.IsMine)
            PhotonNetwork.Destroy(gameObject);
        else if (!useNetwork)
            Destroy(gameObject);
    }
}
