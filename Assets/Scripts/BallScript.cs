using UnityEngine;

public class BallScript : MonoBehaviour
{   
    private Rigidbody2D rbody;
    private Camera mainCam;
    [SerializeField] GameObject ExplosionObject;

    public float detTime = 0;
    public bool canDet = false;

    public bool stuck = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rbody = GetComponent<Rigidbody2D>();

        //rbody.linearVelocity = new Vector3(0, 2, 0);

        mainCam = Camera.main;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        //rbody.velocity = new Vector3(0, 5, 0);

        // Calculate rotation towards mouse position
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);// * Mathf.Rad2Deg;

        Vector3 releaseVector = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        rbody.linearVelocity = releaseVector * 10f;
    }

    public void Explode()
    {
        //holy shit Run or Explode referencia
        Instantiate(ExplosionObject, transform.position, Quaternion.identity);
        Destroy(gameObject,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (canDet == false)
        {
            detTime += Time.deltaTime;

            if (detTime >= 0.4)
            {
                canDet = true;
            }
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") && !stuck)
        {
            stuck = true;
            var joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.rigidbody; // can be null if floor has no Rigidbody
            rbody.linearVelocity = Vector2.zero;
            rbody.angularVelocity = 0;
        }
    }


}
