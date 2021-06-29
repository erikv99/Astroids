using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // If it isnt another rock we want the rock to destroy (it means it has touched a border or player)
        if (collision.tag.ToLower() != "rock") 
        {
            // If the rock hit a player we destroy the rock and the player
            if (collision.tag.ToLower() == "player")
            {
                Destroy(collision.gameObject);
                gameManagerScript.LostLive();
            }
            // If the rock didnt hit a player it hit a border so it will be destroyed as well
            else 
            {
                Debug.Log("Destroyed rock (OOB)");
            }
            Destroy(this.gameObject);
        }
    }
}
