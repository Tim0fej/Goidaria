using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health")]
    public Image healthImage;

    [Header("Wall Sliding")]
    public float wallCheckDistance = -0.87f;
    public float wallSlideSpeed = 2f;

    private bool isTouchingWall;
    private bool isWallSliding;

    private int health = 100;
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

        healthImage.fillAmount = health / 100;

        HandleWallSlide(moveInput);
    }

    private void FixedUpdate()
    {
        isTouchingWall = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left : Vector2.right, wallCheckDistance, groundLayer);
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
            if(isWallSliding)
            {
                animator.Play("Wall-Slide");
            }
            else if(rb.linearVelocityY > 0)
            {
                animator.Play("jump");
            }
            else
            {
                animator.Play("Fall");
            }
        }
    }

    private void HandleWallSlide(float moveInput)
    {
        if (isTouchingWall && !isGrounded && moveInput != 0 && rb.linearVelocityY < 0)
        {
            isWallSliding = true;
            rb.linearVelocityY = -wallSlideSpeed;
        }
        else
        {
            isWallSliding = false;
        }
    }
}
