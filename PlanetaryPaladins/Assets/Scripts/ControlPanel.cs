using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ControlPanel : MonoBehaviour
{
    public Transform cameraRigTransform;
    public Transform playerCam;
    public Transform tpLocation;
    public Transform saber;
    public Transform controllerR;

    private GameObject[] allSpawners;
    //public Transform SelectedObject;
    /*public void SetX(float x)
    {
        Vector3 pos = SelectedObject.localPosition;
        pos.x = x;
        SelectedObject.localPosition = pos;
    }
    public void SetY(float y)
    {
        Vector3 pos = SelectedObject.localPosition;
        pos.y = y;
        SelectedObject.localPosition = pos;
    }
    public void SetZ(float z)
    {
        Vector3 pos = SelectedObject.localPosition;
        pos.z = z;
        SelectedObject.localPosition = pos;
    }
    public void RotX(float theta)
    {
        //SelectedObject.localEulerAngles = new Vector3(theta, SelectedObject.eulerAngles.y, SelectedObject.eulerAngles.z);
        Quaternion rotation = SelectedObject.localRotation;
        SelectedObject.localRotation = Quaternion.Euler(theta, rotation.y, rotation.z);
        *//*Quaternion rotation = SelectedObject.localRotation;
        rotation.x = theta;
        SelectedObject.localRotation = rotation;*//*
    }*/

    private void Start()
    {
        allSpawners = GameObject.FindGameObjectsWithTag("Spawner");
        DisableSpawners();
    }
    public void Bing(bool a)
    {
        SteamVR_Fade.View(Color.black, 0.3f);
        Invoke("teleport", 1f);
        //Time.timeScale = (a) ? 0 : 1;
    }

    private void teleport()
    { 
        Vector3 difference = cameraRigTransform.position - playerCam.position;
        difference.y = 1;
        cameraRigTransform.position = tpLocation.position;
        saber.transform.position = controllerR.position;
        SteamVR_Fade.View(Color.clear, 0.3f);
        EnableSpawners();
    }

    void DisableSpawners()
    {

        foreach (GameObject spawner in allSpawners)
        { 
            spawner.SetActive(false);
        }
    }

    void EnableSpawners()
    {
        foreach (GameObject spawner in allSpawners)
        {
            Debug.Log("hi");
            spawner.SetActive(true);
        }
    }

}
