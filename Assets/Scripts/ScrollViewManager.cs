using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScrollViewManager : MonoBehaviour
{
    [Header("UI")]
    public Transform contentParent;       // Контейнер ScrollView (Content)
    public GameObject buttonPrefab;       // Префаб кнопки
    public TMP_InputField filterInput;    // Поле для фильтра
    public ScrollRect scrollRect;         // Ссылка на ScrollRect

    [Header("Filter Buttons")]
    public Button allButton;
    public Button availableButton;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    [Header("Спавн")]
    public Camera spawnCamera;            // Камера для спавна
    public float spawnDistance = 2f;      // Дистанция перед камерой
    public LayerMask obstacleLayers = ~0; // Слои для проверки препятствий

    private static GameObject[] prefabs;          // Префабы из Prefabs
    private static GameObject[] availablePrefabs; // Префабы из Available
    private GameObject[] buttons;                 // Кнопки в UI
    private bool buttonsCreated = false;          // Флаг — созданы ли кнопки

    private enum FilterMode { All, Available }
    private FilterMode currentMode = FilterMode.Available; // по умолчанию available

    void Awake()
    {
        // Загружаем префабы из Prefabs
        prefabs = Resources.LoadAll<GameObject>("Prefabs")
                           .OrderBy(p => p.name)
                           .ToArray();

        Debug.Log($"[ScrollViewManager] Загружено {prefabs.Length} префабов из Resources/Prefabs");

        // Загружаем префабы из Available
        availablePrefabs = Resources.LoadAll<GameObject>("Available")
                                   .OrderBy(p => p.name)
                                   .ToArray();

        Debug.Log($"[ScrollViewManager] Загружено {availablePrefabs.Length} префабов из Resources/Available");
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;

        yield return StartCoroutine(CreateButtonsGradually());

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        SetupFilterButtons();
        UpdateFilterUI();
    }

    IEnumerator CreateButtonsGradually()
    {
        if (buttonsCreated) yield break;

        // Выбираем источник префабов в зависимости от режима
        GameObject[] sourcePrefabs = GetCurrentPrefabs();

        buttons = new GameObject[sourcePrefabs.Length];

        for (int i = 0; i < sourcePrefabs.Length; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, contentParent);
            var prefabButton = btn.GetComponent<PrefabButton>();
            prefabButton.Setup(sourcePrefabs[i].name, sourcePrefabs[i], this);
            buttons[i] = btn;

            if (i % 5 == 0)
                yield return null;
        }

        buttonsCreated = true;
        filterInput.onValueChanged.AddListener(FilterButtons);
        FilterButtons(filterInput.text);
    }

    private void SetupFilterButtons()
    {
        if (allButton != null)
            allButton.onClick.AddListener(() =>
            {
                currentMode = FilterMode.All;
                UpdateFilterUI();
                RebuildButtons();
            });

        if (availableButton != null)
            availableButton.onClick.AddListener(() =>
            {
                currentMode = FilterMode.Available;
                UpdateFilterUI();
                RebuildButtons();
            });
    }

    private void UpdateFilterUI()
    {
        if (allButton != null && availableButton != null)
        {
            var allColors = allButton.GetComponent<Image>();
            var availColors = availableButton.GetComponent<Image>();

            if (allColors != null)
                allColors.color = (currentMode == FilterMode.All) ? activeColor : inactiveColor;if (availColors != null)
                availColors.color = (currentMode == FilterMode.Available) ? activeColor : inactiveColor;
        }
    }

    private GameObject[] GetCurrentPrefabs()
    {
        if (currentMode == FilterMode.Available)
        {
            return availablePrefabs;
        }
        else
        {
            // Объединяем Prefabs и Available, исключая дубликаты по имени
            return prefabs.Concat(availablePrefabs)
                          .GroupBy(p => p.name)
                          .Select(g => g.First())
                          .OrderBy(p => p.name)
                          .ToArray();
        }
    }

    private void RebuildButtons()
    {
        // Удаляем старые кнопки
        if (buttons != null)
        {
            foreach (var b in buttons)
                if (b != null) Destroy(b);
        }

        buttonsCreated = false;
        StartCoroutine(CreateButtonsGradually());
    }

    void FilterButtons(string input)
    {
        string lower = input.ToLower();

        for (int i = 0; i < buttons.Length; i++)
        {
            var prefabButton = buttons[i].GetComponent<PrefabButton>();
            string prefabName = prefabButton.prefab.name.ToLower();
            bool matchesFilter = prefabName.Contains(lower);
            buttons[i].SetActive(matchesFilter);
        }

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public void SpawnPrefab(GameObject prefab)
    {
        if (spawnCamera == null)
        {
            Debug.LogError("Spawn Camera не назначена в инспекторе!");
            return;
        }

        Vector3 origin = spawnCamera.transform.position;
        Vector3 direction = spawnCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, spawnDistance, obstacleLayers))
        {
            Vector3 spawnPos = hit.point - direction * 0.1f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = origin + direction * spawnDistance;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}