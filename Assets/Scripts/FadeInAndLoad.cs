using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FadeInAndLoad : MonoBehaviour
{
    public TextMeshProUGUI textToFade;
    public float fadeInDuration = 3f;
    public float visibleDuration = 2f;
    public float fadeOutDuration = 3f;
    public string nextSceneName = "Lobby";

    void Start()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        Color color = textToFade.color;
        color.a = 0f;
        textToFade.color = color;

        // Fade In
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeInDuration);
            textToFade.color = color;
            yield return null;
        }

        // Stay visible
        yield return new WaitForSeconds(visibleDuration);

        // Fade Out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsed / fadeOutDuration));
            textToFade.color = color;
            yield return null;
        }

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}