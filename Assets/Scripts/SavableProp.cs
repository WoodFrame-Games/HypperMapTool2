using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Globalization;
using System.Collections.Generic;

[Serializable]
public class SerializableLevel
{
    public string map;
    public SerializableProp[] props;
}

[Serializable]
public class SerializableProp
{
    public string name;
    public float uniqueId; // float: печать краткой формой, как у оригинала (35.8939362)
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public bool isKinematic = true;

    public InstantiationData instantiationData = null;
    public RuntimeData runtimeData = null;
}

public enum PropType
{
    Prop,
    Monitor,
    MonitorController,
    Button,
    LogicGate,
    Timer,
    Spawner,
    Remover,
    Repulsor,
    Nextbot,
    Mover,
    LocalRotator,
    LocalMover,
    Trigger,
    PasswordPanel,
    Lamp
}

[Serializable]
public class RuntimeData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Text = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Color = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float?[] connectedIds = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? Signal = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? PropUniqueId = null;

    // Устаревшее поле (тул больше не пишет): оставлено только для чтения
    // старых сейвов, где объект-движения хранился под нашим ключом.
    [JsonProperty("Move Id", NullValueHandling = NullValueHandling.Ignore)]
    public float? MoveId = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? PointAB = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? MoveProcess = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? RotateProcess = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? TempDegree = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public SerializableVector3? FinalVector = null;
}

[Serializable]
public class InstantiationData
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Text = null;

    [JsonProperty("Background color", NullValueHandling = NullValueHandling.Ignore)]
    public string BackgroundColor = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? Toggle = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Mode = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Time = null;

    [JsonProperty("Prop name", NullValueHandling = NullValueHandling.Ignore)]
    public string PropName = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? Kinematic = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Distance = null;

    [JsonProperty("Delete after", NullValueHandling = NullValueHandling.Ignore)]
    public float? DeleteAfter = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Force = null;

    [JsonProperty("Image URL", NullValueHandling = NullValueHandling.Ignore)]
    public string ImageURL = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? Animated = null;

    [JsonProperty("Image size", NullValueHandling = NullValueHandling.Ignore)]
    public string ImageSize = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Speed = null;

    [JsonProperty("Position A", NullValueHandling = NullValueHandling.Ignore)]
    public float? PositionA = null;

    [JsonProperty("Position B", NullValueHandling = NullValueHandling.Ignore)]
    public float? PositionB = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Direction = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Degree = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Range = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? Size = null;

    [JsonProperty("Object name", NullValueHandling = NullValueHandling.Ignore)]
    public string ObjectName = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Password = null;
}

[Serializable]
public struct SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public struct SerializableQuaternion
{
    public float x, y, z, w;
    public SerializableQuaternion(Quaternion q) { x = q.x; y = q.y; z = q.z; w = q.w; }
    public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
}

public class SavableProp : MonoBehaviour
{
    [Header("Basic")]
    public string prefabName;
    public double uniqueId;
    public PropType type;
    public Rigidbody rb;

    // Файловый saveId (сумма квадратов из сейва) — база резолва связей при загрузке
    [HideInInspector]
    public float fileSaveId;

    // true, пока проп ждёт резолва связей из только что загруженного файла
    private bool pendingFileResolve;

    [Header("Monitor Data")]
    public string monitorText;
    public string monitorColor;
    public string[] monitorConnected;
    public Text monitorTextUI;
    public Image monitorImageUI;

    [Header("Monitor Controller Data")]
    public string controllerText;
    public string controllerBackground;
    public string[] controllerConnected;

    [Header("Button Data")]
    public bool buttonToggle;
    public bool buttonSignal;
    public string[] buttonConnected;

    [Header("Logic Gate Data")]
    public string logicGateMode;
    public string[] logicGateConnected;
    public TMP_Text logicGateModeText;

    [Header("Timer Data")]
    public float timerTime;
    public string[] timerConnected;

    [Header("Spawner Data")]
    public string spawnerPropName;
    public bool spawnerKinematic;
    public float spawnerDistance;
    public float spawnerDeleteAfter;
    public string[] spawnerConnected;

    [Header("Remover Data")]
    public float removerDistance;
    public string[] removerConnected;

    [Header("Repulsor Data")]
    public float repulsorDistance;
    public float repulsorForce;
    public string[] repulsorConnected;

    [Header("Nextbot Data")]
    public string nextbotImageURL;
    public bool nextbotAnimated;
    public string nextbotImageSize;
    public string[] nextbotConnected;

    [Header("Mover Data")]
    public float moverSpeed;
    public float moverPositionA;
    public float moverPositionB;
    public string moverDirection;
    public float moverPropUniqueId;
    public bool moverPointAB;
    public bool moverMoveProcess;
    public string[] moverConnected;

