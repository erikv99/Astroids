using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // This class will handle spawning rocks, keeping score etc
    public GameObject rockPrefab;
    public GameObject playerPrefab;

    public Text livesLeft;
    public Text score;
    public Text messagePlaceHolder;
    public Text level;
    
    private string messagePlaceholderTxt;
    private float launchRockInterval = 2;
    private bool launchRockIntervalChanged = false;
    private int rocksNeededToPassLevel = 5;
    private int currentLevel = 1;
    private int maxLevel = 10;
    private int rocksTakenOut = 0;
    private int playerScore = 0;
    private bool keepLaunchingRocks = true;
    private bool playerDied = false;
    
    public int rockSpeed = 150;
    public float playerRespawnTime = 3;
    public int playerLivesLeft = 3; // Stored here since player/spaceship obj will be destroyed

    // Start is called before the first frame update
    void Start()
    {

        // Layer 6 = field border, layer 7 = bullets and rocks. these should not bounce back inside camera view. player should
        Physics2D.IgnoreLayerCollision(6, 7, true);

        // Launching a rock every x seconds.
        InvokeRepeating("LaunchRocks", 2.0f, launchRockInterval);
    }

    // Update is called once per frame
    void Update()
    {
        // Cancelling the rock launching if needed.
        if (keepLaunchingRocks == false) 
        {
            CancelInvoke();
        }

        // Re invoking the repeating of launchrocks when the value of launchRockInterval is changed.
        if (launchRockIntervalChanged) 
        {
            CancelInvoke();
            InvokeRepeating("LaunchRocks", 2.0f, launchRockInterval);
            launchRockIntervalChanged = false;
        }

        if (playerDied) 
        {
            if (playerRespawnTime > 0)
            {
                playerRespawnTime -= Time.deltaTime;
            }
            else 
            {
                Debug.Log("lessthen");
                // Spawning new player object
                Instantiate(playerPrefab);
                InvokeRepeating("LaunchRocks", 2.0f, launchRockInterval);
                playerDied = false;
                playerRespawnTime = 3;
            }
        }

        // Updating all the text boxes.
        level.text = "Level: " + currentLevel.ToString() + "/" + maxLevel.ToString();
        livesLeft.text = "Lives left: " + playerLivesLeft.ToString();
        score.text = "Score: " + playerScore.ToString();
        messagePlaceHolder.text = messagePlaceholderTxt;
    }

    // Launch a rock from either left, right or top side.
    private void LaunchRock(string startingSide)
    {
        int ranRotation;
        Rigidbody2D rb;
        GameObject rock;
        Quaternion rotation;

        switch (startingSide)
        {
            case "left":
                ranRotation = Random.Range(-60, -110);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(-9, 0, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.right * rockSpeed);
                break;

            case "right":
                ranRotation = Random.Range(-110, -60);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(9, 0, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.right * -rockSpeed);
                break;

            case "top":
                ranRotation = Random.Range(-40, 40);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(0, 5, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddForce(transform.up * -rockSpeed);
                break;
        }
    }

    private void LaunchRocks()
    {
        int randomNum = Random.Range(1, 4);

        switch (randomNum)
        {
            case 1:
                LaunchRock("left");
                break;
            case 2:
                LaunchRock("top");
                break;
            case 3:
                LaunchRock("right");
                break;
        }
    }
    public void NextLevel() 
    {
        // Increasing level and rocks needed
        currentLevel += 1;
        rocksNeededToPassLevel = (int)(rocksNeededToPassLevel * 1.5);
        launchRockInterval -= 0.1f;

        if (launchRockInterval <= 0)
            launchRockInterval = 0.1f;

        launchRockIntervalChanged = true;
    }

    // Print a message to the message placeholder
    public void PrintMessage(string message) 
    {
        messagePlaceholderTxt = message;
    }

    // Function for handeling losing a live
    public void LostLive()
    {
        keepLaunchingRocks = false;
        DeleteAllRocks();
        playerLivesLeft -= 1;
        PrintMessage("Live lost, respawning in " + playerRespawnTime.ToString() + " seconds");
        playerDied = true;
    }

    // Function to delete all rocks currently in the scene
    private void DeleteAllRocks() 
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");
        
        foreach (GameObject rock in rocks) 
        {
            Destroy(rock);
        }
    }

    // Function to handle a rock being destroyed
    public void RockDestroyed() 
    {
        playerScore += 10;
        rocksTakenOut += 1;
    }
}
