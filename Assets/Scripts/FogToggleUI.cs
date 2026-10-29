using UnityEngine;
using UnityEngine.UI;

public class FogToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle fogToggle; // сюда перетаскиваешь Toggle из Canvas

    private void Start()
    {
        if (fogToggle != null)
        {
            // Подписываемся на событие смены состояния
            fogToggle.onValueChanged.AddListener(OnFogToggleChanged);

            // Синхронизируем начальное значение
            RenderSettings.fog = fogToggle.isOn;
        }
    }

    private void OnFogToggleChanged(bool isOn)
    {
        RenderSettings.fog = isOn;
    }
}