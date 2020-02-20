using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobShadowRotation : MonoBehaviour {

	void Update ()
    {
        //Updating the forward (blue axis) of the blob shadow projector
        //as down, so that even when the game object is rotated, the shadow 
        //is projected on the correct plane (downwards)
        transform.rotation = Quaternion.LookRotation(Vector3.down);
	}
}

