using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour {

    public UnityEvent onBehavior;
    public UnityEvent offBehavior;

    public bool on;
    public bool held;


    // Run positive interaction behavior
    public void Engage()
    {
        // run the behavior
        onBehavior.Invoke();
        // pass a flag to note the on state
        on = true;

    }

    // Run negative interaction behavior
    public void Disengage()
    {
        //run the behavior
        offBehavior.Invoke();
        // pass a flag to note the off state
        on = false;
    }
}
