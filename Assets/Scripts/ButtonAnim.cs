using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки")]
    public AudioClip hoverSound; // Звук при наведении
    public float sizeReduction = 0.1f; // На сколько уменьшать текст (0.1 = 10%)
    
    [Header("Ссылки (опционально)")]
    public Transform textToScale; // Вручную перетащите сюда Text или TMP_Text
    
    private AudioSource audioSource;
    private Vector3 originalTextScale;

    void Start()
    {
        // Настройка AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Автопоиск текста, если не задан вручную
        if (textToScale == null)
        {
            textToScale = transform.Find("Text"); // Для стандартного UI Text
            
            // Для TextMeshPro (раскомментируйте, если используете TMP)
            // if(textToScale == null) textToScale = GetComponentInChildren<TMPro.TextMeshProUGUI>()?.transform;
        }

        if (textToScale != null)
        {
            originalTextScale = textToScale.localScale;
        }
        else
        {
            Debug.LogError("Не найден объект с текстом для масштабирования!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textToScale == null) return;
        
        // Воспроизводим звук
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
        
        // Уменьшаем текст
        textToScale.localScale = originalTextScale * (1f - sizeReduction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textToScale == null) return;
        
        // Возвращаем исходный размер (без звука)
        textToScale.localScale = originalTextScale;
    }

    void OnDisable()
    {
        // Сброс при отключении объекта
        if (textToScale != null)
        {
            textToScale.localScale = originalTextScale;
        }
    }
}