    [Header("Local Rotator Data")]
    public float localRotatorSpeed;
    public float localRotatorDegree;
    public string localRotatorDirection;
    public float localRotatorPropUniqueId;
    public bool localRotatorRotateProcess;
    public float localRotatorTempDegree;
    public string[] localRotatorConnected;

    [Header("Local Mover Data")]
    public float localMoverSpeed;
    public float localMoverRange;
    public string localMoverDirection;
    public float localMoverPropUniqueId;
    public bool localMoverMoveProcess;
    public Vector3 localMoverFinalVector;
    public string[] localMoverConnected;

    [Header("Trigger Data")]
    public float triggerSize;
    public string triggerMode;
    public string triggerObjectName;
    public string[] triggerConnected;

    [Header("Password Panel Data")]
    public string passwordPanelPassword;
    public string[] passwordPanelConnected;
    public Text passwordText;

    [Header("Wire Rendering")]
    [SerializeField] private WireLine wirePrefab;
    [SerializeField] private Transform wireParent;

    private Dictionary<double, WireLine> activeWires = new();


    // ===============================
    // RUNTIME ID SYSTEM (как в оригинале: юзер видит порядковые номера,
    // сумма квадратов позиции вычисляется только при сохранении)
    // ===============================

    private static int runtimeIdCounter = 0;

    public static void ResetRuntimeIds() => runtimeIdCounter = 0;

    public double GetSaveId()
    {
        // Точная эмуляция float-конвейера, как считает игра:
        // округление до float после КАЖДОЙ операции. Mono может держать
        // промежуточные результаты с расширенной точностью — тогда
        // sqrMagnitude расходится с игрой на 1-7 ulp и связи теряются.
        Vector3 pos = transform.position;
        double x = pos.x, y = pos.y, z = pos.z;

        double rx = (float)(x * x);
        double ry = (float)(y * y);
        double rz = (float)(z * z);

        double s = (float)(rx + ry);
        s = (float)(s + rz);

        return s;
    }

    // Конвертация связей из saveId (сумма квадратов) в рантайм-айди после загрузки
    public static void ResolveAllConnections()
    {
        SavableProp[] all = FindObjectsOfType<SavableProp>();
        if (all.Length == 0)
            return;

        // Резолвим ТОЛЬКО свежезагруженные (pendingFileResolve):
        // 1) старые пропы уже имеют порядковые номера в связях — повторный резолв их затирал;
        // 2) при дубликатах позиций (та же точка -> тот же saveId) словарь
        //    должен указывать на объект ЭТОГО батча загрузки.
        var bySaveId = new Dictionary<double, SavableProp>();
        foreach (var p in all)
        {
            if (p.pendingFileResolve)
                bySaveId[p.fileSaveId] = p; // ключ — айди из файла, а не пересчёт из позиции
        }

        foreach (var p in all)
        {
            if (!p.pendingFileResolve)
                continue;

            p.pendingFileResolve = false;
            // Связи (connectedIds): saveId из файла -> порядковый номер
            List<string> ids = p.GetConnectionIds();
            if (ids.Count > 0)
            {
                var resolved = new List<string>();

                foreach (string s in ids)
                {
                    // ВАЖНО: парсим во float — ключ словаря тоже float-значение,
                    // поэтому "37923.34" превратится в 37923.33984375 и совпадёт с ключом
                    if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float saveIdF))
                        continue;

                    double saveId = saveIdF;

                    if (!bySaveId.TryGetValue(saveId, out SavableProp target))
                        target = FindClosestSaveId(bySaveId, saveId);

                    if (target != null)
                        resolved.Add(target.uniqueId.ToString(CultureInfo.InvariantCulture));
                }

                p.SetConnectedArray(resolved.ToArray());
                p.UpdateWires();

                int missed = ids.Count - resolved.Count;
                Debug.Log($"[Resolve] #{(int)p.uniqueId} {p.prefabName}: связей {ids.Count}, резолвнуто {resolved.Count}" + (missed > 0 ? ", ПРОПАЛО " + missed : ""));
            }

            // Системные float-айди у муверов (saveId источника) -> порядковый номер
            float savedRefId;
            if (p.type == PropType.Mover) savedRefId = p.moverPropUniqueId;
            else if (p.type == PropType.LocalRotator) savedRefId = p.localRotatorPropUniqueId;
            else if (p.type == PropType.LocalMover) savedRefId = p.localMoverPropUniqueId;
            else continue;

            if (savedRefId == 0f)
                continue;

            if (!bySaveId.TryGetValue(savedRefId, out SavableProp source))
                source = FindClosestSaveId(bySaveId, savedRefId);

