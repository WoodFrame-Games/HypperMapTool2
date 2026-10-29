using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraSettingsUI : MonoBehaviour
{
    [Header("Ссылки")]
    public Camera targetCamera;

    [Space(10)]
    public TMP_Dropdown renderingPathDropdown;
    public TMP_Dropdown hdrDropdown;
    public TMP_Dropdown msaaDropdown;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        SetupRenderingPathDropdown();
        SetupHDRDropdown();
        SetupMSAADropdown();
    }

    void SetupRenderingPathDropdown()
    {
        if (renderingPathDropdown == null) return;

        renderingPathDropdown.ClearOptions();
        renderingPathDropdown.AddOptions(new System.Collections.Generic.List<string>()
        {
            "Forward",
            "Deferred"
        });

        renderingPathDropdown.onValueChanged.AddListener(OnRenderingPathChanged);
    }

    void SetupHDRDropdown()
    {
        if (hdrDropdown == null) return;

        hdrDropdown.ClearOptions();
        hdrDropdown.AddOptions(new System.Collections.Generic.List<string>()
        {
            "Off",
            "On"
        });

        hdrDropdown.onValueChanged.AddListener(OnHDRChanged);
    }

    void SetupMSAADropdown()
    {
        if (msaaDropdown == null) return;

        msaaDropdown.ClearOptions();
        msaaDropdown.AddOptions(new System.Collections.Generic.List<string>()
        {
            "Off",
            "2x",
            "4x",
            "8x"
        });

        msaaDropdown.onValueChanged.AddListener(OnMSAAChanged);
    }

    void OnRenderingPathChanged(int index)
    {
        if (targetCamera == null) return;

        switch (index)
        {
            case 0:
                targetCamera.renderingPath = RenderingPath.Forward;
                break;
            case 1:
                targetCamera.renderingPath = RenderingPath.DeferredShading;
                break;
        }
    }

    void OnHDRChanged(int index)
    {
        if (targetCamera == null) return;

        targetCamera.allowHDR = (index == 1);
    }

    void OnMSAAChanged(int index)
    {
        switch (index)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = 8;
                break;
        }
    }
}