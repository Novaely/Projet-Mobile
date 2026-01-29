using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DinoCreatorWizard : EditorWindow
{
    // --- Config Visuelle ---
    string dinoName = "New Dino";
    Sprite dinoSprite;
    Color spriteColor = Color.white;

    // --- Config Stats ---
    DietType diet = DietType.Herbivore;
    DinoColor dinoColorEnum = DinoColor.BleuPastel;
    string accessoryTag = "";

    // --- Config Règles ---
    List<SeatRuleSO> rules = new List<SeatRuleSO>();
    
    // --- Chemins de sauvegarde ---
    const string PROFILE_PATH = "Assets/_project/Resources/Dinos/Profiles/";
    const string PREFAB_PATH = "Assets/_project/Resources/Dinos/Prefabs/";

    [MenuItem("Tools/Dino Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<DinoCreatorWizard>("Dino Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Créateur de Dino", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 1. Visuel
        GUILayout.Label("1. Apparence", EditorStyles.boldLabel);
        dinoName = EditorGUILayout.TextField("Nom du Dino", dinoName);
        dinoSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", dinoSprite, typeof(Sprite), false);
        spriteColor = EditorGUILayout.ColorField("Teinte Sprite", spriteColor);

        EditorGUILayout.Space();

        // 2. Caractéristiques
        GUILayout.Label("2. Caractéristiques", EditorStyles.boldLabel);
        diet = (DietType)EditorGUILayout.EnumPopup("Régime", diet);
        dinoColorEnum = (DinoColor)EditorGUILayout.EnumPopup("Couleur (Jeu)", dinoColorEnum);
        accessoryTag = EditorGUILayout.TextField("Accessoire (Tag)", accessoryTag);

        EditorGUILayout.Space();

        // 3. Règles
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

        // BOUTON FINAL
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("CRÉER LE DINO (Prefab + Profil)", GUILayout.Height(40)))
        {
            CreateDino();
        }
        GUI.backgroundColor = Color.white;
    }

    void CreateDino()
    {
        if (string.IsNullOrEmpty(dinoName))
        {
            EditorUtility.DisplayDialog("Erreur", "Le dino doit avoir un nom !", "OK");
            return;
        }

        // Créer les dossiers si besoin
        EnsureDirectoryExists(PROFILE_PATH);
        EnsureDirectoryExists(PREFAB_PATH);

        // 1. Créer le Profil (SO)
        DinoProfileSO newProfile = CreateInstance<DinoProfileSO>();
        newProfile.speciesName = dinoName;
        newProfile.diet = diet;
        newProfile.accessoryTag = accessoryTag;
        
        // Nettoyer liste règles
        newProfile.myRules = new List<SeatRuleSO>();
        foreach (var r in rules) if (r != null) newProfile.myRules.Add(r);

        string profileAssetPath = PROFILE_PATH + dinoName + "_Profile.asset";
        profileAssetPath = AssetDatabase.GenerateUniqueAssetPath(profileAssetPath);
        
        AssetDatabase.CreateAsset(newProfile, profileAssetPath);

        // 2. Créer le GameObject temporaire dans la scène
        GameObject dinoGO = new GameObject(dinoName);
        
        // Sprite
        var sr = dinoGO.AddComponent<SpriteRenderer>();
        sr.sprite = dinoSprite;
        sr.color = spriteColor;
        sr.sortingOrder = 5; // Au dessus des sièges

        // Collider
        dinoGO.AddComponent<CircleCollider2D>(); // Ou BoxCollider2D selon pref

        // Scripts
        var dinoScript = dinoGO.AddComponent<Dino>();
        dinoScript.profile = newProfile;
        dinoScript.color = dinoColorEnum; // La couleur d'instance

        dinoGO.AddComponent<DinoController>(); // Script du collègue
        dinoGO.tag = "Dino";

        // 3. Sauvegarder en Prefab
        string prefabAssetPath = PREFAB_PATH + dinoName + ".prefab";
        prefabAssetPath = AssetDatabase.GenerateUniqueAssetPath(prefabAssetPath);

        PrefabUtility.SaveAsPrefabAsset(dinoGO, prefabAssetPath);

        // 4. Nettoyage
        DestroyImmediate(dinoGO); // On supprime celui de la scène
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Focus sur le fichier créé
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);

        Debug.Log($"<color=green>SUCCÈS : Dino '{dinoName}' créé !</color>\nProfil: {profileAssetPath}\nPrefab: {prefabAssetPath}");
    }

    void EnsureDirectoryExists(string path)
    {
        // Nettoie le path pour Directory.CreateDirectory
        string sysPath = Application.dataPath + path.Substring("Assets".Length);
        if (!Directory.Exists(sysPath))
        {
            Directory.CreateDirectory(sysPath);
        }
    }
}
