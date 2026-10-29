using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class ScreenshotByF7 : MonoBehaviour
{

    public GameObject popupObject;           // объект, который включится на 3 секунды
    public float popupTime = 3f;             // длительность отображения

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string folderPath = Path.Combine(picturesPath, "HypperMapTool", "scrn");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = "scrn_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);



        if (popupObject != null)
            StartCoroutine(ShowPopup());
    }

    IEnumerator ShowPopup()
    {
        popupObject.SetActive(true);

        yield return new WaitForSeconds(popupTime);

        popupObject.SetActive(false);
    }
}