using UnityEngine;

public class Dino : MonoBehaviour
{
    [Header("Profil Espèce")]
    public DinoProfileSO profile;

    [Header("Instance")]
    public DinoColor color; 

    public string dinoName => profile ? profile.speciesName : "Unknown";
    public DietType diet => profile ? profile.diet : DietType.Herbivore;
}
