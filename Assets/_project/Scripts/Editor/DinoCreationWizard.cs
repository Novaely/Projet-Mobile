using UnityEngine;
using UnityEditor;

public class DinoCreationWizard : EditorWindow
{
    // --- DONNÉES ---
    string dinoName = "Nouveau Dino";
    DinoProfileSO profile;

    // --- VISUELS DINO ---
    Sprite passiveSprite;
    Sprite idleSprite;
    Sprite happySprite;
    Sprite angrySprite;

    // --- VISUELS PARTICULES ---
    Sprite particleHappy; // Cœur
    Sprite particleAngry; // Éclair

    // --- VISUELS BULLE FIXE ---
    Sprite bubbleHappy;
    Sprite bubbleAngry;

    [MenuItem("Tools/Dino Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<DinoCreationWizard>("Dino Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("CONFIGURATION DINO", EditorStyles.boldLabel);
        dinoName = EditorGUILayout.TextField("Nom du GameObject", dinoName);
        profile = (DinoProfileSO)EditorGUILayout.ObjectField("Profil (SO)", profile, typeof(DinoProfileSO), false);

        EditorGUILayout.Space();
        GUILayout.Label("SPRITES DINO", EditorStyles.boldLabel);
        passiveSprite = (Sprite)EditorGUILayout.ObjectField("Passive (Drag)", passiveSprite, typeof(Sprite), false);
        idleSprite = (Sprite)EditorGUILayout.ObjectField("Idle (Assis)", idleSprite, typeof(Sprite), false);
        happySprite = (Sprite)EditorGUILayout.ObjectField("Happy (Content)", happySprite, typeof(Sprite), false);
        angrySprite = (Sprite)EditorGUILayout.ObjectField("Angry (Énervé)", angrySprite, typeof(Sprite), false);

        EditorGUILayout.Space();
        GUILayout.Label("SPRITES FEEDBACK (Particules)", EditorStyles.boldLabel);
        particleHappy = (Sprite)EditorGUILayout.ObjectField("Particule Content (Cœur)", particleHappy, typeof(Sprite), false);
        particleAngry = (Sprite)EditorGUILayout.ObjectField("Particule Énervé (Éclair)", particleAngry, typeof(Sprite), false);

        EditorGUILayout.Space();
        GUILayout.Label("SPRITES BULLE (Persistante)", EditorStyles.boldLabel);
        bubbleHappy = (Sprite)EditorGUILayout.ObjectField("Bulle Content", bubbleHappy, typeof(Sprite), false);
        bubbleAngry = (Sprite)EditorGUILayout.ObjectField("Bulle Énervé", bubbleAngry, typeof(Sprite), false);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("GRÉER / METTRE À JOUR LE DINO", GUILayout.Height(40)))
        {
            CreateOrUpdateDino();
        }
    }

    void CreateOrUpdateDino()
    {
        GameObject dinoObj;

        // 1. Si on a sélectionné un Dino, on le met à jour, sinon on en crée un
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Dino>() != null)
        {
            dinoObj = Selection.activeGameObject;
            Undo.RecordObject(dinoObj, "Update Dino");
        }
        else
        {
            dinoObj = new GameObject(dinoName);
            Undo.RegisterCreatedObjectUndo(dinoObj, "Create Dino");
            dinoObj.AddComponent<SpriteRenderer>();
            dinoObj.AddComponent<Dino>();
            // Ajouter un BoxCollider2D si besoin pour le clic
            dinoObj.AddComponent<BoxCollider2D>(); 
        }

        Dino dinoScript = dinoObj.GetComponent<Dino>();
        SpriteRenderer mainRenderer = dinoObj.GetComponent<SpriteRenderer>();

        // --- CONFIGURATION PRINCIPALE ---
        dinoScript.profile = profile;
        dinoScript.passiveSprite = passiveSprite;
        dinoScript.idleSprite = idleSprite;
        dinoScript.happySprite = happySprite;
        dinoScript.angrySprite = angrySprite;
        dinoScript.particleHappy = particleHappy;
        dinoScript.particleAngry = particleAngry;
        dinoScript.bubbleHappySprite = bubbleHappy;
        dinoScript.bubbleAngrySprite = bubbleAngry;

        // Assigner le sprite par défaut
        if (passiveSprite != null) mainRenderer.sprite = passiveSprite;
        mainRenderer.sortingOrder = 10; // Valeur par défaut pour être devant le siège

        // --- 2. GESTION DES PARTICULES (Enfant) ---
        Transform particlesTransform = dinoObj.transform.Find("FeedbackParticles");
        GameObject particlesObj;

        if (particlesTransform == null)
        {
            particlesObj = new GameObject("FeedbackParticles");
            particlesObj.transform.SetParent(dinoObj.transform);
            particlesObj.transform.localPosition = new Vector3(0, 1.2f, 0); // Position au-dessus de la tête
        }
        else
        {
            particlesObj = particlesTransform.gameObject;
        }

        // Configuration du Particle System
        ParticleSystem ps = particlesObj.GetComponent<ParticleSystem>();
        if (ps == null) ps = particlesObj.AddComponent<ParticleSystem>();
        
        // Configuration automatique des modules pour l'effet "POUF"
        var main = ps.main;
        main.loop = true; // On boucle (géré par le script)
        main.duration = 2f;
        main.startLifetime = 1.2f;
        main.startSpeed = 2f;
        main.startSize = 0.5f;
        
        var emission = ps.emission;
        emission.rateOverTime = 0f; // Pas de flux continu
        // On configure le Burst
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 3) }); // 3 particules d'un coup

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 0.2f;

        var texSheet = ps.textureSheetAnimation;
        texSheet.enabled = true;
        texSheet.mode = ParticleSystemAnimationMode.Sprites;
        
        // IMPORTANT : Le Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default")); // Matériel par défaut pour éviter le rose
        renderer.sortingOrder = 20; // Devant le dino

        // Lier au script
        dinoScript.feedbackParticles = ps;


        // --- 3. GESTION DE LA BULLE PERSISTANTE (Enfant) ---
        Transform bubbleTransform = dinoObj.transform.Find("PersistentBubble");
        GameObject bubbleObj;

        if (bubbleTransform == null)
        {
            bubbleObj = new GameObject("PersistentBubble");
            bubbleObj.transform.SetParent(dinoObj.transform);
            bubbleObj.transform.localPosition = new Vector3(0, 1.5f, 0); // Un peu plus haut que les particules
        }
        else
        {
            bubbleObj = bubbleTransform.gameObject;
        }

        SpriteRenderer bubbleSr = bubbleObj.GetComponent<SpriteRenderer>();
        if (bubbleSr == null) bubbleSr = bubbleObj.AddComponent<SpriteRenderer>();

        bubbleSr.sortingOrder = 21; // Devant tout le monde
        bubbleObj.SetActive(false); // Caché par défaut

        // Lier au script
        dinoScript.bubbleRenderer = bubbleSr;

        Debug.Log($"Dino '{dinoObj.name}' configuré avec succès !");
        Selection.activeGameObject = dinoObj;
    }
}