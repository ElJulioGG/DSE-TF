using UnityEngine;

public class BatEnemySpace_RL : MonoBehaviour
{
    [SerializeField] private BatEnemyMove_RL enemyReference;
    [SerializeField] private Transform limitL;
    [SerializeField] private Transform limitR;
    void Start()
    {
        // Podemos cargar al enemigo con script
        float leftX = Mathf.Min(limitL.localPosition.x, limitR.localPosition.x);
        float rightX = Mathf.Max(limitL.localPosition.x, limitR.localPosition.x);
        enemyReference.StartEnemy(leftX, rightX);
    }
}
