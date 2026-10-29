using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameVersionDisplay : MonoBehaviour
{
    public TMP_Text versionText;

    void Start()
    {
        if (versionText != null)
        {
            versionText.text = "Ver. " + Application.version;
        }
    }
}