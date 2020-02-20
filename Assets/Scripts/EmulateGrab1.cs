using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//********************************************************************************************//
//*****DEACTIVATE THIS SCRIPT AND ACTIVATE GRAB1 BEFORE TAKING TEST BUILDS FOR OCULUS GO******//
//**REMOVE THIS SCRIPT FROM THE HAND GAME OBJECT BEFORE TAKING THE FINAL BUILD FOR OCULUS GO**//
//********************************************************************************************//

//Press W while the Game View is active, rotate the Oculus Go controller with the left mouse button
//Grab objects by hovering and clicking down the left mouse button
//Scroll up and down the mouse wheel to move the grabbed object in z-axis

public class EmulateGrab1 : MonoBehaviour
{
    float controllerSpeedHorizontal = 1.5f;
    float controllerSpeedVertical = 1.5f;
    float controllerYaw = 0.0f;
    float controllerPitch = 0.0f;

    private bool isGrabbing = false;
    private Transform grabbedTransform;
    public float zSpeed = 4.5f;
    public float rotationSpeedMultiplier = 5.0f;
    private Transform hitTransform;

    void Start()
    {
        transform.localPosition = new Vector3(0.2f, -0.4f, 0.6f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            EmulateHeadRotation.isLimited = true;
            controllerYaw += controllerSpeedHorizontal * Input.GetAxis("Mouse X") * rotationSpeedMultiplier;
            controllerPitch += controllerSpeedVertical * Input.GetAxis("Mouse Y") * -rotationSpeedMultiplier;
            transform.localRotation = Quaternion.Euler(controllerPitch, controllerYaw, 0.0f);
        }

        if (Input.GetKeyUp(KeyCode.W))
            EmulateHeadRotation.isLimited = false;

        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if (hitInfo2.transform.tag == "Grabbable" && !isGrabbing || hitInfo2.transform.tag == "Interactable")
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

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo))
            {
                if (hitInfo.transform.tag == "Grabbable")
                {

                    if (hitInfo.transform.GetComponent<Seed>() != null)
                    {
                        if (FindObjectOfType<GameManager>().startBoard.activeSelf)
                        {
                            FindObjectOfType<GameManager>().StartClock(); // start the timer
                        }

                    }

                    isGrabbing = true;

                    grabbedTransform = hitInfo.transform;
                    grabbedTransform.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedTransform.GetComponent<Rigidbody>().useGravity = false;
                    grabbedTransform.parent = transform;
                }
            }
        }

        // MC - Add simple interaction (button click)
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Check raycast
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo))
            {
                if (hitInfo.transform.tag == "Interactable")
                {
                    var interactable = hitInfo.transform.GetComponentInChildren<InteractableObject>();
                    if (interactable.on)
                    {
                        interactable.Disengage();
                    }
                    else interactable.Engage();
                
                }
            }
        }

        if (isGrabbing && Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (grabbedTransform != null)
            {



                grabbedTransform.GetComponent<Rigidbody>().isKinematic = false;
                grabbedTransform.GetComponent<Rigidbody>().useGravity = true;
                grabbedTransform.parent = null;

                isGrabbing = false;
            }



        }

        if (isGrabbing)
        {
            float distance = Input.mouseScrollDelta.y;

            grabbedTransform.position += distance * zSpeed * Time.deltaTime * transform.forward;
            grabbedTransform.localPosition = new Vector3(grabbedTransform.localPosition.x, grabbedTransform.localPosition.y, Mathf.Clamp(grabbedTransform.localPosition.z, 0.4f, 7.0f));
        }
    }

    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            //t.GetComponent<Renderer>().material.color = Color.cyan;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        }
        else
        {
            //t.GetComponent<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
        }
    }
}
