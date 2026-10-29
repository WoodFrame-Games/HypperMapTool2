using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class SaveButton : MonoBehaviour
{
    public TMP_Text fileNameText; // UI Text для отображения имени файла
    public Button button;
    public Button deleteButton;   // Кнопка удаления сохранения
    public TMP_Text mapNameText;  // TMP Text для названия карты из сейва

    [Header("Опционально: передавать имя в LevelLoaderUI вместо SaveManager")]
    [SerializeField] private LevelLoaderUI levelLoader;
    [SerializeField] private GameObject listPanel; // Панель списка — закроется при загрузке

    private string fileName;

    public void SetLevelLoader(LevelLoaderUI loader)
    {
        levelLoader = loader;
    }

    public void SetListPanel(GameObject panel)
    {
        listPanel = panel;
    }

    public void SetUp(string fname)
    {
        fileName = fname;

        if (fileNameText != null)
            fileNameText.text = fname;

        if (mapNameText != null)
        {
            string mapName = ReadMapName(fname);
            mapNameText.text = string.IsNullOrEmpty(mapName) ? "-" : mapName;
        }

        if (button != null)
            button.onClick.AddListener(OnClick);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDelete);
    }

    // Быстрое чтение имени карты из заголовка сейва (без полной десериализации)
    private string ReadMapName(string fname)
    {
        string json = SaveSystem.LoadFile(fname);
        if (string.IsNullOrEmpty(json))
            return null;

        Match match = Regex.Match(json, "\"map\"\\s*:\\s*\"([^\"]*)\"");
        return match.Success ? match.Groups[1].Value : null;
    }

    private void OnClick()
    {
        if (levelLoader != null)
        {
            // Режим списка загрузки уровня: имя -> LevelLoaderUI
            levelLoader.LoadByName(fileName);

            if (listPanel != null)
                listPanel.SetActive(false);

            return;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Load(fileName);
        }
        else
        {
            Debug.LogError("SaveManager instance not found!");
        }
    }

    private void OnDelete()
    {
        SaveSystem.DeleteFile(fileName);
        Destroy(gameObject);
    }
}
