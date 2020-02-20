using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To be added to the Hand game object

public class ControllerPositionRotation : MonoBehaviour
{
    void Update()
    {
        //We are creating a variable to hold the active controller
        OVRInput.Controller activeController = OVRInput.GetActiveController();

        //We are setting the position of the Hand game object as the calculated position of the
        //active controller (it is not tracked but an estimate is hold from the rotation)
        transform.localPosition = OVRInput.GetLocalControllerPosition(activeController);

        //We are setting the rotation of the Hand game object as the rotation of the active controller
        transform.localRotation = OVRInput.GetLocalControllerRotation(activeController);
    }
}

