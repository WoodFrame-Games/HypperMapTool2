using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RuntimeGizmos;
using System.Globalization;

public class InputScript : MonoBehaviour
{
    [Header("Transform Inputs")]
    public TMP_InputField posXInput, posYInput, posZInput;
    public TMP_InputField rotXInput, rotYInput, rotZInput;

    [Header("Physics")]
    public Toggle isKinematicToggle;

    [Header("Buttons")]
    public Button deleteButton;

    [Header("Panels")]
    public GameObject buttonPanel;
    public Toggle buttonToggle; // Заменен с TMP_InputField на Toggle

    public GameObject logicGatePanel;
    public TMP_Dropdown logicGateModeDropdown; // Заменен с TMP_InputField на TMP_Dropdown

    [Header("Monitor")]
    public GameObject MonitorPanel;
    public TMP_InputField MonitorTextInput; 
    public TMP_InputField MonitorColorInput; 

    [Header("MonitorController")]
    public GameObject MonitorControllerPanel;
    public TMP_InputField MonitorControllerTextInput; 
    public TMP_InputField MonitorControllerColorInput; 

    [Header("Timer")]
    public GameObject timerPanel;
    public TMP_InputField timerTimeInput;

    [Header("Spawner")]
    public GameObject spawnerPanel;
    public TMP_InputField spawnerPropNameInput;
    public TMP_InputField spawnerDistanceInput;
    public Toggle spawnerKinematicToggle;
    public TMP_InputField spawnerDeleteAfterInput;

    [Header("Remover")]
    public GameObject removerPanel;
    public TMP_InputField removerDistanceInput;

    [Header("Repulsor")]
    public GameObject repulsorPanel;
    public TMP_InputField repulsorDistanceInput;
    public TMP_InputField repulsorForceInput;

    [Header("Nextbot")]
    public GameObject nextbotPanel;
    public TMP_InputField nextbotImageURLInput;
    public Toggle nextbotAnimatedToggle;
    public TMP_InputField nextbotImageSizeInput;

    [Header("Mover")]
    public GameObject moverPanel;
    public TMP_InputField moverSpeedInput;
    public TMP_InputField moverPositionAInput;
    public TMP_InputField moverPositionBInput;
    public TMP_Dropdown moverDirectionDropdown;
    public TMP_InputField moverPropIdInput;

    [Header("LocalRotator")]
    public GameObject localRotatorPanel;
    public TMP_InputField localRotatorSpeedInput;
    public TMP_InputField localRotatorDegreeInput;
    public TMP_Dropdown localRotatorDirectionDropdown; // Заменен на Dropdown
    public TMP_InputField localRotatorPropIdInput;

    [Header("LocalMover")]
    public GameObject localMoverPanel;
    public TMP_InputField localMoverSpeedInput;
    public TMP_InputField localMoverRangeInput;
    public TMP_Dropdown localMoverDirectionDropdown; // Заменен на Dropdown
    public TMP_InputField localMoverPropIdInput;

    [Header("Trigger")]
    public GameObject triggerPanel;
    public TMP_InputField triggerSizeInput;
    public TMP_Dropdown triggerModeDropdown; // Заменен на Dropdown
    public TMP_InputField triggerObjectNameInput;

    [Header("PasswordPanel")]
    public GameObject passwordPanelPanel;
    public TMP_InputField passwordPanelPasswordInput;

    private List<Transform> rootsLink;
    private SavableProp currentSavableProp;

