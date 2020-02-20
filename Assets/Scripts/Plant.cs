using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public float plantGermTimeLag; // instance property
    public float plantETrate; // instance property
    public float plantGrowthRate; // instance property
    [Range(0, 99)]
    public float percentVariation; // how variable can the starting values be

    PottedPlant baseValues; // get the starting values we'll sample around
    public bool ripe;
    public bool harvested; // did the plant leave the pot since germination?


    // Initialize

	// Set up random growth parameters for a new plant
	public void Init () {
        baseValues = FindObjectOfType<PottedPlant>();

        // Add variation to germTimeLag for each new plant
        plantGermTimeLag = Random.Range(baseValues.germTimeLag - baseValues.germTimeLag * percentVariation,
                                    baseValues.germTimeLag + baseValues.germTimeLag * percentVariation);

        // Add variation to etRate for each new plant
        plantETrate = Random.Range(baseValues.etRate - baseValues.etRate * percentVariation,
                              baseValues.etRate + baseValues.etRate * percentVariation);

        // Add variation to growthRate for each new plant
        plantGrowthRate = Random.Range(baseValues.growthRate - baseValues.growthRate * percentVariation,
                                       baseValues.growthRate + baseValues.growthRate * percentVariation);
	}


    // called each frame
    private void Update()
    {
        // only allow harvesting after at least 20% growth
        if (!ripe && transform.lossyScale.x > 0.2f)
        {
            gameObject.tag = "Grabbable"; //make it grabbable
            ripe = true; // flag it as able to be grabbed
        }

        // if hte plant has been pulled far enough from the soil
        // to count as "harvested"
        if (harvested)
        {
            // turn off it's active properties
            GetComponent<Plant>().plantGrowthRate = 0;
            GetComponent<Plant>().plantETrate = 0;
        }
        else
        {
            // Otherwise, it is stationary in space
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

}
