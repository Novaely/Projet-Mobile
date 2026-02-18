using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DinoCreatorWizard : EditorWindow
{
    string dinoName = "New Dino";
    
    [Header("Sprites Dino")]
    Sprite passiveSprite; 
    Sprite idleSprite;    
    Sprite happySprite;  
    Sprite angrySprite;

    [Header("Bulles & Particules")]
    Sprite bubbleHappy;
    Sprite bubbleAngry;
    Sprite particleHappy;
    Sprite particleAngry;

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
        GUILayout.Label("🦖 CRÉATEUR DE DINO COMPLET", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // --- 1. VISUELS CORPS ---
        GUILayout.Label("1. Apparence du Dinosaure", EditorStyles.boldLabel);
        dinoName = EditorGUILayout.TextField("Nom du Dino", dinoName);
        passiveSprite = (Sprite)EditorGUILayout.ObjectField("Drag (Debout)", passiveSprite, typeof(Sprite), false);
        idleSprite = (Sprite)EditorGUILayout.ObjectField("Assis (Neutre)", idleSprite, typeof(Sprite), false);
        happySprite = (Sprite)EditorGUILayout.ObjectField("Visuel Content", happySprite, typeof(Sprite), false);
        angrySprite = (Sprite)EditorGUILayout.ObjectField("Visuel Pas Content", angrySprite, typeof(Sprite), false);

        EditorGUILayout.Space();

        // --- 2. FEEDBACKS (BULLLES & PARTICULES) ---
        GUILayout.Label("2. Feedbacks Visuels (UI & FX)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        bubbleHappy = (Sprite)EditorGUILayout.ObjectField("Bulle 😊", bubbleHappy, typeof(Sprite), false);
        bubbleAngry = (Sprite)EditorGUILayout.ObjectField("Bulle 😡", bubbleAngry, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        particleHappy = (Sprite)EditorGUILayout.ObjectField("Particule ✨", particleHappy, typeof(Sprite), false);
        particleAngry = (Sprite)EditorGUILayout.ObjectField("Particule 💢", particleAngry, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // --- 3. DATA ---
        GUILayout.Label("3. Game Design & Textes", EditorStyles.boldLabel);
        diet = (DietType)EditorGUILayout.EnumPopup("Régime", diet);
        accessoryTag = EditorGUILayout.TextField("Accessoire (Tag)", accessoryTag);
        
        GUILayout.Label("Texte VERT (J'aime) :", EditorStyles.miniLabel);
        positiveCond = EditorGUILayout.TextArea(positiveCond, GUILayout.Height(30));
        GUILayout.Label("Texte ROUGE (Je déteste) :", EditorStyles.miniLabel);
        negativeCond = EditorGUILayout.TextArea(negativeCond, GUILayout.Height(30));

        EditorGUILayout.Space();

        // --- 4. RÈGLES ---
        GUILayout.Label("4. Règles de Satisfaction", EditorStyles.boldLabel);
        for (int i = 0; i < rules.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            rules[i] = (SeatRuleSO)EditorGUILayout.ObjectField($"Règle {i+1}", rules[i], typeof(SeatRuleSO), false);
            if (GUILayout.Button("X", GUILayout.Width(20))) rules.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Ajouter une Règle")) rules.Add(null);

        EditorGUILayout.Space(20);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("GÉNÉRER CE PUTAIN DE DINO COMPLET", GUILayout.Height(45)))
        {
            CreateDino();
        }
        GUI.backgroundColor = Color.white;
    }

    void CreateDino()
    {
        if (string.IsNullOrEmpty(dinoName)) {
            EditorUtility.DisplayDialog("Erreur", "Donne un nom au Dino !", "OK");
            return;
        }

        EnsureDirectoryExists(PROFILE_PATH);
        EnsureDirectoryExists(PREFAB_PATH);

        // --- SCRIPTABLE OBJECT ---
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

        // --- GAMEOBJECT ---
        GameObject dinoGO = new GameObject(dinoName);
        dinoGO.layer = LayerMask.NameToLayer("Dino");

        var sr = dinoGO.AddComponent<SpriteRenderer>();
        sr.sprite = passiveSprite;
        sr.sortingOrder = 5;

        dinoGO.AddComponent<CircleCollider2D>().radius = 0.5f;

        // --- SCRIPT DINO & ASSIGNATION ---
        var dinoScript = dinoGO.AddComponent<Dino>();
        dinoScript.profile = newProfile;
        
        // Assignation des sprites de corps
        dinoScript.passiveSprite = passiveSprite;
        dinoScript.idleSprite = idleSprite;

        // Assignation des sprites de bulles
        dinoScript.bubbleHappySprite = bubbleHappy;
        dinoScript.bubbleAngrySprite = bubbleAngry;

        // --- CRÉATION DE LA BULLE (ENFANT) ---
        GameObject bubbleGO = new GameObject("Bubble");
        bubbleGO.transform.SetParent(dinoGO.transform);
        bubbleGO.transform.localPosition = new Vector3(0.7f, 0.8f, 0);
        var bubbleSR = bubbleGO.AddComponent<SpriteRenderer>();
        bubbleSR.sortingOrder = 10;
        dinoScript.bubbleRenderer = bubbleSR; // On lie le Renderer au script

        // --- TENTATIVE DE RÉCUPÉRATION DU PARTICLE SYSTEM ---
        // On cherche si un ParticleSystem existe déjà dans tes prefabs ou on en crée un vide
        // Ici on suppose que tu as un ParticleSystem sur le Dino ou un enfant
        // Pour faire simple, on ajoute un ParticleSystem de base s'il n'existe pas
        var ps = dinoGO.GetComponentInChildren<ParticleSystem>();
        if(ps == null) {
            GameObject psGO = new GameObject("FeedbackParticles");
            psGO.transform.SetParent(dinoGO.transform);
            psGO.transform.localPosition = Vector3.zero;
            ps = psGO.AddComponent<ParticleSystem>();
            // Config de base rapide pour que ça ne spam pas au lancement
            var main = ps.main;
            main.playOnAwake = false;
        }
        dinoScript.feedbackParticles = ps;

        // --- SAUVEGARDE ---
        string prefabAssetPath = AssetDatabase.GenerateUniqueAssetPath(PREFAB_PATH + dinoName + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(dinoGO, prefabAssetPath);

        DestroyImmediate(dinoGO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
        Debug.Log($"<color=green>Dino {dinoName} créé avec tous ses sprites de bulles et particules !</color>");
    }

    void EnsureDirectoryExists(string path)
    {
        string sysPath = Application.dataPath + path.Substring("Assets".Length);
        if (!Directory.Exists(sysPath)) Directory.CreateDirectory(sysPath);
    }
}