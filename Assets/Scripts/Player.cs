using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int movementSpeed = 250;
    public int bulletSpeed = 400;
    public int rotationDegreesPerSecond = 360;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        // Getting the rigid body of the player
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    // Function for handeling the general movement of the player/spaceship
    private void Movement() 
    {
        // Fire gun
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Vector3 spawnPosition = this.gameObject.transform.Find("BulletSpawn").position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, this.gameObject.transform.rotation);
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(transform.up * bulletSpeed);
        }
        // Left/right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * (movementSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.right * (-movementSpeed * Time.deltaTime));
        }

        // Up/Down
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.up * (movementSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.up * (-movementSpeed * Time.deltaTime));
        }

        // Rotate left/right
        float rotateAmount = rotationDegreesPerSecond * Time.deltaTime;
        float currentRotation = transform.rotation.eulerAngles.z;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotation + rotateAmount));
        }
        if (Input.GetKey(KeyCode.RightArrow)) 
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRotation - rotateAmount));
        }
    }

}
