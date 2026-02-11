using UnityEngine;

[CreateAssetMenu(menuName = "Rules/At The Back")]
public class AtTheBackRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        // "Au fond" typically means no seat behind, or last row (Y=0)
        // Adjust logic based on grid orientation
        // Here: No seat behind OR seat behind is empty

        return seat.back == null; 
    }
}
