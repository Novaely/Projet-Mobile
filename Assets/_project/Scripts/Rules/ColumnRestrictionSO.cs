using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Column Restriction")]
public class ColumnRestrictionSO : SeatRuleSO
{
    public SeatColumn requiredColumn;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatColumn == requiredColumn;
    }
}