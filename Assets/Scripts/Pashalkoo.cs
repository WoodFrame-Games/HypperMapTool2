using UnityEngine;
using System;

public class Pashalkoo : MonoBehaviour
{
    [Header("UI панели")]
    [SerializeField] private GameObject panelEasterEgg; // Панель пасхалки
    [SerializeField] private GameObject panelMenu;      // Панель меню

    void Update()
    {
        DateTime now = DateTime.Now;

        bool isEasterTime = (now.Hour == 0 && now.Minute < 5); // 00:00 - 00:04 включительно

        if (isEasterTime)
        {
            // Включаем пасхалку
            if (panelEasterEgg != null && !panelEasterEgg.activeSelf)
                panelEasterEgg.SetActive(true);

            // Выключаем меню
            if (panelMenu != null && panelMenu.activeSelf)
                panelMenu.SetActive(false);
        }
        else
        {
            // Выключаем пасхалку
            if (panelEasterEgg != null && panelEasterEgg.activeSelf)
                panelEasterEgg.SetActive(false);

            // Включаем меню
            if (panelMenu != null && !panelMenu.activeSelf)
                panelMenu.SetActive(true);
        }
    }
}