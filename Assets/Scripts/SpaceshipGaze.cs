using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceshipGaze : MonoBehaviour
{
    public GameObject UFOPrefab;         //A variable that holds the spaceship prefab (dragged and dropped on the Inspector)
    public GameObject killEffect;        //A variable that holds the explosion particle prefab
    private GameObject UFOInstance;      //A variable that holds the instance of the spaceship prefab
    public GameObject[] asteroids;       //An array that holds the five asteroid prefabs (dragged and dropped on the Inspector)
    private GameObject AsteroidInstance; //A variable that holds the instantiated astroid instance
    public Material ReticleMaterial;     //A variable that holds the reticle's material for color change

    public ParticleSystem hitEffect;     //A variable that holds the sparks particle (already in the Hierarchy)

    public float timeToSelect = 3.0f;    //A variable that holds the time to look at the spaceship for it to explode (3 seconds)
    private float countDown;             //A variable that counts down from the timeToSelect
    private float asteroidPosX;          //A variable that holds the x position of the asteroid, this will be used to move the instantiated
                                         //asteroids to have the required spacing between them

    private float rotationSpd = 10.0f;   //A variable that holds the rotational speed of the spaceship

    private int score;                   //A variable that holds the score
    private int asteroidIndex;           //A variable that holds the asteroid index to pick asteroid prefabs from the asteroids array

    void Start()
    {
        score = 0;
        countDown = timeToSelect;
        asteroidPosX = -5.0f; //We'll start instantiating the asteroids from x = -5 and give 1 unit spacing between each

        ReticleMaterial.color = Color.blue; //We are setting the reticle's material color to blue

        //We instantiate the spaceship at the position x = 10, y = random between 1.0 and 6.0, and z = 0
        UFOInstance = Instantiate(UFOPrefab, new Vector3(10.0f, Random.Range(1.0f, 6.0f), 0f), UFOPrefab.transform.rotation);

        //We then rotate the spaceship around the origin axis by a random degree between 0 and 360. This ensures the
        //requirement of "instantiation at a random point on the imaginary cylindrical surface"
        UFOInstance.transform.RotateAround(Vector3.zero, -Vector3.up, Random.Range(0, 360));
    }

    void Update()
    {
        RaycastHit hit;
        Transform camera = Camera.main.transform;
        Ray ray = new Ray(camera.position, camera.transform.forward);

        //We are rotating the spaceship around the origin
        UFOInstance.transform.RotateAround(Vector3.zero, -Vector3.up, rotationSpd * Time.deltaTime);

        //If the user hasn't exploded 10 spaceships yet
        if (score < 10)
        {
            //We cast a ray and look for hits with the spaceship (the spaceship is tagged as Ship on the Inspector).
            if (Physics.Raycast(ray, out hit) && (hit.collider.tag == "Ship"))
            {
                //If the user is looking at the spaceship, we change the reticle's material color to yellow
                ReticleMaterial.color = Color.yellow;

                //If the spaceship hasn't exploded yet (the user hasn't looked for 3 continuous seconds)
                if (countDown > 0.0f)
                {
                    if (!hitEffect.isPlaying)
                        hitEffect.Play(); //We play the hit effect particle (sparks)

                    countDown -= Time.deltaTime;
                    hitEffect.transform.position = hit.point; //We update the sparks particle system's position as the hit point of the raycast

                    UFOInstance.transform.GetComponent<Renderer>().material.color = Color.yellow; //We change the spaceship's material color to yellow
                }

                else //If the user has looked at the spaceship for three continuous seconds
                {
                    //We instantiate the explosion particle at the spaceship's position and rotation
                    Instantiate(killEffect, UFOInstance.transform.position, UFOInstance.transform.rotation);

                    score++;
                    countDown = timeToSelect;
                    SetRandomPosition(); //We call this method to get a new random position for the spaceship
                    asteroidIndex = Random.Range(0, 5); //We pick a random index for the asteroid prefab

                    //We instantiate the asteroid prefab
                    AsteroidInstance = Instantiate(asteroids[asteroidIndex], new Vector3(asteroidPosX, asteroids[asteroidIndex].transform.position.y, 2.0f), Quaternion.identity);
                    asteroidPosX += 1.0f; //We give one unit spacing in x-axis
                    AsteroidInstance.transform.GetComponent<Renderer>().material.color = Random.ColorHSV(); //We assign a random color to the asteroid

                    if (score == 10) //If the user has exploded 10 spaceships
                    {
                        //We revert the spaceship's material color to white (no additional color will be projected on the texture)
                        UFOInstance.transform.GetComponent<Renderer>().material.color = Color.white;
                        //We change the reticle's material color to blue
                        ReticleMaterial.color = Color.blue;
                        //We stop the spark particle system and clear the already emitted particles
                        hitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                    }
                }
            }

            else //If the user has lost contact with the spaceship (looked somewhere else)
            {
                UFOInstance.transform.GetComponent<Renderer>().material.color = Color.white; //We revert the spaceship's material color to white
                ReticleMaterial.color = Color.blue; //We revert the reticle's material color to blue
                countDown = timeToSelect; //We reset the selection countdown
                hitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); //We stop the spark particle system
            }
        }
    }

    void SetRandomPosition() //This method sets a new random position and a random rotational speed for the spaceship
    {
        float y = Random.Range(1.0f, 6.0f);
        UFOInstance.transform.position = new Vector3(10.0f, y, 0f);
        UFOInstance.transform.rotation = UFOPrefab.transform.rotation;
        UFOInstance.transform.RotateAround(Vector3.zero, -Vector3.up, Random.Range(0, 360));
        rotationSpd = Random.Range(10.0f, 40.0f);
    }

    void OnDisable()
    {
        ReticleMaterial.color = Color.white; //We revert the reticle's material color to white when this script instance is
                                             //destroyed, so that the material's color remains as it was in the project originally
    }

}

