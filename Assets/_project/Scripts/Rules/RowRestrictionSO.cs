using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Row Restriction")]
public class RowRestrictionSO : SeatRuleSO
{
    public SeatRow requiredRow;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatRow == requiredRow;
    }
}