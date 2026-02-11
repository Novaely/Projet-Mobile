using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Seat Type Restriction")]
public class SeatTypeRestrictionSO : SeatRuleSO
{
    public DietType allowedDiet;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        return dino.diet == allowedDiet;
    }
}