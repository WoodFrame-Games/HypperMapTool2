using UnityEngine;
using Newtonsoft.Json;

public static class SaveLoader
{
    public static SerializableLevel Load(string path)
    {
        string json = SaveSystem.LoadFile(path);
        if (string.IsNullOrEmpty(json)) return null;

        return JsonConvert.DeserializeObject<SerializableLevel>(json);
    }
}