using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//************************************************************************************************//
//****DEACTIVATE THIS SCRIPT AND ACTIVATE GRABTELEPORT BEFORE TAKING TEST BUILDS FOR OCULUS GO****//
//****REMOVE THIS SCRIPT FROM THE HAND GAME OBJECT BEFORE TAKING THE FINAL BUILD FOR OCULUS GO****//
//************************************************************************************************//

//Press W while the Game View is active, rotate the Oculus Go controller with the left mouse button
//Grab objects by hovering and clicking down the left mouse button
//Scroll up and down the mouse wheel to move the grabbed object in z-axis
//Press the 'D' key to see teleportation destination visual
//Press the 'F' key to get teleported there

public class EmulateGrabTeleport : MonoBehaviour
{
    float controllerSpeedHorizontal = 1.5f;
    float controllerSpeedVertical = 1.5f;
    float controllerYaw = 0.0f;
    float controllerPitch = 0.0f;

    private bool isGrabbing = false;
    private Transform grabbedTransform;
    public float zSpeed = 4.0f;
    public float rotationSpeedMultiplier = 5.0f;
    private Transform hitTransform;
    public GameObject playerController;
    public GameObject targetVisual;
    private Material targetMaterial;
    private bool waitTeleportation;
    private Vector3 hitPos;

    void Start()
    {
        transform.localPosition = new Vector3(0.2f, -0.4f, 1.0f);
        targetMaterial = targetVisual.GetComponent<Renderer>().material;
        waitTeleportation = false;
        targetVisual.transform.GetComponent<Renderer>().enabled = false;
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

        if (Input.GetKey(KeyCode.D))
        {
            targetVisual.transform.GetComponent<Renderer>().enabled = true;

            RaycastHit hitInfo3;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo3))
            {
                if (hitInfo3.transform.tag == "Ground")
                {
                    targetVisual.transform.position = new Vector3(hitInfo3.point.x,
                                                      hitInfo3.point.y + 0.01f, hitInfo3.point.z);
                    targetVisual.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo3.normal);

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hitPos = new Vector3(hitInfo3.point.x, hitInfo3.point.y + 1.02f, hitInfo3.point.z);

                        if (!waitTeleportation)
                            StartCoroutine(Teleportation());

                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            targetVisual.transform.GetComponent<Renderer>().enabled = false;
        }
    }

    IEnumerator Teleportation()
    {
        waitTeleportation = true;
        targetMaterial.color = new Color(0.0f, 1.0f, 0.0f, 0.7f);
        targetVisual.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.35f);

        playerController.transform.position = hitPos;
        targetMaterial.color = new Color(1.0f, 1.0f, 0.0f, 0.7f);
        targetVisual.transform.GetComponent<Renderer>().enabled = false;
        waitTeleportation = false;
    }

    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            t.GetComponent<Renderer>().material.color = Color.cyan;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else
        {
            t.GetComponent<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar;
            t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
        }
    }
}
