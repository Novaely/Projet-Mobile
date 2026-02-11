using UnityEngine;
using UnityEditor;
using System.IO;

public class RuleCreatorWizard : EditorWindow
{
    public enum RuleType
    {
        IncompatibleNeighbor,
        MustBeNeighborDiet,
        NoSpecificNeighborDiet,
        SeatTypeRestriction,
        MustBeEmptyDirection,
        ForbiddenSeatType, 
        ForbiddenEdge      
    }

    [Header("Configuration")]
    private string ruleFileName = "NouvelleRegle";
    private RuleType selectedRuleType = RuleType.IncompatibleNeighbor;

    [Header("Paramètres selon la règle")]
    private string hatedDinoName = "T-Rex"; 
    private DietType targetDiet = DietType.Herbivore; 
    private SeatType targetSeatType = SeatType.Fenetre; 
    private NeighborDirection targetDirection = NeighborDirection.Derriere; 

    const string RULES_PATH = "Assets/_project/Resources/Rules/";

    [MenuItem("Tools/Rule Creator Wizard")]
    public static void ShowWindow()
    {
        GetWindow<RuleCreatorWizard>("Créateur de Règles");
    }

    private void OnGUI()
    {
        GUILayout.Label("🛠️ Générateur de Règles pour le GD", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("1. Informations Générales", EditorStyles.boldLabel);
        ruleFileName = EditorGUILayout.TextField("Nom du fichier (Asset)", ruleFileName);
        selectedRuleType = (RuleType)EditorGUILayout.EnumPopup("Type de Règle", selectedRuleType);

        EditorGUILayout.Space();
        GUILayout.Label("2. Paramètres de la Règle", EditorStyles.boldLabel);

        switch (selectedRuleType)
        {
            case RuleType.IncompatibleNeighbor:
                EditorGUILayout.HelpBox("Le dino refusera d'être à côté de l'espèce indiquée ci-dessous.", MessageType.Info);
                hatedDinoName = EditorGUILayout.TextField("Espèce détestée (Nom)", hatedDinoName);
                break;

            case RuleType.MustBeNeighborDiet:
                EditorGUILayout.HelpBox("Le dino DOIT avoir au moins UN voisin avec ce régime.", MessageType.Info);
                targetDiet = (DietType)EditorGUILayout.EnumPopup("Régime Obligatoire", targetDiet);
                break;

            case RuleType.NoSpecificNeighborDiet:
                EditorGUILayout.HelpBox("Le dino REFUSE d'avoir des voisins avec ce régime.", MessageType.Info);
                targetDiet = (DietType)EditorGUILayout.EnumPopup("Régime Interdit", targetDiet);
                break;

            case RuleType.SeatTypeRestriction:
                EditorGUILayout.HelpBox("Le dino DOIT absolument être sur ce type de siège (ex: Fenêtre).", MessageType.Info);
                targetSeatType = (SeatType)EditorGUILayout.EnumPopup("Type de Siège Requis", targetSeatType);
                break;

            case RuleType.ForbiddenSeatType:
                EditorGUILayout.HelpBox("Le dino REFUSE d'être sur ce type de siège (ex: Pas côté Fenêtre).", MessageType.Info);
                targetSeatType = (SeatType)EditorGUILayout.EnumPopup("Type de Siège Interdit", targetSeatType);
                break;
                
            case RuleType.MustBeEmptyDirection:
                EditorGUILayout.HelpBox("Le dino exige que la place dans la direction choisie soit VIDE (ou inexistante).", MessageType.Info);
                targetDirection = (NeighborDirection)EditorGUILayout.EnumPopup("Direction à vérifier", targetDirection);
                break;

            case RuleType.ForbiddenEdge:
                EditorGUILayout.HelpBox("Le dino REFUSE d'être collé au bord dans cette direction (ex: pas au fond = Derrière).", MessageType.Info);
                targetDirection = (NeighborDirection)EditorGUILayout.EnumPopup("Bord Interdit", targetDirection);
                break;
        }

        EditorGUILayout.Space(20);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("CRÉER LA RÈGLE", GUILayout.Height(40)))
        {
            CreateRule();
        }
        GUI.backgroundColor = Color.white;
    }

    private void CreateRule()
    {
        if (string.IsNullOrEmpty(ruleFileName))
        {
            EditorUtility.DisplayDialog("Erreur", "Le fichier doit avoir un nom !", "OK");
            return;
        }

        EnsureDirectoryExists(RULES_PATH);
        
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(RULES_PATH + ruleFileName + ".asset");
        SeatRuleSO newRule = null;

        switch (selectedRuleType)
        {
            case RuleType.IncompatibleNeighbor:
                var incompatibleRule = CreateInstance<IncompatibleNeighborRuleSO>();
                incompatibleRule.hatedDinoName = hatedDinoName;
                newRule = incompatibleRule;
                break;

            case RuleType.MustBeNeighborDiet:
                var mustBeRule = CreateInstance<MustBeNeighborDietSO>();
                mustBeRule.requiredDiet = targetDiet;
                newRule = mustBeRule;
                break;

            case RuleType.NoSpecificNeighborDiet:
                var noSpecificRule = CreateInstance<NoSpecificNeighborDietSO>();
                noSpecificRule.forbiddenDiet = targetDiet;
                newRule = noSpecificRule;
                break;

            case RuleType.SeatTypeRestriction:
                var seatTypeRule = CreateInstance<SeatTypeRestrictionSO>();
                seatTypeRule.requiredSeatType = targetSeatType;
                newRule = seatTypeRule;
                break;

            case RuleType.ForbiddenSeatType:
                var forbiddenSeatRule = CreateInstance<ForbiddenSeatTypeRuleSO>();
                forbiddenSeatRule.forbiddenSeatType = targetSeatType;
                newRule = forbiddenSeatRule;
                break;

            case RuleType.MustBeEmptyDirection:
                var emptyDirRule = CreateInstance<MustBeEmptyDirectionRuleSO>();
                emptyDirRule.directionToCheck = targetDirection;
                newRule = emptyDirRule;
                break;

            case RuleType.ForbiddenEdge:
                var forbiddenEdgeRule = CreateInstance<ForbiddenEdgeRuleSO>();
                forbiddenEdgeRule.forbiddenEdge = targetDirection;
                newRule = forbiddenEdgeRule;
                break;
        }

        if (newRule != null)
        {
            AssetDatabase.CreateAsset(newRule, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newRule;

            Debug.Log($"<color=cyan>SUCCÈS : Règle '{ruleFileName}' ({selectedRuleType}) créée !</color>");
        }
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