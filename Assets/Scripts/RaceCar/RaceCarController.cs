using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RaceCarController : MonoBehaviour
{
    public Camera playerCamera;
    Rigidbody2D rb;

    float x;
    float y;
    Transform playerCameraTransform;
    TMP_Text timePassedText;

    float moveSpeed = 200f;
    float maxSpeed = 250f;
    float rotateSpeed = 8000f;
    float speedMultiplier = 1;
    bool wonGame = false;
    float timePassed;
    int currentTarget;
    int currentLap;
    int targetLaps = 3;
    float lastLapTime;

    [HideInInspector] public Transform[] raceTargets;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        playerCameraTransform = playerCamera.transform;
        playerCameraTransform.SetParent(null);

        raceTargets = RaceCarModeManager.Instance.targets;

        for (int i = 0; i < raceTargets.Length; i++)
        {
            if (i == currentTarget)
            {
                raceTargets[i].gameObject.SetActive(true);
            }
            else
            {
                raceTargets[i].gameObject.SetActive(false);
            }
        }

        timePassedText = RaceCarModeManager.Instance.timePassedText;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            currentTarget++;

            for (int i = 0; i < raceTargets.Length; i++)
            {
                if (i == currentTarget)
                {
                    raceTargets[i].gameObject.SetActive(true);
                }
                else
                {
                    raceTargets[i].gameObject.SetActive(false);
                }
            }

            if (currentTarget == raceTargets.Length)
            {
                RaceCarModeManager.Instance.AddLapTimer(timePassed - lastLapTime);
                currentTarget = 0;
                lastLapTime = timePassed;
                currentLap++;
                SoundManager.Instance.Play("Field");

                for (int i = 0; i < raceTargets.Length; i++)
                {
                    if (i == currentTarget)
                    {
                        raceTargets[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        raceTargets[i].gameObject.SetActive(false);
                    }
                }

                if (currentLap == targetLaps)
                {
                    RaceCarModeManager.Instance.AddScore(transform, timePassed, PhotonNetwork.NickName);
                    wonGame = true;
                }
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Road"))
            speedMultiplier = 1;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Road"))
            speedMultiplier = 0.5f;
    }

    void Update()
    {
        playerCameraTransform.rotation = Quaternion.identity;
        playerCameraTransform.position = MakeVector2(transform.position, playerCameraTransform.position);

        x = Input.GetAxis("Horizontal");

        if (x != 0)
            SoundManager.Instance.Play("Thruster");
        else
            SoundManager.Instance.Stop("Thruster");

        if (y > 0)
            rb.angularVelocity = -rotateSpeed * x * y * Time.fixedDeltaTime * speedMultiplier;
        else if (y < 0)
            rb.angularVelocity = rotateSpeed * x * y * Time.fixedDeltaTime * speedMultiplier;

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        if (!wonGame)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (y < 1)
                    y += Time.deltaTime;
                if (y > 1)
                    y = 1;

                rb.AddForce(moveSpeed * y * transform.up * Time.fixedDeltaTime * speedMultiplier, ForceMode2D.Impulse);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (y > -1)
                    y -= Time.deltaTime;
                if (y < -1)
                    y = -1;

                rb.AddForce(moveSpeed * y * transform.up * Time.fixedDeltaTime * speedMultiplier, ForceMode2D.Impulse);
            }
            else
            {
                if (y > 0)
                    y -= Time.deltaTime;
                if (y < 0)
                    y += Time.deltaTime;
                if (y < -1)
                    y = -1;
                if (y > 1)
                    y = 1;

                rb.velocity = rb.velocity.magnitude * transform.up * Time.fixedDeltaTime * speedMultiplier;
            }
        }

        if (wonGame)
        {
            if (y > 0)
                y -= Time.deltaTime;
            if (y < 0)
                y += Time.deltaTime;
            if (y < -1)
                y = -1;
            if (y > 1)
                y = 1;
            if (x > 0)
                x -= Time.deltaTime;
            if (x < 0)
                x += Time.deltaTime;
            if (x < -1)
                x = -1;
            if (x > 1)
                x = 1;
        }

        rb.velocity = moveSpeed * y * rb.transform.up * Time.fixedDeltaTime * speedMultiplier;

        if (!wonGame)
        {
            timePassed += Time.deltaTime;
            timePassedText.text = timePassed.ToString("F2");
        }
    }

    public Vector3 MakeVector2(Vector3 target, Vector3 self)
    {
        return new Vector3(target.x, target.y, self.z);
    }
}
