using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // Required for file writing

public class players : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; 
    public Sprite skin1;
    public Sprite skin2;
    public Sprite skin3;
    public Sprite skin4;
    public Sprite skin5;
    public Sprite skin6;
    public Sprite skin7;
    public Sprite skin8;
    public Sprite skin9;

    private Animator animator; 
    public RuntimeAnimatorController anim1; 
    public RuntimeAnimatorController anim2; 
    public RuntimeAnimatorController anim3;
    public RuntimeAnimatorController anim4;
    public RuntimeAnimatorController anim5;
    public RuntimeAnimatorController anim6;
    public RuntimeAnimatorController anim7;
    public RuntimeAnimatorController anim8;
    public RuntimeAnimatorController anim9;

    public float maxHP = 100f;
    public float HP = 100f;

    private Transform healthBar;
    public Text textHP;
    
    public float vel = 5f;
    public bool playerNumber;

    public GameObject bullet;
    public GameObject instPoint;
    public GameObject explosion;
    public GameObject background;
    public GameObject BG;

    public AudioClip pju;
    public float playerWidth;
    public float playerHeight;
    public float bulletWidth;
    public float bulletHeight;
    public float backgroundWidth;
    public float backgroundHeight;
    public float backgroundOffsetX;
    public float backgroundOffsetY;

    int r, skin;
    GameObject go;
    private Rigidbody2D _rb;
    private AudioSource source;
   
    KeyCode upKey;
    KeyCode downKey;
    KeyCode leftKey;
    KeyCode rightKey;
    KeyCode fireKey;

    private Vector2 startPosition; // To track the starting position
    private float playTime = 0f; // Timer to track the time played

    private string filePath = "PlayerData.csv"; // Path to the CSV file

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "enemy"){
            Instantiate(explosion, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            HP -= 10;
            if (HP <= 0) dead();
        }
        else if (collision.gameObject.transform.tag == "bullet"){
            Instantiate(explosion, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            HP -= 5;
            if (HP <= 0) dead();
        }
        else if (collision.gameObject.transform.tag == "asteroid"){
            Instantiate(explosion, collision.gameObject.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            HP -= 20;
            if (HP <= 0) dead();
        }
    }

    void dead(){
        gameObject.SetActive(false);
        _rb.transform.position = new Vector3(-50, -50, -50);
        
        // Log the time played to the CSV file when the player dies
        LogPlayTime();
    }

    void Start()
    {
        if(playerNumber) skin = PlayerPrefs.GetInt("player1skin");
        else skin = PlayerPrefs.GetInt("player2skin");

        switch (skin)
        {
            case 0: this.GetComponent<Animator>().runtimeAnimatorController = anim1; break;
            case 1: this.GetComponent<Animator>().runtimeAnimatorController = anim2; break;
            case 2: this.GetComponent<Animator>().runtimeAnimatorController = anim3; break;
            case 3: this.GetComponent<Animator>().runtimeAnimatorController = anim4; break;
            case 4: this.GetComponent<Animator>().runtimeAnimatorController = anim5; break;
            case 5: this.GetComponent<Animator>().runtimeAnimatorController = anim6; break;
            case 6: this.GetComponent<Animator>().runtimeAnimatorController = anim7; break;
            case 7: this.GetComponent<Animator>().runtimeAnimatorController = anim8; break;
            case 8: this.GetComponent<Animator>().runtimeAnimatorController = anim9; break;
        }

        source = GetComponent<AudioSource>();
        source.clip = pju;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.isKinematic = true;
        
        playerWidth = GetComponent<Collider2D>().bounds.size.x;
        playerHeight = GetComponent<Collider2D>().bounds.size.y;
        bulletHeight = bullet.GetComponent<Collider2D>().bounds.size.x;
        bulletWidth = bullet.GetComponent<Collider2D>().bounds.size.y;

        background = GameObject.Find("background");
        backgroundHeight = background.GetComponent<MeshCollider>().bounds.size.y;
        backgroundWidth = background.GetComponent<MeshCollider>().bounds.size.x;
        backgroundOffsetX = background.GetComponent<MeshCollider>().transform.position.x;
        backgroundOffsetY = background.GetComponent<MeshCollider>().transform.position.y;

        instPoint.transform.position = new Vector2(_rb.transform.position.x + playerWidth + bulletWidth, _rb.transform.position.y);
        
        if (playerNumber)
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            leftKey = KeyCode.D;
            rightKey = KeyCode.A;
            fireKey = KeyCode.T;
        }
        else
        {
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            leftKey = KeyCode.RightArrow;
            rightKey = KeyCode.LeftArrow;
            fireKey = KeyCode.Period;
        }

        // Store the starting position of the player
        startPosition = transform.position;

        // Create or open the CSV file to log data
        if (!File.Exists(filePath))
        {
            // Create a new file with headers if it doesn't exist
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Player 1,Player 2");
            }
        }
    }

