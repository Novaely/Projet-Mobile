using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

public class RuleCreatorWizard : EditorWindow
{
    private string ruleFileName = "NouvelleRegle";
    
    private Type[] availableRuleTypes;
    private string[] ruleTypeNames;
    private int selectedIndex = 0;

    private SeatRuleSO tempRuleInstance;
    private Editor tempRuleEditor;

    const string RULES_PATH = "Assets/_project/Datas/dino_condition/";

    [MenuItem("Tools/Rule Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<RuleCreatorWizard>("Créateur de Règles");
    }

    private void OnEnable()
    {
        availableRuleTypes = TypeCache.GetTypesDerivedFrom<SeatRuleSO>()
            .Where(t => !t.IsAbstract)
            .ToArray();

        ruleTypeNames = availableRuleTypes.Select(t => t.Name).ToArray();

        CreateTempInstance();
    }

    private void OnDisable()
    {
        if (tempRuleInstance != null && !AssetDatabase.Contains(tempRuleInstance)) 
            DestroyImmediate(tempRuleInstance);
            
        if (tempRuleEditor != null) 
            DestroyImmediate(tempRuleEditor);
    }

    private void CreateTempInstance()
    {
        if (tempRuleInstance != null && !AssetDatabase.Contains(tempRuleInstance)) 
            DestroyImmediate(tempRuleInstance);
            
        if (tempRuleEditor != null) 
            DestroyImmediate(tempRuleEditor);

        if (availableRuleTypes != null && availableRuleTypes.Length > 0)
        {
            tempRuleInstance = (SeatRuleSO)CreateInstance(availableRuleTypes[selectedIndex]);
            
            tempRuleEditor = Editor.CreateEditor(tempRuleInstance);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("🛠️ Générateur Automatique de Règles", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (availableRuleTypes == null || availableRuleTypes.Length == 0)
        {
            EditorGUILayout.HelpBox("Aucun script de règle trouvé ! Vérifiez qu'ils héritent bien de SeatRuleSO.", MessageType.Warning);
            return;
        }

        GUILayout.Label("1. Informations Générales", EditorStyles.boldLabel);
        ruleFileName = EditorGUILayout.TextField("Nom du fichier (Asset)", ruleFileName);

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("Type de Règle", selectedIndex, ruleTypeNames);
        if (EditorGUI.EndChangeCheck())
        {
            CreateTempInstance();
        }

        EditorGUILayout.Space();
        GUILayout.Label("2. Paramètres de la Règle (Générés Auto)", EditorStyles.boldLabel);

        if (tempRuleEditor != null)
        {
            EditorGUILayout.BeginVertical("box");
            tempRuleEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(20);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("CRÉER LA RÈGLE DANS LE PROJET", GUILayout.Height(40)))
        {
            SaveRule();
        }
        GUI.backgroundColor = Color.white;
    }

    private void SaveRule()
    {
        if (string.IsNullOrEmpty(ruleFileName))
        {
            EditorUtility.DisplayDialog("Erreur", "Le fichier doit avoir un nom !", "OK");
            return;
        }

        EnsureDirectoryExists(RULES_PATH);
        
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(RULES_PATH + ruleFileName + ".asset");

        AssetDatabase.CreateAsset(tempRuleInstance, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tempRuleInstance;

        Debug.Log($"<color=cyan>SUCCÈS : Règle '{ruleFileName}' ({availableRuleTypes[selectedIndex].Name}) créée !</color>");

        CreateTempInstance();
    }

    private void EnsureDirectoryExists(string path)
    {
        string sysPath = Application.dataPath + path.Substring("Assets".Length);
        if (!Directory.Exists(sysPath))
        {
            Directory.CreateDirectory(sysPath);
        }
    }
}