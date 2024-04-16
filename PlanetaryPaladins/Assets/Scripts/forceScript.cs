using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class forceScript : MonoBehaviour
{

    // Start is called before the first frame update

    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    
    public SteamVR_Action_Boolean toggleForce;
    public SteamVR_Action_Boolean pushForce;
    public SteamVR_Action_Boolean pullForce;

    public Transform controller;
    public GameObject visualForce;
    public float forceThrust = 20.0f;

    public SteamVR_Action_Vibration hapticAction;

    private bool force;
    private bool x;
    private  List<GameObject> objectsInHand = new List<GameObject>();


    void Start()
    {
        visualForce = Instantiate(visualForce, controller.position + visualForce.transform.localPosition, Quaternion.identity * visualForce.transform.localRotation);
        visualForce.SetActive(false);
        visualForce.transform.parent = controller.transform;

        // toggle force
        toggleForce.AddOnStateDownListener(TriggerDown, handType);
        toggleForce.AddOnStateUpListener(TriggerUp, handType);
        // push force
        pushForce.AddOnStateDownListener(YDown, handType);
        // pull force
        pullForce.AddOnStateDownListener(XDown, handType);
        pullForce.AddOnStateUpListener(XUp, handType);
    }
    // haptics

    private void Pulse(float dur, float freq, float amp, SteamVR_Input_Sources fromSource)
    {
        hapticAction.Execute(0, dur, freq, amp, fromSource);
    }


    // toggle force on and off
    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        force = true;
        visualForce.SetActive(true);
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        force = false;
        actualForceScript childScript = GetComponentInChildren<actualForceScript>();
        visualForce.SetActive(false);
        childScript.ReleaseObject();
        childScript.clearList();
        childScript.destroyAllJoints();
    }

    // use the push power

    public void YDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        actualForceScript childScript = GetComponentInChildren<actualForceScript>();
        objectsInHand = childScript.getInHandList();
        childScript.destroyAllJoints();
        childScript.toThrowable();
        if (force)
        {            
            foreach (GameObject g in objectsInHand)
            {
                if (g)
                {
                    g.GetComponent<Rigidbody>().AddForce(controller.transform.forward * forceThrust);
                }
            }
        }
        Pulse(0.1f, 1, 50, fromSource);
    }

    // use the pull pwoer

    public void XDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        x = true;
    }

    public void XUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        x = false;
    }

    void Update()
    {// need to check if objectsInhand exists nullpointer exception here
        if (x)
        {
            actualForceScript childScript = GetComponentInChildren<actualForceScript>();
            if (childScript != null)
            {
                objectsInHand = childScript.getInHandList();
                childScript.destroyAllJoints();
                if (force)
                {
                    foreach (GameObject g in objectsInHand)
                    {
                        if (g)
                        {
                            g.transform.position = Vector3.Lerp(g.transform.position, controller.position, 0.1f);
                        }
                    }

                }
            }
        }
    }
}
