using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Must Be Neighbor Diet")]
public class MustBeNeighborDietSO : SeatRuleSO
{
    [Tooltip("Le régime obligatoire du voisin (ex: Herbivore)")]
    public DietType requiredDiet;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                // On vérifie le régime (Diet) au lieu de la couleur
                if (neighbor.occupant.diet == requiredDiet)
                    return true;
            }
        }
        return false;
    }
}