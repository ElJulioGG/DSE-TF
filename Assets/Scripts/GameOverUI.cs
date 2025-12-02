using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Textos opcionales")]
    [SerializeField] private Text titleText;   // Texto grande "GAME OVER"
    [SerializeField] private Text coinsText;   // Texto para mostrar monedas
    [SerializeField] private Text ducksText;   // Texto para mostrar patos

    private void Start()
    {
        // Mostrar el cursor para usar los botones
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Título
        if (titleText != null)
        {
            titleText.text = "GAME OVER";
        }

        // Monedas
        if (coinsText != null && GameManager.instance != null)
        {
            coinsText.text = "Monedas: " + GameManager.instance.coins;
        }

        // Patos
        if (ducksText != null && GameManager.instance != null)
        {
            ducksText.text = "Patos: " + GameManager.instance.ducksCollected;
        }
    }

    // 👉 Botón "Reintentar"
    public void OnRetryButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.playerDied = false;

            // Nivel que se estaba jugando
            string levelName = GameManager.instance.lastLevelSceneName;
            if (string.IsNullOrEmpty(levelName))
            {
                levelName = "LEVEL 1"; // por si acaso
            }

            SceneManager.LoadScene(levelName);
        }
        else
        {
            // Si por alguna razón no hay GameManager
            SceneManager.LoadScene("LEVEL 1");
        }
    }

    // 👉 Botón "Menú"
    public void OnMenuButton()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.playerDied = false;
        }

        SceneManager.LoadScene("MENU");
    }
}

