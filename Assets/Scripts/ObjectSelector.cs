using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask selectableLayer;
    public Transform selectedObject;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, selectableLayer))
            {
                selectedObject = hit.transform;
            }
            else
            {
                selectedObject = null;
            }
        }
    }
}