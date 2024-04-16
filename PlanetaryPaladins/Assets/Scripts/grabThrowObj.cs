using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class grabThrowObj : MonoBehaviour
{
    public SteamVR_Action_Boolean grabObj;
    public SteamVR_Action_Boolean toggleObj;

    public SteamVR_Input_Sources handType;

    private Transform hand;

    public UnityEvent togObj;

    public ControlPanel ctrlPanel;

    private FixedJoint joint;

    private GameObject objInRange;

    private Interactable interactibleInRange;

    public Interactable interactibleInHand;

    private GameObject objInHand;

    private GameObject objOutHand;


    private bool returning;

    // Start is called before the first frame update
    void Start()
    {
        hand = transform;
        joint = GetComponent<FixedJoint>();
        objInRange = null;
        objInHand = null;
        objOutHand = null;
        returning = false;
        grabObj.AddOnStateDownListener(TriggerDown, handType);
        grabObj.AddOnStateUpListener(TriggerUp, handType);
        toggleObj.AddOnStateDownListener(ButtonDown, handType);
    }

    void FixedUpdate()
    {
        if (objOutHand)
        {
            Rigidbody objRigidbody = objOutHand.GetComponent<Rigidbody>();
            if (!returning)
            {
                if (objRigidbody && objRigidbody.velocity.magnitude < 0.2f)
                {
                    returning = true;
                }
            }
            else
            {
                float dist = Vector3.Distance(objRigidbody.position, hand.position);
                objRigidbody.position = Vector3.Lerp(objRigidbody.position, hand.position, (15 / dist) * Time.deltaTime);
                objRigidbody.transform.up = Vector3.RotateTowards(objRigidbody.transform.up, hand.forward, 1 / dist * 0.15f * Time.deltaTime, 0.0f);
            }
        }
    }
    public void OnItemDetach(Interactable item)
    {
        interactibleInHand = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("attachedbody " + other.attachedRigidbody);
        if (!objInRange && !objInHand && other.name == "Saber")
        {
            objInRange = other.gameObject;
            return;
        }
        if (other.attachedRigidbody == null) return;
        var interactable = other.attachedRigidbody.GetComponent<Interactable>();
        Debug.Log("interact enter " + interactable);
        if (interactable == null) return;
        if (interactibleInHand == interactable) return;
        if (!interactibleInRange) interactibleInRange = interactable; //not sure if this should js return instead
        interactable.OnHoverEnter(this);
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("attachedbody " + other.attachedRigidbody);
        if (!objInRange && !objInHand && other.name == "Saber")
        {
            objInRange = other.gameObject;
            return;
        }
        if (other.attachedRigidbody == null) return;
        var interactable = other.attachedRigidbody.GetComponent<Interactable>();
        if (interactable == null) return;
        if (interactibleInHand == interactable) return;
        if (!interactibleInRange) interactibleInRange = interactable;
        // Start Interacting 
        if (grabObj.GetStateUp(handType)) interactable.OnHoverStay(this); //chck if needed
    }

    private void OnTriggerExit(Collider other)
    {
        objInRange = null;
        if (other.attachedRigidbody == null) return;
        var interactable = other.attachedRigidbody.GetComponent<Interactable>();
        if (interactable == null) return;
        if (interactibleInHand == interactable) return;
        interactibleInRange = null;
        interactable.OnHoverStay(this); //exit?
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (objInRange)
        {
            grabObjct();
        }
        else if (interactibleInRange && interactibleInRange.TryAttach(this))
        {
            interactibleInHand = interactibleInRange;
            interactibleInRange = null;
        }
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("ihandd " + objInHand + "|" + interactibleInHand);
        if (objInHand)
        {
            releaseObjct();
        }
        else if (interactibleInHand)
        {
            Debug.Log("Detached");
            interactibleInHand.DetachController();
        }
    }
    public void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (objInHand)
        {
            togObj.Invoke();
        }
    }
    private void grabObjct()
    {
        if (objInRange.GetComponent<Rigidbody>())
        {
            joint.connectedBody = objInRange.GetComponent<Rigidbody>();
        }
        else if (objInRange.GetComponentInParent<Rigidbody>())
        {
            joint.connectedBody = objInRange.GetComponentInParent<Rigidbody>();
        }
        if (joint.connectedBody)
        {
            objInHand = objInRange;
            objInRange = null;
        }
    }

    private void releaseObjct()
    {
        Rigidbody objInHandRb = joint.connectedBody;
        joint.connectedBody = null;
        hand.GetComponent<SteamVR_Behaviour_Pose>().GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity);
        objInHandRb.velocity = velocity * 10;
        objInHandRb.angularVelocity = angularVelocity * 20;

        if (objInHand.name == "Saber")
        {
            objOutHand = objInHand;
            returning = false;
        }
        objInHand = null;

    }
}
