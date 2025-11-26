using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D rbody;
    private Camera mainCam;

    [SerializeField] GameObject ExplosionObject;
    [SerializeField] GameObject RangeSprite;

    public float detTime = 0;
    public float cooldown = 0.7f;
    public bool canDet = false;

    public bool stuck = false;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x);
        Vector2 releaseVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        rbody.linearVelocity = releaseVector * 10f;
    }

    private void Update()
    {
        // Cooldown to permit detonation
        if (!canDet)
        {
            detTime += Time.deltaTime;
            if (detTime >= cooldown)
                canDet = true;
        }
    }

    public void Explode()
    {
        if (!canDet) return;     // Prevent early explosion

        // Create explosion
        GameObject obj = Instantiate(ExplosionObject, transform.position, Quaternion.identity);

        // Safely scale range sprite
        if (RangeSprite != null)
        {
            float radius = obj.GetComponent<Explosion>().explosionRadius;
            RangeSprite.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        Destroy(gameObject);
    }

    public void ForceExplode()
    {
        // Always explodes even if cooldown not finished
        GameObject obj = Instantiate(ExplosionObject, transform.position, Quaternion.identity);

        if (RangeSprite != null)
        {
            float radius = obj.GetComponent<Explosion>().explosionRadius;
            RangeSprite.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") && !stuck)
        {
            stuck = true;

            var joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.rigidbody; // floor may not have rigidbody

            rbody.linearVelocity = Vector2.zero;
            rbody.angularVelocity = 0;
        }
    }
}
