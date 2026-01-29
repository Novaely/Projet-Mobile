using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Near Window")]
public class NearWindowRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        // Check immediate neighbors for "Fenetre" type
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.seatType == SeatType.Fenetre)
                return true;
        }

        // Also consider if the seat itself is a window seat
        if (seat.seatType == SeatType.Fenetre) return true;

        return false;
    }
}
