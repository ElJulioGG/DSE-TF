using UnityEngine;

public class BasicEnemyMove : MonoBehaviour
{
    [SerializeField] private float speedX;

    private float limitLeft;
    private float limitRight;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
    public void StartEnemy(float limitLeft, float limitRight)
    {
        this.limitLeft = limitLeft;
        this.limitRight = limitRight;
    }

    void Update()
    {
        float posX = transform.localPosition.x;
        if (posX < limitLeft && speedX < 0 || posX > limitRight && speedX > 0)
        {
            speedX *= -1;
        }
        body.linearVelocityX = speedX;
        //sprite.flipX = speedX < 0 ? true : false;
    }
}
