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
    public void Death()
    {
        SoundFXManager.instance.PlaySoundByName("EnemyDeath",gameObject.transform,1f,1f,false);
        canMove = false;
        isDeath = true;
        animator.SetBool("isDeath", isDeath);
        Physics2D.IgnoreLayerCollision(enemyLayer, platformLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, borderLayer, true);

        float t = 0f;

        while (t < deathDuration)
        {
            t += Time.deltaTime;
        }


        Invoke("DestroyEnemy", 2f);
    }
    private void DestroyEnemy()
    {
        SoundFXManager.instance.PlaySoundByName("EnemyExplode", gameObject.transform, 1f, 1f, false);
        Instantiate(enemyDeatheffect, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
