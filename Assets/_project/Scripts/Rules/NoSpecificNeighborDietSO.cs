using UnityEngine;

[CreateAssetMenu(menuName = "Rules/No Specific Neighbor Diet")]
public class NoSpecificNeighborDietSO : SeatRuleSO
{
    public DietType forbiddenDiet;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.diet == forbiddenDiet)
                    return false;
            }
        }
        return true;
    }
}