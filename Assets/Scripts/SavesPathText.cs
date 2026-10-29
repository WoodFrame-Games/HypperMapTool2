using UnityEngine;
using System.IO;
using TMPro;

public class SavesPathText : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    private void Start()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        if (targetText == null)
        {
            Debug.LogWarning("SavesPathText: TMP_Text не назначен");
            return;
        }

        // Forward slash вместо backslash: TMP парсит "\U" (C:\Users) как юникод-escape
        string path = Path.Combine(Application.persistentDataPath, "saves").Replace('\\', '/');

        targetText.text = prefix + path + suffix;
    }
}
