using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonScene : MonoBehaviour
{
    public RawImage fadeImage;
    public float fadeDuration = 0.5f;

    public Canvas fadeCanvas;

    private void Awake()
    {
        fadeCanvas = fadeImage.GetComponentInParent<Canvas>();
    }

    public void Start()
    {
        StartCoroutine(FadeOutScene());
    }

    private IEnumerator FadeOutScene()
    {
        fadeCanvas.sortingOrder = 3;

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        fadeCanvas.sortingOrder = 1;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        fadeCanvas.sortingOrder = 3;

        fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
