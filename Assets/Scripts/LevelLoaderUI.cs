using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using RuntimeGizmos;

public class LevelLoaderUI : MonoBehaviour
{
    private TransformGizmo gizmo;

    private void Awake()
    {
        gizmo = GetComponent<TransformGizmo>();
    }

    // Публичный вход для списков сохранений (SaveButton/LevelListSpawner)
    public void LoadByName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("Пустое имя сохранения!");
            return;
        }

        SerializableLevel level = SaveLoader.Load(fileName);

        if (level == null)
        {
            Debug.LogWarning("Сохранение не найдено: " + fileName);
            return;
        }

        StartCoroutine(LoadLevelRoutine(level));
    }

    private System.Collections.IEnumerator LoadLevelRoutine(SerializableLevel level)
    {
        // Убираем ранее загруженные сохранения, чтобы не наслаивались друг на друга
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root != null && root.name.StartsWith("LoadedLevel_"))
                Destroy(root);
        }

        // Даём Unity уничтожить старые объекты до создания новых
        yield return null;

        GameObject levelRoot = new GameObject("LoadedLevel_" + level.map);

        List<GameObject> spawned = new List<GameObject>();

        foreach (var propData in level.props)
        {
            GameObject prefab = Resources.Load<GameObject>("Available/" + propData.name);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found: " + propData.name);
                continue;
            }

            GameObject obj = Instantiate(prefab, levelRoot.transform);

            obj.GetComponent<SavableProp>()?.ApplySerializableProp(propData);

            spawned.Add(obj);
        }

        // Связи из файла (saveId) -> порядковые номера + отрисовка проводов
        SavableProp.ResolveAllConnections();

        Debug.Log($"Loaded {spawned.Count} objects.");

        // WAIT ONE FRAME ⚠️ (Critical fix)
        yield return null;

        // Теперь gizmo можно обновлять
        if (gizmo == null)
            gizmo = GetComponent<TransformGizmo>();

        if (gizmo == null)
            gizmo = FindObjectOfType<TransformGizmo>();

        if (gizmo != null)
        {
            gizmo.ClearTargets();

            foreach (var o in spawned)
                gizmo.AddTarget(o.transform);
        }
        else
        {
            Debug.LogWarning("LevelLoaderUI: TransformGizmo не найден — селекция пропущена");
        }
    }
}
