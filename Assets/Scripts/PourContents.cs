using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourContents : MonoBehaviour {


    private ParticleSystem water; // instance of our particle prefab
    private AudioSource noise; // pouring sound effecrt
    public float pourRotation = 110.0f; // how far must this object tilt in x or z to pour?
    
	// Use this for initialization
	void Start () {
        // Grab the attached  particle emitter
        water = GetComponentInChildren<ParticleSystem>();
        // grab the attached audio source
        noise = GetComponentInChildren<AudioSource>();
    }


    void Update()
    {

        // Check if the pouring object is rotated far enough (account for circular wrapping)
        if ((transform.eulerAngles.x > pourRotation && transform.eulerAngles.x < 360 - pourRotation) || 
            (transform.eulerAngles.z > pourRotation && transform.eulerAngles.z < 360 - pourRotation))
        {
            // If not playing already, play the pour effect
            if (!water.isPlaying)
            {

                water.Play();

                // if not playing already, play the pour noise
                if (!noise.isPlaying)
                {
                    noise.Play();
                }
            }
        }
        // If those conditions aren't met, we shouldn't see or hear pouring
        else
        {
            // if the particles are running, turn them off
            if (water.isPlaying)
            {
                water.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                // if the noise is running, turn it off
                if (noise.isPlaying)
                {
                    noise.Stop();
                }
            }
        }
    }

}
