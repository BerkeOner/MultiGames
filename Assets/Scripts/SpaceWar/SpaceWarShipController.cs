using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SpaceWarShipController : MonoBehaviour
{
    public enum SpaceWarShip { Laser, Mini }

    public SpaceWarShip ship;

    public Camera playerCamera;
    public GameObject[] flares;
    public Transform firePoint;

    [HideInInspector] public float currentHealth;

    float moveSpeed = 5f;
    float maxSpeed = 15f;
    float rotateSpeed = 8000;
    float fireRate;
    float bulletSpeed = 25f;
    float bulletSpread;

    Rigidbody2D rb;
    Transform playerCameraTransform;
    bool isShooting;
    bool movedUp;
    float x;
    float y;
    float fireRate_;
    float maxHealth = 9999f;
    float bulletDamage = 10f;
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

        currentHealth = maxHealth;

        if (ship == SpaceWarShip.Laser)
        {
            fireRate = 1f;
            bulletDamage = 3f;
        }
        else if (ship == SpaceWarShip.Mini)
        {
            fireRate = 10f;
            bulletDamage = 0.4f;
            bulletSpread = 10f;
        }

        if (!PV.IsMine)
        {
            Destroy(playerCamera);
            Destroy(rb);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
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

        if (Input.GetKey(KeyCode.Space) && Time.time >= fireRate_)
        {
            fireRate_ = Time.time + 1 / fireRate;
            Shoot();
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

    public void Shoot()
    {
        if (ship == SpaceWarShip.Laser)
        {
            SoundManager.Instance.Play("Laser");

            GameObject bulletPrefab = PhotonNetwork.Instantiate(Path.Combine("SpaceWar", "Laser"), firePoint.position, firePoint.rotation);
            SpaceWarShipBullet bulletScript = bulletPrefab.GetComponent<SpaceWarShipBullet>();
            bulletScript.damage = bulletDamage;
            bulletScript.ownerLaser = true;
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletPrefab.GetComponent<Collider2D>());

            StartCoroutine(LateDestroy(bulletPrefab, fireRate));
        }
        else if (ship == SpaceWarShip.Mini)
        {
            SoundManager.Instance.Play("Mini");

            Vector3 randomSpread = new Vector3(0, 0, Random.Range(-bulletSpread, bulletSpread));

            GameObject bulletPrefab = PhotonNetwork.Instantiate(Path.Combine("SpaceWar", "MiniBullet"), firePoint.position, firePoint.rotation * Quaternion.Euler(randomSpread));
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletPrefab.GetComponent<Collider2D>());
            bulletPrefab.GetComponent<Rigidbody2D>().AddForce(bulletPrefab.transform.up * bulletSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            bulletPrefab.GetComponent<SpaceWarShipBullet>().damage = bulletDamage;
        }
    }

    IEnumerator LateDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            PhotonNetwork.Destroy(obj);
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) // Already Dead
            return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            PhotonNetwork.Instantiate(Path.Combine("SpaceWar", "Explosion"), transform.position, Quaternion.identity);
            playerCameraTransform.position = MakeVector2(Vector3.zero, playerCameraTransform.position);
            playerCamera.orthographicSize = 10;
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mine"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            TakeDamage(1);
        }
    }
}
