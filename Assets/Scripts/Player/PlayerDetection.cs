using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerDetection : MonoBehaviour
{
    private Vector3 checkpointPosition;
    private int playerLayer;
    private int enemyLayer;
    private int obstacleLayer;
    private bool isInvincible;
    private float countingInvincible;
    public float iframeDuration = 3f;

    [SerializeField] private PlayerStatus playerStats;
    private void Start()
    {
        // Ubicamos el checkpoint usando la etiqueta
        // GameObject checkpoint = GameObject.FindGameObjectWithTag("Checkpoint");
        // Guardamos su posición en una variable (campo o propiedad)
        // checkpointPosition = checkpoint.transform.localPosition;
        countingInvincible = 0f;
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    private void Update()
    {
        if (isInvincible)
        {
            countingInvincible += Time.deltaTime;
            print("Timer: " + countingInvincible);
        }
        if (countingInvincible >= 2f)
        {
            isInvincible = false;
            countingInvincible = 0f;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
        }
    }

    private void Invincible()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, true);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detectamos al enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerStats.damage();
            this.Invincible();
            isInvincible = true;
            Debug.Log("Damage, te quedan vida: " + playerStats.get_lifes());
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            playerStats.damage();
            this.Invincible();
            isInvincible = true;
            Debug.Log("Damage, te quedan vida: " + playerStats.get_lifes());
        }

        if (playerStats.get_lifes() == 0)
        {
            playerStats.total_damage();
            playerStats.restart_lifes();
            Debug.Log("Total Damage, te quedan full_vida: " + playerStats.get_full_lifes());
        }
    }
}
