using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GlowOutline : MonoBehaviour
{
    [Header("Glow")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowSize = 10f;
    [SerializeField, Range(0f, 1f)] private float glowAlpha = 0.8f;

    private GameObject _glowObject;
    private Image _glowImage;
    private bool _dirty;

    private void OnEnable()
    {
        _dirty = true;
    }

    private void OnDisable()
    {
        DestroyGlow();
    }

    private void OnValidate()
    {
        _dirty = true;
    }

    private void Update()
    {
        if (_dirty && isActiveAndEnabled)
        {
            _dirty = false;
            CreateGlow();
        }
    }

    private void CreateGlow()
    {
        DestroyGlow();

        _glowObject = new GameObject("Glow");
        _glowObject.transform.SetParent(transform, false);
        _glowObject.transform.SetAsFirstSibling();

        _glowImage = _glowObject.AddComponent<Image>();

        var parentImage = GetComponent<Image>();
        if (parentImage != null)
            _glowImage.sprite = parentImage.sprite;

        _glowImage.material = new Material(Shader.Find("UI/Glow"));
        _glowImage.raycastTarget = false;

        var rt = _glowObject.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(-glowSize, -glowSize);
        rt.offsetMax = new Vector2(glowSize, glowSize);

        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (_glowImage == null || _glowImage.material == null) return;

        var mat = _glowImage.material;
        mat.SetColor("_GlowColor", glowColor);
        mat.SetFloat("_AlphaStart", glowAlpha);
        mat.SetFloat("_AlphaEnd", 0f);
        mat.SetFloat("_TransitionMid", 0.5f);

        var parentRT = GetComponent<RectTransform>();
        if (parentRT != null)
        {
            float minDim = Mathf.Min(parentRT.rect.width, parentRT.rect.height);
            float uvRange = (glowSize * 2f) / Mathf.Max(minDim, 1f);
            mat.SetFloat("_GlowRangeUV", uvRange);
        }

        Color c = _glowImage.color;
        c.a = 1f;
        _glowImage.color = c;
    }

    private void DestroyGlow()
    {
        if (_glowObject != null)
            Destroy(_glowObject);
        _glowObject = null;
        _glowImage = null;
    }

    private void OnDestroy()
    {
        DestroyGlow();
    }
}