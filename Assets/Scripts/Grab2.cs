using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab2 : MonoBehaviour
{
    bool isGrabbing = false;
    Transform grabbedTransform;
    float zSpeed = 3.8f;
    private Transform hitTransform;

    //A variable to hold if the user pointing at (hovering) a grabbable object
    bool isLooking = false;

    //Two prefabs to be instantiated at the marker position
    public GameObject CubePrefabVar, SpherePrefabVar;

    void Update()
    {
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        transform.localPosition = OVRInput.GetLocalControllerPosition(activeController);
        transform.localRotation = OVRInput.GetLocalControllerRotation(activeController);

        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if (hitInfo2.transform.tag == "Grabbable")
            {
                if (hitTransform != null)
                    SetHighlight(hitTransform, false);

                hitTransform = hitInfo2.transform;
                SetHighlight(hitTransform, true);
                isLooking = true; //The user is pointing at a grabbable object
            }
            else
            {
                if (hitTransform != null && !isGrabbing)
                    SetHighlight(hitTransform, false);
                isLooking = false; //The user isn't pointing at a grabbable object
            }
        }
        else
        {
            if (hitTransform != null && !isGrabbing)
            {
                SetHighlight(hitTransform, false);
            }
            isLooking = false; //The user isn't pointing at a grabbable object
        }



        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
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

        if (isGrabbing && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
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
            //If the user is grabbing an object and touching the touchpad,
            //we are moving the grabbed object in its local z-axis based on the touch input 
            float distance = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

            grabbedTransform.position += distance * Time.deltaTime * zSpeed * transform.forward;
            grabbedTransform.localPosition = new Vector3(grabbedTransform.localPosition.x,
                                             grabbedTransform.localPosition.y,
                                             Mathf.Clamp(grabbedTransform.localPosition.z, 1.0f, 7.0f));
        }

        //If the user is grabbing an object and touching the touchpad, we move the purple 
        //marker based on the touch input (x and y axes in 2D corresponds to the x and z axes in 3D)
        if (!isGrabbing && OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).magnitude != 0)
        {
            float distance = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

            transform.GetChild(0).position += distance * Time.deltaTime * zSpeed * transform.forward;
            transform.GetChild(0).localPosition = new Vector3(transform.GetChild(0).localPosition.x,
                                                  transform.GetChild(0).localPosition.y,
                                                  Mathf.Clamp(transform.GetChild(0).localPosition.z, 1.0f, 7.0f));
        }

        //If the user is not pointing at an already existing grabbable object and presses the 
        //touchpad button (one) or the back button, we instantiate the cube or the sphere (based 
        //on the prefab references on the inspector) at the purple marker's position
        if (!isLooking && !isGrabbing && OVRInput.GetDown(OVRInput.Button.One))
        {
            Instantiate(CubePrefabVar, transform.GetChild(0).position, transform.GetChild(0).rotation);
        }

        if (!isLooking && !isGrabbing && OVRInput.GetDown(OVRInput.Button.Two))
        {
            Instantiate(SpherePrefabVar, transform.GetChild(0).position, Quaternion.identity);
        }

    }

    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            t.GetComponent<Renderer>().material.color = Color.cyan;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        }
        else
        {
            t.GetComponent<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
        }
    }
}
