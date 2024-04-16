using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : Interactable
{
    public bool Value;

    // See end of file (below) for a thorough explanation of this field.
    public BoolEvent OnValueChanged;

    // References to child objects.
    private Transform button;

    public float height;
    private float onPos;
    private bool clicked;
    // We also have access (from the base class) to:
    // bool Stealable;
    // WandController attachedController;

    void Awake()
    {
        button = transform.Find("Button");
        onPos = button.transform.localPosition.y;
        Debug.Log("onpos" + onPos);
    }
    //before first frame update
    void Start()
    {
        button = transform.Find("Button");
        height = button.GetComponent<Collider>().bounds.size.y;
        Value = button.transform.localPosition.y < onPos; //button y when not pressed = 0
        clicked = false;
    }


    // Update is called once per frame
    void Update()
    {
        // bool newValue = Value;
        if (attachedController != null)
        {
            Debug.Log("attached");
            // only changes once at end of interaction
            clicked = true;
        }
        else
        {
            if (clicked)
            {
                Debug.Log("click");
                Vector3 buttonPos = button.transform.localPosition;
                float newY = (Value) ? buttonPos.y + (height / 2) : buttonPos.y - (height / 2);  //set control to next state
                button.transform.localPosition = new Vector3(buttonPos.x, newY, buttonPos.z);
                Value = !Value;
                OnValueChanged.Invoke(Value);
                clicked = false;
            }
        }
    }
}
[Serializable] public class BoolEvent : UnityEvent<bool> { }