void Update()
{
    // Adjust speed based on HP
    AdjustSpeedBasedOnHP();

    if (playerNumber) textHP.text = "   Player 1: " + HP + " / " + maxHP;
    else textHP.text = "   Player 2: " + HP + " / " + maxHP;

    // Determine the constraints based on HP
    float verticalConstraint;
    if (HP > 60) 
    {
        verticalConstraint = 3.0f; // Stricter constraint when HP is 60-100
    }
    else 
    {
        verticalConstraint = 5.0f; // More lenient constraint when HP is less than 60
    }
    float horizontalConstraint = 8.0f; // Keep horizontal constraint constant

    // Apply vertical constraints
    if (((transform.position.y + playerHeight / 2) >= verticalConstraint) || 
        ((transform.position.y - playerHeight / 2) <= -verticalConstraint)) 
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
    }

    // Apply horizontal constraints
    if (((transform.position.x + playerWidth / 2) >= horizontalConstraint) || 
        ((transform.position.x - playerWidth / 2) <= -horizontalConstraint)) 
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
    }

    // Movement controls
    if (Input.GetKeyUp(upKey)) _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);
    else if (Input.GetKey(upKey) && ((transform.position.y + playerHeight / 2) < verticalConstraint)) 
        _rb.velocity = new Vector3(_rb.velocity.x, vel, 0);

    if (Input.GetKeyUp(downKey)) _rb.velocity = new Vector3(_rb.velocity.x, 0, 0);
    else if (Input.GetKey(downKey) && ((transform.position.y - playerHeight / 2) > -verticalConstraint)) 
        _rb.velocity = new Vector3(_rb.velocity.x, -vel, 0);

    if (Input.GetKeyUp(leftKey)) _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
    else if (Input.GetKey(leftKey) && ((transform.position.x + playerWidth / 2) < horizontalConstraint)) 
        _rb.velocity = new Vector3(vel, _rb.velocity.y, 0);

    if (Input.GetKeyUp(rightKey)) _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
    else if (Input.GetKey(rightKey) && ((transform.position.x - playerWidth / 2) > -horizontalConstraint)) 
        _rb.velocity = new Vector3(-vel, _rb.velocity.y, 0);

    if (Input.GetKeyDown(fireKey))
    {
        go = Instantiate(bullet, instPoint.transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().velocity = vel * 1.5f * Vector2.right;
        source.PlayOneShot(pju, 1f);
        Destroy(go, 3f);
    }
}

    void AdjustSpeedBasedOnHP()
    {
        // Sets speed based on HP value
        if (HP > 60)
        {
            vel = 5f;
        }
        else
        {
            vel = 10f;
        }

        float scaleMultiplier = 1 + (100f - HP) * 0.005f; 
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

   void LogPlayTime()
{
    // Format the play time to two decimal places
    string formattedPlayTime = playTime.ToString("F2");

    // If playerNumber is true (Player 1), append the time in the Player 1 column.
    // If playerNumber is false (Player 2), append the time in the Player 2 column.
    string player1Time = (playerNumber ? formattedPlayTime : "");
    string player2Time = (playerNumber ? "" : formattedPlayTime);

    // Check if the file already has data (other than the header)
    if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
    {
        // Append the times to the last row of the CSV file
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(player1Time + "," + player2Time); // Append both players' times in the same row
        }
    }
    else
    {
        // If file is empty (just created), write header and then the data for the first time
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Player 1,Player 2"); // Write header
            writer.WriteLine(player1Time + "," + player2Time); // Write the first row with player times
        }
    }
}

}
