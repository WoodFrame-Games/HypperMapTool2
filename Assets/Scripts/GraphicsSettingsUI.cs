using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GraphicsSettingsDropdown : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown graphicsDropdown;

    private const string GraphicsPrefKey = "GraphicsLevel";

    private readonly string[] optionNames = 
    {
        "Low",
        "Medium",
        "High",
        "Ultra",
        "Ultra+"
    };

    private void Start()
    {
        if (graphicsDropdown == null)
        {
            Debug.LogError("Graphics dropdown not assigned!");
            return;
        }

        SetupDropdownOptions();

        // Ставим обработчик выбора
        graphicsDropdown.onValueChanged.AddListener(OnDropdownChanged);

        // Загружаем настройки
        LoadGraphicsSettings();
    }

    // --------------------------
    // Создание пунктов меню
    // --------------------------
    private void SetupDropdownOptions()
    {
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string>(optionNames));
    }

    // --------------------------
    // Реакция на выбор в меню
    // --------------------------
    private void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 0: SetGraphicsLevel(0, true); break;  // Low
            case 1: SetGraphicsLevel(2, true); break;  // Medium
            case 2: SetGraphicsLevel(4, true); break;  // High
            case 3: SetGraphicsLevel(5, true); break;  // Ultra
            case 4: SetUltraPlus(true); break;         // Ultra+
        }
    }

    // --------------------------
    // Применение пресетов Unity
    // --------------------------
    private void SetGraphicsLevel(int qualityIndex, bool save)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        Debug.Log($"Graphics level set to: {QualitySettings.names[qualityIndex]}");

        if (save)
        {
            PlayerPrefs.SetInt(GraphicsPrefKey, qualityIndex);
            PlayerPrefs.Save();
        }
    }

    // --------------------------
    // Ultra+
    // --------------------------
    private void SetUltraPlus(bool save)
    {
        QualitySettings.globalTextureMipmapLimit = 0;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        QualitySettings.antiAliasing = 8;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        QualitySettings.vSyncCount = 1;
        QualitySettings.pixelLightCount = 8;
        QualitySettings.realtimeReflectionProbes = true;
        QualitySettings.billboardsFaceCameraPosition = true;

        Debug.Log("Ultra+ graphics applied");

        if (save)
        {
            PlayerPrefs.SetInt(GraphicsPrefKey, -1);
            PlayerPrefs.Save();
        }
    }

    // --------------------------
    // Загрузка сохранённых настроек
    // --------------------------
    private void LoadGraphicsSettings()
    {
        if (!PlayerPrefs.HasKey(GraphicsPrefKey))
        {
            Debug.Log("No saved graphics settings, using default.");
            return;
        }

        int savedLevel = PlayerPrefs.GetInt(GraphicsPrefKey);

        if (savedLevel == -1)
        {
            SetUltraPlus(false);
            graphicsDropdown.value = 4; // Ultra+
        }
        else
        {
            SetGraphicsLevel(savedLevel, false);

            // Определяем индекс dropdown по quality index
            int dropdownIndex = savedLevel switch
            {
                0 => 0, // Low
                2 => 1, // Medium
                4 => 2, // High
                5 => 3, // Ultra
                _ => 0
            };

            graphicsDropdown.value = dropdownIndex;
        }

        graphicsDropdown.RefreshShownValue();
    }
}