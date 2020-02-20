using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedChute : MonoBehaviour {

    public GameObject seedPrefab; // what comes out of the chute

    [HideInInspector]
    public static GameObject seedInstance; // instantiation of the seedPrefab

	// Use this for initialization
	void Start () {
        FeedASeed(); // run behavior when game begins
	}


    // Feed a new seed through the chute
    public void FeedASeed()
    {
        // If there is a seed
        if (seedInstance != null)
        {
            // Get rid of it
            Destroy(seedInstance);
        }
        // Create a new seed
        seedInstance = Instantiate(seedPrefab);
        // make sure it is not a child of anything
        seedInstance.transform.parent = null;
        // ensures the new seed will drop from this chute
        seedInstance.transform.position = transform.position;
    }
}
