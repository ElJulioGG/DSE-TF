using UnityEngine;
using System.Collections;

public class SpikeEnemyMove : MonoBehaviour
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
    public bool isAlmostDeath;
    private bool isDeath;
    public float painDuration = 1f;
    public float deathDuration = 2f;
    private int platformLayer;
    private int borderLayer;
    private int enemyLayer;
    private bool canMove = true;

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
        if (posX < limitLeft && speedX < 0 || posX > limitRight && speedX > 0)
        {
            speedX *= -1;
        }
        body.linearVelocityX = speedX;
        sprite.flipX = speedX < 0 ? true : false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            if (!isAlmostDeath)
            {
                Debug.Log("PreDeath Spike Enemy");
                PreDeath();
            }
            else if (!isDeath)
            {
                Debug.Log("Death Spike Enemy");
                Death();
            }
        }
    }

    public void PreDeath()
    {
        StartCoroutine(PreDeathCoroutine());
    }

    private IEnumerator PreDeathCoroutine()
    {
        canMove = false;

        body.linearVelocity = Vector2.zero;
        body.simulated = false;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var c in colliders)
            c.enabled = false;

        animator.SetBool("Pain", true);
        SoundFXManager.instance.PlaySoundByName("pain", transform, 1f, 1f, false);
        Debug.Log("Spike Enemy Pain Animation");

        yield return new WaitForSeconds(painDuration);

        body.simulated = true;
        foreach (var c in colliders)
            c.enabled = true;

        isAlmostDeath = true;
        animator.SetBool("isAlmostDeath", isAlmostDeath);

        Debug.Log("Pain duration finished → now AlmostDeath");
    }

    public void Death()
    {

        SoundFXManager.instance.PlaySoundByName("fly-spit", transform, 1f, 1f, false);
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
