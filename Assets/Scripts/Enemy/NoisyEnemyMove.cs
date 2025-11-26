using UnityEngine;
using System.Collections;

public class NoisyEnemyMove : MonoBehaviour
{
    [SerializeField] private float speedX;
    [SerializeField] private GameObject enemyDeatheffect;
    [SerializeField] private PhysicsMaterial2D deathMaterial;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 10;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private float coinSpawnForce = 5f;

    private float limitLeft;
    private float limitRight;
    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private bool isDeath = false;
    public float noiseDuration = 1f;
    public float deathDuration = 2f;
    private int platformLayer;
    private int borderLayer;
    private int enemyLayer;
    private bool canMove = true;
    private bool isNoisePlaying = false;


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
        if (!canMove)
        {
            body.linearVelocityX = 0;
            return;
        }

        float posX = transform.localPosition.x;

        if (!isNoisePlaying && ((posX < limitLeft && speedX < 0) || (posX > limitRight && speedX > 0)))
        {
            StartCoroutine(MakeNoise());
            return; 
        }

        body.linearVelocityX = speedX;
        sprite.flipX = speedX > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            Death();
        }
    }

    private IEnumerator MakeNoise()
    {
        isNoisePlaying = true;

        // Detener movimiento
        body.linearVelocityX = 0;
        canMove = false;

        // Reproducir animación
        animator.SetBool("isMakingNoise", true);
        SoundFXManager.instance.PlaySoundByName("noise-roar", transform, 1f, 1f, false);

        // Duración del ruido
        yield return new WaitForSeconds(noiseDuration);

        animator.SetBool("isMakingNoise", false);

        // Reanudar movimiento
        Debug.Log("Noise finished, resuming movement.");
        speedX *= -1;
        canMove = true;
        isNoisePlaying = false;
    }

    private IEnumerator DeathCoroutine()
    {
        canMove = false;
        isDeath = true;

        animator.SetBool("isDeath", true);

        Physics2D.IgnoreLayerCollision(enemyLayer, platformLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, borderLayer, true);

        yield return new WaitForSeconds(deathDuration);

        Destroy(gameObject);
    }

    public void Death()
    {
        SoundFXManager.instance.PlaySoundByName("fly-spit", transform, 1f, 1f, false);
        canMove = false;
        isDeath = true;

        animator.SetBool("isDeath", isDeath);

        // Ignore collisions so it doesn’t collide horizontally
        Physics2D.IgnoreLayerCollision(enemyLayer, platformLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, borderLayer, true);

        // Apply the BOUNCY MATERIAL
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = deathMaterial;
        }

        // Give it a vertical bounce impulse (optional)
        if (body != null)
        {
            body.linearVelocity = new Vector2(0, 10f); // modify force if needed
            body.gravityScale = 1; // ensure gravity works
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
