using UnityEngine;
using UnityEngine.UI; // Для обычного UI Text
using System.Collections.Generic;
using TMPro;

public class LocalizationApplier : MonoBehaviour
{
    public enum Language
    {
        English = 0,
        Russian = 1
    }

    [System.Serializable]
    public class LocalizedText
    {
        public TMP_Text uiTextTMP;  // TextMeshPro
        public Text uiText;         // Обычный UI Text
        public string englishText;
        public string russianText;
    }

    public List<LocalizedText> localizedTexts = new List<LocalizedText>();

    private void Start()
    {
        int savedLang = PlayerPrefs.GetInt("Language", 0);
        ApplyLanguage((Language)savedLang);
    }

    public void ApplyLanguage(Language lang)
    {
        foreach (var item in localizedTexts)
        {
            string localizedString = (lang == Language.English)
                ? item.englishText
                : item.russianText;

            // {spath} -> папка сохранений (C:/Users/<User>/AppData/LocalLow/<Company>/<Product>/saves)
            localizedString = localizedString.Replace("{spath}", GetSavesPath());

            // TMP парсит "\U"/"\u" (напр. C:\Users) как юникод-escape,
            // причём "\\ " не спасает — экранирование работает только в TMP_InputField.
            // Поэтому выводим пути через forward slash.
            localizedString = localizedString.Replace('\\', '/');

            if (item.uiTextTMP != null)
                item.uiTextTMP.text = localizedString;

            if (item.uiText != null)
                item.uiText.text = localizedString;
        }
    }

    private static string GetSavesPath()
    {
        return (Application.persistentDataPath + "/saves").Replace('\\', '/');
    }
}