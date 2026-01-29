using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/No Specific Neighbor Color")]
public class NoSpecificNeighborColorSO : SeatRuleSO
{
    public DinoColor forbiddenNeighborColor;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null && neighbor.occupant.color == forbiddenNeighborColor)
                return false;
        }
        return true;
    }
}
