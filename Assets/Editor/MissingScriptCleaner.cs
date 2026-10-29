using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingScriptsCleaner : EditorWindow
{
    [MenuItem("Tools/Cleanup Missing Scripts in Scene")]
    private static void CleanupInScene()
    {
        int count = 0;
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }
        Debug.Log($"Удалено {count} Missing Script из текущей сцены.");
    }

    [MenuItem("Tools/Cleanup Missing Scripts in ALL Prefabs (Project)")]
    private static void CleanupInProject()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int total = 0;
        int processedPrefabs = 0;

        EditorUtility.DisplayProgressBar("Очистка Missing Scripts", "Поиск префабов...", 0f);

        try
        {
            for (int i = 0; i < prefabGUIDs.Length; i++)
            {
                string guid = prefabGUIDs[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                EditorUtility.DisplayProgressBar("Очистка Missing Scripts", 
                    $"Обработка: {System.IO.Path.GetFileName(path)}", 
                    (float)i / prefabGUIDs.Length);

                if (prefab != null)
                {
                    // Рекурсивная проверка всех дочерних объектов
                    int count = RemoveMissingScriptsRecursively(prefab);
                    if (count > 0)
                    {
                        Debug.Log($"Prefab {path}: удалено {count} Missing Script");
                        EditorUtility.SetDirty(prefab);
                        total += count;
                    }
                    processedPrefabs++;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Итого удалено {total} Missing Script из {processedPrefabs} префабов.");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    // Рекурсивное удаление Missing Scripts со всех дочерних объектов
    private static int RemoveMissingScriptsRecursively(GameObject gameObject)
    {
        int count = 0;
        
        // Удаляем Missing Scripts с текущего объекта
        count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        
        // Рекурсивно проверяем всех детей
        foreach (Transform child in gameObject.transform)
        {
            count += RemoveMissingScriptsRecursively(child.gameObject);
        }
        
        return count;
    }

    [MenuItem("Tools/Add SavableProp to All Prefabs")]
    private static void AddSavablePropToAllPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int addedCount = 0;

        EditorUtility.DisplayProgressBar("Добавление SavableProp", "Поиск префабов...", 0f);

        try
        {
            for (int i = 0; i < prefabGUIDs.Length; i++)
            {
                string guid = prefabGUIDs[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                EditorUtility.DisplayProgressBar("Добавление SavableProp", 
                    $"Обработка: {System.IO.Path.GetFileName(path)}", 
                    (float)i / prefabGUIDs.Length);

                if (prefab != null && prefab.GetComponent<SavableProp>() == null)
                {
                    var savable = prefab.AddComponent<SavableProp>();
                    savable.prefabName = prefab.name;

                    EditorUtility.SetDirty(prefab);
                    Debug.Log($"Добавлен SavableProp на {path}");
                    addedCount++;
                }
            }AssetDatabase.SaveAssets();
            Debug.Log($"SavableProp добавлен на {addedCount} префаб(ов).");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    // Окно редактора для массовой очистки
    [MenuItem("Tools/Missing Scripts Cleaner Window")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptsCleaner>("Missing Scripts Cleaner");
    }

    private Vector2 scrollPosition;

    void OnGUI()
    {
        GUILayout.Label("Очистка Missing Scripts", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        if (GUILayout.Button("Очистить Missing Scripts на сцене", GUILayout.Height(30)))
        {
            CleanupInScene();
        }
        
        if (GUILayout.Button("Очистить Missing Scripts во ВСЕХ префабах", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Подтверждение", 
                "Вы уверены, что хотите удалить ВСЕ Missing Scripts со ВСЕХ префабов в проекте? Это действие нельзя отменить.", 
                "Да", "Нет"))
            {
                CleanupInProject();
            }
        }
        
        if (GUILayout.Button("Добавить SavableProp на все префабы", GUILayout.Height(30)))
        {
            AddSavablePropToAllPrefabs();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.HelpBox("Внимание: Очистка Missing Scripts во всех префабах может занять некоторое время в больших проектах.", MessageType.Info);
    }
}