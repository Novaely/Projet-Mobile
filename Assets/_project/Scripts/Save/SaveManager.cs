using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save<T>(T data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static T Load<T>() where T : class
    {
        if (!File.Exists(SavePath))
            return null;

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<T>(json);
    }
}
