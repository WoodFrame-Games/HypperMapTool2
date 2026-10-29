using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance; // Singleton

    public InputField InputFieldfn;    // UI InputField для имени файла

    [Header("UI")]
    public GameObject LoadPanel;
    public Slider progressBar;
    public Text loadingText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    public void Save()
    {
        string fname = InputFieldfn.text;
        var level = new SerializableLevel
        {
            map = SceneManager.GetActiveScene().name,
            props = CollectProps()
        };

        string json = JsonConvert.SerializeObject(level, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented
        });

        SaveSystem.SaveFile(fname, json);
        Debug.Log($"Level saved to {fname}");
    }

    private SerializableProp[] CollectProps()
    {
        // Сортируем по порядковому айди (порядок спавна), чтобы в файле
        // объекты шли от первого к последнему и грузились так же
        List<SavableProp> props = new List<SavableProp>(FindObjectsOfType<SavableProp>());
        props.Sort((a, b) => a.uniqueId.CompareTo(b.uniqueId));

        List<SerializableProp> list = new List<SerializableProp>();
        foreach (var obj in props)
            list.Add(obj.ToSerializable());
        return list.ToArray();
    }

    public void Load(string fname)
    {
        string json = SaveSystem.LoadFile(fname);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"No save found with name {fname}");
            return;
        }

        var level = JsonConvert.DeserializeObject<SerializableLevel>(json);
        loadingLevel = level;

        // Одноразовая подписка: снимаем предыдущую, иначе каждый следующий
        // переход сцены (в меню и обратно) спавнил сохранение заново.
        SceneManager.sceneLoaded -= OnSceneLoadedSpawnLevel;
        SceneManager.sceneLoaded += OnSceneLoadedSpawnLevel;

        StartCoroutine(LoadSceneAsync2(level.map));
        Debug.Log($"Loading level {level.map} from {fname}");
    }

    private SerializableLevel loadingLevel;

    private void OnSceneLoadedSpawnLevel(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedSpawnLevel;

        if (loadingLevel == null)
            return;

        // Спавним только в ту сцену, которую просили загрузить
        if (!string.Equals(scene.name, loadingLevel.map, System.StringComparison.OrdinalIgnoreCase))
            return;

        SerializableLevel level = loadingLevel;
        loadingLevel = null;

        SavableProp.ResetRuntimeIds();

        foreach (var p in level.props)
        {
            GameObject prefab = Resources.Load<GameObject>("Available/" + p.name);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab missing: " + p.name);
                continue;
            }

            var obj = Instantiate(prefab);
            obj.GetComponent<SavableProp>()?.Apply(p);
        }

        SavableProp.ResolveAllConnections();
    }

        // --- асинхронная загрузка ---
    private IEnumerator LoadSceneAsync2(string sceneName)
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
}