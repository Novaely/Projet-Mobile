#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

public static class DicoLocalizationSync
{
    [MenuItem("Tools/Sync DicoDino Localization")]
    public static void Sync()
    {
        var database = AssetDatabase.LoadAssetAtPath<DicoDinoDatabase>("Assets/_project/Datas/DicoDino.asset");

        if (database == null)
        {
            Debug.LogError("Database not found.");
            return;
        }

        var collection = LocalizationEditorSettings.GetStringTableCollection("DicoDino");

        if (collection == null)
        {
            Debug.LogError("StringTableCollection 'DicoDino' not found.");
            return;
        }

        var sharedData = collection.SharedData;
        sharedData.Clear();

        var frTable = collection.GetTable("fr") as StringTable;
        var enTable = collection.GetTable("en") as StringTable;

        frTable.Clear();
        enTable.Clear();

        foreach (var dino in database.Dictionnary)
        {
            var key = dino.DinoWord;

            sharedData.AddKey(key);
            frTable.AddEntry(key, dino.French);
            enTable.AddEntry(key, dino.English);
        }

        EditorUtility.SetDirty(frTable);
        EditorUtility.SetDirty(enTable);
        AssetDatabase.SaveAssets();

        Debug.Log("DicoDino synced successfully.");
    }
}
#endif