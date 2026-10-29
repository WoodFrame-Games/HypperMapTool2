using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class LocalizedText
{
    public TMP_Text uiText;   // Первый текстовый элемент (TMP_Text или Text)
    public Text uiText2;  // Второй текстовый элемент (опционально)
    public string englishText;
    public string russianText;
}

public class LocalizationManager : MonoBehaviour
{
    public Button languageButton;
    public TMP_Text languageButtonText;   // Первый текст кнопки
    public TMP_Text languageButtonText2;  // Второй текст кнопки (опционально)
    public List<LocalizedText> localizedTexts = new List<LocalizedText>();

    public enum Language
    {
        English = 0,
        Russian = 1
    }
    public Language CurrentLanguage => currentLanguage;
    private Language currentLanguage;

    private void Start()
    {
        currentLanguage = (Language)PlayerPrefs.GetInt("Language", 0);
        UpdateButtonTexts();
        ApplyLanguage(currentLanguage);

        languageButton.onClick.AddListener(OnLanguageButtonClicked);
    }

    public void OnLanguageButtonClicked()
    {
        // Переключаем язык по кругу (только 2 языка)
        currentLanguage = (Language)(((int)currentLanguage + 1) % 2);
        PlayerPrefs.SetInt("Language", (int)currentLanguage);
        PlayerPrefs.Save();
        
        UpdateButtonTexts();
        ApplyLanguage(currentLanguage);
    }

    private void UpdateButtonTexts()
    {
        string buttonText = "";
        
        switch (currentLanguage)
        {
            case Language.English: buttonText = "English"; break;
            case Language.Russian: buttonText = "Русский"; break;
        }

        if (languageButtonText != null)
            languageButtonText.text = buttonText;

        if (languageButtonText2 != null)
            languageButtonText2.text = buttonText;
    }

    private void ApplyLanguage(Language lang)
    {
        foreach (var item in localizedTexts)
        {
            string localizedString = GetLocalizedString(item, lang);

            if (item.uiText != null)
                item.uiText.text = localizedString;

            if (item.uiText2 != null)
                item.uiText2.text = localizedString;
        }
    }

    private string GetLocalizedString(LocalizedText item, Language lang)
    {
        switch (lang)
        {
            case Language.English: return item.englishText;
            case Language.Russian: return item.russianText;
            default: return item.englishText; // fallback
        }
    }
}