using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

    public int dayLengthSeconds = 24; // what number should reset the clock
    public Color noon; // skybox solid color for day
    public Color midnight; // skybox solid color for night
    public Vector2 heightInSkyRange = new Vector2(-12, 12);
    public bool startAtNight; // what point in the day at runtime?
    //public float rateMultiplier = 3.0f; // how fast shoudl time go

    private Vector3 noonPosition; // where should the sun object be at noon
    private Vector3 midnightPosition; // where should the sun object be at midnight

	// Use this for initialization
	void Start () {
        // Set the appropriate starting color based on selections
        if (startAtNight) Camera.main.backgroundColor = midnight;
        else Camera.main.backgroundColor = noon;

        // Place the this gameObject (sun) at the starting location (day/night)
        midnightPosition = new Vector3(transform.position.x, heightInSkyRange.x, transform.position.z);
        noonPosition = new Vector3(transform.position.x, heightInSkyRange.y, transform.position.z);
        if (startAtNight) transform.position = midnightPosition; // set at lowest point
        else transform.position = noonPosition; // set at apex
    }
	
	// Update is called once per frame
	void Update () {
        // Use color.lerp to transition between skybox day/night states at low cost
        // Retrieved from unity manual (https://docs.unity3d.com/ScriptReference/Color.Lerp.html)
        if (startAtNight) Camera.main.backgroundColor = Color.Lerp(midnight, noon, Mathf.PingPong(Time.time / (dayLengthSeconds / 2), 1));
        else Camera.main.backgroundColor = Color.Lerp(noon, midnight,  Mathf.PingPong(Time.time / (dayLengthSeconds / 2), 1));


        // constantly update the position of the sun between height ranges across the day/night cycle time
        var tmp = transform.position;
        if (startAtNight) tmp.y = Vector3.Lerp(midnightPosition, noonPosition, Mathf.PingPong(Time.time / (dayLengthSeconds / 2), 1)).y;
        transform.position = tmp;
    }
}
