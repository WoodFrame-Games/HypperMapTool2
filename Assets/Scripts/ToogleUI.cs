using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    private Canvas[] canvases;
    private bool uiHidden = false;

    void Start()
    {
        canvases = FindObjectsOfType<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            uiHidden = !uiHidden;
            ToggleAllUI(uiHidden);
        }
    }

    void ToggleAllUI(bool hide)
    {
        foreach (Canvas canvas in canvases)
        {
            CanvasGroup cg = canvas.GetComponent<CanvasGroup>();

            if (cg == null)
                cg = canvas.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = hide ? 0f : 1f;
            cg.blocksRaycasts = !hide; // Чтобы при скрытии UI не ловил клики
        }
    }
}