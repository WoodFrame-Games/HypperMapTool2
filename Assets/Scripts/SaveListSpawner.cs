using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveListSpawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject saveButtonPrefab; // Префаб кнопки
    public Transform contentParent; // Content ScrollView


    void Start()
    {
        // На карте за список отвечает LevelListSpawner (он же прокидывает
        // levelLoader в кнопки). Если он есть — не наполняем контейнер второй раз.
        if (FindObjectsOfType<LevelListSpawner>(true).Length > 0)
            return;

        string saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(saveFolder))
        {
            Debug.LogWarning("Папка сохранений не найдена: " + saveFolder);
            return;
        }

        if (contentParent != null)
        {
            for (int i = contentParent.childCount - 1; i >= 0; i--)
                Destroy(contentParent.GetChild(i).gameObject);
        }

        string[] files = Directory.GetFiles(saveFolder, "*.svn");
        System.Array.Sort(files);
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject buttonObj = Instantiate(saveButtonPrefab, contentParent);
            SaveButton sb = buttonObj.GetComponent<SaveButton>();
            if (sb != null)
            {
                sb.SetUp(fileName);
            }
        }
    }
}