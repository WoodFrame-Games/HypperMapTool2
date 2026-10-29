using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string Folder =
        Path.Combine(Application.persistentDataPath, "Saves");

    static SaveSystem()
    {
        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);
    }

    public static void SaveFile(string filename, string data)
    {
        string path = Path.Combine(Folder, filename + ".svn");
        File.WriteAllText(path, data);
        Debug.Log("[SaveSystem] Saved → " + path);
    }

    public static string LoadFile(string filename)
    {
        string path = Path.Combine(Folder, filename + ".svn");

        if (!File.Exists(path))
        {
            Debug.LogWarning("[SaveSystem] File missing: " + path);
            return null;
        }

        return File.ReadAllText(path);
    }

    public static void DeleteFile(string filename)
    {
        string path = Path.Combine(Folder, filename + ".svn");

        if (!File.Exists(path))
        {
            Debug.LogWarning("[SaveSystem] File missing: " + path);
            return;
        }

        File.Delete(path);
        Debug.Log("[SaveSystem] Deleted → " + path);
    }
}