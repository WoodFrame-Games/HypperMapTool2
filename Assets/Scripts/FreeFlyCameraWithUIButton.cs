using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class FreeFlyCameraWithUIButton : MonoBehaviour
{
    [Header("UI Button для движения вперёд")]
    public Button forwardButton;

    [Header("FreeFlyCamera Settings")]
    [SerializeField] private bool _active = true;
    [SerializeField] private bool _enableRotation = true;
    [SerializeField] private float _mouseSense = 1.8f;
    [SerializeField] private bool _enableTranslation = true;
    [SerializeField] private float _translationSpeed = 55f;
    [SerializeField] private bool _enableMovement = true;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _boostedSpeed = 50f;
    [SerializeField] private KeyCode _boostSpeed = KeyCode.LeftShift;
    [SerializeField] private KeyCode _moveUp = KeyCode.E;
    [SerializeField] private KeyCode _moveDown = KeyCode.Q;
    [SerializeField] private bool _enableSpeedAcceleration = true;
    [SerializeField] private float _speedAccelerationFactor = 1.5f;
    [SerializeField] private KeyCode _initPositonButton = KeyCode.R;

    private bool simulateForward = false;
    private CursorLockMode _wantedMode;
    private float _currentIncrease = 1f;
    private float _currentIncreaseMem;
    private Vector3 _initPosition;
    private Vector3 _initRotation;

#if UNITY_ANDROID || UNITY_IOS
    private Vector2 lastTouchPos;
#endif

    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;

        if (forwardButton != null)
        {
            forwardButton.onClick.AddListener(() => { });
            forwardButton.gameObject.AddComponent<UIButtonHold>().Init(this);
        }
    }

    private void OnEnable()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (_active)
        {
            _wantedMode = CursorLockMode.Locked;
        }
#endif
    }

#if UNITY_STANDALONE || UNITY_EDITOR
    private void SetCursorState()
    {
        if (Input.GetMouseButtonUp(1) && _wantedMode == CursorLockMode.Locked)
        {
            Cursor.lockState = (_wantedMode = CursorLockMode.None);
        }
        if (Input.GetMouseButtonDown(1))
        {
            _wantedMode = CursorLockMode.Locked;
        }
        Cursor.lockState = _wantedMode;
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }
#endif

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;
        if (!_enableSpeedAcceleration || (_enableSpeedAcceleration && !moving))
        {
            _currentIncreaseMem = 0f;
            return;
        }
        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1f);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3f) * Time.deltaTime;
    }

    private void Update()
    {
        if (!_active) return;

#if UNITY_STANDALONE || UNITY_EDITOR
        SetCursorState();
        if (Cursor.visible) return;
#endif

        if (_enableTranslation)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed);
        }

        if (_enableMovement)
        {
            Vector3 vector = Vector3.zero;
            float speed = _movementSpeed;

            if (Input.GetKey(_boostSpeed))
                speed = _boostedSpeed;

#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetKey(KeyCode.W) || simulateForward) vector += transform.forward;
#else
            if (simulateForward) vector += transform.forward; // на мобилке только кнопка
#endif

            if (Input.GetKey(KeyCode.S)) vector -= transform.forward;
            if (Input.GetKey(KeyCode.A)) vector -= transform.right;
            if (Input.GetKey(KeyCode.D)) vector += transform.right;
            if (Input.GetKey(_moveUp)) vector += transform.up;
            if (Input.GetKey(_moveDown)) vector -= transform.up;CalculateCurrentIncrease(vector != Vector3.zero);
            transform.position += vector * speed * _currentIncrease;
        }

        if (_enableRotation)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            transform.rotation *= Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * _mouseSense, Vector3.right);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense, transform.eulerAngles.z);
#else
            // поворот пальцем по экрану
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved)
                {
                    Vector2 delta = t.deltaPosition;
                    transform.rotation *= Quaternion.AngleAxis(-delta.y * _mouseSense * 0.02f, Vector3.right);
                    transform.rotation = Quaternion.Euler(
                        transform.eulerAngles.x,
                        transform.eulerAngles.y + delta.x * _mouseSense * 0.02f,
                        transform.eulerAngles.z
                    );
                }
            }
#endif
        }

        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
    }

    public void SetForward(bool state)
    {
        simulateForward = state;
    }
}

public class UIButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private FreeFlyCameraWithUIButton cam;

    public void Init(FreeFlyCameraWithUIButton cameraScript)
    {
        cam = cameraScript;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cam?.SetForward(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cam?.SetForward(false);
    }
}