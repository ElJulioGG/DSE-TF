using UnityEngine;
using Random = UnityEngine.Random;

public class BatEnemyMove_RL : MonoBehaviour
{
    [SerializeField] private float speedX;

    private float limitLeft;
    private float limitRight;
    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private bool isDeath;
    public float deathDuration = 2f;
    private int platformLayer;
    private int borderLayer;
    private int enemyLayer;
    private bool canMove = true;

    [SerializeField] private GameObject enemyDeatheffect;
    [SerializeField] private PhysicsMaterial2D deathMaterial;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 10;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private float coinSpawnForce = 5f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        platformLayer = LayerMask.NameToLayer("Floor");
        borderLayer = LayerMask.NameToLayer("Border");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    public void StartEnemy(float limitLeft, float limitRight)
    {
        this.limitLeft = limitLeft;
        this.limitRight = limitRight;
    }

    void Update()
    {
        // STOP movement overrides once dead
        if (isDeath) return;

        if (!canMove)
        {
            body.linearVelocityX = 0;
            return;
        }

        float posX = transform.localPosition.x;

        if (posX < limitLeft && speedX < 0 || posX > limitRight && speedX > 0)
        {
            speedX *= -1;
        }

        body.linearVelocityX = speedX;
        sprite.flipX = speedX < 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            Death();
        }
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
