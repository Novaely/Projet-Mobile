using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Diplodocus No Front")]
public class DiplodocusNoFrontSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.front == null || seat.front.occupant == null;
    }
}
