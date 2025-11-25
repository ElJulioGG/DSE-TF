using UnityEngine;

public class NoisyEnemySpace : MonoBehaviour
{
    [SerializeField] private NoisyEnemyMove enemyReference;
    [SerializeField] private Transform limitL;
    [SerializeField] private Transform limitR;

    void Start()
    {
        float leftX = Mathf.Min(limitL.localPosition.x, limitR.localPosition.x);
        float rightX = Mathf.Max(limitL.localPosition.x, limitR.localPosition.x);
        enemyReference.StartEnemy(leftX, rightX);
    }
}
