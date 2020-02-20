using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilCollision : MonoBehaviour {
    public ParticleSystem triggerParticles;
    public List<ParticleCollisionEvent> collisions;


    // Handle how interactables work with the pot via it's soil level collider
    private void OnTriggerExit(Collider other)
    {
        // did a seed just pass through our collider (and is it now below it)?
        if (other.gameObject.GetComponent<Seed>() != null &&
            other.gameObject.GetComponent<Seed>().transform.position.y < transform.position.y)
        {
            // if so, run the PlantSeed() method on the PottedPlant class
            GetComponentInParent<PottedPlant>().PlantSeed(other.GetComponent<Seed>()); 
        }

        // did a plant just leave the topsoil?
        if (other.gameObject.GetComponent<Plant>() != null)
        {
            // if so, flag the plant class property as harvested
            other.gameObject.GetComponent<Plant>().harvested = true;
        }
    }
}
