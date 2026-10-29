using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class Updater : MonoBehaviour
{
    public string htmlPageURL;
    public GameObject updatePanel;
    public Button updateButton;

    private string downloadURL;
    private string localVersion;

    void Start()
    {
        localVersion = Application.version;
        Debug.Log("Локальная версия: " + localVersion);
        StartCoroutine(CheckUpdateViaHtml());
    }

    IEnumerator CheckUpdateViaHtml()
    {
        string url = htmlPageURL + "?t=" + System.DateTime.Now.Ticks;
        Debug.Log("Загружаем URL: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки: " + request.error);
                yield break;
            }

            string html = request.downloadHandler.text;
            Debug.Log("Получен HTML: " + html);
            ParseSimpleHtml(html);
        }
    }

    private void ParseSimpleHtml(string html)
    {
        // Метод 1: Парсим из meta тегов
        string version = ExtractMetaTag(html, "version");
        string url = ExtractMetaTag(html, "download_url");

        Debug.Log($"Meta version: {version}, Meta URL: {url}");

        // Метод 2: Парсим из div
        if (string.IsNullOrEmpty(version))
            version = ExtractDivContent(html, "version");
        if (string.IsNullOrEmpty(url))
            url = ExtractDivContent(html, "download_url");

        Debug.Log($"Div version: {version}, Div URL: {url}");

        // Метод 3: Парсим из простого текста
        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(url))
            ParseFromPlainText(html, out version, out url);

        Debug.Log($"Final version: {version}, Final URL: {url}");

        if (!string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(url))
        {
            downloadURL = url;
            
            if (version != localVersion)
            {
                Debug.Log($"Требуется обновление: {version} (текущая: {localVersion})");
                ShowUpdatePanel();
            }
            else
            {
                Debug.Log("Версия актуальна: " + version);
            }
        }
        else
        {
            Debug.LogError("Не найдены данные в HTML. Полный HTML: " + html);
        }
    }

    private string ExtractMetaTag(string html, string name)
    {
        try
        {
            // Несколько вариантов паттернов для meta тегов
            string[] patterns = {
                $"<meta\\s+name\\s*=\\s*[\"']{name}[\"']\\s+content\\s*=\\s*[\"']([^\"']+)[\"']",
                $"<meta\\s+content\\s*=\\s*[\"']([^\"']+)[\"']\\s+name\\s*=\\s*[\"']{name}[\"']"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups[1].Value.Trim();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка парсинга meta: " + e.Message);
        }
        
        return null;
    }

    private string ExtractDivContent(string html, string id)
    {
        try
        {
            string pattern = $"<div\\s+id\\s*=\\s*[\"']{id}[\"'][^>]*>([^<]+)</div>";
            Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Groups[1].Value.Trim();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка парсинга div: " + e.Message);
        }
        
        return null;
    }

    private void ParseFromPlainText(string html, out string version, out string url)
    {
        version = null;
        url = null;try
        {
            // Ищем в формате VERSION:1.2.3
            Match versionMatch = Regex.Match(html, @"VERSION:([^\s]+)");
            if (versionMatch.Success)
                version = versionMatch.Groups[1].Value.Trim();

            // Ищем в формате URL:https://...
            Match urlMatch = Regex.Match(html, @"URL:([^\s]+)");
            if (urlMatch.Success)
                url = urlMatch.Groups[1].Value.Trim();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка парсинга plain text: " + e.Message);
        }
    }

    void ShowUpdatePanel()
    {
        if (updatePanel != null)
            updatePanel.SetActive(true);

        if (updateButton != null)
        {
            updateButton.onClick.RemoveAllListeners();
            updateButton.onClick.AddListener(() => Application.OpenURL(downloadURL));
        }
    }

    // Метод для принудительной проверки
    public void ForceCheck()
    {
        StartCoroutine(CheckUpdateViaHtml());
    }
}