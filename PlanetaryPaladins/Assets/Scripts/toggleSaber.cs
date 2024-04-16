using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class toggleSaber : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Behaviour_Pose controllerPose;

    public GameObject saber;
    public Transform controller;


    // Start is called before the first frame update
    void Start()
    {
        saber = Instantiate(saber, controller.position + saber.transform.localPosition, Quaternion.identity * saber.transform.localRotation);
        saber.SetActive(false);
        saber.transform.parent = controller.transform;
        grabAction.AddOnStateDownListener(ButtonDown, handType);
        grabAction.AddOnStateUpListener(ButtonUp, handType);
    }


    public void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        saber.SetActive(true);
        
        //Debug.Log(controller.position);
        
    }

    public void ButtonUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        saber.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
