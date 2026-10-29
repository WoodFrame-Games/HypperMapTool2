using UnityEngine;
using UnityEngine.UI;

public class LightAndSkyboxToggle : MonoBehaviour
{
    [Header("UI Toggle")]
    public Toggle toggle; // Ссылка на UI Toggle

    [Header("Lighting Settings")]
    public Light directionalLight; // Ссылка на Directional Light
    public Color lightOffColor = Color.black; // Цвет при включённом toggle
    public Color lightOnColor = Color.white; // Цвет при выключенном toggle

    [Header("Skyboxes")]
    public Material defaultSkybox; // Исходный скайбокс
    public Material newSkybox;     // Новый скайбокс при включённом toggle

    private void Start()
    {
        // Проверяем, чтобы все ссылки были установлены
        if (toggle == null)
        {
            Debug.LogWarning("Toggle не назначен! Скрипт не будет работать корректно.");
            return;
        }

        if (directionalLight == null)
        {
            Debug.LogWarning("Directional Light не назначен!");
        }

        // Подписываемся на событие изменения toggle
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // Устанавливаем исходное состояние при старте
        UpdateState(toggle.isOn);
    }

    private void OnToggleChanged(bool isOn)
    {
        UpdateState(isOn);
    }

    private void UpdateState(bool isOn)
    {
        // Меняем цвет света
        if (directionalLight != null)
        {
            directionalLight.color = isOn ? lightOffColor : lightOnColor;
        }

        // Меняем скайбокс
        if (isOn && newSkybox != null)
            RenderSettings.skybox = newSkybox;
        else if (!isOn && defaultSkybox != null)
            RenderSettings.skybox = defaultSkybox;

        // Обновляем освещение (чтобы изменения сразу применились)
        DynamicGI.UpdateEnvironment();
    }
}