using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Variables
    //Movement variables
    [Header("Run Values")]
    [SerializeField] private float maxWalkSpeed = 6f;
    [SerializeField] private float maxRunSpeed = 10f;
    [SerializeField] private float runAccelAmount = 13f;
    [SerializeField] private float runDeccelAmount = 16f;

    //Jump variables
    [Header("Jump Values")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;
    [SerializeField] private float cloudJumpForce = 15f;

    //Jump boxcast variables
    [Header("Jump Boxcast")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask cloudLayer;
    const int CLOUD_LAYER = 9;


    //Falling variables
    [Header("Fall Values")]
    [SerializeField] private float fallGravityMultiplier = 1.4f;
    [SerializeField] private float gravityScale = 1.3f;
    [SerializeField] private float maxFallSpeed = -15f;

    //Slide variables
    [Header("Slide Values")]
    [SerializeField] private float slideForce = 10f;
    [SerializeField] private float slideLength = 0.5f;

    //Knockback effect variables
    [Header("Knockback Values")]
    [SerializeField] public Vector2 KBForce = new Vector2(6f,3f);
    [SerializeField] public float KBCounter;
    [SerializeField] public float KBTotalTime = 0.5f;
    [SerializeField] public bool KnockFromRight;

    //Coyote/buffer time variables
    [Header("Coyote/Buffer Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float bufferTime = 0.1f;


    //Input System 
    private PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction run;
    private InputAction attack;
    private InputAction duck;
    
    //Rigidbody2D reference
    private Rigidbody2D rb;

    //Sprite reference
    private Transform sprite;
    private SpriteRenderer spriteRenderer;
    private Vector3 spriteScale;

    //Tracks which direction is being moved in
    private float moveDirection;

    //Timers
    private float slideTimer = 0f;
    private float jumpCoyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private float bowTimer = -1f;

    //Various trackers
    bool jump_pressed = false;
    bool is_running = false;
    bool is_ducking = false;
    bool attack_pressed = false;
    bool is_sliding = false;
    bool is_jumping = false;
    bool jump_released = false;
    bool attack_released = false;
    bool is_on_cloud = false;
    bool has_cloud_boots = false;

    //Weapon tracker
    enum ActiveWeapon {NONE, SWORD, BOW, SPARK};
    ActiveWeapon activeWeapon = ActiveWeapon.NONE;

    //Weapon hitbox
    [SerializeField] private SwordAnimator swordAnimator;

    //Arrow Reference
    [SerializeField] private GameObject Arrow;
    [SerializeField] private Transform arrowSpawnPoint;

    //Bow Variables
    [SerializeField] private float BowChargeTime = 0.3f;


    //Debug -- set to true for extra debugging tools
    bool debug = true;
    bool flyMode = false;
    #endregion Variables


    private void Awake()
    {
        playerControls = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprite");
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        spriteScale = sprite.localScale;

        //Ignore collision with clouds if player does not have cloud boots
        Physics2D.IgnoreLayerCollision(gameObject.layer, CLOUD_LAYER, !has_cloud_boots);
    }

    private void OnEnable()
    {
        //Need to enable player inputs
        move = playerControls.Player.Move;
        jump = playerControls.Player.Jump;
        run = playerControls.Player.Run;
        attack = playerControls.Player.Attack;
        duck = playerControls.Player.Duck;
        playerControls.Enable();
       
    }

    private void OnDisable()
    {
       playerControls.Disable();
    }

    private void Update()
    {
        moveDirection = (!is_ducking) ? move.ReadValue<float>() : 0f;
        FlipHandle();
        JumpCheck();
        RunCheck();
        DuckCheck();
        AttackCheck();
        AnimationHandle();
        if (debug) DebugTools();

    }

    private void DebugTools()
    {
        //This function contains debugging tools
        
        //Changes active weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (activeWeapon)
            {
                //Perform no attack if player has no weapon
                case ActiveWeapon.NONE:
                    activeWeapon = ActiveWeapon.SWORD;
                    Debug.Log("sword equipped");
                    break;
                case ActiveWeapon.SWORD:
                    activeWeapon = ActiveWeapon.BOW;
                    Debug.Log("bow equipped");
                    break;
                case ActiveWeapon.BOW:
                    activeWeapon = ActiveWeapon.SPARK;
                    Debug.Log("spark equipped");
                    break;
                case ActiveWeapon.SPARK:
                    activeWeapon = ActiveWeapon.NONE;
                    Debug.Log("nothing equipped");
                    break;
            }

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            flyMode = !flyMode;
            Debug.Log("Flying mode:" +  flyMode);
        }

        //Gives player cloud boots
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (has_cloud_boots)
            {
                LoseCloudBoots();
                Debug.Log("Lost cloud boots");
            }
            else
            {
                GetCloudBoots();
                Debug.Log("Get cloud boots");
            }
        }

        //Save system -- will refactor later
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SavePlayer();
            Debug.Log("Progress saved.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadPlayer();
            Debug.Log("Progress loaded. Position: " + transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DeleteSavePlayer();
            Debug.Log("Save file deleted.");
        }
    }

    private void JumpCheck()
    {
        //Check if character is jumping
        if (jump.triggered)
        {
            jump_pressed = true;
            jumpBufferTimer = bufferTime;
        }

        //Check if jump button was released
        if (jump.WasReleasedThisFrame())
        {
            jump_released = true;
            //Debug.Log("jump released");
        }
    }

    private void AttackCheck()
    {
        //Check if character is attacking
        if (attack.triggered) attack_pressed = true;
        if (attack.WasReleasedThisFrame()) attack_released = true;

    }

    private void RunCheck()
    {
        //Check if character is running
        if (run.IsPressed())
        {
            is_running = true;
            Debug.Log("Running");
        }
        else
        {
            is_running=false;
        }
    }

    private void DuckCheck()
    {
        //Check if character should be ducking
        if (duck.IsPressed() && IsGrounded() && !is_sliding)
        {
            is_ducking = true;
        }
        else
        {
            is_ducking = false;
        }
    }

    private void AnimationHandle()
    {
        //Temporary (hopefully) animation to represent ducking
        if (is_ducking) sprite.localScale = new Vector3(spriteScale.x, spriteScale.y / 2f, spriteScale.z);
        else sprite.localScale = new Vector3(spriteScale.x, spriteScale.y, spriteScale.z);
    }

    private void FixedUpdate()
    {
 
            MoveHandle();
            JumpHandle();
            jump_pressed = false;
            FallHandle();
            AttackHandle();
            attack_pressed = false;
            AttackReleaseHandle();
            attack_released = false;
            HandleTimers();
            GroundedHandle();
            JumpReleaseHandle();
            jump_released = false;
            KnockbackHandle();
    }

    private void KnockbackHandle()
    {
        if (KBCounter > 0)
        {
            //Set these to false so nothing weird happens
            is_sliding = false;
            is_jumping = false;

            KBCounter -= Time.deltaTime;
        }
    }

    private void AttackHandle()
    {
        //Return if being knocked back
        if (isKnocback()) return;

        //Check if attack was pressed
        if (!attack_pressed) return;

        //Don't attack if already sliding
        if (is_sliding) return;

        //If character is ducking, perform slide attack and don't do any other attack
        if (is_ducking)
        {
            SlideAttack();
            return;
        }

        WeaponAttack();
    }

    private void AttackReleaseHandle()
    {
        //Return if attack wasn't released
        if (!attack_released) return;

        if (activeWeapon == ActiveWeapon.BOW)
        {
            if (bowTimer > -1f && bowTimer <= 0f)
            {
                GameObject arrow = Instantiate(Arrow, arrowSpawnPoint);
                arrow.GetComponent<Arrow>().ShootArrow(!IsFlipped());
                Debug.Log("bow fired!");
            }
        }
        bowTimer = -1f;
    }

    private void WeaponAttack()
    {
        switch (activeWeapon)
        {
            //Perform no attack if player has no weapon
            case ActiveWeapon.NONE:
                return;
            case ActiveWeapon.SWORD:
                swordAnimator.SwingSword();
                break;
            case ActiveWeapon.BOW:
                BowStartCharge();
                break;
            case ActiveWeapon.SPARK:
                break;
        }
    }

    private void BowStartCharge()
    {
        bowTimer = BowChargeTime;
        Debug.Log("Bow begin charge. Charge time: " + BowChargeTime);
    }
    private void MoveHandle()
    {
        //Return if sliding
        if (is_sliding) return;

        //Return if being knocked back
        if (isKnocback()) return;

        //Speed is dependent on whether character is running
        float maxSpeed = (is_running) ? maxRunSpeed : maxWalkSpeed;
        //Calculate our desired velocity
        float targetSpeed = moveDirection * maxSpeed;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop).
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;

        //Calculate force along x-axis to apply to thr player
        float force = speedDif * accelRate;

        rb.AddForce(force * Vector2.right);
    }

    private void JumpHandle()
    {
        //Return if being knocked back
        if (isKnocback()) return;

        //Check to see if player jumped
        if (!jumpBuffered()) return;

        //Check if player is grounded and can jump, or if they were recently on ground and can do coyote jump, or can do cloud jump
        if (!IsGrounded() && !CanCloudJump() && !jumpCoyoted() && !flyMode) return;


        //Either sliding prevents jumping, or jumping can cancel sliding.
        //Return if sliding
        //if (is_sliding) return;

        //Cancels slide;
        is_sliding = false;

        //Cancel vertical velocity
        rb.velocity = new Vector2(rb.velocity.x, 0);

        //Apply upward impulse
        //Use stronger jump if on cloud; otherwise, do normal jump.
        if (CanCloudJump())
        {
            rb.AddForce(cloudJumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        is_jumping = true;


    }

    private bool IsGrounded()
    {
        //Boxcasts below player to detect ground
        return (Physics2D.BoxCast(transform.position, boxSize, 0, Vector3.down, castDistance, groundLayer));

    }

    private bool IsOnCloud()
    {
        return (Physics2D.BoxCast(transform.position, boxSize, 0, Vector3.down, castDistance, cloudLayer));
    }

    private bool CanCloudJump()
    {
        return IsOnCloud() && has_cloud_boots;
    }

    private void OnDrawGizmos()
    {
        //Draws gizmo of ground detect Boxcast for easier editing
        Gizmos.DrawWireCube(transform.position - (transform.up * castDistance), boxSize);
    }

    private void FlipHandle()
    {
        //Flips character sprite
        if (moveDirection != 0)
        {
            spriteRenderer.flipX = (moveDirection < 0);
            FlipWeapon();
        }
    }

    private void FlipWeapon()
    {
        swordAnimator.GetComponent<SpriteRenderer>().flipX = (moveDirection < 0);
        swordAnimator.transform.localPosition = new Vector3(moveDirection * Mathf.Abs(swordAnimator.transform.localPosition.x), swordAnimator.transform.localPosition.y, swordAnimator.transform.localPosition.z);

        arrowSpawnPoint.transform.localPosition = new Vector3(moveDirection * Mathf.Abs(arrowSpawnPoint.transform.localPosition.x), arrowSpawnPoint.transform.localPosition.y, arrowSpawnPoint.transform.localPosition.z);
    }

    private void FallHandle()
    {
        //Increase gravity when falling
        if (rb.velocity.y < 0){
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        //Terminal velocity
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, maxFallSpeed));
    }

    private void SlideAttack()
    {
        is_sliding = true;

        //Reset current momentum?
        rb.velocity = Vector2.zero;

        //Find direction
        Vector2 slideDirection = (spriteRenderer.flipX) ? Vector2.left : Vector2.right;

        //Apply sideways force
        rb.AddForce(slideForce * slideDirection, ForceMode2D.Impulse);

        //Start slide timer
        slideTimer = slideLength;
    }

    private void GroundedHandle()
    {
        if (IsGrounded() && rb.velocity.y <= 0)
        {
            is_jumping = false;
        }
        if (IsGrounded())
        {
            jumpCoyoteTimer = coyoteTime;
        }
    }

    private void JumpReleaseHandle()
    {
        if (!jump_released) return;

        //Return if player isn't jumping or is falling
        if (!is_jumping || rb.velocity.y <= 0f) return;

        //If player releases jump early, add downward impulse so the player falls early.
        //Debug.Log("Jump cut!");
        rb.AddForce(Vector2.down * rb.velocity * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        is_jumping = false;
    }



    private void HandleTimers()
    {
        //Slide timer
        if (is_sliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer < 0) is_sliding = false;
        }

        //Jump timers
        if (jumpCoyoteTimer > 0f) jumpCoyoteTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;

        //Bow timers; only activate if bow is equipped and charging
        if (activeWeapon == ActiveWeapon.BOW)
        {
            if (bowTimer > 0f) bowTimer -= Time.deltaTime;
        }
        else
        {
            bowTimer = -1f;
        }
        
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        SaveData data = SaveSystem.LoadData();
        if (data == null) return;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;

    }

    private bool jumpBuffered()
    {
        //Check if player has jump buffered
        return (jumpBufferTimer > 0f);
    }

    private bool jumpCoyoted()
    {
        return (jumpCoyoteTimer > 0f);
    }

    private bool isKnocback()
    {
        return (KBCounter > 0);
    }

    public void Knockback()
    {
        KBCounter = KBTotalTime;
        rb.velocity = Vector2.zero;
        if (KnockFromRight)
        {
            // rb.velocity = new Vector2(-KBForce, KBForce);
            rb.AddForce(KBForce * new Vector2(-1, 1), ForceMode2D.Impulse);
        }
        else if (!KnockFromRight)
        {
            //rb.velocity = new Vector2(KBForce, KBForce);
            rb.AddForce(KBForce, ForceMode2D.Impulse);

        }
        //Debug.Log("knockbackright: " + KnockFromRight);

    }

    private bool IsFlipped()
    {
        return spriteRenderer.flipX;
    }

    public void GetCloudBoots()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, CLOUD_LAYER, false);
        has_cloud_boots = true;
    }

    public void LoseCloudBoots()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, CLOUD_LAYER, true);
        has_cloud_boots = false;
    }

    public void DeleteSavePlayer()
    {
        SaveSystem.DeleteData();
    }
}
