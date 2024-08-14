using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    #region Variables
    //Movement variables
    [Header("Run Values")]
    [SerializeField] private float currentMaxSpeed = 11f;
    [SerializeField] private float maxWalkSpeed = 11f;
    [SerializeField] private float maxRunSpeed = 15f;
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
    [SerializeField] private int slideStrengh = 1;

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
    private InputAction interact;
    
    //Rigidbody2D reference
    private Rigidbody2D rb;

    //Health reference
    private PlayerHealth playerHealth;

    //Sprite reference
    private Transform sprite;
    private SpriteRenderer spriteRenderer;
    private Vector3 spriteScale;
    private SpriteAnimator spriteAnimator;

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
    public bool has_cloud_boots { get; private set; } = false;
    bool interact_pressed = false;
    bool interact_pressed_fixed = false;

    //Enchantment trackers
    bool damage_up = false;
    bool has_crit_chance = false;
    bool has_hp_drain = false;

    bool hp_up = false;
    bool speed_up = false;
    public bool has_counter { get; private set; } = false;

    //Enchantment variables
    [SerializeField] private float crit_chance = 0.2f;

    
    //Weapon tracker
    public enum Weapon {NONE, SWORD, BOW, SPARK};
    Weapon activeWeapon = Weapon.NONE;
    //private Item ArmorItem;

    //Equipped weapon strength
    

    //Weapon hitbox
    [Header("Weapons")]
    [SerializeField] private int weaponStrength = 1;
    [SerializeField] private Item TestWeaponItem;
    public Item WeaponItem;
    [SerializeField] private SwordAnimator swordAnimator;

    //Slide Hitbox Reference
    [SerializeField] private GameObject slideHitbox;



    //Arrow Reference
    [SerializeField] private GameObject Arrow;
    [SerializeField] private Transform arrowSpawnPoint;

    //Bow Variables
    [SerializeField] private float BowChargeTime = 0.3f;

    public enum Armor { NONE, LEATHER, IRON};
    [Header("Armor")]
    [SerializeField] private Item TestArmorItem;
    public Item ArmorItem;
    Armor activeArmor = Armor.NONE;
    


    //Inventory reference
    [Header("Inventory")]
    public InventoryManager Inventory;

    //Animation
    const string IS_CROUCHING = "IsCrouching";


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
        spriteAnimator = sprite.GetComponent<SpriteAnimator>();
        playerHealth = GetComponent<PlayerHealth>();
        SetArmor(ArmorItem); //this is the line I (Kevin) added to fix the bug about HP being 3 after pressing New Game

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
        interact = playerControls.Player.Interact;
        playerControls.Enable();

        //Subscribe load data to event
        Events.Loadprogress += LoadPlayer;
       
    }

    private void OnDisable()
    {
       playerControls.Disable();
        Events.Loadprogress -= LoadPlayer;
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
        InteractCheck();
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
                case Weapon.NONE:
                    activeWeapon = Weapon.SWORD;
                    Debug.Log("sword equipped");
                    break;
                case Weapon.SWORD:
                    activeWeapon = Weapon.BOW;
                    Debug.Log("bow equipped");
                    break;
                case Weapon.BOW:
                    activeWeapon = Weapon.SPARK;
                    Debug.Log("spark equipped");
                    break;
                case Weapon.SPARK:
                    activeWeapon = Weapon.NONE;
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
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Progress loaded. Position: " + transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DeleteSavePlayer();
            Debug.Log("Save file deleted.");
        }

        if (Input.GetKeyDown(KeyCode.O)) SetWeapon(TestWeaponItem);
        if (Input.GetKeyDown(KeyCode.I)) SetArmor(TestArmorItem);
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

    private void InteractCheck()
    {
        if (interact.triggered) interact_pressed = true;
    }

    private void AnimationHandle()
    {
        //Temporary (hopefully) animation to represent ducking
        //if (is_ducking) sprite.localScale = new Vector3(spriteScale.x, spriteScale.y / 2f, spriteScale.z);
        //else sprite.localScale = new Vector3(spriteScale.x, spriteScale.y, spriteScale.z);
        spriteAnimator.SetGrounded(IsGrounded());
        spriteAnimator.SetMoveSpeed(Mathf.Abs(rb.velocity.x));
        spriteAnimator.SetJumpSpeed(rb.velocity.y);
        spriteAnimator.SetCrouching(is_ducking);
        spriteAnimator.SetHurt(isKnocback());
        spriteAnimator.SetSliding(is_sliding);
        GetComponent<Animator>().SetBool(IS_CROUCHING, is_ducking || is_sliding);
        

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
        InteractHandle();
            interact_pressed = false;
    }

    private void InteractHandle()
    {
        //Causes bugs without this extra function...
        interact_pressed_fixed = interact_pressed;
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

        if (activeWeapon == Weapon.BOW)
        {
            if (bowTimer > -1f && bowTimer <= 0f)
            {
                GameObject arrow = Instantiate(Arrow, arrowSpawnPoint.position, Quaternion.identity);
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
            case Weapon.NONE:
                return;
            case Weapon.SWORD:
                swordAnimator.SwingSword();
                break;
            case Weapon.BOW:
                BowStartCharge();
                break;
            case Weapon.SPARK:
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
        float maxSpeed = (is_running) ? maxRunSpeed : currentMaxSpeed;
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
        

        //Reset jump buffer and coyote time
        jumpBufferTimer = -1f;
        jumpCoyoteTimer = -1f;


        //Either sliding prevents jumping, or jumping can cancel sliding.
        //Return if sliding
        //if (is_sliding) return;

        //Cancels slide;
        is_sliding = false;
        slideHitbox.SetActive(false);
        
        //Cancel vertical velocity
        rb.velocity = new Vector2(rb.velocity.x, 0);

        //Apply upward impulse
        //Use stronger jump if on cloud; otherwise, do normal jump.
        if (IsGrounded())
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        else if (CanCloudJump() || flyMode)
        {
            rb.AddForce(cloudJumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        //If not on ground or cloud, must be a coyote jump
        else
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        is_jumping = true;

        //Play jump SFX
        AudioManager.instance.PlaySoundFXClip(AudioManager.instance.Jump, transform);


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

        //Old flip code
        //if (moveDirection != 0)
        //{
        //    spriteRenderer.flipX = (moveDirection < 0);
        //    FlipWeapon();
        //}

        //Ignore flip handle if sliding
        if (is_sliding) return;


        if ((moveDirection > 0) && IsFlipped()) transform.localScale = new Vector3 (1, 1, 1);
        if (moveDirection < 0 && !IsFlipped()) transform.localScale = new Vector3(-1, 1, 1);
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
        Vector2 slideDirection = (IsFlipped()) ? Vector2.left : Vector2.right;

        //Apply sideways force
        rb.AddForce(slideForce * slideDirection, ForceMode2D.Impulse);

        //Start slide timer
        slideTimer = slideLength;
        slideHitbox.SetActive(true);
        AudioManager.instance.PlaySoundFXClip(AudioManager.instance.Slide, transform);
    }

    private void GroundedHandle()
    {
        if (IsGrounded() && rb.velocity.y <= 0)
        {
            is_jumping = false;
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
            if (slideTimer < 0)
            {
                is_sliding = false;
                slideHitbox.SetActive(false);
            }
        }

        //Jump timers
        if (jumpCoyoteTimer > 0f) jumpCoyoteTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;

        //Bow timers; only activate if bow is equipped and charging
        if (activeWeapon == Weapon.BOW)
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
        SaveSystem.Save(this, Inventory);
    }

    public void LoadPlayer()
    {
        //SceneManager.LoadSceneAdsync("PlayerTest");

        SaveData data = SaveSystem.LoadData();
        Inventory.LoadItems(data);


        if (data == null) return;
        //Load position
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;
        rb.velocity = Vector3.zero;

        //Cloud boots
        if (data.has_cloud_boots) GetCloudBoots();
        else LoseCloudBoots();

        //Current weapon and armor
        SetWeapon(data.weaponItem);
        SetArmor(data.armorItem);
        
        //Restore health
        playerHealth.RestoreHealth();

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
        AudioManager.instance.PlaySoundFXClip(AudioManager.instance.PlayerHurt, transform);
        //Debug.Log("knockbackright: " + KnockFromRight);

    }

    private bool IsFlipped()
    {
        return transform.localScale.x < 0;
    }

    public bool IsSliding()
    {
        return is_sliding;
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

    public void SetEquipment(Item item)
    {
        if (item.ItemType == ItemBase.ItemTypes.WEAPON)
        {
            SetWeapon(item);
        }
        else if (item.ItemType == ItemBase.ItemTypes.ARMOR)
        {
            SetArmor(item);
        }
    }

    public void SetWeapon(Item weaponItem)
    {
        WeaponItem = weaponItem;
        if (weaponItem.ItemName == "Sword") activeWeapon = Weapon.SWORD;
        else if (weaponItem.ItemName == "Bow") activeWeapon = Weapon.BOW;
        else if (weaponItem.ItemName == "Spark") activeWeapon = Weapon.SPARK;
        else activeWeapon = Weapon.NONE;
        ApplyWeaponEnchantments();

    }

    public void SetArmor(Item armorItem)
    {
        if (armorItem.ItemName == "Leather Armor") activeArmor = Armor.LEATHER;
        else if (armorItem.ItemName == "Iron Armor") activeArmor = Armor.IRON;
        ArmorItem = armorItem;
        ApplyArmorEnchantments();

    }

    private void ApplyWeaponEnchantments()
    {
        //Set weapon enchantments based on what's in the weapon Item
        damage_up = (WeaponItem.WeaponEnchantmentSlot == Item.WeaponEnchantment.DAMAGE_UP);
        has_crit_chance = (WeaponItem.WeaponEnchantmentSlot == Item.WeaponEnchantment.CRIT_CHANCE);
        has_hp_drain = (WeaponItem.WeaponEnchantmentSlot == Item.WeaponEnchantment.HP_DRAIN);
    }

    private void ApplyArmorEnchantments()
    {
        hp_up = (ArmorItem.ArmorEnchantmentSlot == Item.ArmorEnchantment.HP_UP);
        has_counter = (ArmorItem.ArmorEnchantmentSlot == Item.ArmorEnchantment.COUNTER);
        speed_up = (ArmorItem.ArmorEnchantmentSlot == Item.ArmorEnchantment.SPEED_UP);

        //Some have instant effects that need to be applied
        playerHealth.HandleArmorHealth(ArmorItem.ItemName, hp_up);
        currentMaxSpeed = (speed_up) ? maxRunSpeed : maxWalkSpeed;

    }

    public bool HasHPDrain()
    {
        return has_hp_drain;
    }

    public int GetWeaponStrength()
    {
        //Calculate strength of weapon
        int strength = weaponStrength;
        if (damage_up)
        {
            strength += 1;
            print("Strength buffed!");
        }
        if (has_crit_chance)
        {
            if (Random.Range(0f, 1f) < crit_chance)
            {
                strength *= 3;
                print("CRITICAL HIT!!");
            }
        }

        return strength;
        
    }

    public int GetSlideStrength()
    {
        return slideStrengh;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint") && interact_pressed_fixed && IsGrounded())
        {
            interact_pressed_fixed = false;
            playerHealth.RestoreHealth();
            SaveSystem.Save(this, Inventory);
            print("Saved!");
        }
    }
}
