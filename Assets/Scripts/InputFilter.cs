using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFilterWithPassword : MonoBehaviour
{
    [Header("Ссылки")]
    public TMP_InputField inputField;

    [Header("Настройки")]
    public string secretPassword = "opm-unlock";
    public int defaultCharacterLimit = 19;
    public int unlockedCharacterLimit = 500;

    private bool isUnlocked = false;

    private void Start()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

        inputField.characterLimit = defaultCharacterLimit;
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    private void OnTextChanged(string text)
    {
        if (!isUnlocked)
        {
            if (text.Contains("<") || text.Contains(">"))
            {
                inputField.text = "-1000 social credit";
                return;
            }

            if (text == secretPassword)
            {
                isUnlocked = true;
                inputField.characterLimit = unlockedCharacterLimit;
                Debug.Log("✅ Input unlocked!");
            }
        }
    }
}