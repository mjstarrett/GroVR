using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    public float timeLimit = 300; // game duration in seconds

    public GameObject player; // over player object (for rotation info)
    public GameObject startBoard; // the panel for the start board
    public GameObject endBoard; // the panel for the endboard

    [HideInInspector]
    public float score; // current point total

    private bool timerStarted; // flag to say if they player is playing
    private bool gameOver; // mark if we are in the endgame or playing
    private float startTime; // when did the game start
    private float elapsedTime; // how long since start?
    private float timeLeft; // how much time remains?


	// Use this for initialization
	void Start () {
        // how much time is left at the beginning
        timeLeft = timeLimit;

        // hide the end board
        endBoard.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        // Update the endboard score
        GameObject.FindWithTag("Score").GetComponent<Text>().text = score.ToString();

        // display the time
        // CURRENTLY COUNTS DOWN BEFORE GRABBING THE SEED
        GameObject.FindWithTag("Timer").GetComponent<Text>().text = timeLeft.ToString() + " sec";


        // Keep the endboard score report updated even when it's invisible
        endBoard.GetComponent<GameOver>().scoreText.text = score.ToString() + " points";

        // Update the message players get depending on their score
        if (score > 1 && score <= 300)
        {
            endBoard.GetComponent<GameOver>().message.text = "Well at least you tried.";
        }
        else if (score > 300 && score <= 600)
        {
            endBoard.GetComponent<GameOver>().message.text = "Don't quit your day-job.";
        }
        else if (score > 600 && score <= 1000)
        {
            endBoard.GetComponent<GameOver>().message.text = "Look at you and your green thum!";
        }
        else if (score > 1000)
        {
            endBoard.GetComponent<GameOver>().message.text = "Amazing job! Are you a professional?";
        }
        else endBoard.GetComponent<GameOver>().message.text = "Is this thing on?";

        // Only count time if we're in the play state
        if (timerStarted && !gameOver)
        {
            // update the time
            elapsedTime = Time.time - startTime;
            timeLeft = Mathf.Round(timeLimit - elapsedTime);

            // What if time is up?
            if (elapsedTime >= timeLimit)
            {
                // activate the end board and allow restart
                EndGame();
            }
        }

        // Allow the game to be restarted with the click of any button
        if (gameOver && (Input.GetKeyDown(KeyCode.R) || OVRInput.GetDown(OVRInput.Button.Any)))
        {
            // if the game is over and any key is hit, reset the gamestate 
            ResetGame();
        }
    }

    // method to start the game, accessible to other scripts
    public void StartClock()
    {
        startBoard.SetActive(false); // turn off the start board
        timerStarted = true; // flag the timer as running
        startTime = Time.time; // set the base time to compare for elapsed time
    }


    // report the score and explain how to play again
    public void EndGame()
    {
        // destroy anything and get the game back in pre-potted state
        FindObjectOfType<PottedPlant>().Shatter();

        gameOver = true; // flag the state of the game

        // temporarily make the endboard a child of the player object
        var oldparent = endBoard.transform.parent;
        endBoard.transform.parent = player.transform;

        // put the end board in front of the player (without changing height or distance from player)
        endBoard.transform.localPosition = new Vector3(0, 0.5f, 1.5f);
        endBoard.transform.eulerAngles = new Vector3(endBoard.transform.eulerAngles.x,
                                                     player.transform.eulerAngles.y,
                                                     endBoard.transform.eulerAngles.z);
        // Now put it back to being free
        endBoard.transform.parent = oldparent;

        // show the end board (start should already be hidden
        endBoard.SetActive(true);
    }


    // return to the start state
    public void ResetGame()
    {
        endBoard.SetActive(false); // turn off the end board

        // reset all the game variables
        timerStarted = false;
        score = 0;
        elapsedTime = 0;
        timeLeft = timeLimit;

        startBoard.SetActive(true); // turn on the start board

        gameOver = false; // put us back into game state (flag not gameover)
    }
}