    private void Start()
    {
        isKinematicToggle.interactable = false;
        MonitorTextInput.onValueChanged.AddListener(UpdateMonitorText);
        MonitorColorInput.onValueChanged.AddListener(UpdateMonitorColor);

        MonitorControllerTextInput.onValueChanged.AddListener(UpdateMonitorControllerText);
        MonitorControllerColorInput.onValueChanged.AddListener(UpdateMonitorControllerColor);

        posXInput.onValueChanged.AddListener(_ => UpdateTransform());
        posYInput.onValueChanged.AddListener(_ => UpdateTransform());
        posZInput.onValueChanged.AddListener(_ => UpdateTransform());
        rotXInput.onValueChanged.AddListener(_ => UpdateRotate());
        rotYInput.onValueChanged.AddListener(_ => UpdateRotate());
        rotZInput.onValueChanged.AddListener(_ => UpdateRotate());

        isKinematicToggle.onValueChanged.AddListener(UpdateKinematic);
        deleteButton.onClick.AddListener(DeleteSelectedObjects);

        // Подписываемся на события для всех панелей
        buttonToggle.onValueChanged.AddListener(UpdateButtonToggle);
        logicGateModeDropdown.onValueChanged.AddListener(UpdateLogicGateMode);
        timerTimeInput.onValueChanged.AddListener(UpdateTimerTime);
        
        spawnerPropNameInput.onValueChanged.AddListener(UpdateSpawnerPropName);
        spawnerDistanceInput.onValueChanged.AddListener(UpdateSpawnerDistance);
        spawnerKinematicToggle.onValueChanged.AddListener(UpdateSpawnerKinematic);
        spawnerDeleteAfterInput.onValueChanged.AddListener(UpdateSpawnerDeleteAfter);

        removerDistanceInput.onValueChanged.AddListener(UpdateRemoverDistance);
        
        repulsorDistanceInput.onValueChanged.AddListener(UpdateRepulsorDistance);
        repulsorForceInput.onValueChanged.AddListener(UpdateRepulsorForce);
        
        nextbotImageURLInput.onValueChanged.AddListener(UpdateNextbotImageURL);
        nextbotAnimatedToggle.onValueChanged.AddListener(UpdateNextbotAnimated);
        nextbotImageSizeInput.onValueChanged.AddListener(UpdateNextbotImageSize);
        
        moverSpeedInput.onValueChanged.AddListener(UpdateMoverSpeed);
        
        moverPositionAInput.onValueChanged.AddListener(UpdateMoverPositionA);
        moverPositionBInput.onValueChanged.AddListener(UpdateMoverPositionB);
        moverDirectionDropdown.onValueChanged.AddListener(UpdateMoverDirection);
        moverPropIdInput.onValueChanged.AddListener(UpdateMoverPropId);
        
        localRotatorSpeedInput.onValueChanged.AddListener(UpdateLocalRotatorSpeed);
        localRotatorDegreeInput.onValueChanged.AddListener(UpdateLocalRotatorDegree);
        localRotatorDirectionDropdown.onValueChanged.AddListener(UpdateLocalRotatorDirection);
        localRotatorPropIdInput.onValueChanged.AddListener(UpdateLocalRotatorPropId);
        
        localMoverSpeedInput.onValueChanged.AddListener(UpdateLocalMoverSpeed);
        localMoverRangeInput.onValueChanged.AddListener(UpdateLocalMoverRange);
        localMoverDirectionDropdown.onValueChanged.AddListener(UpdateLocalMoverDirection);
        localMoverPropIdInput.onValueChanged.AddListener(UpdateLocalMoverPropId);
        
        triggerSizeInput.onValueChanged.AddListener(UpdateTriggerSize);
        triggerModeDropdown.onValueChanged.AddListener(UpdateTriggerMode);
        triggerObjectNameInput.onValueChanged.AddListener(UpdateTriggerObjectName);
        
        passwordPanelPasswordInput.onValueChanged.AddListener(UpdatePasswordPanelText);
    }


    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.C))
        {
            CopySelectedObjects();
        }
        rootsLink = GetComponent<TransformGizmo>().targetRootsOrdered;

        if (rootsLink == null || rootsLink.Count == 0)
        {
            if (currentSavableProp != null)
            {
                currentSavableProp = null;
                DisableAllUI();
                isKinematicToggle.interactable = false;
            }
            return;
        }
        rootsLink = GetComponent<TransformGizmo>().targetRootsOrdered;

        if (rootsLink != null && rootsLink.Count > 0)
        {
            currentSavableProp = rootsLink[0].GetComponent<SavableProp>();
            if (currentSavableProp != null)
            {
                // Пока пользователь редактирует поля — не затираем их ввод каждый кадр
                if (!AnyUiFieldFocused())
                {
                    UpdateTransformUI();
                    UpdateKinematicToggle();
                    UpdatePanels();
                }
            }
        }
        else
        {
            currentSavableProp = null;
            DisableAllUI();
        }
    }
    public TransformGizmo gizmo;

    void CopySelectedObjects()
    {
        if (gizmo == null || gizmo.targetRootsOrdered.Count == 0)
            return;

        List<Transform> newCopies = new List<Transform>();
        Dictionary<double, double> idMap = new Dictionary<double, double>(); // старый айди -> новый айди
        List<SavableProp> clones = new List<SavableProp>();

        foreach (var t in gizmo.targetRootsOrdered)
        {
            Transform clone = Instantiate(t.gameObject).transform;
            clone.position = t.position;
            clone.rotation = t.rotation;
            clone.localScale = t.localScale;

            newCopies.Add(clone);

            SavableProp orig = t.GetComponent<SavableProp>();
            SavableProp copy = clone.GetComponent<SavableProp>();
            if (orig != null && copy != null)
            {
                idMap[orig.uniqueId] = copy.uniqueId;
                clones.Add(copy);
            }
            else if (copy != null)
            {
                clones.Add(copy);
            }
        }

        // Связи внутри набора перекидываются на копии:
        // оригинальная кнопка -> оригинальный таймер, копия кнопки -> копия таймера
        foreach (var copy in clones)
            copy.RemapConnections(idMap);

        // Сбрасываем выделение
        gizmo.ClearTargets();

        // Добавляем новые объекты в выделение
        foreach (Transform t in newCopies)
        {
            gizmo.AddTarget(t, true);  // true — чтобы обновить гизмо
        }
    }

    private bool AnyUiFieldFocused()
    {
        return posXInput.isFocused || posYInput.isFocused || posZInput.isFocused ||
               rotXInput.isFocused || rotYInput.isFocused || rotZInput.isFocused ||
               MonitorTextInput.isFocused || MonitorColorInput.isFocused ||
               MonitorControllerTextInput.isFocused || MonitorControllerColorInput.isFocused ||
               timerTimeInput.isFocused ||
               spawnerPropNameInput.isFocused || spawnerDistanceInput.isFocused || spawnerDeleteAfterInput.isFocused ||
               removerDistanceInput.isFocused ||
               repulsorDistanceInput.isFocused || repulsorForceInput.isFocused ||
               nextbotImageURLInput.isFocused || nextbotImageSizeInput.isFocused ||
               moverSpeedInput.isFocused || moverPositionAInput.isFocused || moverPositionBInput.isFocused || moverPropIdInput.isFocused ||
               localRotatorSpeedInput.isFocused || localRotatorDegreeInput.isFocused || localRotatorPropIdInput.isFocused ||
               localMoverSpeedInput.isFocused || localMoverRangeInput.isFocused || localMoverPropIdInput.isFocused ||
               triggerSizeInput.isFocused || triggerObjectNameInput.isFocused ||
               passwordPanelPasswordInput.isFocused;
    }

    private Vector3 lastUiPos;
    private Vector3 lastUiRot;

    private void UpdateTransformUI()
    {
        Vector3 pos = currentSavableProp.transform.position;
        Vector3 rot = currentSavableProp.transform.eulerAngles;

        // Обновляем текст только при реальном изменении значений,
        // иначе TMP перестраивается каждый кадр и дёргается каретка
        if (pos != lastUiPos)
        {
            posXInput.SetTextWithoutNotify(pos.x.ToString("F2"));
            posYInput.SetTextWithoutNotify(pos.y.ToString("F2"));
            posZInput.SetTextWithoutNotify(pos.z.ToString("F2"));
            lastUiPos = pos;
        }

        if (rot != lastUiRot)
        {
            rotXInput.SetTextWithoutNotify(rot.x.ToString("F2"));
            rotYInput.SetTextWithoutNotify(rot.y.ToString("F2"));
            rotZInput.SetTextWithoutNotify(rot.z.ToString("F2"));
            lastUiRot = rot;
        }
    }

    private void UpdateTransform()
    {
        if (currentSavableProp == null || rootsLink == null) return;

        Vector3 oldPos = currentSavableProp.transform.position;
        Vector3 target = oldPos;
        // Плюём на пустые/некорректные поля — оси применяются независимо
        if (float.TryParse(posXInput.text, out float x)) target.x = x;
        if (float.TryParse(posYInput.text, out float y)) target.y = y;
        if (float.TryParse(posZInput.text, out float z)) target.z = z;

        Vector3 delta = target - oldPos;
        if (delta == Vector3.zero) return;

        // Двигаем ВСЕ выделенные объекты на ту же дельту
        foreach (Transform root in rootsLink)
        {
            if (root != null)
                root.position += delta;
        }

        lastUiPos += delta;
    }

    private void UpdateRotate()
    {
        if (currentSavableProp == null || rootsLink == null) return;

        Vector3 oldRot = currentSavableProp.transform.eulerAngles;
        Vector3 targetRot = oldRot;
        if (float.TryParse(rotXInput.text, out float x)) targetRot.x = x;
        if (float.TryParse(rotYInput.text, out float y)) targetRot.y = y;
        if (float.TryParse(rotZInput.text, out float z)) targetRot.z = z;

        Vector3 deltaEuler = new Vector3(
            Mathf.DeltaAngle(oldRot.x, targetRot.x),
            Mathf.DeltaAngle(oldRot.y, targetRot.y),
            Mathf.DeltaAngle(oldRot.z, targetRot.z));

        if (deltaEuler == Vector3.zero) return;

        // Поворачиваем все выделенные вокруг пивота первого объекта
        Quaternion deltaQ = Quaternion.Euler(deltaEuler);
        Vector3 pivot = currentSavableProp.transform.position;

        foreach (Transform root in rootsLink)
        {
            if (root == null) continue;

            Vector3 rel = root.position - pivot;
            root.SetPositionAndRotation(pivot + deltaQ * rel, deltaQ * root.rotation);
        }

        lastUiRot = currentSavableProp.transform.eulerAngles;
    }

    private void UpdateKinematic(bool val)
    {
        if (currentSavableProp != null)
            currentSavableProp.IsKinematic = val;
    }

    private void UpdateKinematicToggle()
    {
        if (currentSavableProp != null)
        {
            isKinematicToggle.interactable = true; // ← добавь
            isKinematicToggle.SetIsOnWithoutNotify(currentSavableProp.IsKinematic);
        }
    }

    private void UpdatePanels()
    {
        // Активируем соответствующие панели

        MonitorPanel.SetActive(currentSavableProp.isMonitor);
        MonitorControllerPanel.SetActive(currentSavableProp.isMonitorController);
        buttonPanel.SetActive(currentSavableProp.isButton);
        logicGatePanel.SetActive(currentSavableProp.isLogicGate);
        timerPanel.SetActive(currentSavableProp.isTimer);
        spawnerPanel.SetActive(currentSavableProp.isSpawner);
        removerPanel.SetActive(currentSavableProp.isRemover);
        repulsorPanel.SetActive(currentSavableProp.isRepulsor);
        nextbotPanel.SetActive(currentSavableProp.isNextbot);
        moverPanel.SetActive(currentSavableProp.isMover);
        localRotatorPanel.SetActive(currentSavableProp.isLocalRotator);
        localMoverPanel.SetActive(currentSavableProp.isLocalMover);
        triggerPanel.SetActive(currentSavableProp.isTrigger);
        passwordPanelPanel.SetActive(currentSavableProp.isPasswordPanel);

        // Обновляем значения в панелях

        if (currentSavableProp.isMonitor)
        {
            MonitorTextInput.SetTextWithoutNotify(currentSavableProp.monitorText);
            MonitorColorInput.SetTextWithoutNotify(currentSavableProp.monitorColor);
        }



        if (currentSavableProp.isMonitorController)
        {
            MonitorControllerTextInput.SetTextWithoutNotify(currentSavableProp.controllerText);
            MonitorControllerColorInput.SetTextWithoutNotify(currentSavableProp.controllerBackground);
        }

        if (currentSavableProp.isButton)
        {
            buttonToggle.SetIsOnWithoutNotify(currentSavableProp.buttonToggle);
        }

        if (currentSavableProp.isLogicGate)
        {
            int index = logicGateModeDropdown.options.FindIndex(o => o.text == currentSavableProp.logicGateMode);
            if (index >= 0)
                logicGateModeDropdown.SetValueWithoutNotify(index);
        }

        if (currentSavableProp.isTimer)
        {
            timerTimeInput.SetTextWithoutNotify(currentSavableProp.timerTime.ToString("F2"));
        }

        if (currentSavableProp.isSpawner)
        {
            spawnerPropNameInput.SetTextWithoutNotify(currentSavableProp.spawnerPropName);
            spawnerDistanceInput.SetTextWithoutNotify(currentSavableProp.spawnerDistance.ToString("F2"));
            spawnerKinematicToggle.SetIsOnWithoutNotify(currentSavableProp.spawnerKinematic);
            spawnerDeleteAfterInput.SetTextWithoutNotify(currentSavableProp.spawnerDeleteAfter.ToString("F2"));
        }

        if (currentSavableProp.isRemover)
        {
            removerDistanceInput.SetTextWithoutNotify(currentSavableProp.removerDistance.ToString("F2"));
        }

        if (currentSavableProp.isRepulsor)
        {
            repulsorDistanceInput.SetTextWithoutNotify(currentSavableProp.repulsorDistance.ToString("F2"));
            repulsorForceInput.SetTextWithoutNotify(currentSavableProp.repulsorForce.ToString("F2"));
        }

        if (currentSavableProp.isNextbot)
        {
            nextbotImageURLInput.SetTextWithoutNotify(currentSavableProp.nextbotImageURL);
            nextbotAnimatedToggle.SetIsOnWithoutNotify(currentSavableProp.nextbotAnimated);
            
            // Убираем символ % при отображении
            string sizeWithoutPercent = currentSavableProp.nextbotImageSize.Replace("%", "");
            nextbotImageSizeInput.SetTextWithoutNotify(sizeWithoutPercent);
        }

        if (currentSavableProp.isMover)
        {
            moverSpeedInput.SetTextWithoutNotify(currentSavableProp.moverSpeed.ToString("F2"));
            moverPositionAInput.SetTextWithoutNotify(currentSavableProp.moverPositionA.ToString("F2"));
            moverPositionBInput.SetTextWithoutNotify(currentSavableProp.moverPositionB.ToString("F2"));
            int index = moverDirectionDropdown.options.FindIndex(o => o.text == currentSavableProp.moverDirection);
            if (index >= 0)
                moverDirectionDropdown.SetValueWithoutNotify(index);
            moverPropIdInput.SetTextWithoutNotify(currentSavableProp.moverPropUniqueId.ToString(CultureInfo.InvariantCulture));
        }

        if (currentSavableProp.isLocalRotator)
        {
            localRotatorSpeedInput.SetTextWithoutNotify(currentSavableProp.localRotatorSpeed.ToString("F2"));
            localRotatorDegreeInput.SetTextWithoutNotify(currentSavableProp.localRotatorDegree.ToString("F2"));
            
            int index = localRotatorDirectionDropdown.options.FindIndex(o => o.text == currentSavableProp.localRotatorDirection);
            if (index >= 0)
                localRotatorDirectionDropdown.SetValueWithoutNotify(index);
            localRotatorPropIdInput.SetTextWithoutNotify(currentSavableProp.localRotatorPropUniqueId.ToString(CultureInfo.InvariantCulture));
        }

        if (currentSavableProp.isLocalMover)
        {
            localMoverSpeedInput.SetTextWithoutNotify(currentSavableProp.localMoverSpeed.ToString("F2"));
            localMoverRangeInput.SetTextWithoutNotify(currentSavableProp.localMoverRange.ToString("F2"));
            
            int index = localMoverDirectionDropdown.options.FindIndex(o => o.text == currentSavableProp.localMoverDirection);
            if (index >= 0)
                localMoverDirectionDropdown.SetValueWithoutNotify(index);
            localMoverPropIdInput.SetTextWithoutNotify(currentSavableProp.localMoverPropUniqueId.ToString(CultureInfo.InvariantCulture));
        }

        if (currentSavableProp.isTrigger)
        {
            triggerSizeInput.SetTextWithoutNotify(currentSavableProp.triggerSize.ToString("F2"));
            
            int index = triggerModeDropdown.options.FindIndex(o => o.text == currentSavableProp.triggerMode);
            if (index >= 0)
                triggerModeDropdown.SetValueWithoutNotify(index);
                
            triggerObjectNameInput.SetTextWithoutNotify(currentSavableProp.triggerObjectName);
        }

        if (currentSavableProp.isPasswordPanel)
        {
            passwordPanelPasswordInput.SetTextWithoutNotify(currentSavableProp.passwordPanelPassword);
        }
    }

    private void DisableAllUI()
    {
        posXInput.text = posYInput.text = posZInput.text = "-";
        rotXInput.text = rotYInput.text = rotZInput.text = "-";
        isKinematicToggle.interactable = false;
        
        // Деактивируем все панели
        MonitorPanel.SetActive(false);
        MonitorControllerPanel.SetActive(false);
        buttonPanel.SetActive(false);
        logicGatePanel.SetActive(false);
        timerPanel.SetActive(false);
        spawnerPanel.SetActive(false);
        removerPanel.SetActive(false);
        repulsorPanel.SetActive(false);
        nextbotPanel.SetActive(false);
        moverPanel.SetActive(false);
        localRotatorPanel.SetActive(false);
        localMoverPanel.SetActive(false);
        triggerPanel.SetActive(false);
        passwordPanelPanel.SetActive(false);
    }

    // --- Методы обновления для каждого типа объектов ---
    private void UpdateButtonToggle(bool val)
    {
        if (currentSavableProp != null && currentSavableProp.isButton)
            currentSavableProp.buttonToggle = val;
    }

    private void UpdateLogicGateMode(int index)
    {
        if (currentSavableProp != null && currentSavableProp.isLogicGate)
        {
            currentSavableProp.logicGateMode = logicGateModeDropdown.options[index].text;
            currentSavableProp.UpdateVisual();
        }
    }

    private void UpdateTimerTime(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isTimer && float.TryParse(val, out float time))
            currentSavableProp.timerTime = time;
    }

    private void UpdateSpawnerPropName(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isSpawner)
            currentSavableProp.spawnerPropName = val;
    }

    private void UpdateSpawnerDistance(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isSpawner && float.TryParse(val, out float distance))
            currentSavableProp.spawnerDistance = distance;
    }

    private void UpdateSpawnerKinematic(bool val)
    {
        if (currentSavableProp != null && currentSavableProp.isSpawner)
            currentSavableProp.spawnerKinematic = val;
    }

    private void UpdateSpawnerDeleteAfter(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isSpawner && float.TryParse(val, out float deleteAfter))
            currentSavableProp.spawnerDeleteAfter = deleteAfter;
    }

    private void UpdateRemoverDistance(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isRemover && float.TryParse(val, out float distance))
            currentSavableProp.removerDistance = distance;
    }

    private void UpdateRepulsorDistance(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isRepulsor && float.TryParse(val, out float distance))
            currentSavableProp.repulsorDistance = distance;
    }

    private void UpdateRepulsorForce(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isRepulsor && float.TryParse(val, out float force))
            currentSavableProp.repulsorForce = force;
    }

    private void UpdateNextbotImageURL(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isNextbot)
            currentSavableProp.nextbotImageURL = val;
    }

    private void UpdateNextbotAnimated(bool val)
    {
        if (currentSavableProp != null && currentSavableProp.isNextbot)
            currentSavableProp.nextbotAnimated = val;
    }

    private void UpdateNextbotImageSize(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isNextbot)
        {
            // Добавляем символ % к значению
            if (!string.IsNullOrEmpty(val) && !val.EndsWith("%"))
                currentSavableProp.nextbotImageSize = val + "%";
            else
                currentSavableProp.nextbotImageSize = val;
        }
    }

    private void UpdateMoverSpeed(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMover && float.TryParse(val, out float speed))
            currentSavableProp.moverSpeed = speed;
    }

    private void UpdateMoverPositionA(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMover && float.TryParse(val, out float pos))
            currentSavableProp.moverPositionA = pos;
    }

    private void UpdateMoverPositionB(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMover && float.TryParse(val, out float pos))
            currentSavableProp.moverPositionB = pos;
    }

    private void UpdateMoverDirection(int index)
    {
        if (currentSavableProp != null && currentSavableProp.isMover)
            currentSavableProp.moverDirection = moverDirectionDropdown.options[index].text;
    }

    private void UpdateMoverPropId(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMover &&
            float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float id))
            currentSavableProp.moverPropUniqueId = id;
    }

    private void UpdateLocalRotatorSpeed(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalRotator && float.TryParse(val, out float speed))
            currentSavableProp.localRotatorSpeed = speed;
    }

    private void UpdateLocalRotatorDegree(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalRotator && float.TryParse(val, out float degree))
            currentSavableProp.localRotatorDegree = degree;
    }

    private void UpdateLocalRotatorDirection(int index)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalRotator)
            currentSavableProp.localRotatorDirection = localRotatorDirectionDropdown.options[index].text;
    }

    private void UpdateLocalRotatorPropId(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalRotator &&
            float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float id))
            currentSavableProp.localRotatorPropUniqueId = id;
    }

    private void UpdateLocalMoverSpeed(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalMover && float.TryParse(val, out float speed))
            currentSavableProp.localMoverSpeed = speed;
    }

    private void UpdateLocalMoverRange(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalMover && float.TryParse(val, out float range))
            currentSavableProp.localMoverRange = range;
    }

    private void UpdateLocalMoverDirection(int index)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalMover)
            currentSavableProp.localMoverDirection = localMoverDirectionDropdown.options[index].text;
    }

    private void UpdateLocalMoverPropId(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isLocalMover &&
            float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float id))
            currentSavableProp.localMoverPropUniqueId = id;
    }

    private void UpdateTriggerSize(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isTrigger &&
            float.TryParse(val, out float size))
        {
            currentSavableProp.triggerSize = size;
        }
    }

    private void UpdateTriggerMode(int index)
    {
        if (currentSavableProp != null && currentSavableProp.isTrigger)
            currentSavableProp.triggerMode = triggerModeDropdown.options[index].text;
    }

    private void UpdateTriggerObjectName(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isTrigger)
            currentSavableProp.triggerObjectName = val;
    }

    private void UpdatePasswordPanelPassword(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isPasswordPanel)
            currentSavableProp.passwordPanelPassword = val;
    }

    private void UpdateMonitorText(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMonitor)
        {
            currentSavableProp.monitorText = val;
            currentSavableProp.UpdateVisual();
        }
    }

    private void UpdatePasswordPanelText(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isPasswordPanel)
        {
            currentSavableProp.passwordPanelPassword = val;
            currentSavableProp.UpdateVisual();
        }
    }

    private void UpdateMonitorColor(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMonitor)
        {
            currentSavableProp.monitorColor = val;
            currentSavableProp.UpdateVisual();
        }
    }

    private void UpdateMonitorControllerText(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMonitorController)
        {
            currentSavableProp.controllerText = val;
            currentSavableProp.UpdateVisual();
        }
    }

    private void UpdateMonitorControllerColor(string val)
    {
        if (currentSavableProp != null && currentSavableProp.isMonitorController)
        {
            currentSavableProp.controllerBackground = val;
            currentSavableProp.UpdateVisual();
        }
    }

    private void DeleteSelectedObjects()
    {
        if (rootsLink == null) return;
        foreach (var t in new List<Transform>(rootsLink))
            Destroy(t.gameObject);

        GetComponent<TransformGizmo>().ClearTargets(false);
    }
}