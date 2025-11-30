using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class SpaceShipController : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject[] flares;
    public Transform targetIndicator;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public Transform[] targetIndicatorTargets;

    float moveSpeed = 5f;
    float maxSpeed = 15f;
    float rotateSpeed = 8000;
    bool wonGame = false;

    Rigidbody2D rb;
    Transform playerCameraTransform;
    TMP_Text timePassedText;
    int currentTarget;
    bool movedUp;
    bool isShooting;
    float x;
    float y;
    float timePassed;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();

        targetIndicatorTargets = SpaceShipModeManager.Instance.targets;

        for (int i = 0; i < targetIndicatorTargets.Length; i++)
        {
            if (i == currentTarget)
            {
                targetIndicatorTargets[i].gameObject.SetActive(true);
            }
            else
            {
                targetIndicatorTargets[i].gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        playerCameraTransform = playerCamera.transform;
        playerCameraTransform.SetParent(null);

        timePassedText = SpaceShipModeManager.Instance.timePassedText;

        if (!PV.IsMine)
        {
            Destroy(playerCamera);
            Destroy(rb);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            SoundManager.Instance.Play("SpaceEngine");
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        playerCameraTransform.rotation = Quaternion.identity;
        playerCameraTransform.position = MakeVector2(transform.position, playerCameraTransform.position);

        x = Input.GetAxis("Horizontal");

        rb.angularVelocity = -rotateSpeed * x * Time.fixedDeltaTime;

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (y < 1)
                y += Time.deltaTime;
            if (y > 1)
                y = 1;

            rb.AddForce(moveSpeed * y * transform.up * Time.deltaTime, ForceMode2D.Impulse);

            movedUp = true;

            ToggleFlares(true);
        }
        else
        {
            movedUp = false;

            ToggleFlares(false);
        }

        if (!movedUp)
        {
            if (y > 0)
                y -= Time.deltaTime;
            if (y < 0)
                y = 0;
        }

        if (!wonGame)
        {
            Vector3 dir = targetIndicatorTargets[currentTarget].position - targetIndicator.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            targetIndicator.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            timePassed += Time.deltaTime;
            timePassedText.text = timePassed.ToString("F2");
        }
    }

    public Vector3 MakeVector2(Vector3 target, Vector3 self)
    {
        return new Vector3(target.x, target.y, self.z);
    }

    public void ToggleFlares(bool state)
    {
        for (int i = 0; i < flares.Length; i++)
        {
            flares[i].SetActive(state);
        }

        if (state)
        {
            SoundManager.Instance.Play("Thruster");
        }
        else
        {
            SoundManager.Instance.Stop("Thruster");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!PV.IsMine)
            return;

        if (other.CompareTag("Target"))
        {
            SoundManager.Instance.Play("Field");

            currentTarget++;

            Instantiate(SpaceShipModeManager.Instance.explosionEffect, other.transform.position, Quaternion.identity);

            for (int i = 0; i < targetIndicatorTargets.Length; i++)
            {
                if (i == currentTarget)
                {
                    targetIndicatorTargets[i].gameObject.SetActive(true);
                }
                else
                {
                    targetIndicatorTargets[i].gameObject.SetActive(false);
                }
            }

            if (currentTarget == targetIndicatorTargets.Length)
            {
                Destroy(targetIndicator.gameObject);
                SpaceShipModeManager.Instance.AddScore(transform, timePassed, PhotonNetwork.NickName);

                wonGame = true;
            }
        }
    }
}
