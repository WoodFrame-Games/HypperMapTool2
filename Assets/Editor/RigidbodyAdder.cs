using UnityEngine;
using UnityEditor;
using System.IO;

public class RigidbodyAdder : EditorWindow
{
    [MenuItem("Tools/Add Rigidbody To Prefabs")]
    public static void AddRigidbodyToPrefabs()
    {
        string folderPath = "Assets/Resources/Available";

        // Получаем все GUID префабов в папке
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        int addedCount = 0;
        int skippedCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab == null)
                continue;

            // Проверяем наличие Rigidbody
            if (prefab.GetComponent<Rigidbody>() != null)
            {
                skippedCount++;
                continue;
            }

            // Открываем префаб для редактирования
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

            if (prefabRoot.GetComponent<Rigidbody>() == null)
            {
                prefabRoot.AddComponent<Rigidbody>();
                addedCount++;
            }

            // Сохраняем изменения
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        Debug.Log($"Rigidbodies added: {addedCount}, skipped: {skippedCount}");
    }
}