using System.Collections;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField] private Transform destino;

    [Header("Transición (opcional)")]
    [SerializeField] private bool usarFade = false;
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float duracionFade = 0.4f;
    [SerializeField] private float tiempoAntesDeMover = 0.1f;

    private bool estaTeletransportando = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Esto se ejecuta mientras algo esté dentro del trigger
        Debug.Log("Dentro del trigger: " + other.name);

        if (estaTeletransportando) return;
        if (!other.CompareTag("Player")) return;

        // W o flecha arriba
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("TELETRANSPORTE disparado");
            StartCoroutine(Teletransportar(other.transform));
        }
    }

    private IEnumerator Teletransportar(Transform player)
    {
        estaTeletransportando = true;

        // Bloquear input SOLO si existe GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.playerCanInput = false;
        }

        if (usarFade && fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(1f));
        }

        yield return new WaitForSeconds(tiempoAntesDeMover);

        if (destino != null)
        {
            player.position = destino.position;
        }
        else
        {
            Debug.LogWarning("DoorTeleport: Destino no asignado en el Inspector");
        }

        if (usarFade && fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(0f));
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.playerCanInput = true;
        }

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
