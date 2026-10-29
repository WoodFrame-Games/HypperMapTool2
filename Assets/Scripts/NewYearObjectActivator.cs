using UnityEngine;

public class NewYearObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("NewYearObjectActivator: Target не назначен");
            return;
        }

        System.DateTime now = System.DateTime.Now;

        bool isNewYear =
            (now.Month == 12 && now.Day >= 30) ||
            (now.Month == 1 && now.Day <= 2);

        target.SetActive(isNewYear);
    }
}
