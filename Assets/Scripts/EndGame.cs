using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MENU";
    [SerializeField] private string victorySoundName = "victory";
    [SerializeField] private float loadDelay = 1f;

    private bool finished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (finished) return;

        if (collision.CompareTag("Player"))
        {
            finished = true;

            // Reproducir sonido de victoria
            SoundFXManager.instance.PlaySoundByName(victorySoundName, transform, 1f, 1f, false);
            SoundFXManager.instance.StopSoundByName("bgm");

            // Cargar escena con delay
            Invoke(nameof(LoadMenu), loadDelay);
        }
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
