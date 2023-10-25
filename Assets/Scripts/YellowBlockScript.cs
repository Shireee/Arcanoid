using UnityEngine;


public class YellowBlockScript : BlockScript
{
    // if the a problem with slowing ball when it hit this block, change mass of the block in Rigid2D
    Rigidbody2D rb;
    public Vector2 ballInitialForce;
    public float speed = 2f;
    private bool moveRight = true;


    override public void Start()
    {
        base.Start();  
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(ballInitialForce); // giving initial force to the ball
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // preserve from rotation
    }

    // Add velocity to the block
    void Update()
    {
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.x = moveRight ? speed : -speed;
        rb.velocity = currentVelocity;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Wall"))
        {
            moveRight = !moveRight; // Changing direction
        }
    }
}
