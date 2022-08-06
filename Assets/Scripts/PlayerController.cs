using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    //********* Input Direction ************//
    private float moveInput;


    //********* Jump count tracker **************//
    private int jumpsLeft;
    private int facingDirection = 1;


    //********* State declarations **************//
    private bool isFacingRight = true;
    private bool isRunning;
    private bool isGrounded;
    private bool isWalled;
    private bool isWallSliding;
    private bool canJump;


    //********** Component variables **************//
    private Rigidbody2D rb;
    private Animator animator;


    //********* Public Component Variables *************//
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Transform wallCheck;


    //*************** Public Vectors ***************//
    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    //*********** Character Movement Settings *********//
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public int jumpCount = 2;
    public float airMoveForce = 5f;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeight = 0.5f;
    public float wallSlideSpeed = 4f;
    public float wallHopForce = 5f;
    public float wallJumpForce = 20f;


    //********** Surroundings detector settings ***********//
    public float groundCheckRadius = 1f;
    public float wallCheckDistance = 1f;


    //********** Variable declarations ***********//
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpsLeft = jumpCount;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }


    //************* Updates ***************//
    void Update()
    {
        CheckInput();
        moveDirection();
        AnimationsUpdate();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        Move();
        detectSurroundings();
    }

    private void AnimationsUpdate()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetInteger("jumpsLeft", jumpsLeft);
        animator.SetBool("isWallSliding", isWallSliding);
    }


    //*************** Checks ***************//
    private void detectSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isWalled = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0) || isWallSliding)
        {
            jumpsLeft = jumpCount;
        }

        if (jumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void CheckIfWallSliding()
    {
        if (isWalled && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeight);
        }
    }


    // ************ Character Manipulations ************* //
    private void moveDirection()
    {
        if (isFacingRight && moveInput < 0)
        {
            Flip();
        }
        else if (!isFacingRight && moveInput > 0)
        {
            Flip();
        }
    }

    private void Move()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(moveSpeed * moveInput, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && moveInput != 0)
        {
            rb.velocity = new Vector2(moveSpeed * moveInput, rb.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && moveInput == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

        if (rb.velocity.x != 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }
        else if (isWallSliding && moveInput == 0 && canJump)
        {
            isWallSliding = false;
            jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((isWallSliding || isWalled) && moveInput != 0 && canJump)
        {
            isWallSliding = false;
            jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * moveInput, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }


    //******** Debug Visuals ***********//
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
