using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

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
    private float launchRockInterval = 1;
    private bool launchRockIntervalChanged = false;
    private int currentLevel = 1;
    private int maxLevel = 10;
    private int playerScore = 0;
    private int scoreNeededForNextLevel = 100;
    private bool keepLaunchingRocks = true;
    private bool playerDied = false;
    private string[] astroidNames;
    private bool gameLost = false;
    
    public int rockSpeed = 100;
    public float playerRespawnTime = 3;
    public int playerLivesLeft = 3; // Stored here since player/spaceship obj will be destroyed

    // Start is called before the first frame update
    void Start()
    {
        GetAstroidNames();
        
        // Layer 6 = field border, layer 7 = bullets and rocks. these should not bounce back inside camera view. player should
        Physics2D.IgnoreLayerCollision(6, 7, true);

        // Announcing the level
        StartCoroutine(AnnounceLevel());

        // Launching a rock every x seconds.
        InvokeRepeating("LaunchRocks", 1.0f, launchRockInterval);
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
            InvokeRepeating("LaunchRocks", 1.0f, launchRockInterval);
            launchRockIntervalChanged = false;
        }

        if (playerDied && !gameLost) 
        {
            if (playerRespawnTime > 0)
            {
                playerRespawnTime -= Time.deltaTime;
            }
            else
            {
                PlayerRespawn();
            }
        }

        // Updating all the text boxes.
        level.text = "Level: " + currentLevel.ToString() + " / " + maxLevel.ToString();
        livesLeft.text = "Lives left: " + playerLivesLeft.ToString();
        score.text = "Score: " + playerScore.ToString() + " / " + scoreNeededForNextLevel.ToString();
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
                ranRotation = UnityEngine.Random.Range(0, -40);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(-9, 0, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddRelativeForce(Vector3.right * rockSpeed);
                break;

            case "right":
                ranRotation = UnityEngine.Random.Range(-20, 40);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(9, 0, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddRelativeForce(Vector3.right * -rockSpeed);
                break;

            case "top":
                ranRotation = UnityEngine.Random.Range(-120, -240);
                rotation = Quaternion.Euler(0, 0, ranRotation);
                rock = Instantiate(rockPrefab, new Vector3(0, 5, -1), rotation);
                rb = rock.GetComponent<Rigidbody2D>();
                rb.AddRelativeForce(Vector3.up * rockSpeed);
                break;
        }
    }
    private void LaunchRocks()
    {
        int randomNum = UnityEngine.Random.Range(1, 4);

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
        // Checking if game is won.
        if (currentLevel == 10)
        {
            StartCoroutine(GameWon());
        }

        // Increasing level and score needed needed
        currentLevel += 1;
        scoreNeededForNextLevel = scoreNeededForNextLevel + 100;
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
        if (playerLivesLeft == 0)
        {
            gameLost = true;
            StartCoroutine(GameLost());
            return;
        }

        keepLaunchingRocks = false;
        DeleteAllRocks();
        playerLivesLeft -= 1;
        PrintMessage("Live lost, respawning in " + playerRespawnTime.ToString() + " seconds");
        playerDied = true;
    }

    private void PlayerRespawn() 
    {
        Instantiate(playerPrefab);
        playerDied = false;
        keepLaunchingRocks = true;
        InvokeRepeating("LaunchRocks", 2.0f, launchRockInterval);
        playerRespawnTime = 3;
        PrintMessage("");
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

        if (playerScore >= scoreNeededForNextLevel) 
        {
            AnnounceLevel();
            NextLevel();
        }
    }

    private void GetAstroidNames() 
    {
        // Running the .exe of the python script (no python needed on pc) then handleing the output
        string procToRun = Directory.GetCurrentDirectory() + "\\Assets\\Scripts\\GetAstroidNames.exe";
        Process proc = new Process();
        proc.StartInfo.FileName = "GetAstroidNames.exe";
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.Arguments = procToRun + " 250";
        proc.Start();
        StreamReader sr = proc.StandardOutput;
        astroidNames = sr.ReadToEnd().Replace("[", "").Replace("]", "").Split(',');
        proc.WaitForExit();
    }

    // Function to announce which level it is and which astroid has shatterd.
    private IEnumerator AnnounceLevel() 
    {
        // Getting a astroid name from our astroid list.
        int ran = UnityEngine.Random.Range(0, astroidNames.Length);
        string astroidName = astroidNames[ran];

        // Printing the announcement.
        PrintMessage("Astroid " + astroidName + " has been shattered. Avoid getting hit!");

        // waiting 3 seconds then removing the message
        yield return new WaitForSeconds(3);
        PrintMessage("");
    }

    private IEnumerator GameLost() 
    {
        CancelInvoke();
        DeleteAllRocks();
        PrintMessage("Game lost. Restarting in 10 seconds!");
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private IEnumerator GameWon() 
    {
        
        keepLaunchingRocks = false;
        DeleteAllRocks();
        PrintMessage("Game won. Restarting in 10 seconds!");
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
