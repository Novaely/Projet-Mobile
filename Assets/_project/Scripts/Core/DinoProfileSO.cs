using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DinoSystem/Dino Profile")]
public class DinoProfileSO : ScriptableObject
{
    [Header("Accessoires")]
    public string accessoryTag;

    [Header("GD Info")]
    
    [TextArea(3, 5)]
    public string designerDescription = "Description du comportement de ce dino...";


    [Header("Infos Espèce")]
    public string speciesName;
    public DietType diet;

    [Header("Règles propres à ce Dino")]
    [Tooltip("Les conditions que ce dino DOIT valider pour être posé")]
    public List<SeatRuleSO> myRules = new List<SeatRuleSO>();
}
