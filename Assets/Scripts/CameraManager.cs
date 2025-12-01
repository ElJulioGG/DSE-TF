using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras Registradas Automáticamente")]
    public List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    [Header("Prioridades")]
    public int activePriority = 20;
    public int inactivePriority = 5;

    void Awake()
    {
        // Registrar automáticamente todas las cámaras CinemachineCamera de la escena
        cameras.Clear();
        cameras.AddRange(FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None));
    }

    /// <summary>
    /// Activa una cámara específica y desactiva el resto ajustando prioridades.
    /// </summary>
    public void ActivateCamera(CinemachineCamera targetCam)
    {
        if (targetCam == null)
        {
            Debug.LogWarning("CameraManager: La cámara a activar es null.");
            return;
        }

        foreach (var cam in cameras)
        {
            if (cam == null)
                continue;

            cam.Priority = (cam == targetCam) ? activePriority : inactivePriority;
        }
    }

    /// <summary>
    /// Activa una cámara buscándola por nombre en la lista.
    /// </summary>
    public void ActivateCamera(string cameraName)
    {
        CinemachineCamera cam = cameras.Find(c => c.name == cameraName);

        if (cam == null)
        {
            Debug.LogWarning($"CameraManager: No se encontró la cámara con nombre '{cameraName}'.");
            return;
        }

        ActivateCamera(cam);
    }

    /// <summary>
    /// Cambia la prioridad de una cámara específica sin modificar las otras.
    /// </summary>
    public void SetCameraPriority(CinemachineCamera cam, int newPriority)
    {
        if (cam == null)
        {
            Debug.LogWarning("CameraManager: La cámara recibida es null.");
            return;
        }

        cam.Priority = newPriority;
    }
}
