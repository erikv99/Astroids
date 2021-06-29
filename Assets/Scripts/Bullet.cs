using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // If the Out of bounds border (trigger) gets hit the bullet should destroy itself
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "OutOfBoundsBorder") 
        {
            Destroy(this.gameObject);
        }
    }
}
