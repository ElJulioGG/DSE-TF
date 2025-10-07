using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float shotDelay = 0.5f;//segundso
    [SerializeField] private float shotTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float explosionForce = 10f;

    public GameObject explosionEffect; // optional particle prefab

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
 
    void Start()
    {
        
        Explode();
    }
    public void Explode()
    {
        // Spawn visual effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);

        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Calculate push direction
                Vector2 direction = (enemyRb.position - (Vector2)transform.position).normalized;
                enemyRb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject); // remove bullet
    }
    // Update is called once per frame
    void Update()
    {
        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }
        if (shotTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
