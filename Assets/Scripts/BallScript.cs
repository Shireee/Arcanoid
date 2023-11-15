using UnityEngine;
public class BallScript : MonoBehaviour
{
    public Vector2 ballInitialForce;
    Rigidbody2D rb;
    GameObject playerObj;
    float deltaX;
    AudioSource audioSrc;
    public AudioClip hitSound;
    public AudioClip loseSound;
    public GameDataScript gameData;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");// init player object
        deltaX = transform.position.x;
        audioSrc = Camera.main.GetComponent<AudioSource>(); // init audio source
    }

    void Update()
    {
        if (rb.isKinematic)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                rb.isKinematic = false; // switching to Dynamic body type
                rb.AddForce(ballInitialForce); // giving initial force to the ball
            }
            else // if not kinematic follow to the player object
            {
                var pos = transform.position;
                pos.x = playerObj.transform.position.x + deltaX;
                transform.position = pos;
            }
        }

        // Changing ball trajectory by pressing J
        if (!rb.isKinematic && Input.GetKeyDown(KeyCode.J))
        {
            var v = rb.velocity;
            if (Random.Range(0, 2) == 0)
                v.Set(v.x - 0.1f, v.y + 0.1f);
            else
                v.Set(v.x + 0.1f, v.y - 0.1f);
            rb.velocity = v;
        }
    }

    // Add get-score sound when destroying block
    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameData.sound) audioSrc.PlayOneShot(loseSound, 5);
        Destroy(gameObject);
        playerObj.GetComponent<PlayerScript>().BallDestroyed();
    }

    // Add hit sound when touch block
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Yellow Block"))
        {
            ContactPoint2D contact = collision.GetContact(0); // Получение информации о контакте

            Vector2 normal = contact.normal; // Получение нормали к поверхности столкновения

            // Отражение вектора скорости относительно нормали
            Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, normal);

            rb.velocity = reflectedVelocity; // Установка отраженной скорости для шарика
        }

        if (gameData.sound) audioSrc.PlayOneShot(hitSound, 4);
    }


}
