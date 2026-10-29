using UnityEngine;
using System.IO;

public class SaveFolderInitializer : MonoBehaviour
{
    void Awake()
    {
        // Формируем путь к папке saves
        string savesFolder = Path.Combine(Application.persistentDataPath, "saves");

        // Проверяем, существует ли папка
        if (!Directory.Exists(savesFolder))
        {
            Directory.CreateDirectory(savesFolder);
            Debug.Log($"Папка 'saves' создана по пути: {savesFolder}");
        }
        else
        {
            Debug.Log($"Папка 'saves' уже существует: {savesFolder}");
        }
    }
}