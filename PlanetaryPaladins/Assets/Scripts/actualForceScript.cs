using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class actualForceScript : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject collidingObject;
    private GameObject objectInHand;
    public List<GameObject> objectsInHand = new List<GameObject>();
    public List<Joint> joints = new List<Joint>();

    private SteamVR_Behaviour_Pose controller;
    void Start()
    {
        controller = GetComponentInParent<SteamVR_Behaviour_Pose>();
    }


    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider col)
    {

        SetCollidingObject(col);
        if ((col.gameObject.tag == "Forceable" || col.gameObject.tag == "Enemy") && !objectsInHand.Contains(col.gameObject))
        {
            objectsInHand.Add(col.gameObject);
            var joint = AddFixedJoint();
            joint.connectedBody = col.gameObject.GetComponent<Rigidbody>();
            joints.Add(joint);
        }
        //GrabObject();
    }

    public void OnTriggerStay(Collider col)
    {
        /*SetCollidingObject(col);
        GrabObject();*/
    }

    public void OnTriggerExit(Collider col)
    {
        if (!collidingObject)
        {
            return;
        }
        if (objectsInHand.Contains(col.gameObject))
        {
            objectsInHand.Remove(col.gameObject);
        }
    }
   

    private FixedJoint AddFixedJoint()
    {
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.breakForce = 10000;
        joint.breakTorque = 10000;
        return joint;
    }

    public void ReleaseObject()
    {
        /*if (GetComponent<FixedJoint>() && objectInHand)
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            Debug.Log(controller.GetAngularVelocity());
            objectInHand.GetComponent<Rigidbody>().velocity = controller.GetVelocity() * 5;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controller.GetAngularVelocity() * 5;
        }
        objectInHand = null;*/
       /* if (GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
            GetComponent<FixedJoint>().connectedBody = null;*/

            foreach (GameObject go in objectsInHand)
            {
                if(go)
                { 
                    go.GetComponent<Rigidbody>().velocity = controller.GetVelocity() * 5;
                    go.GetComponent<Rigidbody>().angularVelocity = controller.GetAngularVelocity() * 5;
                }
            }

            //}
        }

    public void clearList()
    {
        objectsInHand.Clear();
        
    }

    public void destroyAllJoints()
    {
        foreach (Joint j in joints)
        {
            Destroy(j);
        }
    }

    public List<GameObject> getInHandList()
    {
        return objectsInHand;
    }

    public void toThrowable()
    {
        foreach (GameObject obj in objectsInHand)
        {
            if (obj)
            {
                obj.AddComponent<Thrown>();
            }       
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
