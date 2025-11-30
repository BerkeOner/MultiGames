using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TankController : MonoBehaviour
{
    public Transform firePoint;
    public Camera playerCamera;

    float moveSpeed = 150f;
    float rotateSpeed = 20000f;
    float bulletSpeed = 30f;
    float fireRate = 1f;
    float x;
    float y;
    float fireRate_;
    Transform playerCameraTransform;
    Rigidbody2D rb;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        playerCameraTransform = playerCamera.transform;

        if (!PV.IsMine)
        {
            Destroy(playerCamera);
            Destroy(rb);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            SoundManager.Instance.Play("SpaceEngine");
            TankModeManager.Instance.UpdatePlayerCount(1);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        playerCameraTransform.position = MakeVector2(transform.position, playerCameraTransform.position);
        playerCameraTransform.rotation = Quaternion.identity;

        if (Input.GetKey(KeyCode.Space) && Time.time >= fireRate_)
        {
            fireRate_ = Time.time + 1 / fireRate;
            Shoot();
        }

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (x != 0 || y != 0)
            SoundManager.Instance.Play("Thruster");
        else
            SoundManager.Instance.Stop("Thruster");

        rb.angularVelocity = -rotateSpeed * x * Time.fixedDeltaTime;
        rb.velocity = moveSpeed * y * transform.up * Time.fixedDeltaTime;
    }

    public Vector3 MakeVector2(Vector3 target, Vector3 self)
    {
        return new Vector3(target.x, target.y, self.z);
    }

    public void Shoot()
    {
        SoundManager.Instance.Play("Laser");

        GameObject bulletPrefab = PhotonNetwork.Instantiate(Path.Combine("Tank", "TankBullet"), firePoint.position, firePoint.rotation);
        bulletPrefab.GetComponent<Rigidbody2D>().AddForce(bulletPrefab.transform.up * bulletSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletPrefab.GetComponent<Collider2D>());
    }
}
