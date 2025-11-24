using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionFade : MonoBehaviour
{
    [SerializeField] private GameObject blackImg;
    [SerializeField] private float fadeTime = 1f;

    private Image img;
    private float fadeTimer = 0;

    void Awake()
    {
        img = blackImg.GetComponent<Image>();
    }

    public void FadeIn()
    {
        blackImg.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        fadeTimer = 0;

        Color imgColor = img.color;
        imgColor.a = 1;
        img.color = imgColor;

        while (fadeTimer < fadeTime)
        {
            fadeTimer += Time.deltaTime;
            imgColor.a = Mathf.Lerp(1, 0, fadeTimer / fadeTime);
            img.color = imgColor;
            yield return null;
        }

        imgColor.a = 0;
        img.color = imgColor;

        blackImg.SetActive(false);
    }
    private void Start()
    {
        FadeIn();
    }
}
