using UnityEngine;

public class BatEnemySpace_UD : MonoBehaviour
{
    [SerializeField] private BatEnemyMove_UD enemyReference;
    [SerializeField] private Transform limitUp;
    [SerializeField] private Transform limitDown;

    void Start()
    {
        float downY = Mathf.Min(limitUp.localPosition.y, limitDown.localPosition.y);
        float upY = Mathf.Max(limitUp.localPosition.y, limitDown.localPosition.y);

        enemyReference.StartEnemy(downY, upY);
    }
}
