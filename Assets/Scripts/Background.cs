using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    [SerializeField] private string cameraTag = "ShowcaseCamera";
    private Transform _cameraTransform;

    void Start()
    {
        // Buscar tag de camara
        GameObject camObj = GameObject.FindGameObjectWithTag(cameraTag);
        if (camObj != null)
        {
            _cameraTransform = camObj.transform;
        }
        else
        {
            Debug.LogError("No se encontr� ninguna c�mara con el tag");
        }
    }

    void LateUpdate()
    {
        if (_cameraTransform == null) return;

        // Sincronizar posici�n del background con la cam
        transform.position = new Vector3(
            transform.position.x,
            _cameraTransform.position.y,
            transform.position.z
        );
    }
}
