using UnityEngine;
using UnityEngine.UI;
using System;

public class HolidayImageSwitcher : MonoBehaviour
{
    [Header("Target UI Image")]
    public Image targetImage;

    [Header("Sprite shown on holidays (Dec 30 – Jan 2)")]
    public Sprite holidaySprite;

    private void Update()
    {
        UpdateImageBasedOnDate();
    }



    private void UpdateImageBasedOnDate()
    {
        if (targetImage == null)
        {
            Debug.LogWarning("HolidayImageSwitcher: Target Image is not assigned!");
            return;
        }

        DateTime now = DateTime.Now;

        bool isHoliday = 
            (now.Month == 12 && now.Day >= 30) ||   // Dec 30, 31
            (now.Month == 1 && now.Day <= 2);       // Jan 1, 2

        if (isHoliday)
        {
            // Holiday appearance
            targetImage.sprite = holidaySprite;
            targetImage.color = Color.white;
        }
        else
        {
            // Default non-holiday state
            targetImage.sprite = null;

            // #1E1E1E
            Color defaultColor;
            ColorUtility.TryParseHtmlString("#1E1E1E", out defaultColor);
            targetImage.color = defaultColor;
        }
    }
}