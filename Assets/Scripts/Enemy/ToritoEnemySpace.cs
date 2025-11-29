using UnityEngine;

public class ToritoEnemySpace : MonoBehaviour
{
    [SerializeField] private ToritoEnemyMove enemyReference;
    [SerializeField] private Transform limitL;
    [SerializeField] private Transform limitR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Podemos cargar al enemigo con script
        float leftX = Mathf.Min(limitL.localPosition.x, limitR.localPosition.x);
        float rightX = Mathf.Max(limitL.localPosition.x, limitR.localPosition.x);
        enemyReference.StartEnemy(leftX, rightX);
    }

}
