using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBox : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {

        // When a plant enters the attached collider
        if (other.gameObject.GetComponent<Plant>() != null)
        {
            // Play the, attached, celebretory particle system
            GetComponentInChildren<ParticleSystem>().Play();
            // Play the, attached, celebratory spatial audio
            GetComponent<AudioSource>().Play();
            // Access the game's scoring mechanism
            FindObjectOfType<PottedPlant>().Score();
        }
    }
}
