using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GlowEffect : Graphic
{
    [Header("Glow")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowRange = 20f;
    [SerializeField] private float glowIntensity = 1f;

    [Header("Transparency")]
    [SerializeField, Range(0f, 1f)] private float alphaStart = 1f;
    [SerializeField, Range(0f, 1f)] private float alphaEnd = 0f;
    [SerializeField, Range(0.01f, 1f)] private float transitionMid = 0.5f;

    private Material _material;
    private float _lastRange = -1f;

    private static readonly int GlowColorId = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowRangeUVId = Shader.PropertyToID("_GlowRangeUV");
    private static readonly int GlowIntensityId = Shader.PropertyToID("_GlowIntensity");
    private static readonly int AlphaStartId = Shader.PropertyToID("_AlphaStart");
    private static readonly int AlphaEndId = Shader.PropertyToID("_AlphaEnd");
    private static readonly int TransitionMidId = Shader.PropertyToID("_TransitionMid");

    public override Texture mainTexture => material == null ? null : material.mainTexture;

    public Color GlowColor
    {
        get => glowColor;
        set { glowColor = value; Apply(); SetVerticesDirty(); }
    }

    public float GlowRange
    {
        get => glowRange;
        set { glowRange = Mathf.Max(0f, value); Apply(); SetVerticesDirty(); }
    }

    public float GlowIntensity
    {
        get => glowIntensity;
        set { glowIntensity = Mathf.Max(0f, value); Apply(); }
    }

    public float AlphaStart
    {
        get => alphaStart;
        set { alphaStart = Mathf.Clamp01(value); Apply(); }
    }

    public float AlphaEnd
    {
        get => alphaEnd;
        set { alphaEnd = Mathf.Clamp01(value); Apply(); }
    }

    public float TransitionMid
    {
        get => transitionMid;
        set { transitionMid = Mathf.Clamp(value, 0.01f, 1f); Apply(); }
    }

    protected override void Awake()
    {
        SetupMaterial();
        base.Awake();
    }

    private void OnEnable()
    {
        SetupMaterial();
        Apply();
        SetVerticesDirty();
    }

    private void OnDisable()
    {
        SetVerticesDirty();
    }

    private void OnValidate()
    {
        if (isActiveAndEnabled)
        {
            SetupMaterial();
            Apply();
            SetVerticesDirty();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying && _material == null)
            SetupMaterial();

        if (Mathf.Abs(_lastRange - glowRange) > 0.01f)
        {
            _lastRange = glowRange;
            SetVerticesDirty();
        }
    }

    private void SetupMaterial()
    {
        if (_material == null)
        {
            var shader = Shader.Find("UI/Glow");
            if (shader == null)
            {
                Debug.LogError("[GlowEffect] Shader 'UI/Glow' not found.");
                return;
            }
            _material = new Material(shader);
        }
        material = _material;
    }

    private void Apply()
    {
        if (_material == null) return;
        _material.SetColor(GlowColorId, glowColor);

        float uvRange = 0.1f;
        if (rectTransform != null)
        {
            Rect r = rectTransform.rect;
            if (r.width > 0 && r.height > 0)
                uvRange = glowRange / Mathf.Min(r.width, r.height);
        }
        _material.SetFloat(GlowRangeUVId, uvRange);

        _material.SetFloat(GlowIntensityId, glowIntensity);
        _material.SetFloat(AlphaStartId, alphaStart);
        _material.SetFloat(AlphaEndId, alphaEnd);
        _material.SetFloat(TransitionMidId, transitionMid);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = rectTransform.rect;
        float padX = glowRange;
        float padY = glowRange;

        float left = rect.x - padX;
        float right = rect.xMax + padX;
        float bottom = rect.y - padY;
        float top = rect.yMax + padY;

        float uvLeft = -padX / rect.width;
        float uvRight = 1f + padX / rect.width;
        float uvBottom = -padY / rect.height;
        float uvTop = 1f + padY / rect.height;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = new Vector2(left, bottom);
        vert.uv0 = new Vector2(uvLeft, uvBottom);
        vh.AddVert(vert);

        vert.position = new Vector2(left, top);
        vert.uv0 = new Vector2(uvLeft, uvTop);
        vh.AddVert(vert);

        vert.position = new Vector2(right, top);
        vert.uv0 = new Vector2(uvRight, uvTop);
        vh.AddVert(vert);

        vert.position = new Vector2(right, bottom);
        vert.uv0 = new Vector2(uvRight, uvBottom);
        vh.AddVert(vert);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    protected override void OnDestroy()
    {
        if (_material != null)
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }
    }
}