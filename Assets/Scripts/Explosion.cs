using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] public float explosionRadius = 3f;
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
        SoundFXManager.instance.PlaySoundByName("bomb-explode", gameObject.transform, 1f, 1f, false);
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Projectile"))
                continue;

            if (col.CompareTag("Enemy"))
            {
                BasicEnemyMove enemy = col.GetComponent<BasicEnemyMove>();
                col.attachedRigidbody.linearVelocity = Vector2.zero; // Cancel current velocity
                enemy.Death();
                
                if (enemy != null) {
                    Debug.Log("Enemy hit by explosion");
                    enemy.Death();
                }

                SpikeEnemyMove spikeEnemy = col.GetComponent<SpikeEnemyMove>();
                if (spikeEnemy != null && spikeEnemy.isAlmostDeath) {
                    Debug.Log("Spike Enemy hit by explosion");
                    spikeEnemy.Death();

                } /*else if (spikeEnemy != null) {
                    spikeEnemy.PreDeath();
                    Debug.Log("Spike Enemy hit but not almost death");
                    return;
                }*/

                NoisyEnemyMove noisyEnemy = col.GetComponent<NoisyEnemyMove>();
                if (noisyEnemy != null) {
                    Debug.Log("Noisy Enemy hit by explosion");
                    noisyEnemy.Death();
                }

                RunnerEnemyMove runnerEnemy = col.GetComponent<RunnerEnemyMove>();
                if (runnerEnemy != null) {
                    Debug.Log("Runner Enemy hit by explosion");
                    runnerEnemy.Death();
                }

                Rigidbody2D enemyRb = col.attachedRigidbody;
                if (enemyRb != null)
                {
                    // Apply random torque so the body spins
                    //float randomTorque = Random.Range(-30f, 30f);
                    //enemyRb.AddTorque(randomTorque, ForceMode2D.Impulse);
                }
            }



            Rigidbody2D rb = col.attachedRigidbody;
            if (rb == null) continue;

            // Cancel downward momentum before explosion force
            if (rb.linearVelocity.y < 0)

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            // Direction from explosion center to object
            Vector2 direction = (rb.position - (Vector2)transform.position).normalized;

            // Distance from explosion center
            float distance = Vector2.Distance(rb.position, transform.position);

            // --- Distance-based force scaling ---
            // At center -> 100% force
            // At edge -> 50% force
            float t = Mathf.Clamp01(distance / explosionRadius);  // 0 = center, 1 = edge
            float forceMultiplier = Mathf.Lerp(1f, 0.75f, t);

            // Apply explosion impulse
            rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
            print("Forceaplied");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
