using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float coinVolume = 1f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float flashPercent = 0.3f; // Last 30% flashes
    [SerializeField] private float flashSpeed = 0.15f;

    [SerializeField] private AudioSource coinBounceSound;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private int floorLayer;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        floorLayer = LayerMask.NameToLayer("Floor");

        // --- RANDOM STARTING TORQUE ---
        if (rb != null)
        {
            float randomTorque = Random.Range(-1f, 1f);
            rb.AddTorque(randomTorque, ForceMode2D.Impulse);
        }

        StartCoroutine(AutoDestroyRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundFXManager.instance.PlaySoundByName("getcoin", transform, 1f, 1f, false);
            GameManager.instance.coins++;
            DestroyCoin();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == floorLayer)
        {
            float randomPitch = Random.Range(0.7f, 1f);
            coinBounceSound.Play();
            coinBounceSound.pitch = randomPitch;
        }
    }

    private void DestroyCoin()
    {
        Destroy(gameObject);
    }

    private IEnumerator AutoDestroyRoutine()
    {
        float flashStartDelay = lifetime * (1f - flashPercent);
        float flashDuration = lifetime * flashPercent;

        // Wait until it’s time to flash
        yield return new WaitForSeconds(flashStartDelay);

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += flashSpeed;
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(flashSpeed);
        }

        sprite.enabled = true;
        Destroy(gameObject);
    }
}
