using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class TestSceneGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Test Scene 2D")]
    public static void ShowWindow()
    {
        GetWindow<TestSceneGenerator>("Scene Generator 2D");
    }

    private int gridWidth = 4;
    private int gridHeight = 3;
    private float spacing = 1.5f;

    private void OnGUI()
    {
        GUILayout.Label("Configuration Scène 2D", EditorStyles.boldLabel);
        
        gridWidth = EditorGUILayout.IntField("Largeur Grille", gridWidth);
        gridHeight = EditorGUILayout.IntField("Hauteur Grille", gridHeight);
        spacing = EditorGUILayout.FloatField("Espacement", spacing);

        if (GUILayout.Button("Générer Scène 2D"))
        {
            GenerateScene();
        }
    }

    private void GenerateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // --- CAMÉRA & MANAGER ---
        GameObject camGO = new GameObject("Main Camera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3((gridWidth * spacing) / 2f - 0.5f, (gridHeight * spacing) / 2f - 0.5f, -10f);
        camGO.tag = "MainCamera";

        // Ajout du DragManager sur la caméra (ou un objet manager)
        var dragManager = camGO.AddComponent<DragManager>();

        GameObject managerGO = new GameObject("ConditionManager");
        var condManager = managerGO.AddComponent<ConditionManager>();
        
        // Lier le ConditionManager au DragManager automatiquement (via sérialisation privée c'est dur en éditeur, mais on tente)
        // Note: Le DragManager fait un FindObjectOfType au Start, donc ça marchera même sans lien direct ici.

        // --- GRILLE ---
        GameObject gridRoot = new GameObject("GridRoot");
        Seat[,] seatGrid = new Seat[gridWidth, gridHeight];
        Sprite squareSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject seatGO = new GameObject($"Seat_{x}_{y}");
                seatGO.transform.parent = gridRoot.transform;
                seatGO.transform.position = new Vector3(x * spacing, y * spacing, 0);
                
                var sr = seatGO.AddComponent<SpriteRenderer>();
                sr.sprite = squareSprite;
                sr.color = new Color(1f, 1f, 1f, 0.3f);
                sr.sortingOrder = 0;

                seatGO.AddComponent<BoxCollider2D>();
                
                // AJOUT DES DEUX SCRIPTS (Toi + Collègue)
                var seat = seatGO.AddComponent<Seat>();
                seatGO.AddComponent<EmplacementController>(); // <-- AJOUT DU SCRIPT DU COLLÈGUE
                
                if (x == 0 || x == gridWidth - 1) seat.seatType = SeatType.Fenetre;
                else seat.seatType = SeatType.Normal;

                seatGrid[x, y] = seat;
            }
        }

        // Connexions Voisins
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var seat = seatGrid[x, y];
                if (x > 0) seat.left = seatGrid[x - 1, y];
                if (x < gridWidth - 1) seat.right = seatGrid[x + 1, y];
                if (y > 0) seat.back = seatGrid[x, y - 1];
                if (y < gridHeight - 1) seat.front = seatGrid[x, y + 1];
            }
        }

        // --- DINOS ---
        CreateTestDino2D("Rex_Test", DietType.Carnivore, DinoColor.RougeRose, new Vector3(-2, 0, 0), squareSprite);
        CreateTestDino2D("Diplo_Test", DietType.Herbivore, DinoColor.BleuPastel, new Vector3(-2, 1.5f, 0), squareSprite);
        CreateTestDino2D("Stego_Test", DietType.Herbivore, DinoColor.Turquoise, new Vector3(-2, 3, 0), squareSprite);

        Debug.Log("Scène 2D générée avec succès (Scripts fusionnés) !");
    }

    private void CreateTestDino2D(string name, DietType diet, DinoColor color, Vector3 pos, Sprite sprite)
    {
        GameObject dinoGO = new GameObject(name);
        dinoGO.transform.position = pos;
        dinoGO.tag = "Dino"; // Important pour certains raycasts
        
        var sr = dinoGO.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = GetColorFromEnum(color);
        sr.sortingOrder = 1;

        dinoGO.AddComponent<CircleCollider2D>();

        // AJOUT DES DEUX SCRIPTS (Toi + Collègue)
        var dino = dinoGO.AddComponent<Dino>();
        dinoGO.AddComponent<DinoController>(); // <-- AJOUT DU SCRIPT DU COLLÈGUE
        
        dino.color = color; 

        // Profil Temporaire
        DinoProfileSO tempProfile = ScriptableObject.CreateInstance<DinoProfileSO>();
        tempProfile.speciesName = name;
        tempProfile.diet = diet;
        
        dino.profile = tempProfile;
    }

    private Color GetColorFromEnum(DinoColor color)
    {
        switch (color)
        {
            case DinoColor.BleuPastel: return new Color(0.67f, 0.77f, 0.91f);
            case DinoColor.RougeRose: return new Color(1f, 0.01f, 0.24f);
            case DinoColor.Turquoise: return new Color(0.4f, 0.86f, 0.71f);
            case DinoColor.Violet: return new Color(0.87f, 0.45f, 1f);
            default: return Color.white;
        }
    }
}
