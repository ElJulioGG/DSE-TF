using System.Collections;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Destino (si no es safeRoomDoor)")]
    [SerializeField] private Transform destino;

    [Header("Transición (opcional)")]
    [SerializeField] private bool usarFade = false;
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float duracionFade = 0.4f;
    [SerializeField] private float tiempoAntesDeMover = 0.1f;
    [SerializeField] private float tiempoOscuro = 0.5f;

    [Header("Visual")]
    [SerializeField] private GameObject visualPuerta;

    [Header("Safe Room")]
    [SerializeField] private bool safeRoomDoor = false;

    [Header("ID de esta puerta (solo si NO es safe room)")]
    public int myDoorIndex = 0;

    private bool estaTeletransportando = false;

    private void Start()
    {
        visualPuerta.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (estaTeletransportando) return;
        if (!other.CompareTag("Player")) return;

        visualPuerta.SetActive(true);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Teletransportar(other.transform));
            visualPuerta.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        visualPuerta.SetActive(false);
    }

    private IEnumerator Teletransportar(Transform player)
    {
        estaTeletransportando = true;

        // Bloquear input
        if (GameManager.instance != null)
            GameManager.instance.playerCanInput = false;

        // Fade OUT
        if (usarFade && fadeCanvas != null)
            yield return StartCoroutine(Fade(1f));

        yield return new WaitForSeconds(tiempoAntesDeMover);

        // -----------------------------------------
        //        LÓGICA SAFE ROOM / NORMAL
        // -----------------------------------------

        Transform destinoFinal = destino;

        if (safeRoomDoor)
        {
            // TP a la última puerta usada
            string doorName = "Door" + GameManager.instance.doorIndex;
            GameObject puertaDestino = GameObject.Find(doorName);

            if (puertaDestino != null)
            {
                destinoFinal = puertaDestino.transform;
            }
            else
            {
                Debug.LogWarning("SafeRoomDoor: No se encontró '" + doorName + "'");
            }
        }
        else
        {
            // PUERTA NORMAL → guardar este index como última puerta usada
            GameManager.instance.doorIndex = myDoorIndex;
        }

        // Teletransporte
        if (destinoFinal != null)
        {
            player.position = destinoFinal.position;
        }
        else
        {
            Debug.LogWarning("DoorTeleport: Destino final es null.");
        }

        // -----------------------------------------

        yield return new WaitForSeconds(tiempoOscuro);

        // Fade IN
        if (usarFade && fadeCanvas != null)
            yield return StartCoroutine(Fade(0f));

        // Activar input
        if (GameManager.instance != null)
            GameManager.instance.playerCanInput = true;

        estaTeletransportando = false;
    }

    private IEnumerator Fade(float alphaObjetivo)
    {
        if (fadeCanvas == null) yield break;

        float alphaInicial = fadeCanvas.alpha;
        float t = 0f;

        while (t < duracionFade)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(alphaInicial, alphaObjetivo, t / duracionFade);
            yield return null;
        }

        fadeCanvas.alpha = alphaObjetivo;
    }
}
