using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;

public class enemyController : MonoBehaviour
{
    public GameObject boom;
    public Transform bulletSpawn;
    public GameObject bullet;
    public int killCount = 0;
    [SerializeField] public float bulletSpeed = 10f;
   


    private NavMeshAgent agent;
   
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("ShootAtPlayer", 2.0f, 7f);
        transform.forward = transform.forward * -1;    
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            transform.position = transform.position + new Vector3(0, 1f, 0);
        }
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Finish" || col.tag == "TheForce")
        {
            AgentOff();
        }
        if (col.tag == "AllyProj")
        {
            boom = Instantiate(boom, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
            Destroy(boom, 2f);
            killCount++;
        }
        
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Wall" && agent.enabled == false)
        {
            boom = Instantiate(boom, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
            Destroy(boom, 2f);
            killCount++;
            AgentOff();
        }
        if (col.gameObject.GetComponent<Thrown>())
        {
            boom = Instantiate(boom, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
            Destroy(boom, 2f);
            killCount++;
            AgentOff();
        }
        
    }

    public void AgentOff()
    {
        agent.enabled = false;
    }

    public void AgentOn()
    {
        agent.enabled = true;   
    }

    public int getKillCount()
    {
        return killCount;
    }

    void ShootAtPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            //if (agent.enabled == false)
            {
                GameObject projectile = Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
                projectile.transform.forward = projectile.transform.up;
                projectile.transform.LookAt(player.transform.position);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    
                    Vector3 direction = (player.transform.position - bulletSpawn.position).normalized;
                    rb.velocity = direction * bulletSpeed;
                    projectile.transform.forward = transform.forward;
                }
                if (projectile != null && projectile.activeSelf)
                {
                    // Destroy the bullet after a certain time (e.g., 3 seconds)
                    Destroy(projectile, 3f); // Adjust the time as needed
                }
            }
        }
    }
}
