using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Seat Type Restriction")]
public class SeatTypeRestrictionSO : SeatRuleSO
{
    [Tooltip("Le type de siège sur lequel le dino DOIT être assis")]
    public SeatType requiredSeatType;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return seat.seatType == requiredSeatType;
    }
}