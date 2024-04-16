using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;

public class spawnScript : MonoBehaviour
{
    public GameObject enemy;
    private GameObject spawnedEnemy;
    public Transform spawner;
    public Transform goal;

    [SerializeField] public int amountToSpawn = 1;
    [SerializeField] public bool infiniteSpawn = false;
    [SerializeField] public float spawnCooldown = 5f;
    [SerializeField] public float initSpawnTime = 5f;

    private NavMeshAgent agent;

    void Start()
    {
        if (infiniteSpawn)
        {
            InvokeRepeating("spawnEnemy", initSpawnTime, spawnCooldown);
        }
        else
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                spawnEnemy();
            }
        }
  
    }

    void Update()
    {
        
    }
    
    void spawnEnemy()
    {
        if (isActiveAndEnabled)
        {

            spawnedEnemy = Instantiate(enemy, spawner.transform.position, spawner.transform.rotation);
            agent = spawnedEnemy.GetComponent<NavMeshAgent>();
        }
        if (agent != null && agent.enabled)
        {
            agent.destination = goal.position;
        }
    }

 /*   public void shoot(Transform bulletspawn, GameObject bullet, float bulletSpeed)
    {
        bullet = Instantiate(bullet, bulletspawn.transform.position, bulletspawn.transform.rotation);
        Vector3 dir = (player.position - bulletspawn.transform.position).normalized;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(dir * bulletSpeed, ForceMode.Impulse);
    }*/
}

