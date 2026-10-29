using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [Header("Панели, которые будут включаться при нажатии")]
    [SerializeField] private GameObject buildPanel;
    [SerializeField] private GameObject decodePanel;
    [SerializeField] private GameObject settPanel;
    [SerializeField] private UIDocument uidoc1;

    [Header("Локализация")]
    [SerializeField] private LocalizationManager localizationManager;

    private Button buildButton;
    private Button decodeButton;
    private Button exitButton;
    private Button settButton;
    private Label title;
    private Button languageButton;
    private VisualElement internetImage;
    private VisualElement discord;

    private UIDocument uiDocument;
    private bool initialized = false;

    void OnEnable()
    {
        uiDocument = uidoc1;
        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        yield return null; // ЖДЁМ 1 КАДР чтобы UI Toolkit точно развернул панель

        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("rootVisualElement == null");
            yield break;
        }

        // Находим элементы по имени (из UXML)
        buildButton = root.Q<Button>("build-button");
        title = root.Q<Label>("title");
        decodeButton = root.Q<Button>("decode-button");
        exitButton = root.Q<Button>("exit-button");
        languageButton = root.Q<Button>("language-button");
        internetImage = root.Q<VisualElement>("internet-image");
        discord = root.Q<VisualElement>("discord");
        settButton = root.Q<Button>("settings-button");

        // Снимаем старые события (если UI включается повторно)
        buildButton.clicked -= OnBuildClicked;
        decodeButton.clicked -= OnDecodeClicked;
        settButton.clicked -= OnSettClicked;
        exitButton.clicked -= OnExitClicked;

        // Подключаем события кнопок
        buildButton.clicked += OnBuildClicked;
        decodeButton.clicked += OnDecodeClicked;
        settButton.clicked += OnSettClicked;
        exitButton.clicked += OnExitClicked;

        // Языковая кнопка
        if (localizationManager != null)
        {
            UpdateMenuTexts();
            languageButton.clicked -= OnLanguageClicked;
            languageButton.clicked += OnLanguageClicked;
        }

        // Клики по картинкам (без накопления подписок)
        internetImage.UnregisterCallback<ClickEvent>(OnWebsiteClick);
        discord.UnregisterCallback<ClickEvent>(OnDiscordClick);
        internetImage.RegisterCallback<ClickEvent>(OnWebsiteClick);
        discord.RegisterCallback<ClickEvent>(OnDiscordClick);

        initialized = true;
    }

    private void OnLanguageClicked()
    {
        localizationManager.OnLanguageButtonClicked();
        UpdateMenuTexts();
    }

    private void UpdateMenuTexts()
    {
        if (!initialized) return;

        var lang = localizationManager.CurrentLanguage;

        languageButton.text = lang == LocalizationManager.Language.Russian ? "Русский" : "English";
        buildButton.text = lang == LocalizationManager.Language.Russian ? "Строить" : "Build";
        decodeButton.text = lang == LocalizationManager.Language.Russian ? "Расшифровать" : "Decode Save";
        settButton.text = lang == LocalizationManager.Language.Russian ? "Настройки" : "Settings";
        exitButton.text = lang == LocalizationManager.Language.Russian ? "Выход" : "Exit";
        title.text = lang == LocalizationManager.Language.Russian ? "Главное Меню" : "Main Menu";
    }

    private void OnSettClicked() => SwitchToPanel(settPanel);
    private void OnBuildClicked() => SwitchToPanel(buildPanel);
    private void OnDecodeClicked() => SwitchToPanel(decodePanel);
    private void OnExitClicked() => Application.Quit();

    private void OnWebsiteClick(ClickEvent evt) => OpenWebsite();
    private void OnDiscordClick(ClickEvent evt) => OpenDiscord();
    private void OpenWebsite() => Application.OpenURL("https://sites.google.com/view/tubmaptool/home");
    private void OpenDiscord() => Application.OpenURL("https://discord.gg/j2aXJcZQXp");

    private void SwitchToPanel(GameObject panel)
    {
        var root = uiDocument.rootVisualElement;

        root.style.display = DisplayStyle.None;
        root.pickingMode = PickingMode.Ignore;

        if (panel != null)
            panel.SetActive(true);
    }
}