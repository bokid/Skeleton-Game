using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    public float speed;
    public float jumpForce;
    private float moveInput;
    private float climbInput;

    private Rigidbody2D rigidbody;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    bool isTouchingFront;
    public Transform frontCheck;
    bool wallSliding;
    public float wallSlidingSpeed;

    bool wallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;

    public float sprintSpeed;

    private int extraJumps;
    public int extraJumpsValue;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    
    private bool isClimbing;
    public float climbSpeed;

    private bool isWalking;

    public PlayerSounds playerSounds;


    void Start()
    {
        extraJumps = extraJumpsValue;
        rigidbody = GetComponent<Rigidbody2D>();
        //audiosource = GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);

        moveInput = Input.GetAxis("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        climbInput = Input.GetAxis("Vertical");
        

        if ((wallJumping == true) && (moveInput > 0))
        {
            rigidbody.velocity = new Vector2(xWallForce * -moveInput, yWallForce);
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else if ((wallJumping == true) && (moveInput < 0))
        {
            rigidbody.velocity = new Vector2(xWallForce * -moveInput, yWallForce);
            transform.eulerAngles = new Vector3(0,180,0);
        }


        animation_params();
        
        ambulation_delegation();

        

    }
    void Update()
    {
       
        
        jump();

        climb();

        wallJump();
    }

    void ambulation_delegation()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector2 temp_vector = new Vector2(moveInput * sprintSpeed, rigidbody.velocity.y);
            ambulation(true, temp_vector);
            //SoundManager.Instance.PlaySFX(playerSounds.sprint);
        }
        else
        {
            Vector2 temp_vector = new Vector2(moveInput * speed, rigidbody.velocity.y);
            ambulation(false, temp_vector);
            //SoundManager.Instance.PlaySFX(playerSounds.walk);
        }
    }

    void ambulation(bool sprint_state, Vector2 velocity)
    {
        rigidbody.velocity = velocity;
        animator.SetBool("isSprinting", sprint_state);
        
    }

    void jump()
    {
        

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rigidbody.velocity = Vector2.up * jumpForce;
            extraJumps--;
            SoundManager.Instance.PlaySFX(playerSounds.jump);
            //Debug.Log("Playing Jump Sound" + playerSounds.jumpSFX);

        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rigidbody.velocity = Vector2.up * jumpForce;
        }
    
        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rigidbody.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
                SoundManager.Instance.PlaySFX(playerSounds.jump);
                //Debug.Log("Playing Jump Sound" + playerSounds.jumpSFX);
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    void wallJump()
    {
        if (isTouchingFront == true && isGrounded == false && moveInput != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding == true)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue));
            Debug.Log("You're wall sliding");
            animator.SetBool("isSliding", true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallSliding == true)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
            
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isClimbing == true)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }

        if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0,180,0);
        }


        

    }

    void climb()
    {
        if (isTouchingFront == true && Input.GetKey(KeyCode.LeftShift))
        {
            isClimbing = true;
            Debug.Log("you climbin");
            moveInput = 0;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, climbInput * climbSpeed);
            animator.SetBool("isClimbing", true);
            SoundManager.Instance.PlaySFX(playerSounds.climb);
        }
        else
        {
            isClimbing = false;
        }
    }

    void animation_params()
    {
        if (isGrounded == true)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isSliding", false);
            animator.SetBool("isClimbing", false);
            animator.SetBool("isWallJumping", false);
        }
        if (isJumping == true)
        {
            animator.SetBool("isJumping", true);
        }
        
        
        if (wallSliding == true)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
            animator.SetBool("isWallJumping", false);
            animator.SetBool("isClimbing", false);
        }
        if (wallJumping == true) 
        {
            animator.SetBool("isWallJumping", true);
            animator.SetBool("isSliding", false);
            animator.SetBool("isClimbing", false);
        }
    }

    void SetWallJumpingToFalse()  
    {
        wallJumping = false;
    }
}
