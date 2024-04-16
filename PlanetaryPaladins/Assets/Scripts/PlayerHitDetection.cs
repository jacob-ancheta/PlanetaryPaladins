using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHitDetection : MonoBehaviour
{
    public Image damageImage;
    public GameObject image;
    public float flashSpeed = 15f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    public Color regenColor = new Color(0f, 0f, 1f, 0.1f);

    public TextMeshProUGUI textMesh;
    public GameObject txt;

    public Transform cameraRigTransform;
    public Transform tpLocation;
    public Transform tpPlay;

    public GameObject saber;
    public GameObject controllerR;

    public GameObject restartGO;
    
    private bool isHit = false;
    private bool isRegenerating = false;
    private bool lose = false;

    private int hp = 5;
    private int hpFull = 5;
    private float lastHitTime;
    private float regenDelay = 5f;
    private float time = 0.0f;
    private bool timeCheck = true;

    void Start()
    {
        damageImage.color = Color.clear;
    }

    void Update()
    {
        
        /*enemyController ecScript = GetComponentInChildren<enemyController>();
        int killCount = ecScript.getKillCount();*/
        if (isHit || isRegenerating)
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

            if (damageImage.color.a <= 0.01f)
            {
                damageImage.color = Color.clear;
                image.SetActive(false);
                isHit = false;
                isRegenerating = false;
            }
        }

        // Regeneration logic
        if (Time.time > lastHitTime + regenDelay && hp < hpFull)
        {
            hp = hpFull;
            Debug.Log("Health regenerated to " + hp);
            TriggerFlash(regenColor);
        }

        if (hp <= 0)
        {
            lose = true;
        }

        if (lose && timeCheck)
        {
            txt.SetActive(true);
            time = Time.time;
            textMesh.text = $"Game Over\r\nTime Survived: {time} \r\n";
            hp = 5;
            timeCheck = false;
            cameraRigTransform.position = tpLocation.position;
            saber.transform.position = controllerR.transform.position;
            DestroyEnemies("Enemy");
        }

        // When you restart stuff
  

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProj")
        {
            Debug.Log("hit");
            hp -= 1;
            Debug.Log("Current HP: " + hp);
            lastHitTime = Time.time;
            TriggerFlash(flashColor); 
            isHit = true;
        }
    }

 
    private void TriggerFlash(Color flashColor)
    {
        image.SetActive(true);
        damageImage.color = flashColor;
        if (flashColor == regenColor)
            isRegenerating = true;
        else
            isHit = true;
    }

    void DestroyEnemies(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    
}