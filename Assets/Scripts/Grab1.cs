using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab1 : MonoBehaviour
{
    private bool isGrabbing = false;     //A variable to hold if the
                                         //user is grabbing a grabbable object
    private Transform grabbedTransform;  //A variable to hold the transform of
                                         //the grabbed object
    public float zSpeed = 4.5f;          //A variable to hold the speed of bringing
                                         //an object near the user in z-axis (adjustable)
    private Transform hitTransform;      //A variable to hold the hit object's 
                                         //transform for color change & outlining

    void Update()
    {
        //Creating a variable to hold the active controller (comes from the OVR plugin)
        OVRInput.Controller activeController = OVRInput.GetActiveController();

        //Setting the position of the ray as the calculated position of the active
        //controller (comes from the OVR plugin, it is not actually tracked, but 
        //an estimate is kept from the rotation)
        transform.localPosition = OVRInput.GetLocalControllerPosition(activeController);

        //Setting the rotation of the ray as the rotation of the active controller 
        //(comes from the OVR plugin)
        transform.localRotation = OVRInput.GetLocalControllerRotation(activeController);

        //Using raycasting to highlight & outline objects on hover
        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if ((hitInfo2.transform.tag == "Grabbable" && !isGrabbing) ||
                 hitInfo2.transform.tag == "Interactable")
            {
                if (hitTransform != null)
                    SetHighlight(hitTransform, false);

                hitTransform = hitInfo2.transform;
                SetHighlight(hitTransform, true);
            }
            else
            {
                if (hitTransform != null && !isGrabbing)
                    SetHighlight(hitTransform, false);
            }
        }
        else
        {
            if (hitTransform != null && !isGrabbing)
            {
                SetHighlight(hitTransform, false);
            }
        }

        //Using raycasting to grab hovered objects by pulling the trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo))
            {
                //Using the "Grabbable' tag to exclude hits with other objects
                if (hitInfo.transform.tag == "Grabbable")
                {

                    //----------------------------------------------------------------
                    // Added by Group 6 - MC

                    // Check if they're grabbing the seed at start (this hides the startboard
                    // and begins the game
                    if (hitInfo.transform.GetComponent<Seed>() != null)
                    {
                        if (FindObjectOfType<GameManager>().startBoard.activeSelf)
                        {
                            FindObjectOfType<GameManager>().StartClock(); // start the timer
                        }

                    }//---------------------------------------------------------------

                    //The user is grabbing a grabbable object
                    isGrabbing = true;

                    //Getting the transform value of the hit object
                    grabbedTransform = hitInfo.transform;

                    //Setting "isKinematic" as true and "useGravity" as false so
                    //that we can control the object with the controller rotation,
                    //as if it was snapped to the ray's hitpoint
                    grabbedTransform.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedTransform.GetComponent<Rigidbody>().useGravity = false;

                    //Setting the Hand object (the object to which this script 
                    //isattached) as the parent of the hit object. This way, we will 
                    //be able to control its movement by rotating the controller.
                    grabbedTransform.parent = transform;
                }

                //--------------------------------------------------------------
                // Added by Group 6 - MC

                // add behavior for simple interactions (no grab)
                if (hitInfo.transform.tag == "Interactable")
                {
                    // get the interactable object
                    var interactable = hitInfo.transform.GetComponentInChildren<InteractableObject>();
                    if (interactable.on)
                    {
                        interactable.Disengage(); // run the custom disengage Unity Event (InteractableObject.cs)
                    }
                    else interactable.Engage(); // run the custom engage unity event (InteractableObject.cs)

                }//-------------------------------------------------------------

            }
        }

        //Releasing the grabbed object when the trigger is released
        if (isGrabbing && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            //Reversing the "isKinematic" and "useGravity" settings, 
            //so that the released object can move independent of the Hand
            grabbedTransform.GetComponent<Rigidbody>().isKinematic = false;
            grabbedTransform.GetComponent<Rigidbody>().useGravity = true;

            //Setting the parent as none, so that the Hand object
            //no longer controls the movement of this object (Physics Engine
            //will control the movement through the rigidbody component)
            grabbedTransform.parent = null;

            //The user is not grabbing a grabbable object
            isGrabbing = false;
        }

        //If the user is grabbing an object, we are taking care of bringing
        //the object close or away by touching the touchpad. We cater for
        //the lack of positional tracking in the Oculus Go controller.
        if (isGrabbing)
        {
            //Moving the grabbed object in its local z-axis based on the touch
            //input (x and y axes in 2D corresponds to x and z axes in 3D)
            float distance = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

            //We can adjust the movement speed through the zSpeed variable on the Inspector
            //We are clamping the position of the grabbed object between 1 and 7 (a limitation 
            //to prevent moving too far or too close).
            grabbedTransform.position += distance * Time.deltaTime * zSpeed * transform.forward;
            grabbedTransform.localPosition = new Vector3(grabbedTransform.localPosition.x,
                                             grabbedTransform.localPosition.y,
                                             Mathf.Clamp(grabbedTransform.localPosition.z, 1.0f, 7.0f));
        }
    }

    //Changing the material color to cyan or the original color (taken from a script attached
    //to the object itself). Changing the opacity of the line renderer's material color.
    //Activating/deactivatingthe outline throught the Outline script. We pass in the
    //transform of the object to be affected, and true for highlighting, false for dimming.
    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            //t.GetComponent<Renderer>().material.color = Color.cyan; // Group 6 - removed; not needed
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        }
        else
        {
            //t.GetComponent<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar; // group 6 - removed; not needed
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
        }
    }
}

