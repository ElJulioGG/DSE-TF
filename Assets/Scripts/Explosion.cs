using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float lifetime = 0.5f;

    private float timer;

    void Start()
    {
        Explode();
        timer = lifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        // Optional explosion VFX
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Detect ALL objects with colliders in the explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            Rigidbody2D rb = col.attachedRigidbody;
            if (rb != null)
            {
                // Calculate push direction
                Vector2 direction = (rb.position - (Vector2)transform.position).normalized;

                // Apply force
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }
        }
    }

    // Draw the explosion radius in the Scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