            switch (p.type)
            {
                case PropType.Mover:
                    p.moverPropUniqueId = source != null ? (float)source.uniqueId : 0f;
                    break;
                case PropType.LocalRotator:
                    p.localRotatorPropUniqueId = source != null ? (float)source.uniqueId : 0f;
                    break;
                case PropType.LocalMover:
                    p.localMoverPropUniqueId = source != null ? (float)source.uniqueId : 0f;
                    break;
            }

            Debug.Log($"[Resolve] #{(int)p.uniqueId} {p.prefabName}: PropUniqueId -> " + (source != null ? ("#" + (int)source.uniqueId + " " + source.prefabName) : "НЕ НАЙДЕН"));
        }
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(prefabName))
            prefabName = gameObject.name.Replace("(Clone)", "").Trim();

        uniqueId = ++runtimeIdCounter;

        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.isKinematic = true;

        UpdateVisual();

        Invoke(nameof(DelayedWiresRefresh), 0.15f);
    }

    private void DelayedWiresRefresh()
    {
        UpdateWires();
    }

    public static void RefreshAllWires()
    {
        foreach (var sp in FindObjectsOfType<SavableProp>())
            sp.UpdateWires();
    }
    private void UpdateWires()
    {
        string[] connections = type switch
        {
            PropType.Monitor => monitorConnected,
            PropType.MonitorController => controllerConnected,
            PropType.Button => buttonConnected,
            PropType.LogicGate => logicGateConnected,
            PropType.Timer => timerConnected,
            PropType.Spawner => spawnerConnected,
            PropType.Remover => removerConnected,
            PropType.Repulsor => repulsorConnected,
            PropType.Nextbot => nextbotConnected,
            PropType.Mover => moverConnected,
            PropType.LocalRotator => localRotatorConnected,
            PropType.LocalMover => localMoverConnected,
            PropType.Trigger => triggerConnected,
            PropType.PasswordPanel => passwordPanelConnected,
            _ => null
        };

        if (connections == null)
            return;

        SavableProp[] all = FindObjectsOfType<SavableProp>();
        HashSet<double> validConnections = new();

        foreach (string idStr in connections)
        {
            if (!double.TryParse(idStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double id))
                continue;

            if (Math.Abs(id - uniqueId) < 0.0001)
                continue;

            foreach (var sp in all)
            {
                if (Math.Abs(sp.uniqueId - id) < 0.0001)
                {
                    validConnections.Add(id);

                    if (!activeWires.ContainsKey(id))
                    {
                        SpawnWire(sp.transform, id);
                    }

                    break;
                }
            }
        }

        // Удаляем лишние провода
        List<double> toRemove = new();
        foreach (var kvp in activeWires)
        {
            if (!validConnections.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        foreach (double id in toRemove)
        {
            Destroy(activeWires[id].gameObject);
            activeWires.Remove(id);
        }

        Debug.Log($"[Wires] #{(int)uniqueId} {prefabName}: связей {validConnections.Count}, проводов {activeWires.Count}");
    }

    private void SpawnWire(Transform target, double targetId)
    {
        WireLine prefab = wirePrefab;
        if (prefab == null)
            prefab = Resources.Load<WireLine>("WirePrefab");

        if (prefab == null)
        {
            Debug.LogError($"Wire prefab is not assigned and Resources/WirePrefab not found ({prefabName})!");
            return;
        }

        WireLine wire = Instantiate(
            prefab,
            wireParent != null ? wireParent : null
        );

        wire.enabled = true; // защита: в префабе компонент мог быть выключен

        int wiresLayer = LayerMask.NameToLayer("Wires");
        if (wiresLayer >= 0)
            wire.gameObject.layer = wiresLayer;

        wire.Init(transform, target);
        activeWires.Add(targetId, wire);
    }

    // ===============================
    // WIRE CONNECTIONS API
    // ===============================

    public bool HasWireSocket()
    {
        switch (type)
        {
            case PropType.Monitor:
            case PropType.MonitorController:
            case PropType.Button:
            case PropType.LogicGate:
            case PropType.Timer:
            case PropType.Spawner:
            case PropType.Remover:
            case PropType.Repulsor:
            case PropType.Nextbot:
            case PropType.Mover:
            case PropType.LocalRotator:
            case PropType.LocalMover:
            case PropType.Trigger:
            case PropType.PasswordPanel:
                return true;
            default:
                return false;
        }
    }

    private bool TryGetConnectedArray(out string[] arr)
    {
        switch (type)
        {
            case PropType.Monitor: arr = monitorConnected; return true;
            case PropType.MonitorController: arr = controllerConnected; return true;
            case PropType.Button: arr = buttonConnected; return true;
            case PropType.LogicGate: arr = logicGateConnected; return true;
            case PropType.Timer: arr = timerConnected; return true;
            case PropType.Spawner: arr = spawnerConnected; return true;
            case PropType.Remover: arr = removerConnected; return true;
            case PropType.Repulsor: arr = repulsorConnected; return true;
            case PropType.Nextbot: arr = nextbotConnected; return true;
            case PropType.Mover: arr = moverConnected; return true;
            case PropType.LocalRotator: arr = localRotatorConnected; return true;
            case PropType.LocalMover: arr = localMoverConnected; return true;
            case PropType.Trigger: arr = triggerConnected; return true;
            case PropType.PasswordPanel: arr = passwordPanelConnected; return true;
            default: arr = null; return false;
        }
    }

    // Поиск ближайшего saveId с допуском (для сейвов старой double-точности)
    private static SavableProp FindClosestSaveId(Dictionary<double, SavableProp> map, double saveId)
    {
        SavableProp best = null;
        double bestDiff = 0.0001;

        foreach (var kvp in map)
        {
            double diff = Math.Abs(kvp.Key - saveId);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = kvp.Value;
            }
        }
        return best;
    }

    private void SetConnectedArray(string[] value)
    {
        switch (type)
        {
            case PropType.Monitor: monitorConnected = value; break;
            case PropType.MonitorController: controllerConnected = value; break;
            case PropType.Button: buttonConnected = value; break;
            case PropType.LogicGate: logicGateConnected = value; break;
            case PropType.Timer: timerConnected = value; break;
            case PropType.Spawner: spawnerConnected = value; break;
            case PropType.Remover: removerConnected = value; break;
            case PropType.Repulsor: repulsorConnected = value; break;
            case PropType.Nextbot: nextbotConnected = value; break;
            case PropType.Mover: moverConnected = value; break;
            case PropType.LocalRotator: localRotatorConnected = value; break;
            case PropType.LocalMover: localMoverConnected = value; break;
            case PropType.Trigger: triggerConnected = value; break;
            case PropType.PasswordPanel: passwordPanelConnected = value; break;
        }
    }

    public List<string> GetConnectionIds()
    {
        var result = new List<string>();
        if (!TryGetConnectedArray(out string[] arr) || arr == null)
            return result;

        foreach (string s in arr)
        {
            if (!string.IsNullOrEmpty(s))
                result.Add(s);
        }

        return result;
    }

    public bool HasConnection(double id)
    {
        string idStr = id.ToString(CultureInfo.InvariantCulture);
        return GetConnectionIds().Contains(idStr);
    }

    public bool AddConnection(double id)
    {
        if (!TryGetConnectedArray(out string[] arr))
            return false;

        string idStr = id.ToString(CultureInfo.InvariantCulture);
        var list = new List<string>(arr ?? new string[0]);
        if (list.Contains(idStr))
            return false;

        list.Add(idStr);
        SetConnectedArray(list.ToArray());
        return true;
    }

    public bool RemoveConnection(double id)
    {
        if (!TryGetConnectedArray(out string[] arr) || arr == null)
            return false;

        string idStr = id.ToString(CultureInfo.InvariantCulture);
        var list = new List<string>(arr);
        if (!list.Remove(idStr))
            return false;

        SetConnectedArray(list.ToArray());
        return true;
    }

    public void RefreshWires()
    {
        UpdateWires();
    }

    // После копирования набора объектов: связи, указывающие на объекты из набора,
    // переключаются с оригиналов на копии (кнопка -> СВОЙ таймер, а не общий)
    public void RemapConnections(Dictionary<double, double> oldToNewId)
    {
        if (oldToNewId == null || oldToNewId.Count == 0)
            return;

        if (TryGetConnectedArray(out string[] arr) && arr != null && arr.Length > 0)
        {
            var list = new List<string>(arr);
            bool changed = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (double.TryParse(list[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double id)
                    && oldToNewId.TryGetValue(id, out double newId))
                {
                    list[i] = newId.ToString(CultureInfo.InvariantCulture);
                    changed = true;
                }
            }

            if (changed)
            {
                SetConnectedArray(list.ToArray());
                UpdateWires();
            }
        }

        switch (type)
        {
            case PropType.Mover:
                if (oldToNewId.TryGetValue(moverPropUniqueId, out double m1)) moverPropUniqueId = (float)m1;
                break;
            case PropType.LocalRotator:
                if (oldToNewId.TryGetValue(localRotatorPropUniqueId, out double m2)) localRotatorPropUniqueId = (float)m2;
                break;
            case PropType.LocalMover:
                if (oldToNewId.TryGetValue(localMoverPropUniqueId, out double m3)) localMoverPropUniqueId = (float)m3;
                break;
        }
    }

    // ===============================
    // SAVE FORMAT HELPERS (совместимость с оригиналом: connectedIds — числа)
    // ===============================

    // Конвертация рантайм-айди (строки) в float saveId (сумма квадратов цели) при сохранении
    private float?[] ToSaveIds(string[] arr)
    {
        if (arr == null || arr.Length == 0)
            return new float?[] { };

        var result = new List<float?>();
        foreach (string s in arr)
        {
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double runtimeId))
                continue;

            SavableProp target = FindByRuntimeId(runtimeId);
            if (target != null)
                result.Add((float)target.GetSaveId());
        }
        return result.ToArray();
    }

    private SavableProp FindByRuntimeId(double runtimeId)
    {
        foreach (var sp in FindObjectsOfType<SavableProp>())
        {
            if (sp != null && Math.Abs(sp.uniqueId - runtimeId) < 0.0001)
                return sp;
        }
        return null;
    }

    // Конвертация saveId из файла в строки-рантайм-айди при загрузке
    private static string[] FromSaveIds(float?[] arr)
    {
        if (arr == null || arr.Length == 0)
            return new string[0];

        var result = new List<string>();
        foreach (float? v in arr)
        {
            if (v.HasValue)
                result.Add(v.Value.ToString("R", CultureInfo.InvariantCulture));
        }
        return result.ToArray();
    }

    // Ручной ввод айди в панели: конвертируем порядковый номер цели в сумму квадратов при сохранении
    private float ResolveManualPropId(float manualId)
    {
        if (manualId == 0f)
            return 0f;

        SavableProp target = FindByRuntimeId(manualId);
        return target != null ? (float)target.GetSaveId() : manualId;
    }

    public SerializableProp ToSerializable()
    {
        var sp = new SerializableProp
        {
            name = prefabName,
            uniqueId = (float)GetSaveId(), // в файл пишется float-сумма квадратов, как в оригинале
            position = new SerializableVector3(transform.position),
            rotation = new SerializableQuaternion(transform.rotation),
            isKinematic = rb != null && rb.isKinematic,
            instantiationData = null,
            runtimeData = null
        };

        // Муверы: "PropUniqueId" — объект, который двигаем/вращаем (стена),
        // ровно как пишет оригинальная игра (см. Dacha.svn). Источник
        // (кнопка/таймер) в данных мувера не хранится: он ссылается на
        // мувера в своём connectedIds.
        float propMoveSaveId = 0f;
        if (type == PropType.Mover || type == PropType.LocalRotator || type == PropType.LocalMover)
        {
            float manualId;
            if (type == PropType.Mover) manualId = moverPropUniqueId;
            else if (type == PropType.LocalRotator) manualId = localRotatorPropUniqueId;
            else manualId = localMoverPropUniqueId;

            if (manualId != 0f)
            {
                // в поле панели хранится порядковый номер объекта -> конвертим в saveId файла
                propMoveSaveId = ResolveManualPropId(manualId);
            }
        }

        switch (type)
        {
            case PropType.Prop:
                break;

            case PropType.Monitor:
                sp.runtimeData = new RuntimeData
                {
                    Text = monitorText ?? "",
                    Color = string.IsNullOrEmpty(monitorColor) ? null : monitorColor,
                    connectedIds = ToSaveIds(monitorConnected)
                };
                break;

            case PropType.MonitorController:
                sp.instantiationData = new InstantiationData
                {
                    Text = controllerText ?? "",
                    BackgroundColor = string.IsNullOrEmpty(controllerBackground) ? null : controllerBackground
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(controllerConnected)
                };
                break;

            case PropType.Button:
                sp.instantiationData = new InstantiationData { Toggle = buttonToggle };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(buttonConnected),
                    Signal = buttonSignal
                };
                break;

            case PropType.LogicGate:
                sp.instantiationData = new InstantiationData { Mode = logicGateMode };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(logicGateConnected)
                };
                break;

            case PropType.Timer:
                sp.instantiationData = new InstantiationData { Time = timerTime };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(timerConnected)
                };
                break;

            case PropType.Spawner:
                sp.instantiationData = new InstantiationData
                {
                    PropName = spawnerPropName,
                    Kinematic = spawnerKinematic,
                    Distance = spawnerDistance,
                    DeleteAfter = spawnerDeleteAfter
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(spawnerConnected)
                };
                break;

            case PropType.Remover:
                sp.instantiationData = new InstantiationData { Distance = removerDistance };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(removerConnected)
                };
                break;

            case PropType.Repulsor:
                sp.instantiationData = new InstantiationData
                {
                    Distance = repulsorDistance,
                    Force = repulsorForce
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(repulsorConnected)
                };
                break;

            case PropType.Nextbot:
                sp.instantiationData = new InstantiationData
                {
                    ImageURL = nextbotImageURL,
                    Animated = nextbotAnimated,
                    ImageSize = nextbotImageSize
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(nextbotConnected)
                };
                break;

            case PropType.Mover:
                sp.instantiationData = new InstantiationData
                {
                    Speed = moverSpeed,
                    PositionA = moverPositionA,
                    PositionB = moverPositionB,
                    Direction = moverDirection
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(moverConnected),
                    PropUniqueId = propMoveSaveId != 0f ? propMoveSaveId : (float?)null,
                    PointAB = moverPointAB,
                    MoveProcess = moverMoveProcess
                };
                break;

            case PropType.LocalRotator:
                sp.instantiationData = new InstantiationData
                {
                    Speed = localRotatorSpeed,
                    Degree = localRotatorDegree,
                    Direction = localRotatorDirection
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(localRotatorConnected),
                    PropUniqueId = propMoveSaveId != 0f ? propMoveSaveId : (float?)null,
                    RotateProcess = localRotatorRotateProcess,
                    TempDegree = localRotatorTempDegree
                };
                break;

            case PropType.LocalMover:
                sp.instantiationData = new InstantiationData
                {
                    Speed = localMoverSpeed,
                    Range = localMoverRange,
                    Direction = localMoverDirection
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(localMoverConnected),
                    PropUniqueId = propMoveSaveId != 0f ? propMoveSaveId : (float?)null,
                    MoveProcess = localMoverMoveProcess,
                    FinalVector = new SerializableVector3(localMoverFinalVector)
                };
                break;

            case PropType.Trigger:
                sp.instantiationData = new InstantiationData
                {
                    Size = triggerSize,
                    Mode = triggerMode,
                    ObjectName = triggerObjectName
                };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(triggerConnected)
                };
                break;

            case PropType.PasswordPanel:
                sp.instantiationData = new InstantiationData { Password = passwordPanelPassword };
                sp.runtimeData = new RuntimeData
                {
                    connectedIds = ToSaveIds(passwordPanelConnected)
                };
                break;
        }

        return sp;
    }

    public void Apply(SerializableProp sp)
    {
        prefabName = sp.name.Replace("(Clone)", "").Trim();

        // uniqueId НЕ перезаписываем: в рантайме он порядковый (выдан в Awake),
        // а суммы квадратов из файла используются только для резолва связей (ResolveAllConnections).
        fileSaveId = sp.uniqueId;
        pendingFileResolve = true;
        transform.position = sp.position.ToVector3();
        transform.rotation = sp.rotation.ToQuaternion();
        if (rb != null) rb.isKinematic = sp.isKinematic;

        // Определяем тип по имени префаба

        type = GetPropTypeByName(prefabName);
        if (sp.instantiationData != null)
        {
            switch (type)
            {
                case PropType.MonitorController:
                    controllerText = sp.instantiationData.Text ?? "";
                    controllerBackground = sp.instantiationData.BackgroundColor ?? "";
                    break;

                case PropType.Button:
                    buttonToggle = sp.instantiationData.Toggle ?? false;
                    break;

                case PropType.LogicGate:
                    logicGateMode = sp.instantiationData.Mode ?? "AND";
                    break;

                case PropType.Timer:
                    timerTime = sp.instantiationData.Time ?? 1.0f;
                    break;

                case PropType.Spawner:
                    spawnerPropName = sp.instantiationData.PropName ?? "Crate";
                    spawnerKinematic = sp.instantiationData.Kinematic ?? false;
                    spawnerDistance = sp.instantiationData.Distance ?? 1.0f;
                    spawnerDeleteAfter = sp.instantiationData.DeleteAfter ?? 0.0f;
                    break;

                case PropType.Remover:
                    removerDistance = sp.instantiationData.Distance ?? 1.0f;
                    break;

                case PropType.Repulsor:
                    repulsorDistance = sp.instantiationData.Distance ?? 1.0f;
                    repulsorForce = sp.instantiationData.Force ?? 1.0f;
                    break;

                case PropType.Nextbot:
                    nextbotImageURL = sp.instantiationData.ImageURL ?? "";
                    nextbotAnimated = sp.instantiationData.Animated ?? false;
                    nextbotImageSize = sp.instantiationData.ImageSize ?? "100%";
                    break;

                case PropType.Mover:
                    moverSpeed = sp.instantiationData.Speed ?? 1.0f;
                    moverPositionA = sp.instantiationData.PositionA ?? 0.0f;
                    moverPositionB = sp.instantiationData.PositionB ?? 0.0f;
                    moverDirection = sp.instantiationData.Direction ?? "X";
                    break;

                case PropType.LocalRotator:
                    localRotatorSpeed = sp.instantiationData.Speed ?? 1.0f;
                    localRotatorDegree = sp.instantiationData.Degree ?? 0.0f;
                    localRotatorDirection = sp.instantiationData.Direction ?? "X";
                    break;

                case PropType.LocalMover:
                    localMoverSpeed = sp.instantiationData.Speed ?? 1.0f;
                    localMoverRange = sp.instantiationData.Range ?? 0.0f;
                    localMoverDirection = sp.instantiationData.Direction ?? "X";
                    break;

                case PropType.Trigger:
                    triggerSize = sp.instantiationData.Size ?? 1.0f;
                    triggerMode = sp.instantiationData.Mode ?? "Everything";
                    triggerObjectName = sp.instantiationData.ObjectName ?? "";
                    break;

                case PropType.PasswordPanel:
                    passwordPanelPassword = sp.instantiationData.Password ?? "";
                    break;
            }
        }

        if (sp.runtimeData != null)
        {
            switch (type)
            {
                case PropType.Monitor:
                    monitorText = sp.runtimeData.Text ?? "";
                    monitorColor = sp.runtimeData.Color ?? "";
                    monitorConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;

                case PropType.MonitorController:
                    controllerConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;

                case PropType.Button:
                    buttonSignal = sp.runtimeData.Signal ?? false;
                    buttonConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;

                case PropType.LogicGate:
                case PropType.Timer:
                case PropType.Spawner:
                case PropType.Remover:
                case PropType.Repulsor:
                case PropType.Nextbot:
                case PropType.Trigger:
                case PropType.PasswordPanel:
                    var connectedIds = FromSaveIds(sp.runtimeData.connectedIds);
                    switch (type)
                    {
                        case PropType.LogicGate: logicGateConnected = connectedIds; break;
                        case PropType.Timer: timerConnected = connectedIds; break;
                        case PropType.Spawner: spawnerConnected = connectedIds; break;
                        case PropType.Remover: removerConnected = connectedIds; break;
                        case PropType.Repulsor: repulsorConnected = connectedIds; break;
                        case PropType.Nextbot: nextbotConnected = connectedIds; break;
                        case PropType.Trigger: triggerConnected = connectedIds; break;
                        case PropType.PasswordPanel: passwordPanelConnected = connectedIds; break;
                    }
                    break;

                case PropType.Mover:
                    // "PropUniqueId" (семантика оригинала) — объект, который двигаем;
                    // старое наше "Move Id" читаем как фолбэк для старых сейвов
                    moverPropUniqueId = sp.runtimeData.PropUniqueId ?? sp.runtimeData.MoveId ?? 0f;
                    moverPointAB = sp.runtimeData.PointAB ?? false;
                    moverMoveProcess = sp.runtimeData.MoveProcess ?? false;
                    moverConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;

                case PropType.LocalRotator:
                    localRotatorPropUniqueId = sp.runtimeData.PropUniqueId ?? sp.runtimeData.MoveId ?? 0f;
                    localRotatorRotateProcess = sp.runtimeData.RotateProcess ?? false;
                    localRotatorTempDegree = sp.runtimeData.TempDegree ?? 0f;
                    localRotatorConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;

                case PropType.LocalMover:
                    localMoverPropUniqueId = sp.runtimeData.PropUniqueId ?? sp.runtimeData.MoveId ?? 0f;
                    localMoverMoveProcess = sp.runtimeData.MoveProcess ?? false;
                    if (sp.runtimeData.FinalVector.HasValue)
                        localMoverFinalVector = sp.runtimeData.FinalVector.Value.ToVector3();
                    localMoverConnected = FromSaveIds(sp.runtimeData.connectedIds);
                    break;
            }
        }

        UpdateVisual();
        UpdateWires();
    }


    private PropType GetPropTypeByName(string name)
    {
        return name switch
        {
            "Monitor" => PropType.Monitor,
            "MonitorController" => PropType.MonitorController,
            "Button" => PropType.Button,
            "LogicGate" => PropType.LogicGate,
            "Timer" => PropType.Timer,
            "Spawner" => PropType.Spawner,
            "Remover" => PropType.Remover,
            "Repulsor" => PropType.Repulsor,
            "Nextbot" => PropType.Nextbot,
            "Mover" => PropType.Mover,
            "LocalRotator" => PropType.LocalRotator,
            "LocalMover" => PropType.LocalMover,
            "Trigger" => PropType.Trigger,
            "PasswordPanel" => PropType.PasswordPanel,
            "Lamp" => PropType.Lamp,
            _ => PropType.Prop
        };
    }

    private void OnDestroy()
    {
        // Удаляем свои провода
        foreach (var kvp in activeWires)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value.gameObject);
        }
        activeWires.Clear();

        // Убираем ссылки на этот проп у остальных и их провода к нам
        SavableProp[] all = FindObjectsOfType<SavableProp>();
        foreach (var p in all)
        {
            if (p == null || p == this)
                continue;

            if (p.HasConnection(uniqueId))
            {
                p.RemoveConnection(uniqueId);
                p.RefreshWires();
            }
        }
    }

    public void Update()
    {
        //UpdateWires();
        

        

        
        
        //UpdateVisual();
    }

    public void UpdateVisual()
    {
        // Монитор работает
        if (type == PropType.Monitor)
        {
            if (monitorTextUI != null) monitorTextUI.text = monitorText;
            if (!string.IsNullOrEmpty(monitorColor) && ColorUtility.TryParseHtmlString(monitorColor, out var col))
                if (monitorImageUI != null) monitorImageUI.color = col;
        }

        // LogicGate — выводим название выбранного вентиля (AND/OR/XOR/NAND/NOR/XNOR)
        if (type == PropType.LogicGate)
        {
            if (logicGateModeText == null)
                logicGateModeText = GetComponentInChildren<TMP_Text>(true);

            if (logicGateModeText != null)
                logicGateModeText.text = string.IsNullOrEmpty(logicGateMode) ? "AND" : logicGateMode;
        }

        // Проверьте, выполняется ли этот блок
        if (type == PropType.PasswordPanel)
        {
            Debug.Log($"PasswordPanel UpdateVisual called. Password: {passwordPanelPassword}");
            
            // Проверка ссылки
            if (passwordText == null)
            {
                Debug.LogWarning("passwordText is NULL!");
                // Попробуйте найти компонент
                passwordText = GetComponentInChildren<Text>();
                if (passwordText == null)
                    Debug.LogError("Text component not found in children!");
            }
            
            if (passwordText != null) 
            {
                passwordText.text = passwordPanelPassword;
                Debug.Log($"Password text set to: {passwordPanelPassword}");
            }
        }
    }
    private void LateUpdate()
    {
        
    }

    // Properties
    public bool IsKinematic { get => rb != null && rb.isKinematic; set { if (rb != null) rb.isKinematic = value; } }

    // Параметры подключения (значения соответствуют префабам оригинальной игры)
    public bool canConnect => type switch
    {
        PropType.Button => true,
        PropType.LogicGate => true,
        PropType.Timer => true,
        PropType.MonitorController => true,
        PropType.Trigger => true,
        PropType.PasswordPanel => true,
        _ => false
    };

    public bool canAttach => type switch
    {
        PropType.Monitor => true,
        PropType.MonitorController => true,
        PropType.LogicGate => true,
        PropType.Timer => true,
        PropType.Spawner => true,
        PropType.Remover => true,
        PropType.Repulsor => true,
        PropType.Nextbot => true,
        PropType.Mover => true,
        PropType.LocalMover => true,
        PropType.LocalRotator => true,
        PropType.Lamp => true,
        _ => false
    };

    public bool multiAttach => type switch
    {
        PropType.Monitor => true,
        PropType.LogicGate => true,
        PropType.Mover => true,
        PropType.LocalMover => true,
        PropType.LocalRotator => true,
        _ => false
    };

    public bool isProp => type == PropType.Prop;
    public bool isMonitor => type == PropType.Monitor;
    public bool isMonitorController => type == PropType.MonitorController;
    public bool isButton => type == PropType.Button;
    public bool isLogicGate => type == PropType.LogicGate;
    public bool isTimer => type == PropType.Timer;
    public bool isSpawner => type == PropType.Spawner;
    public bool isRemover => type == PropType.Remover;
    public bool isRepulsor => type == PropType.Repulsor;
    public bool isNextbot => type == PropType.Nextbot;
    public bool isMover => type == PropType.Mover;
    public bool isLocalRotator => type == PropType.LocalRotator;
    public bool isLocalMover => type == PropType.LocalMover;
    public bool isTrigger => type == PropType.Trigger;
    public bool isPasswordPanel => type == PropType.PasswordPanel;
    public bool isLamp => type == PropType.Lamp;
    
    public string backgroundColor { get => controllerBackground; set => controllerBackground = value; }
    public SerializableProp ToSerializableProp() => ToSerializable();
    public void ApplySerializableProp(SerializableProp sp) => Apply(sp);
}

   