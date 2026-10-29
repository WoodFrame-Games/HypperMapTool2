using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SavableProp))]
public class SavablePropEditor : Editor
{
    private SerializedProperty prefabName;
    private SerializedProperty uniqueId;
    private SerializedProperty type;
    private SerializedProperty rb;

    private void OnEnable()
    {
        prefabName = serializedObject.FindProperty("prefabName");
        uniqueId = serializedObject.FindProperty("uniqueId");
        type = serializedObject.FindProperty("type");
        rb = serializedObject.FindProperty("rb");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSection("Basic");
        Draw("prefabName");
        Draw("uniqueId");
        Draw("type");
        Draw("rb");

        PropType propType = (PropType)type.enumValueIndex;

        switch (propType)
        {
            case PropType.Monitor:
                DrawSection("Monitor Data");
                Draw("monitorText");
                Draw("monitorColor");
                Draw("monitorTextUI");
                Draw("monitorImageUI");
                Draw("monitorConnected");
                break;

            case PropType.MonitorController:
                DrawSection("Monitor Controller Data");
                Draw("controllerText");
                Draw("controllerBackground");
                Draw("controllerConnected");
                break;

            case PropType.Button:
                DrawSection("Button Data");
                Draw("buttonToggle");
                Draw("buttonSignal");
                Draw("buttonConnected");
                break;

            case PropType.LogicGate:
                DrawSection("Logic Gate Data");
                Draw("logicGateMode");
                Draw("logicGateModeText");
                Draw("logicGateConnected");
                break;

            case PropType.Timer:
                DrawSection("Timer Data");
                Draw("timerTime");
                Draw("timerConnected");
                break;

            case PropType.Spawner:
                DrawSection("Spawner Data");
                Draw("spawnerPropName");
                Draw("spawnerKinematic");
                Draw("spawnerDistance");
                Draw("spawnerDeleteAfter");
                Draw("spawnerConnected");
                break;

            case PropType.Remover:
                DrawSection("Remover Data");
                Draw("removerDistance");
                Draw("removerConnected");
                break;

            case PropType.Repulsor:
                DrawSection("Repulsor Data");
                Draw("repulsorDistance");
                Draw("repulsorForce");
                Draw("repulsorConnected");
                break;

            case PropType.Nextbot:
                DrawSection("Nextbot Data");
                Draw("nextbotImageURL");
                Draw("nextbotAnimated");
                Draw("nextbotImageSize");
                Draw("nextbotConnected");
                break;

            case PropType.Mover:
                DrawSection("Mover Data");
                Draw("moverSpeed");
                Draw("moverPositionA");
                Draw("moverPositionB");
                Draw("moverDirection");
                Draw("moverPropUniqueId");
                Draw("moverPointAB");
                Draw("moverMoveProcess");
                Draw("moverConnected");
                break;

            case PropType.LocalRotator:
                DrawSection("Local Rotator Data");
                Draw("localRotatorSpeed");
                Draw("localRotatorDegree");
                Draw("localRotatorDirection");
                Draw("localRotatorPropUniqueId");
                Draw("localRotatorRotateProcess");
                Draw("localRotatorTempDegree");
                Draw("localRotatorConnected");
                break;

            case PropType.LocalMover:
                DrawSection("Local Mover Data");
                Draw("localMoverSpeed");
                Draw("localMoverRange");
                Draw("localMoverDirection");
                Draw("localMoverPropUniqueId");
                Draw("localMoverMoveProcess");
                Draw("localMoverFinalVector");
                Draw("localMoverConnected");
                break;

            case PropType.Trigger:
                DrawSection("Trigger Data");
                Draw("triggerSize");
                Draw("triggerMode");
                Draw("triggerObjectName");
                Draw("triggerConnected");
                break;

            case PropType.PasswordPanel:
                DrawSection("Password Panel Data");
                Draw("passwordPanelPassword");
                Draw("passwordText");
                Draw("passwordPanelConnected");
                break;

            case PropType.Lamp:
                DrawSection("Lamp Data");
                EditorGUILayout.HelpBox("Для этого типа дополнительных полей нет.", MessageType.Info);
                break;

            default:
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Тип \"Prop\" — стандартный объект без специфичных полей.", MessageType.Info);
                break;
        }

        DrawSection("Wire Rendering");
        Draw("wirePrefab");
        Draw("wireParent");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSection(string title)
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
    }

    private void Draw(string propertyName)
    {
        SerializedProperty prop = serializedObject.FindProperty(propertyName);
        if (prop == null)
            return;

        EditorGUILayout.PropertyField(prop, true);
    }
}
