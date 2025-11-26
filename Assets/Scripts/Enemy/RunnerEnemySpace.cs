using UnityEngine;

public class RunnerEnemySpace : MonoBehaviour
{
    [SerializeField] private RunnerEnemyMove enemyReference;
    [SerializeField] private Transform limitL;
    [SerializeField] private Transform limitR;

    void Start()
    {
        float leftX = Mathf.Min(limitL.localPosition.x, limitR.localPosition.x);
        float rightX = Mathf.Max(limitL.localPosition.x, limitR.localPosition.x);
        enemyReference.StartEnemy(leftX, rightX);
    }
}
