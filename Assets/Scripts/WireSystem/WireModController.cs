using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class WireModController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float rayDistance = 12f;

    [Header("UI")]
    [SerializeField] private GameObject hoverPanel;
    [SerializeField] private TMP_Text hoverText;
    [SerializeField] private TMP_Text wireModeText;

    [Header("Wire Preview")]
    [SerializeField] private WireLine previewWirePrefab;

    private bool wireModeEnabled;
    private int wiresLayer;

    private SavableProp startProp;
    private WireLine previewWire;
    private SavableProp[] allProps;

    private void Start()
    {
        wiresLayer = LayerMask.NameToLayer("Wires");

        if (targetCamera == null)
            targetCamera = Camera.main;

        SetWireLayerVisible(false);

        if (hoverPanel != null)
            hoverPanel.SetActive(false);

        if (wireModeText != null)
            wireModeText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleWireMode();
        }

        if (!wireModeEnabled)
            return;

        HandleRaycast();

        if (Input.GetMouseButtonDown(0))
            TryClick();

        if (Input.GetMouseButtonDown(1))
            CancelWire();
    }

    // ===============================
    // MODE
    // ===============================

    private void ToggleWireMode()
    {
        wireModeEnabled = !wireModeEnabled;
        SetWireLayerVisible(wireModeEnabled);

        if (wireModeText != null)
            wireModeText.gameObject.SetActive(wireModeEnabled);

        if (wireModeEnabled)
        {
            RefreshPropCache();
        }
        else
        {
            CancelWire();
            if (hoverPanel != null)
                hoverPanel.SetActive(false);
        }
    }

    private void SetWireLayerVisible(bool visible)
    {
        if (targetCamera == null || wiresLayer < 0)
            return;

        if (visible)
            targetCamera.cullingMask |= (1 << wiresLayer);
        else
            targetCamera.cullingMask &= ~(1 << wiresLayer);
    }

    private void RefreshPropCache()
    {
        allProps = FindObjectsOfType<SavableProp>();
    }

    // ===============================
    // RAYCAST
    // ===============================

    private SavableProp RaycastProp(out Vector3 fallbackPoint)
    {
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        fallbackPoint = ray.origin + ray.direction * rayDistance;

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            return null;

        fallbackPoint = hit.point;
        return hit.collider.GetComponentInParent<SavableProp>();
    }

    private void HandleRaycast()
    {
        SavableProp sp = RaycastProp(out Vector3 fallbackPoint);

        if (sp != null)
        {
            if (hoverPanel != null)
                hoverPanel.SetActive(true);

            if (hoverText != null)
                hoverText.text = BuildInfo(sp);
        }
        else if (hoverPanel != null)
        {
            hoverPanel.SetActive(false);
        }

        if (previewWire != null)
        {
            Vector3 endPos = sp != null ? sp.transform.position : fallbackPoint;
            previewWire.SetEnd(endPos);
        }
    }

    // ===============================
    // INPUT
    // ===============================

    private void TryClick()
    {
        SavableProp sp = RaycastProp(out _);

        if (sp == null)
        {
            CancelWire();
            return;
        }

        if (startProp == null)
        {
            // Провод начинается только от источника (canConnect, как в оригинале)
            if (!sp.canConnect)
            {
                Debug.LogWarning($"[Wire] {sp.prefabName} не может быть источником (canConnect = No). Начни с кнопки/вентиля/таймера/контроллера/триггера/панели.");
                return;
            }

            StartWire(sp);
        }
        else
        {
            FinishWire(sp);
        }
    }

    // ===============================
    // WIRE PROCESS
    // ===============================

    private void StartWire(SavableProp sp)
    {
        startProp = sp;

        if (previewWirePrefab != null)
        {
            previewWire = Instantiate(previewWirePrefab);
            previewWire.Init(startProp.transform, startProp.transform);

            if (wiresLayer >= 0)
                previewWire.gameObject.layer = wiresLayer;
        }

        Debug.Log($"[Wire] Старт: {sp.prefabName} (id {sp.uniqueId}). Выбери цель.");
    }

    private void FinishWire(SavableProp target)
    {
        if (target == startProp)
        {
            CancelWire();
            return;
        }

        // === Правила подключения оригинала (ToolManager.ConnectWire) ===

        // 1. Отключение: клик по уже подключённой цели
        if (startProp.HasConnection(target.uniqueId))
        {
            startProp.RemoveConnection(target.uniqueId);
            startProp.RefreshWires();
            RefreshPropCache();
            Debug.Log($"[Wire] Отключено: {startProp.prefabName} → {target.prefabName} (id {target.uniqueId})");
            CancelWire();
            return;
        }

        // 2. Источник обязан уметь подключать (canConnect)
        if (!startProp.canConnect)
        {
            Debug.LogWarning($"[Wire] {startProp.prefabName} не может быть источником (canConnect = No)");
            CancelWire();
            return;
        }

        // 3. Цель обязана принимать подключения (canAttach)
        if (!target.canAttach)
        {
            Debug.LogWarning($"[Wire] {target.prefabName} не принимает подключения (canAttach = No)");
            CancelWire();
            return;
        }

        // 4. Не подключаться дважды к одной цели
        if (startProp.HasConnection(target.uniqueId))
        {
            CancelWire();
            return;
        }

        // 5. Цель без multiAttach принимает только одно подключение
        if (!target.multiAttach && target.GetConnectionIds().Count > 0)
        {
            Debug.LogWarning($"[Wire] {target.prefabName} уже подключён (multiAttach = No)");
            CancelWire();
            return;
        }

        // 6. Не допускать встречное подключение (кнопка→вентиль уже есть — вентиль→кнопка запрещено)
        if (target.HasConnection(startProp.uniqueId))
        {
            Debug.LogWarning($"[Wire] Обратное подключение уже существует: {target.prefabName} → {startProp.prefabName}");
            CancelWire();
            return;
        }

        startProp.AddConnection(target.uniqueId);
        startProp.RefreshWires();
        RefreshPropCache();

        Debug.Log($"[Wire] Подключено: {startProp.prefabName} → {target.prefabName} (id {target.uniqueId})");

        CancelWire();
    }

    private void CancelWire()
    {
        if (previewWire != null)
            Destroy(previewWire.gameObject);

        previewWire = null;
        startProp = null;
    }

    // ===============================
    // INFO PANEL
    // ===============================

    private string BuildInfo(SavableProp sp)
    {
        StringBuilder sb = new StringBuilder();

        string displayName = string.IsNullOrEmpty(sp.prefabName) ? sp.name : sp.prefabName;
        sb.AppendLine("Name: " + displayName);
        sb.AppendLine("Type: " + sp.type);

        // Простой проп: только айди, название и тип (айди теперь порядковый)
        if (!sp.HasWireSocket())
        {
            sb.AppendLine("ID: " + ((int)sp.uniqueId).ToString(CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        // Логические компоненты: только базовое для подключения
        sb.AppendLine("canConnect: " + Yes(sp.canConnect));
        sb.AppendLine("attach: " + Yes(sp.canAttach));
        sb.AppendLine("multi attach: " + Yes(sp.multiAttach));

        List<string> ids = sp.GetConnectionIds();
        sb.AppendLine("Connected (" + ids.Count.ToString(CultureInfo.InvariantCulture) + "):");
        foreach (string idStr in ids)
            sb.AppendLine("  → " + ResolvePropName(idStr));

        if (startProp != null)
        {
            if (startProp == sp)
                sb.AppendLine("Start. Click empty space to cancel");
            else if (startProp.HasConnection(sp.uniqueId))
                sb.AppendLine("Click to disconnect");
            else if (!sp.canAttach)
                sb.AppendLine("Can't attach (canAttach = No)");
            else if (!sp.multiAttach && sp.GetConnectionIds().Count > 0)
                sb.AppendLine("Already attached (multiAttach = No)");
            else if (sp.HasConnection(startProp.uniqueId))
                sb.AppendLine("Reverse connection exists");
            else
                sb.AppendLine("Click to connect");
        }
        else if (sp.canConnect)
        {
            sb.AppendLine("Click to start connection");
        }

        return sb.ToString();
    }

    private static string Yes(bool value) => value ? "Yes" : "No";

    private string ResolvePropName(string idStr)
    {
        if (!double.TryParse(idStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double id))
            return idStr;

        if (allProps == null)
            RefreshPropCache();

        foreach (var sp in allProps)
        {
            if (sp != null && System.Math.Abs(sp.uniqueId - id) < 0.0001)
                return sp.prefabName + " [" + idStr + "]";
        }

        return "??? [" + idStr + "]";
    }
}
