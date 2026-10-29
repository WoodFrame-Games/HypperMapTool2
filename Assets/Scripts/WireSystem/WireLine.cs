using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireLine : MonoBehaviour
{
    private Transform from;
    private Transform to;
    private Vector3? overrideEnd;

    public LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    public void Init(Transform from, Transform to)
    {
        this.from = from;
        this.to = to;
        overrideEnd = null;
        UpdateLine();
    }

    public void SetEnd(Vector3 point)
    {
        overrideEnd = point;
        UpdateLine();
    }

    private void LateUpdate()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        if (from == null || (to == null && overrideEnd == null))
        {
            Destroy(gameObject);
            return;
        }

        Vector3 end = overrideEnd ?? to.position;
        lr.SetPosition(0, from.position);
        lr.SetPosition(1, end);
    }
}
