using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameManager gameManagerScript;

    private void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // If the Out of bounds border (trigger) gets hit the bullet should destroy itself
    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        // Checking if the rock was hit by a bullet.
        if (collision.tag.Equals("Rock"))
        {
            gameManagerScript.RockDestroyed();

            // Destroying the rock and the bullet.
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
        }

        if (collision.tag == "OutOfBoundsBorder") 
        {
            Destroy(this.gameObject);
        }
    }

}
