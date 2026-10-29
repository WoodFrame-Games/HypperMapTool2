using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PrefabSpawnerUI : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string prefabName;
        public Button button;
    }

    public List<SpawnEntry> spawnEntries = new List<SpawnEntry>();
    public float spawnDistance = 1f;
    public LayerMask obstacleLayers = ~0;
    public InputField spawnInputField;
    public Button spawnButton;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Настраиваем кнопки из инспектора
        foreach (var entry in spawnEntries)
        {
            if (entry.button != null)
            {
                string prefabToSpawn = entry.prefabName;
                entry.button.onClick.AddListener(() => SpawnPrefab(prefabToSpawn));
            }
        }

        // Настраиваем кнопку спавна по вводу
        if (spawnButton != null && spawnInputField != null)
        {
            spawnButton.onClick.AddListener(() => {
                if (!string.IsNullOrEmpty(spawnInputField.text))
                {
                    SpawnPrefab(spawnInputField.text);
                }
            });
        }
    }

    void SpawnPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab == null)
        {
            Debug.LogError("Префаб '" + prefabName + "' не найден в Resources/Prefabs/");
            return;
        }

        Vector3 spawnDirection = cam.transform.forward;
        Vector3 origin = cam.transform.position;

        if (Physics.Raycast(origin, spawnDirection, out RaycastHit hit, spawnDistance, obstacleLayers))
        {
            Vector3 spawnPos = hit.point - spawnDirection * 0.1f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = origin + spawnDirection * spawnDistance;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}