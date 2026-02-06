using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Incompatible Neighbor Rule")]
public class IncompatibleNeighborRuleSO : SeatRuleSO
{
    [Header("Je ne veux PAS de cette couleur à côté :")]
    public DinoColor hatedNeighborColor;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.color == hatedNeighborColor)
                    return false; 
            }
        }
        return true;
    }
}
