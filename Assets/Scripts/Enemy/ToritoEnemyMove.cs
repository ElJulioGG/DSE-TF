using UnityEngine;

public class ToritoEnemyMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speedNormal = 2f;
    [SerializeField] private float speedAggro = 5f;

    private float moveSpeed;   // velocidad real
    private int moveDir = 1;   // dirección: +1 derecha, -1 izquierda

    private float limitLeft;
    private float limitRight;

    [Header("Aggro Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float aggroRange = 3f;

    [SerializeField] private GameObject enemyDeatheffect;
    [SerializeField] private PhysicsMaterial2D deathMaterial;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 10;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private float coinSpawnForce = 5f;
    public float deathDuration = 2f;

    private bool isAggro = false;

    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private bool isDeath = false;
    private bool canMove = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        moveSpeed = speedNormal;

        // Buscar jugador si no está asignado
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    public void StartEnemy(float left, float right)
    {
        limitLeft = left;
        limitRight = right;
    }

    private void Update()
    {
        if (isDeath) return;

        HandleAggroCheck();

        if (!canMove)
        {
            body.linearVelocityX = 0;
            return;
        }

        Move();
    }

    private void HandleAggroCheck()
    {
        if (player == null) return;

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);

        // Está dentro del rango horizontal
        bool isInRange = distanceX <= aggroRange;

        // Detección frontal según moveDir
        bool isInFront = false;

        if (moveDir > 0)
        {
            // Mirando a la derecha → jugador debe estar a la derecha
            isInFront = player.position.x > transform.position.x;
        }
        else
        {
            // Mirando a la izquierda → jugador debe estar a la izquierda
            isInFront = player.position.x < transform.position.x;
        }

        bool shouldAggro = isInRange && isInFront;

        if (shouldAggro && !isAggro)
            EnterAggro();
        else if (!shouldAggro && isAggro)
            ExitAggro();
    }

    private void EnterAggro()
    {
        isAggro = true;
        moveSpeed = speedAggro;
        animator.SetBool("isAggro", true);
    }

    private void ExitAggro()
    {
        isAggro = false;
        moveSpeed = speedNormal;
        animator.SetBool("isAggro", false);
    }

    private void Move()
    {
        float posX = transform.position.x;

        // si toca límites, invierte SOLO la dirección
        if (posX <= limitLeft && moveDir < 0)
            moveDir = 1;

        if (posX >= limitRight && moveDir > 0)
            moveDir = -1;

        // aplicar movimiento
        body.linearVelocityX = moveDir * moveSpeed;

        // flip visual
        sprite.flipX = moveDir < 0;
    }
    public void Death()
    {
        SoundFXManager.instance.PlaySoundByName("fly-spit", transform, 1f, 1f, false);

        canMove = false;
        isDeath = true;
        animator.SetBool("isDeath", true);

        // Disable horizontal collisions so enemy doesn't get stuck
        // Physics2D.IgnoreLayerCollision(enemyLayer, platformLayer, true);
        //Physics2D.IgnoreLayerCollision(enemyLayer, borderLayer, true);

        // Assign the bouncy material
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = deathMaterial;
        }

        // Allow ragdoll physics
        if (body != null)
        {
            body.gravityScale = 1;
            // DO NOT reset velocity here — explosion pushes enemy!
        }

        Invoke("DestroyEnemy", deathDuration);
    }

    private void DestroyEnemy()
    {
        SoundFXManager.instance.PlaySoundByName("block-destroy", transform, 1f, 1f, false);
        Instantiate(enemyDeatheffect, transform.position, Quaternion.identity);

        int coinCount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = PoolManager.instance.Spawn("Coin", transform.position, Quaternion.identity);

            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float angle = Random.Range(0f, 360f);
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                rb.AddForce(dir * coinSpawnForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}

