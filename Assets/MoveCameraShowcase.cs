using UnityEngine;

public class MoveCameraShowcase : MonoBehaviour
{
    [SerializeField] private float _speedX;
    [SerializeField] private float _speedY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;

        // Para controlar la velocidad
        float updatePosX = pos.x + _speedX * Time.deltaTime;
        float updatePosY = pos.y + _speedY * Time.deltaTime;

        transform.localPosition = new Vector3(updatePosX, updatePosY, pos.z);
    }
}
