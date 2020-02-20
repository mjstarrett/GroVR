using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PottedPlant : MonoBehaviour {
    public GameObject plantPrefab; // what's the crop this season?
    public SoilCollision soilCollider; // how we'll detect when a seed is dropped in the pot
    public Transform plantSpawnPoint; // predetermined location for plant to start growing
    public float flowRate = 0.1f; // % fill per second
    public float germTimeLag = 5; // how long after watering the seed will it start growing
    public ParticleSystem BreakEffect;

    [Range(0, 1)]
    public float waterInPot; // how hydrated is the plant's environment
    [Range(0, 1)]
    public float etRate = 0.01f; // start value for how fast is the water being used or evaporated

    // Oculus input
    OVRInput.Controller activeController = OVRInput.GetActiveController();

    public GameObject plantInstance; // This is one plant of this seasons crop
    public bool seeded; // Has a seed been dropped in the pot?
    public bool germinated; // Has the pot been watered AFTER a seed was planted?
    [Range(0, 1)]
    public float growthRate; // how fast is the plant growing (increasing scale)

    private GameManager manager;
    private float germTimeElapsed; // clock for how long since germination


	// Use this for initialization
	void Start () {

        // find the game mangaer
        manager = FindObjectOfType<GameManager>(); 

        // grab our particle system for breaking  a pot
        BreakEffect = transform.GetComponentInChildren<ParticleSystem>(); 
	}
	

	// Update is called once per frame
	void Update () {
    
        // Don't let the turnip get bigger than the max allowed size (.01)
        if (plantInstance != null && plantInstance.transform.lossyScale.x > 1)
        {
            Shatter(); // break the pot because the plant is too big!
        }
        //// FOR DEBUGGING destroying plant
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Shatter();
        //}

        // Events possible after dropping seed into pot
        if (seeded)
        {
            //// FOR DEBUGGING watering the plant w/out Oculus GO
            //if (Input.GetKey("h"))
            //{
            //    AddWater();
            //}


            // What happens if the seed is planted and there is water in the pot
            if (waterInPot > Mathf.Epsilon)
            {
                // Use the water
                waterInPot -= etRate * Time.deltaTime;

                // If seed hasn't germinated, it uses the water to do so
                if (!germinated)
                {
                    // begin the waiting period for germination (between first 
                    // water and sprouting)
                    StartCoroutine(StartGerminationLag());
                }
                // If the seed germinated, the plant uses water to grow
                else
                {
                    plantInstance.transform.localScale += new Vector3(growthRate * Time.deltaTime,
                                                                      growthRate * Time.deltaTime, 
                                                                      growthRate * Time.deltaTime);
                }
            }
        }

        // The pot can never be less than 0% full
        if (waterInPot < 0)
        {
            waterInPot = 0;
        }


        // ----------- Ended up cutting from game. Would have been cool ------------
        // -----------------------------------
        // Handle the water level and state
        // -----------------------------------
        if (waterInPot >= 0.5 )
        {
            // put the plant in overwatered state (darken leaves to indicate over-watering)

        }
        else if (waterInPot < Mathf.Epsilon) 
        { 
            // put the plant in a dehydrated state (gray leaves to indicate dryness)
        }
        else
        {
            // put the plant in a healthy growing state (set leaves to default color)
        }
        // ^^^^^^^^^^^^ Ended up cutting from game. Would have been cool ^^^^^^^^^^^^^

    }


    // Public method to add water to pot
    public void AddWater()
    {
        // can't add water before the seed is in
        if (seeded)
        {
            Debug.Log("WATERING HTE PLANT!!!!!!!");
            // add water at the flowrate/sec
            waterInPot += flowRate * Time.deltaTime;
        }
    }


    // Sequence of events when seed is dropped into pot
    public void PlantSeed(Seed seed)
    {
        // set the seed planted variable to true
        seeded = true;

        // destroy the seed gameobject that passed through the soil collider
        Destroy(seed.gameObject);

        // Instantiate the plant at tiny scale on the plant spawn point
        plantInstance = Instantiate(plantPrefab, plantSpawnPoint.transform);
        plantInstance.transform.localPosition = new Vector3(0, 0, 0);
        plantInstance.transform.localScale = new Vector3(0.0002f, 0.0002f, 0.0002f);
        // randomize the rotation of the pot, to create aesthetic variability
        plantInstance.transform.eulerAngles = new Vector3(plantInstance.transform.eulerAngles.x,
                                                          Random.Range(0, 359.9f),
                                                          plantInstance.transform.eulerAngles.z);
        // Set up the plant class on the instance; this adds variety
        // by varying the growth rate, water use speed (et), and others (see Plant.cs)
        plantInstance.GetComponent<Plant>().Init();
        plantInstance.SetActive(false); // hide the plant (until germinated)
    }


    // public method for whne the plant get's too big and breaks the pot
    public void Shatter()
    {
        // Play the particles
        BreakEffect.Play();
        // Play the spatial audio
        GetComponent<AudioSource>().Play();
        // Destroy the plant instance (it's dead)
        Destroy(plantInstance.gameObject);
        // give them a new seed
        FindObjectOfType<SeedChute>().FeedASeed();
        // reset control variables
        seeded = false;
        germinated = false;
        waterInPot = 0;
    }


    // Public method for when a viable turnip goes in the box
    public void Score()
    {
        // Max lossy scale is 1, so just take the proportion of full growth'
        // and multiply by 100 to score in nice round numbers
        manager.score += Mathf.Round(plantInstance.transform.lossyScale.x * 100);

        // Destroy the successfully harvested plant
        Destroy(plantInstance.gameObject);

        // New seed
        FindObjectOfType<SeedChute>().FeedASeed();
        // reset control variables
        seeded = false;
        germinated = false;
        waterInPot = 0;
    }

    // Co-routine to account for germination time
    IEnumerator StartGerminationLag()
    {
        // Wait the germination lag and then flag it as germinated
        yield return new WaitForSeconds(germTimeLag);
        plantInstance.SetActive(true); // show the plant when germinated
        germinated = true; // set germinated flag to true
    }
}
