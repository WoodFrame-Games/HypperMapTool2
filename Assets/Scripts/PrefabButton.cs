using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PrefabButton : MonoBehaviour
{
    public TMP_Text label;                 // Текст кнопки
    public GameObject prefab;         // Префаб для спавна
    private ScrollViewManager manager; // Ссылка на менеджер

    public void Setup(string prefabName, GameObject prefabToSpawn, ScrollViewManager scrollManager)
    {
        label.text = prefabName;
        prefab = prefabToSpawn;
        manager = scrollManager;

        GetComponent<Button>().onClick.AddListener(() => manager.SpawnPrefab(prefab));
    }
}