using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Near Exit")]
public class NearExitRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        // Assuming "Couloir" or specific seats are exits
        // Modify this check if you add a specific SeatType.Exit
        if (seat.seatType == SeatType.Couloir) return true;

        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.seatType == SeatType.Couloir)
                return true;
        }
        return false;
    }
}
