using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCannon : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ammo;
    public Transform gunPoint;
    [SerializeField] public int bulletSpeed = 5;

    void Start()
    {
        InvokeRepeating("ShootAtPlayer", 8f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            GameObject projectile = Instantiate(ammo, gunPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if(rb)
            {
                Vector3 dir = (player.transform.position - gunPoint.position).normalized;
                rb.velocity = dir * bulletSpeed;
                projectile.transform.forward = transform.forward;
                transform.up = dir;

            }

        }
    }
}