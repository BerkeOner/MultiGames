#pragma warning disable 0649

using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    void Awake() { if (Instance != null) { return; } Instance = this; }

    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        base.OnEnable();
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDisable();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode LoadSceneMode)
    {
        if (scene.name == "MainMenu") { }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("Photon", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
