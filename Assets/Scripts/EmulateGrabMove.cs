using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//************************************************************************************************//
//****DEACTIVATE THIS SCRIPT AND ACTIVATE GRABMOVE BEFORE TAKING TEST BUILDS FOR OCULUS GO****//
//****REMOVE THIS SCRIPT FROM THE HAND GAME OBJECT BEFORE TAKING THE FINAL BUILD FOR OCULUS GO****//
//************************************************************************************************//

//Press C while the Game View is active, rotate the Oculus Go controller with the left mouse button
//Grab objects by hovering and clicking down the left mouse button
//Scroll up and down the mouse wheel to move the grabbed object in z-axis
//Move in the virtual world with the WASD keys

public class EmulateGrabMove : MonoBehaviour
{
    float controllerSpeedHorizontal = 1.5f;
    float controllerSpeedVertical = 1.5f;
    float controllerYaw = 0.0f;
    float controllerPitch = 0.0f;

    private bool isGrabbing = false;
    private Transform grabbedTransform;
    public float zSpeed = 4.5f;
    public float rotationSpeedMultiplier = 4.0f;
    private Transform hitTransform;

    void Start()
    {
        transform.localPosition = new Vector3(0.2f, -0.4f, 1.0f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            EmulateHeadRotation.isLimited = true;
            controllerYaw += controllerSpeedHorizontal * Input.GetAxis("Mouse X") * rotationSpeedMultiplier;
            controllerPitch += controllerSpeedVertical * Input.GetAxis("Mouse Y") * -rotationSpeedMultiplier;
            transform.localRotation = Quaternion.Euler(controllerPitch, controllerYaw, 0.0f);
        }
        if (Input.GetKeyUp(KeyCode.C))
            EmulateHeadRotation.isLimited = false;

        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if (hitInfo2.transform.tag == "Grabbable" && !isGrabbing)
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
                    isGrabbing = true;
                    grabbedTransform = hitInfo.transform;
                    grabbedTransform.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedTransform.GetComponent<Rigidbody>().useGravity = false;
                    grabbedTransform.parent = transform;
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
            }
            isGrabbing = false;
        }

        if (isGrabbing)
        {
            float distance = Input.mouseScrollDelta.y;

            grabbedTransform.position += distance * Time.deltaTime * zSpeed * transform.forward;
            grabbedTransform.localPosition = new Vector3(grabbedTransform.localPosition.x, grabbedTransform.localPosition.y, Mathf.Clamp(grabbedTransform.localPosition.z, 0.4f, 7.0f));
        }

    }

    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            t.GetComponentInChildren<Renderer>().material.color = Color.cyan;
            t.GetComponentInChildren<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            t.GetComponentInChildren<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar;
            t.GetComponentInChildren<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
        }
    }
}
