using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code derived from Unity manaul: https://docs.unity3d.com/Manual/PartSysTriggersModule.html

public class WaterPlant : MonoBehaviour {

    private ParticleSystem waterEffect; // container for particle system

	// Use this for initialization
	void Start () {
        waterEffect = GetComponent<ParticleSystem>();
	}

    private void OnParticleCollision(GameObject other)
    {
        // Are we colliding with topsoil?
        var soilCollider = FindObjectOfType<SoilCollision>().GetComponentInChildren<Rigidbody>().gameObject;
        if (other == soilCollider || other == FindObjectOfType<PottedPlant>().plantInstance)
        {
            // Water the plant!!!
            FindObjectOfType<PottedPlant>().AddWater();
        }
    }




    // HAd to find workaround - UNITY does not yet support specific trigger detection (need colliders)

    //// Callback that works within the ParticleSystem module (Unity editor)
    //// when callback is selected (which it should be
    //private void OnParticleTrigger()
    //{
    //    Debug.Log("----------------WATER IS Triggering THE POT!!!!--------------");
    //    // add water when this callback happens
    //    FindObjectOfType<PottedPlant>().AddWater(); 
    //}
}
