using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Near Exit")]
public class NearExitRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (seat.seatType == SeatType.Exit) return true;

        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.seatType == SeatType.Exit)
                return true;
        }
        
        return false;
    }
}