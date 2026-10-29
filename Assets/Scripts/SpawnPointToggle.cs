using UnityEngine;
using UnityEngine.UI;

public class SpawnPointToggle : MonoBehaviour
{
    [SerializeField] private Toggle targetToggle;
    [SerializeField] private GameObject target;

    private void Start()
    {
        if (targetToggle != null)
            targetToggle.onValueChanged.AddListener(OnToggleChanged);

        if (target != null)
            target.SetActive(targetToggle != null && targetToggle.isOn);
    }

    private void OnToggleChanged(bool value)
    {
        if (target != null)
            target.SetActive(value);
    }
}
