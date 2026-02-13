using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DinoSystem/Dino Profile")]
public class DinoProfileSO : ScriptableObject
{
    [Header("Accessoires")]
    public string accessoryTag;

    [Header("GD Info (Descriptions UI)")]
    [TextArea(2, 4)]
    public string positiveCondition = "J'aime..."; 
    [TextArea(2, 4)]
    public string negativeCondition = "Je déteste...";

    [Header("Infos Espèce")]
    public string speciesName;
    public DietType diet;

    [Header("Règles propres à ce Dino")]
    [Tooltip("Les conditions que ce dino DOIT valider pour être posé")]
    public List<SeatRuleSO> myRules = new List<SeatRuleSO>();
}