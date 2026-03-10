using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity= new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; 

        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true; 
        }

        if(Input.GetAxis("Jump") > 0 && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        SetAnimation(moveInput);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void SetAnimation(float moveInput)
    {
        if(isGrounded)
        {
            if(rb.linearVelocityX == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Run");
            }
        }
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("jump");
            }
            else
            {
                animator.Play("Fall");
            }
        }
    }
}
