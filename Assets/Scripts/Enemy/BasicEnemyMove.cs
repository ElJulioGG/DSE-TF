using UnityEngine;

public class BasicEnemyMove : MonoBehaviour
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
            Death();
        }
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
        SoundFXManager.instance.PlaySoundByName("block-destroy", gameObject.transform, 1f, 1f, false);
        Instantiate(enemyDeatheffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
