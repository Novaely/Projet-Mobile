using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DinoCreatorWizard : EditorWindow
{
    string dinoName = "New Dino";
    
    Sprite passiveSprite; 
    Sprite idleSprite;    

    DietType diet = DietType.Herbivore;
    string accessoryTag = "";
    
    string positiveCond = "J'aime...";
    string negativeCond = "Je déteste...";

    List<SeatRuleSO> rules = new List<SeatRuleSO>();
    
    const string PROFILE_PATH = "Assets/_project/Datas/dino_profiles/";
    const string PREFAB_PATH = "Assets/_project/Prefabs/Dino/";

    [MenuItem("Tools/Dino Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<DinoCreatorWizard>("Dino Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("🦖 Créateur de Dino (V2 - Descriptions Séparées)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("1. Apparence Corps", EditorStyles.boldLabel);
        dinoName = EditorGUILayout.TextField("Nom du Dino", dinoName);
        
        passiveSprite = (Sprite)EditorGUILayout.ObjectField("Sprite Passif (Debout/Drag)", passiveSprite, typeof(Sprite), false);
        idleSprite = (Sprite)EditorGUILayout.ObjectField("Sprite Actif (Assis/Neutre)", idleSprite, typeof(Sprite), false);

        EditorGUILayout.Space();

        GUILayout.Label("2. Caractéristiques & GD", EditorStyles.boldLabel);
        diet = (DietType)EditorGUILayout.EnumPopup("Régime", diet);
        accessoryTag = EditorGUILayout.TextField("Accessoire (Tag)", accessoryTag);
        
        GUILayout.Space(5);
        
        GUILayout.Label("Ce qu'il aime (Affiché en VERT) :", EditorStyles.label);
        positiveCond = EditorGUILayout.TextArea(positiveCond, GUILayout.Height(40));
        
        GUILayout.Label("Ce qu'il déteste (Affiché en ROUGE) :", EditorStyles.label);
        negativeCond = EditorGUILayout.TextArea(negativeCond, GUILayout.Height(40));

        EditorGUILayout.Space();

        GUILayout.Label("3. Contraintes & Règles", EditorStyles.boldLabel);
        
        for (int i = 0; i < rules.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            rules[i] = (SeatRuleSO)EditorGUILayout.ObjectField($"Règle {i+1}", rules[i], typeof(SeatRuleSO), false);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                rules.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Ajouter une Règle"))
        {
            rules.Add(null);
        }

        EditorGUILayout.Space(20);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(" CRÉER CE PUTAIN DE DINO ET SON PROFIL DE CON ", GUILayout.Height(40)))
        {
            CreateDino();
        }
        GUI.backgroundColor = Color.white;
    }

    void CreateDino()
    {
        if (string.IsNullOrEmpty(dinoName))
        {
            EditorUtility.DisplayDialog("Erreur", "Manque un nom !", "OK");
            return;
        }

        EnsureDirectoryExists(PROFILE_PATH);
        EnsureDirectoryExists(PREFAB_PATH);

        DinoProfileSO newProfile = CreateInstance<DinoProfileSO>();
        newProfile.speciesName = dinoName;
        newProfile.diet = diet;
        newProfile.accessoryTag = accessoryTag;
        
        newProfile.positiveCondition = positiveCond;
        newProfile.negativeCondition = negativeCond;
        
        newProfile.myRules = new List<SeatRuleSO>();
        foreach (var r in rules) if (r != null) newProfile.myRules.Add(r);

        string profileAssetPath = AssetDatabase.GenerateUniqueAssetPath(PROFILE_PATH + dinoName + "_Profile.asset");
        AssetDatabase.CreateAsset(newProfile, profileAssetPath);

        GameObject dinoGO = new GameObject(dinoName);
        
        int dinoLayer = LayerMask.NameToLayer("Dino");
        if (dinoLayer != -1) dinoGO.layer = dinoLayer;

        var sr = dinoGO.AddComponent<SpriteRenderer>();
        sr.sprite = passiveSprite != null ? passiveSprite : idleSprite;
        sr.sortingOrder = 5;

        dinoGO.AddComponent<CircleCollider2D>(); 

        var dinoScript = dinoGO.AddComponent<Dino>();
        dinoScript.profile = newProfile;
        dinoScript.tag = "Dino"; 

        dinoScript.passiveSprite = passiveSprite;
        dinoScript.idleSprite = idleSprite;

        GameObject bubbleGO = new GameObject("EmoteBubble");
        bubbleGO.transform.SetParent(dinoGO.transform);
        bubbleGO.transform.localPosition = new Vector3(0, 1.2f, 0); 
        bubbleGO.transform.localScale = Vector3.zero; 

        var bubbleSR = bubbleGO.AddComponent<SpriteRenderer>();
        bubbleSR.sortingOrder = 10; 
        dinoScript.emoteRenderer = bubbleSR;

        string prefabAssetPath = AssetDatabase.GenerateUniqueAssetPath(PREFAB_PATH + dinoName + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(dinoGO, prefabAssetPath);

        DestroyImmediate(dinoGO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);

        Debug.Log($"<color=green>SUCCÈS : Dino '{dinoName}' créé avec ses conditions Positives/Négatives !</color>");
    }

    void EnsureDirectoryExists(string path)
    {
        string sysPath = Application.dataPath + path.Substring("Assets".Length);
        if (!Directory.Exists(sysPath))
        {
            Directory.CreateDirectory(sysPath);
        }
    }
}