using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class TestSceneGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Test Scene 2D")]
    public static void ShowWindow() => GetWindow<TestSceneGenerator>("Scene Generator 2D");

    private int gridWidth = 4;
    private int gridHeight = 3;
    private float spacing = 1.5f;
    private bool modifyCurrentScene = true;
    private string newSceneName = "TestScene_2D";
    private string emplacementLayerName = "Emplacement";

    private void OnGUI()
    {
        GUILayout.Label("Générateur Unity 6 (New Input)", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("1. Configuration Grille", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("Largeur", gridWidth);
        gridHeight = EditorGUILayout.IntField("Hauteur", gridHeight);
        spacing = EditorGUILayout.FloatField("Espacement", spacing);
        emplacementLayerName = EditorGUILayout.TextField("Layer 'Emplacement'", emplacementLayerName);

        GUILayout.Space(10);
        GUILayout.Label("2. Mode Génération", EditorStyles.boldLabel);
        modifyCurrentScene = GUILayout.Toggle(modifyCurrentScene, "✏️ Modifier scène actuelle");
        if (!modifyCurrentScene) newSceneName = EditorGUILayout.TextField("Nom Nouvelle Scène", newSceneName);

        GUILayout.Space(20);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🚀 GÉNÉRER", GUILayout.Height(40)))
        {
            if (modifyCurrentScene) GenerateInCurrentScene();
            else GenerateNewScene();
        }
        GUI.backgroundColor = Color.white;
    }

    private void GenerateInCurrentScene()
    {
        EditorSceneManager.SaveOpenScenes();
        ClearScene();
        GenerateLevel();
        Debug.Log("✅ Scène générée !");
    }

    private void GenerateNewScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GenerateLevel();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), $"Assets/Scenes/{newSceneName}.unity", true);
        Debug.Log($"✅ Nouvelle scène '{newSceneName}' créée !");
    }

    private void GenerateLevel()
    {
        SetupCameraAndManagers();
        Seat[,] grid = CreateGrid();
        ConnectNeighbors(grid);
        CreateTestDinos(); 
        CreateFullUI();    
    }

    private void SetupCameraAndManagers()
    {
        GameObject camGO = GameObject.Find("Main Camera");
        if (camGO == null) 
        { 
            camGO = new GameObject("Main Camera"); 
            camGO.AddComponent<Camera>().orthographic = true; 
            camGO.tag = "MainCamera"; 
        }
        Camera.main.orthographicSize = 5f;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        if (camGO.GetComponent<DragManager>() == null) camGO.AddComponent<DragManager>();
        
        if (!GameObject.Find("ConditionManager")) 
            new GameObject("ConditionManager").AddComponent<ConditionManager>();
        
        if (!GameObject.Find("GameManager")) 
            new GameObject("GameManager").AddComponent<LevelScorer>();
    }

    private Seat[,] CreateGrid()
    {
        GameObject gridRoot = new GameObject("GridRoot");
        Seat[,] seatGrid = new Seat[gridWidth, gridHeight];
        
        Sprite squareSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        float startX = -(gridWidth - 1) * spacing * 0.5f;
        float startY = -(gridHeight - 1) * spacing * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject seatGO = new GameObject($"Seat_{x}_{y}");
                seatGO.transform.parent = gridRoot.transform;
                seatGO.transform.position = new Vector3(startX + x * spacing, startY + y * spacing, 0);

                int layerIndex = LayerMask.NameToLayer(emplacementLayerName);
                if (layerIndex != -1) seatGO.layer = layerIndex;

                var sr = seatGO.AddComponent<SpriteRenderer>();
                sr.sprite = squareSprite;
                sr.color = new Color(1f, 1f, 1f, 0.3f); 

                seatGO.AddComponent<BoxCollider2D>();
                var seat = seatGO.AddComponent<Seat>();
                seatGO.AddComponent<SeatEvaluator>();

                if (x == 0 || x == gridWidth - 1) seat.seatType = SeatType.Fenetre;
                else if (x == gridWidth / 2) seat.seatType = SeatType.Couloir; 
                else seat.seatType = SeatType.Normal;

                seatGrid[x, y] = seat;
            }
        }
        return seatGrid;
    }

    private void ConnectNeighbors(Seat[,] seatGrid)
    {
        int w = seatGrid.GetLength(0);
        int h = seatGrid.GetLength(1);
        for (int x = 0; x < w; x++) for (int y = 0; y < h; y++)
        {
            var s = seatGrid[x, y];
            if (x > 0) s.left = seatGrid[x - 1, y];
            if (x < w - 1) s.right = seatGrid[x + 1, y];
            if (y > 0) s.back = seatGrid[x, y - 1];
            if (y < h - 1) s.front = seatGrid[x, y + 1];
        }
    }

    private void CreateFullUI()
    {
        if (GameObject.Find("EventSystem") == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>(); 
        }

        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("Canvas");
            var c = canvasGO.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject validateBtnGO = CreateButton("Btn_Valider", canvasGO.transform, new Vector2(0, -350), new Vector2(220, 60), Color.green);
        validateBtnGO.GetComponentInChildren<TextMeshProUGUI>().text = "VALIDER NIVEAU";
        Button realValidateButton = validateBtnGO.GetComponent<Button>();

        GameObject resultPanel = new GameObject("Panel_Resultat");
        resultPanel.transform.SetParent(canvasGO.transform, false);
        
        RectTransform panelRT = resultPanel.AddComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero; panelRT.anchorMax = Vector2.one; 
        panelRT.offsetMin = Vector2.zero; panelRT.offsetMax = Vector2.zero;
        Image panelImg = resultPanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.9f);

        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(resultPanel.transform, false);
        CreateText("Titre", "NIVEAU TERMINÉ", 50, contentGO.transform, new Vector2(0, 100));

        GameObject scoreFinalGO = CreateText("ScoreFinal", "Score: 0/0", 40, contentGO.transform, new Vector2(0, 0));
        TextMeshProUGUI scoreTextFinal = scoreFinalGO.GetComponent<TextMeshProUGUI>();

        GameObject starsContainer = new GameObject("StarsContainer");
        starsContainer.transform.SetParent(contentGO.transform, false);
        var hlg = starsContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.spacing = 30;
        starsContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
        starsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 100);

        Image[] starImages = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject s = new GameObject($"Star_Final_{i}");
            s.transform.SetParent(starsContainer.transform, false);
            Image img = s.AddComponent<Image>(); img.color = Color.yellow;
            starImages[i] = img;
        }

        /*LevelScorer scorer = Object.FindFirstObjectByType<LevelScorer>();
        if (scorer != null)
        {
            scorer.validateButton = realValidateButton;
            scorer.resultPanel = resultPanel;
            scorer.scoreText = scoreTextFinal;
            scorer.starImages = starImages;
            resultPanel.SetActive(false);
        }*/
    }

    private void CreateTestDinos()
    {
        Sprite s = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        CreateOneDino("Rex", DietType.Carnivore, DinoColor.RougeRose, new Vector3(-5, 0, 0), s);
        CreateOneDino("Diplo", DietType.Herbivore, DinoColor.BleuPastel, new Vector3(-5, 2, 0), s);
    }

    private void CreateOneDino(string name, DietType diet, DinoColor col, Vector3 pos, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite; sr.color = GetDinoColor(col);
        go.AddComponent<CircleCollider2D>();
        var dino = go.AddComponent<Dino>();
        DinoProfileSO profile = ScriptableObject.CreateInstance<DinoProfileSO>();
        profile.speciesName = name; profile.diet = diet;
        dino.profile = profile; dino.color = col;
    }

    private Color GetDinoColor(DinoColor c) => c switch {
        DinoColor.RougeRose => new Color(1f, 0.2f, 0.2f),
        DinoColor.BleuPastel => new Color(0.4f, 0.6f, 1f),
        _ => Color.white
    };

    private GameObject CreateButton(string name, Transform parent, Vector2 pos, Vector2 size, Color c)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>(); img.color = c;
        go.AddComponent<Button>();
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        CreateText("Text", "Button", 20, go.transform, Vector2.zero);
        return go;
    }

    private GameObject CreateText(string name, string content, int size, Transform parent, Vector2 pos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content; tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(400, 100);
        return go;
    }

    private void ClearScene()
    {
        GameObject[] roots = { GameObject.Find("GridRoot"), GameObject.Find("Canvas"), GameObject.Find("EventSystem"), GameObject.Find("GameManager"), GameObject.Find("ConditionManager") };
        foreach (var r in roots) if (r != null) Undo.DestroyObjectImmediate(r);
        Dino[] dinos = Object.FindObjectsByType<Dino>(FindObjectsSortMode.None);
        foreach (var d in dinos) if (d != null && d.gameObject != null) Undo.DestroyObjectImmediate(d.gameObject);
    }
}