using UnityEngine;

public class RuntimeGizmo : MonoBehaviour
{
    public float handleSize = 1.0f;
    public float moveSpeed = 10f;

    private bool draggingX, draggingY, draggingZ;
    private Camera cam;
    private ObjectSelector selector;

    void Start()
    {
        cam = Camera.main;
        selector = GetComponent<ObjectSelector>();
    }

    void OnGUI()
    {
        if (selector.selectedObject == null) return;

        Vector3 pos = selector.selectedObject.position;
        Vector3 screenPos = cam.WorldToScreenPoint(pos);
        screenPos.y = Screen.height - screenPos.y;

        float size = handleSize * (Vector3.Distance(cam.transform.position, pos) / 10f);

        // X
        if (GUI.Button(new Rect(screenPos.x + 40, screenPos.y - 10, 20, 20), "X"))
            draggingX = true;
        // Y
        if (GUI.Button(new Rect(screenPos.x, screenPos.y - 50, 20, 20), "Y"))
            draggingY = true;
        // Z
        if (GUI.Button(new Rect(screenPos.x - 40, screenPos.y - 10, 20, 20), "Z"))
            draggingZ = true;
    }

    void Update()
    {
        if (selector.selectedObject == null) return;

        Vector3 dir = Vector3.zero;
        if (draggingX) dir = Vector3.right;
        if (draggingY) dir = Vector3.up;
        if (draggingZ) dir = Vector3.forward;

        if (Input.GetMouseButton(0) && (draggingX || draggingY || draggingZ))
        {
            selector.selectedObject.position += dir * moveSpeed * Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            draggingX = draggingY = draggingZ = false;
        }
    }
}