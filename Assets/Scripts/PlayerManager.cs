using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
            CreateController();
    }

    void CreateController()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "SpaceWar")
        {
            int r = Random.Range(1, 3);

            if (r == 1)
            {
                PhotonNetwork.Instantiate(Path.Combine(sceneName, "LaserShip"), Vector3.zero, Quaternion.identity);
            }
            else if (r == 2)
            {
                PhotonNetwork.Instantiate(Path.Combine(sceneName, "MiniShip"), Vector3.zero, Quaternion.identity);
            }
        }
        else if (sceneName == "SpaceShip")
        {
            PhotonNetwork.Instantiate(Path.Combine(sceneName, "SpaceShip"), Vector3.zero, Quaternion.identity);
        }
        else if (sceneName == "Tank")
        {
            string vehicle = "Tank-" + Random.Range(1, 5).ToString();

            Vector3 randomSpawn = new Vector3(Random.Range(-5, 5), Random.Range(-3, 3), 0);

            PhotonNetwork.Instantiate(Path.Combine(sceneName, vehicle), randomSpawn, Quaternion.identity);
        }
        else if (sceneName == "Plane")
        {
            string vehicle = "Plane-" + Random.Range(1, 5).ToString();

            Vector3 randomSpawn = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);

            PhotonNetwork.Instantiate(Path.Combine(sceneName, vehicle), randomSpawn, Quaternion.identity);
        }
        else if (sceneName == "RaceCar")
        {
            string vehicle = "Car-" + Random.Range(1, 5).ToString();

            PhotonNetwork.Instantiate(Path.Combine(sceneName, vehicle), Vector3.zero, Quaternion.identity);
        }
    }
}
