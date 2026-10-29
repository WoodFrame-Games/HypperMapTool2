using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("UI")]
    public GameObject LoadPanel;
    public Slider progressBar;
    public Text loadingText;
    public GameObject UIdoc;

    // --- кнопки ---
    public void GoToMenu()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void GoToMars()
    {
        StartCoroutine(LoadSceneAsync("Mars"));
    }
    public void GoToDesertV2()
    {
        StartCoroutine(LoadSceneAsync("DesertV2"));
    }
    public void GoToBase()
    {
        StartCoroutine(LoadSceneAsync("Base"));
    }

    public void GoToFlatGrass()
    {
        StartCoroutine(LoadSceneAsync("FlatGrass"));
    }

    public void GoToCity()
    {
        StartCoroutine(LoadSceneAsync("City"));
    }

    public void GoToDesert()
    {
        StartCoroutine(LoadSceneAsync("Desert"));
    }

    public void GoToBasev2()
    {
        StartCoroutine(LoadSceneAsync("BaseV2"));
    }

    public void GoToSpace()
    {
        StartCoroutine(LoadSceneAsync("Space"));
    }



    // --- асинхронная загрузка ---
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        LoadPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null) progressBar.value = progress;
            if (loadingText != null) loadingText.text = $"Loading: {(progress * 100f):F0}%";

            // Когда почти готово
            if (operation.progress >= 0.9f)
            {
                // Можно добавить задержку, а можно сразу:
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // --- панель сохранений ---
    public void LoadSavePanel()
    {
        this.UIdoc.SetActive(false);
        this.LoadPanel.SetActive(true);
    }

    public void CloseSavePanel()
    {
        this.LoadPanel.SetActive(false);
        this.UIdoc.SetActive(true);
    }
}