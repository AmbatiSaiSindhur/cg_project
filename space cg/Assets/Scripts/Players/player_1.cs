using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_1 : MonoBehaviour {

    [SerializeField]
    float hp = 5f;
    [SerializeField]
    float vel = 5f;
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    GameObject instPoint_1;
    [SerializeField]
    GameObject light;

    GameObject go;
    private Rigidbody2D _rb;
    private Vector3 lastPosition;
    private float totalDistance = 0f;
    private float scaleIncreaseThreshold = 5f; // Distance threshold for scaling up
    private Vector3 originalScale;

    // Use this for initialization
    void Start ()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        lastPosition = transform.position;
        originalScale = transform.localScale;
    }
	
    // Update is called once per frame
    void Update () {

        // Check for movement inputs
        if (Input.GetKey(KeyCode.W)) _rb.velocity = new Vector3(_rb.velocity.x, vel, 0);
        if (Input.GetKeyUp(KeyCode.W)) _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);

        if (Input.GetKey(KeyCode.S)) _rb.velocity = new Vector3(_rb.velocity.x, -vel, 0);
        if (Input.GetKeyUp(KeyCode.S)) _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);

        if (Input.GetKey(KeyCode.A)) _rb.velocity = new Vector3(-vel, _rb.velocity.y, 0);
        if (Input.GetKeyUp(KeyCode.A)) _rb.velocity = new Vector3(0, _rb.velocity.y, 0);

        if (Input.GetKey(KeyCode.D)) _rb.velocity = new Vector3(vel, _rb.velocity.y, 0);
        if (Input.GetKeyUp(KeyCode.D)) _rb.velocity = new Vector3(0, _rb.velocity.y, 0);

        // Shooting logic
        if (Input.GetKeyDown(KeyCode.T))
        {
            light.transform.position = instPoint_1.transform.position;
            Invoke("StopLight", 0.3f);
            go = Instantiate(bullet, instPoint_1.transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody2D>().velocity = vel * Vector2.right;
            Destroy(go, 3f);
        }

        // Calculate distance traveled since the last frame
        float distanceTraveled = Vector3.Distance(lastPosition, transform.position);
        totalDistance += distanceTraveled;

        // Update last position
        lastPosition = transform.position;

        // Increase size when the total distance threshold is reached
        if (totalDistance >= scaleIncreaseThreshold)
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0f); // Increase the size slightly
            totalDistance = 0; // Reset the distance tracker
        }
    }

    void StopLight()
    {
        light.transform.position = new Vector3(0, 20, 0);
    }
}
