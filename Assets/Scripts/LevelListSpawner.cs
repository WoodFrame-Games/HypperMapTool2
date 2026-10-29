using UnityEngine;
using System.IO;

public class LevelListSpawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject saveButtonPrefab; // Тот же префаб кнопки (SaveButton)
    public Transform contentParent;     // Content ScrollView
    public LevelLoaderUI levelLoader;   // Ссылка на LevelLoaderUI — клик по сейву передаёт имя сюда
    public GameObject panelToClose;     // Панель списка — закроется при клике по сейву

    private void Start()
    {
        string saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(saveFolder))
        {
            Debug.LogWarning("Папка сохранений не найдена: " + saveFolder);
            return;
        }

        // Чистим контейнер: SaveListSpawner из вложенного SavesList мог
        // успеть насыпать туда те же самые сохранения.
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
                sb.SetLevelLoader(levelLoader);
                sb.SetListPanel(panelToClose);
                sb.SetUp(fileName);
            }
        }
    }
}
