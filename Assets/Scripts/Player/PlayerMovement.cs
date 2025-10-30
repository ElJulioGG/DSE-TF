using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The maximum speed the player can reach on the ground.")]
    [SerializeField] private float maxGroundSpeed = 8f;
    [Tooltip("The force applied to the player to move them on the ground.")]
    [SerializeField] private float groundAcceleration = 100f;
    [Tooltip("The maximum speed the player can reach in the air.")]
    [SerializeField] private float maxAirSpeed = 5f;
    [Tooltip("The force applied to the player to move them in the air.")]
    [SerializeField] private float airAcceleration = 50f;
    [Tooltip("Linear drag applied when on the ground to help the player stop.")]
    [SerializeField] private float groundDrag = 6f;


    [Header("Jump Settings")]
    [SerializeField] private float forceJump = 15f;
    [SerializeField] private int jumpsMax = 2;
    [SerializeField] private LayerMask MaskFloor;
    private int jumpsRestants;

    [Header("Fall Settings")]
    [Tooltip("Multiplier for gravity when falling to make jumps feel less floaty.")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [Tooltip("How much to reduce upward linearVelocity when the jump button is released.")]
    [SerializeField] private float lowJumpMultiplier = 2f;


    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f;
    private float knockbackTimer;
    private bool isKnockedBack = false;

    [Header("Health Settings")]
    [SerializeField] private int health;
    [SerializeField] private float damageDelay;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] public List<GameObject> ballArray;
    [SerializeField] private float shotDelay = 0.5f;
    private float shotTimer;

    private bool WatchRight = true;

    private new Rigidbody2D rigidbody;
    private new BoxCollider2D boxCollider;
    private Camera mainCam;
    private float horizontalInput;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        jumpsRestants = jumpsMax;
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Handle input reading in Update
        if (GameManager.instance.playerCanInput && !isKnockedBack)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            HandleJumpInput();
            HandleFire();
        }
        else
        {
            horizontalInput = 0;
        }

        // Handle knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
        }

        // Update timers
        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }

        GestionarOrientacion(horizontalInput);
    }

    private void FixedUpdate()
    {
        // Apply physics-based logic in FixedUpdate
        if (isKnockedBack) return;

        HandleMovement();
        ApplyFallGravity();

    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rigidbody.velocity = Vector2.zero; // Use velocity for consistency
        rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void HandleFire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && shotTimer <= 0)
        {
            ballArray.Add(Instantiate(ballPrefab, this.transform.position, Quaternion.identity));

            if (ballArray.Count > 2)
            {
                ballArray[0].GetComponent<BallScript>()?.Explode();
                ballArray.RemoveAt(0);
            }
            shotTimer = shotDelay;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (ballArray.Count > 0)
            {
                foreach (GameObject ball in ballArray)
                {
                    if (ball != null)
                    {
                        ball.GetComponent<BallScript>()?.Explode();
                    }
                }
                ballArray.Clear();
            }
        }
    }

    private bool IsOnGround()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, MaskFloor);
        return raycastHit.collider != null;
    }

    private void HandleJumpInput()
    {
        if (IsOnGround())
        {
            jumpsRestants = jumpsMax;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpsRestants > 0)
        {
            jumpsRestants--;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
            rigidbody.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);
        }
    }

    private void ApplyFallGravity()
    {
        if (rigidbody.velocity.y < 0)
        {
            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float acceleration = IsOnGround() ? groundAcceleration : airAcceleration;
        float maxSpeed = IsOnGround() ? maxGroundSpeed : maxAirSpeed;

        // Apply force for movement
        rigidbody.AddForce(new Vector2(horizontalInput * acceleration, 0f));

        // Clamp the velocity to the max speed
        rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -maxSpeed, maxSpeed), rigidbody.velocity.y);

        // Apply drag when on ground to stop the player
        if (IsOnGround())
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = 0.5f; // A little bit of air drag
        }
    }

    void GestionarOrientacion(float inputMovimiento)
    {
        if (inputMovimiento < 0 && WatchRight)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            WatchRight = false;
        }
        else if (inputMovimiento > 0 && !WatchRight)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            WatchRight = true;
        }
    }
}