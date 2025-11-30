using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemySpaceWarShip : MonoBehaviour
{
    public enum ShipType { Basic, Boss }
    public ShipType ship;

    [HideInInspector] public float currentHealth;

    public Transform[] firePoints;
    List<GameObject> targets = new List<GameObject>();

    float moveSpeed;
    float rotateSpeed;
    float bulletDamage;
    float bulletSpeed;
    float fireRate;
    float fallBackDistance;
    float range;
    float spotDistance;
    float maxHealth;

    float fireRate_;
    Rigidbody2D rb;

    private enum State { Attack, Fallback, Stay }
    State state;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (ship == ShipType.Basic)
        {
            moveSpeed = 50f;
            rotateSpeed = 1.5f;
            bulletDamage = 0.5f;
            bulletSpeed = 20f;
            fireRate = 1f;
            fallBackDistance = 0.5f;
            range = 2.5f;
            spotDistance = 5f;
            maxHealth = 2f;
        }
        else if (ship == ShipType.Boss)
        {
            moveSpeed = 20f;
            rotateSpeed = 0.7f;
            bulletDamage = 1f;
            bulletSpeed = 20f;
            fireRate = 2f;
            fallBackDistance = 1f;
            range = 3f;
            spotDistance = 4f;
            maxHealth = 8f;
        }

        currentHealth = maxHealth;
        state = State.Stay;

        if (PV.IsMine)
            StartCoroutine(DetectTrigger());

        if (PV.IsMine)
            SpaceWarShipModeManager.Instance.UpdateEnemyCount(1);
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        switch (state)
        {
            case State.Attack:
                rb.velocity = moveSpeed * transform.up * Time.deltaTime;
                break;
            case State.Fallback:
                rb.velocity = -moveSpeed * transform.up * Time.deltaTime;
                break;
            case State.Stay:
                break;
        }

        if (targets.Count != 0 && targets[0] != null)
            Rotate(targets[0].gameObject);
    }

    IEnumerator DetectTrigger()
    {
        for (;;)
        {
            // OnTriggerEnter2D

            targets.Clear();

            Collider2D[] _targets = Physics2D.OverlapCircleAll(transform.position, spotDistance);
            for (int i = 0; i < _targets.Length; i++)
            {
                if (_targets[i].transform.CompareTag("Player"))
                {
                    targets.Add(_targets[i].gameObject);
                }
            }

            targets.Sort(SortByDistance);

            // OnTriggerStay2D

            if (targets.Count > 0)
            {
                Vector2 dir = targets[0].transform.position - transform.position;

                float dist = Vector2.Distance(transform.position, targets[0].transform.position);

                if (dist > range)
                    state = State.Attack;
                else if (dist < fallBackDistance)
                    state = State.Fallback;
                if (dist < range)
                {
                    state = State.Stay;

                    if (Time.time >= fireRate_)
                    {
                        fireRate_ = Time.time + 1 / fireRate;

                        Shoot();
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    int SortByDistance(GameObject a, GameObject b)
    {
        float sqrA = (a.transform.position - transform.position).sqrMagnitude;
        float sqrB = (b.transform.position - transform.position).sqrMagnitude;

        return sqrA.CompareTo(sqrB);
    }

    void Rotate(GameObject target)
    {
        Vector3 pos = target.transform.position - transform.position;
        float angle = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, -Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    public void Shoot()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform[] FP = firePoints;

            GameObject bulletPrefab = PhotonNetwork.Instantiate(Path.Combine("SpaceWar", "EnemyBullet"), FP[i].position, FP[i].rotation);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletPrefab.GetComponent<Collider2D>());
            bulletPrefab.GetComponent<Rigidbody2D>().AddForce(bulletPrefab.transform.up * bulletSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            bulletPrefab.GetComponent<SpaceWarShipBullet>().damage = bulletDamage;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) // Already Dead
            return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            if (PV.IsMine)
                SpaceWarShipModeManager.Instance.UpdateEnemyCount(-1);
                
            PhotonNetwork.Instantiate(Path.Combine("SpaceWar", "Explosion"), transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
