using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxGroundSpeed = 8f;
    [SerializeField] private float groundAcceleration = 100f;
    [SerializeField] private float maxAirSpeed = 5f;
    [SerializeField] private float airAcceleration = 50f;
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private LayerMask wallMask;

    [Header("Jump Settings")]
    [SerializeField] private float forceJump = 15f;
    [SerializeField] private int jumpsMax = 2;
    [SerializeField] private LayerMask MaskFloor;

    [Header("Fall Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Health Settings")]
    [SerializeField] private int health;
    [SerializeField] private float damageDelay;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private List<GameObject> ballArray = new List<GameObject>();
    [SerializeField] private float shotDelay = 0.5f;
    [SerializeField] private GameObject firePoint;

    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnDuration = 2f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PhysicsMaterial2D noFrictionMaterial;

    // Private fields
    private Rigidbody2D rigidbody;
    private CircleCollider2D boxCollider;
    private Camera mainCam;
    private float horizontalInput;
    private float shotTimer;
    private float knockbackTimer;
    private int jumpsRestants;
    private bool wasOnGround;
    private bool isKnockedBack = false;
    private bool isInvulnerable = false;
    private bool canTakeDamage = true;

    public bool WatchRight = true;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CircleCollider2D>();
        mainCam = Camera.main;
        jumpsRestants = jumpsMax;
    }

    private void Update()
    {
        // Clean up destroyed balls every frame
        ballArray.RemoveAll(b => b == null);

        if (GameManager.instance.playerCanInput && !isKnockedBack)
        {
            HandleJumpInput();
            HandleFire();
        }
        else
        {
            horizontalInput = 0f;
        }

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f) isKnockedBack = false;
        }

        if (shotTimer > 0f) shotTimer -= Time.deltaTime;

        GestionarOrientacion(horizontalInput);
    }

    private void FixedUpdate()
    {
        bool grounded = IsOnGround();

        // Reset jumps only when landing
        if (grounded && !wasOnGround)
            jumpsRestants = jumpsMax;

        wasOnGround = grounded;

        if (isKnockedBack) return;

        // Get input in FixedUpdate for smooth movement
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
        // LEFT CLICK - Shoot
        if (Input.GetKeyDown(KeyCode.Mouse0) && shotTimer <= 0f)
        {
            GameObject ball = Instantiate(ballPrefab, firePoint.transform.position, Quaternion.identity);
            ballArray.Add(ball);

            SoundFXManager.instance.PlaySoundByName("sticky_fire", transform, 1f, 1f, false);

            // Max 2 active balls - destroy oldest if we go over
            if (ballArray.Count > 2)
            {
                GameObject oldest = ballArray[0];
                if (oldest != null)
                {
                    BallScript bs = oldest.GetComponent<BallScript>();
                    if (bs != null) bs.ForceExplode();
                }
                ballArray.RemoveAt(0);
            }

            shotTimer = shotDelay;
        }

        // RIGHT CLICK - Detonate only READY balls (canDet == true)
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            for (int i = ballArray.Count - 1; i >= 0; i--)
            {
                GameObject ball = ballArray[i];
                if (ball == null) continue;

                BallScript script = ball.GetComponent<BallScript>();
                if (script != null && script.canDet)
                {
                    script.Explode();
                    // Do NOT remove from list here - it will be cleaned up next frame via RemoveAll(null)
                }
            }
            // We no longer do ballArray.Clear() - this was the bug!
        }
    }

    private bool IsOnGround()
    {
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * 0.6f, 0.1f);
        Vector2 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y - 0.05f);

        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, 0.05f, MaskFloor);

#if UNITY_EDITOR
        Color color = hit.collider ? Color.green : Color.red;
        Debug.DrawLine(boxCenter - new Vector2(boxSize.x / 2, 0), boxCenter + new Vector2(boxSize.x / 2, 0), color);
#endif

        return hit.collider != null;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRestants > 0)
        {
            // Allow jump from ground or mid-air if we still have extra jumps
            if (IsOnGround() || jumpsRestants < jumpsMax)
            {
                jumpsRestants--;
                rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, 0f);
                rigidbody.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);
                SoundFXManager.instance.PlaySoundByName("jump", transform, 1f, 1f, false);
            }
        }
    }

    private void ApplyFallGravity()
    {
        if (rigidbody.linearVelocity.y < 0f)
        {
            rigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void HandleMovement()
    {
        float acceleration = IsOnGround() ? groundAcceleration : airAcceleration;
        float maxSpeed = IsOnGround() ? maxGroundSpeed : maxAirSpeed;

        // Only add force if we're not pushing into a wall
        if (!IsAgainstWall(horizontalInput))
        {
            rigidbody.AddForce(Vector2.right * horizontalInput * acceleration);
        }

        // Clamp horizontal speed
        float clampedX = Mathf.Clamp(rigidbody.linearVelocity.x, -maxSpeed, maxSpeed);
        rigidbody.linearVelocity = new Vector2(clampedX, rigidbody.linearVelocity.y);

        // Stop completely if pressing against wall and no input
        if (horizontalInput == 0f && IsAgainstWall(0))
        {
            rigidbody.linearVelocity = new Vector2(0f, rigidbody.linearVelocity.y);
        }

        // Apply drag only on ground
        rigidbody.linearDamping = IsOnGround() ? groundDrag : 0.5f;
    }

    private bool IsAgainstWall(float inputDirection)
    {
        if (inputDirection == 0f) // used only for the "stop sliding" check
        {
            // Check both sides when no input
            return Physics2D.OverlapBox(boxCollider.bounds.center + Vector3.left * 0.06f,
                new Vector2(0.12f, boxCollider.bounds.size.y * 0.9f), 0f, wallMask) ||
                   Physics2D.OverlapBox(boxCollider.bounds.center + Vector3.right * 0.06f,
                new Vector2(0.12f, boxCollider.bounds.size.y * 0.9f), 0f, wallMask);
        }

        bool left = inputDirection < 0 && Physics2D.OverlapBox(boxCollider.bounds.center + Vector3.left * 0.06f,
            new Vector2(0.12f, boxCollider.bounds.size.y * 0.9f), 0f, wallMask);
        bool right = inputDirection > 0 && Physics2D.OverlapBox(boxCollider.bounds.center + Vector3.right * 0.06f,
            new Vector2(0.12f, boxCollider.bounds.size.y * 0.9f), 0f, wallMask);

        return left || right;
    }

    private void GestionarOrientacion(float input)
    {
        if (input > 0.01f && !WatchRight)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
            WatchRight = true;
        }
        else if (input < -0.01f && WatchRight)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
            WatchRight = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            TakeDamage();

            // Kill enemy
            BasicEnemyMove enemy = collision.gameObject.GetComponent<BasicEnemyMove>();
            if (enemy != null) enemy.Death();

            // Little spin on death
            Rigidbody2D enemyRb = collision.rigidbody;
            if (enemyRb != null)
                enemyRb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Obstacle") && canTakeDamage)
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (isInvulnerable) return;

        GameManager.instance.playerHP--;
        SoundFXManager.instance.PlaySoundByName("hit2", transform, 1f, 1f, false);

        StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        canTakeDamage = false;

        float timer = 0f;
        bool visible = true;

        while (timer < invulnDuration)
        {
            visible = !visible;
            spriteRenderer.enabled = visible;
            timer += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;
        canTakeDamage = true;
    }
}