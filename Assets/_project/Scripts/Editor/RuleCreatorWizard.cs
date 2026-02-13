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
        ForbiddenSeatType,
        MustBeEmptyDirection,
        RowRestriction,    
        ForbiddenRow,      
        ColumnRestriction, 
        ForbiddenColumn,
        NearExit,
        WantsToBeAlone,
        MustNotBeAlone,
        NearWindow,
        AtTheBack,
        MustBeInFrontRow,
        NotBetweenTwoDinosaurs,
        FriendWithAccessory,
        NoFriendWithAccessory,
        TRexNeedsHerbivore,
        DiplodocusNoFront
    }

    [Header("Configuration")]
    private string ruleFileName = "NouvelleRegle";
    private RuleType selectedRuleType = RuleType.IncompatibleNeighbor;

    [Header("Paramètres selon la règle")]
    private string hatedDinoName = "T-Rex"; 
    private DietType targetDiet = DietType.Herbivore; 
    private SeatType targetSeatType = SeatType.Fenetre; 
    private NeighborDirection targetDirection = NeighborDirection.Derriere; 
    private SeatRow targetRow = SeatRow.Devant;
    private SeatColumn targetColumn = SeatColumn.Gauche;
    private string targetAccessory = "Chapeau";

    const string RULES_PATH = "Assets/_project/Datas/dino_condition/";

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
                EditorGUILayout.HelpBox("Refuse d'être à côté de cette espèce.", MessageType.Info);
                hatedDinoName = EditorGUILayout.TextField("Espèce détestée", hatedDinoName);
                break;

            case RuleType.MustBeNeighborDiet:
                EditorGUILayout.HelpBox("DOIT avoir au moins UN voisin avec ce régime.", MessageType.Info);
                targetDiet = (DietType)EditorGUILayout.EnumPopup("Régime Obligatoire", targetDiet);
                break;

            case RuleType.NoSpecificNeighborDiet:
                EditorGUILayout.HelpBox("REFUSE d'avoir des voisins avec ce régime.", MessageType.Info);
                targetDiet = (DietType)EditorGUILayout.EnumPopup("Régime Interdit", targetDiet);
                break;

            case RuleType.SeatTypeRestriction:
                EditorGUILayout.HelpBox("DOIT être sur ce type de siège.", MessageType.Info);
                targetSeatType = (SeatType)EditorGUILayout.EnumPopup("Type Requis", targetSeatType);
                break;

            case RuleType.ForbiddenSeatType:
                EditorGUILayout.HelpBox("REFUSE ce type de siège.", MessageType.Info);
                targetSeatType = (SeatType)EditorGUILayout.EnumPopup("Type Interdit", targetSeatType);
                break;
                
            case RuleType.MustBeEmptyDirection:
                EditorGUILayout.HelpBox("Exige que la place dans cette direction soit VIDE (ou inexistante).", MessageType.Info);
                targetDirection = (NeighborDirection)EditorGUILayout.EnumPopup("Direction Vide", targetDirection);
                break;

            case RuleType.RowRestriction:
                EditorGUILayout.HelpBox("DOIT être sur cette LIGNE.", MessageType.Info);
                targetRow = (SeatRow)EditorGUILayout.EnumPopup("Ligne Obligatoire", targetRow);
                break;

            case RuleType.ForbiddenRow:
                EditorGUILayout.HelpBox("REFUSE d'être sur cette LIGNE.", MessageType.Info);
                targetRow = (SeatRow)EditorGUILayout.EnumPopup("Ligne Interdite", targetRow);
                break;

            case RuleType.ColumnRestriction:
                EditorGUILayout.HelpBox("DOIT être sur cette COLONNE.", MessageType.Info);
                targetColumn = (SeatColumn)EditorGUILayout.EnumPopup("Colonne Obligatoire", targetColumn);
                break;

            case RuleType.ForbiddenColumn:
                EditorGUILayout.HelpBox("REFUSE d'être sur cette COLONNE.", MessageType.Info);
                targetColumn = (SeatColumn)EditorGUILayout.EnumPopup("Colonne Interdite", targetColumn);
                break;

            case RuleType.FriendWithAccessory:
                EditorGUILayout.HelpBox("Veut être à côté d'un Dino portant cet accessoire.", MessageType.Info);
                targetAccessory = EditorGUILayout.TextField("Tag Accessoire", targetAccessory);
                break;

            case RuleType.NoFriendWithAccessory:
                EditorGUILayout.HelpBox("REFUSE d'être à côté d'un Dino portant cet accessoire.", MessageType.Info);
                targetAccessory = EditorGUILayout.TextField("Tag Interdit", targetAccessory);
                break;

            default:
                EditorGUILayout.HelpBox("Règle simple : Aucun paramètre supplémentaire requis.", MessageType.None);
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
                var r1 = CreateInstance<IncompatibleNeighborRuleSO>();
                r1.hatedDinoName = hatedDinoName;
                newRule = r1; break;
            case RuleType.MustBeNeighborDiet:
                var r2 = CreateInstance<MustBeNeighborDietSO>();
                r2.requiredDiet = targetDiet;
                newRule = r2; break;
            case RuleType.NoSpecificNeighborDiet:
                var r3 = CreateInstance<NoSpecificNeighborDietSO>();
                r3.forbiddenDiet = targetDiet;
                newRule = r3; break;
            case RuleType.SeatTypeRestriction:
                var r4 = CreateInstance<SeatTypeRestrictionSO>();
                r4.requiredSeatType = targetSeatType;
                newRule = r4; break;
            case RuleType.ForbiddenSeatType:
                var r5 = CreateInstance<ForbiddenSeatTypeRuleSO>();
                r5.forbiddenSeatType = targetSeatType;
                newRule = r5; break;
            case RuleType.MustBeEmptyDirection:
                var r6 = CreateInstance<MustBeEmptyDirectionRuleSO>();
                r6.directionToCheck = targetDirection;
                newRule = r6; break;
            case RuleType.RowRestriction:
                var r7 = CreateInstance<RowRestrictionSO>();
                r7.requiredRow = targetRow;
                newRule = r7; break;
            case RuleType.ForbiddenRow:
                var r8 = CreateInstance<ForbiddenRowSO>();
                r8.forbiddenRow = targetRow;
                newRule = r8; break;
            case RuleType.ColumnRestriction:
                var r9 = CreateInstance<ColumnRestrictionSO>();
                r9.requiredColumn = targetColumn;
                newRule = r9; break;
            case RuleType.ForbiddenColumn:
                var r10 = CreateInstance<ForbiddenColumnSO>();
                r10.forbiddenColumn = targetColumn;
                newRule = r10; break;
            case RuleType.NoFriendWithAccessory:
                var r12 = CreateInstance<NoFriendWithAccessoryRuleSO>();
                r12.forbiddenAccessory = targetAccessory;
                newRule = r12; break;
            
            case RuleType.NearExit: newRule = CreateInstance<NearExitRuleSO>(); break;
            case RuleType.WantsToBeAlone: newRule = CreateInstance<WantsToBeAloneSO>(); break;
            case RuleType.MustNotBeAlone: newRule = CreateInstance<MustNotBeAloneRuleSO>(); break;
            case RuleType.NearWindow: newRule = CreateInstance<NearWindowRuleSO>(); break;
            case RuleType.AtTheBack: newRule = CreateInstance<AtTheBackRuleSO>(); break;
            case RuleType.MustBeInFrontRow: newRule = CreateInstance<MustBeInFrontRowRuleSO>(); break;
            case RuleType.NotBetweenTwoDinosaurs: newRule = CreateInstance<NotBetweenTwoDinosaursSO>(); break;
            case RuleType.TRexNeedsHerbivore: newRule = CreateInstance<TRexNeedsHerbivoreSO>(); break;
            case RuleType.DiplodocusNoFront: newRule = CreateInstance<DiplodocusNoFrontSO>(); break;
            
            case RuleType.FriendWithAccessory:
                var r11 = CreateInstance<FriendWithAccessoryRuleSO>();
                r11.targetAccessory = targetAccessory;
                newRule = r11; break;
        }

        if (newRule != null)
        {
            AssetDatabase.CreateAsset(newRule, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newRule;
            Debug.Log($"<color=cyan>SUCCÈS : Règle '{ruleFileName}' créée !</color>");
        }
    }

    private void EnsureDirectoryExists(string path)
    {
        string sysPath = Application.dataPath + path.Substring("Assets".Length);
        if (!Directory.Exists(sysPath)) Directory.CreateDirectory(sysPath);
    }
}