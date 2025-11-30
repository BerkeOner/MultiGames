using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlaneController : MonoBehaviour
{
    public Transform[] firePoints;
    public Camera playerCamera;

    float moveSpeed = 150;
    float rotateSpeed = 5000;
    int bulletSpeed = 30;
    int maxHealth = 4;
    float fireRate = 1;

    [HideInInspector] public float currentHealth;

    float x;
    float fireRate_;

    Rigidbody2D rb;
    Transform playerCameraTransform;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        playerCameraTransform = playerCamera.transform;
        playerCameraTransform.SetParent(null);

        if (!PV.IsMine)
        {
            Destroy(rb);
            Destroy(playerCamera.gameObject);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            SoundManager.Instance.Play("SpaceEngine");
            PlaneModeManager.Instance.UpdatePlayerCount(1);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetKey(KeyCode.Space) && Time.time >= fireRate_)
        {
            fireRate_ = Time.time + 1 / fireRate;
            Shoot();
        }

        playerCameraTransform.rotation = Quaternion.identity;
        playerCameraTransform.position = MakeVector2(transform.position, playerCameraTransform.position);

        x = Input.GetAxis("Horizontal");

        rb.angularVelocity = -rotateSpeed * x * Time.fixedDeltaTime;
        rb.velocity = moveSpeed * transform.up * Time.fixedDeltaTime;
    }

    public Vector3 MakeVector2(Vector3 target, Vector3 self)
    {
        return new Vector3(target.x, target.y, self.z);
    }

    public void Shoot()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform[] FP = firePoints;

            GameObject bulletPrefab = PhotonNetwork.Instantiate(Path.Combine("Plane", "PlaneBullet"), FP[i].position, FP[i].rotation);
            bulletPrefab.GetComponent<Rigidbody2D>().AddForce(bulletPrefab.transform.up * bulletSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletPrefab.GetComponent<Collider2D>());
        }
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0 || !PV.IsMine) // Already Dead
            return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            if (PV.IsMine)
                PlaneModeManager.Instance.UpdatePlayerCount(-1);

            PhotonNetwork.Destroy(playerCameraTransform.gameObject);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}


















/*
    AKICI HAREKET KODU
        Tuştan elini çekince ve tuşa basınca akıcı hareket sağlıyor.

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                started = false;

                if (y < 1)
                    y += Time.deltaTime;
                if (y > 1)
                    y = 1;

                rb.drag = rbDrag;

                rb.velocity = moveSpeed * y * transform.up * Time.fixedDeltaTime;
            }
            else if (!started)
            {
                if (y > 0.5f)
                    y -= Time.deltaTime;
                if (y < 0.5f)
                    y = 0.5f;

                rb.drag = rbStopDrag;

                rb.velocity = moveSpeed * y * transform.up * Time.fixedDeltaTime; // Minimum hızı olan akıcı kod.
                           // rb.velocity.magnitude * transform.up; // Hızı gitgide azaltan kod.
            }
*/
