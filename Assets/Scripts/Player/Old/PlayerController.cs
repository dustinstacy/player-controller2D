using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    //********* Input Direction ************//
    private Vector2 moveInput;


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
    private bool isShooting = false;


    //********** Component variables **************//
    private Rigidbody2D rb;
    private Animator animator;


    //********* Public Component Variables *************//
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public Transform wallCheck;


    //*********** Character Movement Settings *********//
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public int jumpCount = 2;
    // public float airMoveForce = 5f;
    public float airDragMultiplier = 0.75f;
    public float variableJumpHeight = 0.5f;
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 20f;


    //********** Surroundings detector settings ***********//
    public float groundCheckRadius = .35f;
    public float wallCheckDistance = .35f;


    //********** Variable declarations ***********//
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpsLeft = jumpCount;
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
        animator.SetBool("isShooting", isShooting);
    }


    //*************** Checks ***************//
    private void detectSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isWalled = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.1)
        {
            jumpsLeft = jumpCount;
        }

        if (isWallSliding)
        {
            jumpsLeft = 1;
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
        moveInput.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeight);
        }

        if (Input.GetButtonDown("Fire1") && isGrounded)
        {
            isShooting = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            isShooting = false;
        }
    }

    // ************ Character Manipulations ************* //
    private void moveDirection()
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Flip();
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Flip();
        }
    }

    private void Move()
    {
        if (!isGrounded && !isWallSliding && moveInput.x == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
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

        if (isShooting)
        {
            rb.velocity = new Vector2(0, 0);
        }
        else if (!isShooting)
        {
            rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
        }

    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }
        else if ((isWallSliding || isWalled) && moveInput.x != 0 && canJump)
        {
            isWallSliding = false;
            jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * moveInput.x, wallJumpForce);
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