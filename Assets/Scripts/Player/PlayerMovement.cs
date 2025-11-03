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
    private bool wasOnGround;

    [Header("Fall Settings")]
    [Tooltip("Multiplier for gravity when falling to make jumps feel less floaty.")]
    [SerializeField] private float fallMultiplier = 2.5f;

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
    [SerializeField] private GameObject firePoint;
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
        if (GameManager.instance.playerCanInput && !isKnockedBack)
        {
            // Input for jump & fire only (movement input moved to FixedUpdate)
            HandleJumpInput();
            HandleFire();
        }
        else
        {
            horizontalInput = 0;
        }

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
                isKnockedBack = false;
        }

        if (shotTimer > 0)
            shotTimer -= Time.deltaTime;

        GestionarOrientacion(horizontalInput);
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;

        // Read movement input here for zero lag
        horizontalInput = Input.GetAxisRaw("Horizontal");

        HandleMovement();
        ApplyFallGravity();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void HandleFire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && shotTimer <= 0)
        {
            ballArray.Add(Instantiate(ballPrefab, firePoint.transform.position, Quaternion.identity));
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
                        ball.GetComponent<BallScript>()?.Explode();
                }
                ballArray.Clear();
            }
        }
    }

    private bool IsOnGround()
    {
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * 0.6f, 0.1f);
        Vector2 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y - 0.05f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0.05f, MaskFloor);

        Color color = raycastHit.collider ? Color.green : Color.red;
        Debug.DrawLine(boxCenter - new Vector2(boxSize.x / 2, 0), boxCenter + new Vector2(boxSize.x / 2, 0), color);

        return raycastHit.collider != null;
    }

    private void HandleJumpInput()
    {
        bool onGround = IsOnGround();

        // Reset jumps only when landing
        if (onGround && !wasOnGround)
        {
            jumpsRestants = jumpsMax;
        }

        // Allow jump on ground OR if double-jump is available
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRestants > 0 && (onGround || jumpsRestants < jumpsMax))
        {
            jumpsRestants--;
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0f);
            rigidbody.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);
        }

        wasOnGround = onGround;
    }

    private void ApplyFallGravity()
    {
        if (rigidbody.linearVelocity.y < 0)
        {
            rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void HandleMovement()
    {
        float acceleration = IsOnGround() ? groundAcceleration : airAcceleration;
        float maxSpeed = IsOnGround() ? maxGroundSpeed : maxAirSpeed;

        // Only apply force if not pushing into a wall
        if (!IsAgainstWall())
        {
            rigidbody.AddForce(new Vector2(horizontalInput * acceleration, 0f));
        }

        // Clamp speed
        rigidbody.linearVelocity = new Vector2(
            Mathf.Clamp(rigidbody.linearVelocity.x, -maxSpeed, maxSpeed),
            rigidbody.linearVelocity.y
        );

        // Stop sliding when not moving and touching wall
        if (horizontalInput == 0 && IsAgainstWall())
        {
            rigidbody.linearVelocity = new Vector2(0, rigidbody.linearVelocity.y);
        }

        // Apply drag
        rigidbody.linearDamping = IsOnGround() ? groundDrag : 0.5f;
    }

    private bool IsAgainstWall()
    {
        float skinWidth = 0.05f;
        Vector2 origin = boxCollider.bounds.center;
        Vector2 size = new Vector2(0.1f, boxCollider.bounds.size.y * 0.9f);

        bool leftHit = Physics2D.OverlapBox(origin + Vector2.left * skinWidth, size, 0f, MaskFloor);
        bool rightHit = Physics2D.OverlapBox(origin + Vector2.right * skinWidth, size, 0f, MaskFloor);

        return (horizontalInput < 0 && leftHit) || (horizontalInput > 0 && rightHit);
    }

    private void GestionarOrientacion(float inputMovimiento)
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