using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Must Be Neighbor Color")]
public class MustBeNeighborColorSO : SeatRuleSO
{
    public DinoColor requiredNeighborColor;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null && neighbor.occupant.color == requiredNeighborColor)
                return true;
        }
        return false;
    }
}
