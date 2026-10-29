using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelController : MonoBehaviour
{
    [Header("Settings")]
    public Color borderColor = Color.red;
    public float pulseDuration = 1f;
    public float startAlpha = 1f;
    public float endAlpha = 0.3f;
    public Ease easeType = Ease.InOutSine;
    public bool loop = true;

    [Header("References")]
    public Image borderImage;

    private void Start()
    {
        // Проверяем, есть ли компонент Image
        if (borderImage == null)
        {
            borderImage = GetComponent<Image>();
            if (borderImage == null)
            {
                Debug.LogError("No Image component found!");
                return;
            }
        }

        // Настраиваем начальные параметры
        borderColor.a = startAlpha;
        borderImage.color = borderColor;

        // Создаем анимацию
        AnimateBorder();
    }

    private void AnimateBorder()
    {
        // Устанавливаем начальный альфа-канал
        borderColor.a = startAlpha;
        borderImage.color = borderColor;

        // Создаем твин для изменения прозрачности
        borderImage.DOFade(endAlpha, pulseDuration)
            .SetEase(easeType)
            .SetLoops(loop ? -1 : 0, LoopType.Yoyo) // -1 означает бесконечное повторение
            .OnUpdate(() => {
                // Обновляем цвет, сохраняя RGB, но изменяя альфа
                Color currentColor = borderImage.color;
                borderColor.a = currentColor.a;
                borderImage.color = borderColor;
            });
    }

    // Для возможности перезапуска анимации из других скриптов
    public void RestartAnimation()
    {
        DOTween.Kill(borderImage);
        AnimateBorder();
    }

    private void OnDestroy()
    {
        // Убиваем все твины при уничтожении объекта
        DOTween.KillAll();
    }
}