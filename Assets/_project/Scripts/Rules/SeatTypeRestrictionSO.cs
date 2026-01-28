using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Seat Type Restriction")]
public class SeatTypeRestrictionSO : SeatRuleSO
{
    [Header("Quel type de siège ?")]
    public SeatType targetSeatType;

    [Header("Conditions requises")]
    public bool mustBeHerbivore;
    public bool mustBeCarnivore;
    public bool mustBeColor;
    public DinoColor requiredColor;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (seat.seatType != targetSeatType) return true;

        if (dino == null) return false;

        if (mustBeHerbivore && dino.diet != DietType.Herbivore) return false;
        if (mustBeCarnivore && dino.diet != DietType.Carnivore) return false;
        if (mustBeColor && dino.color != requiredColor) return false;

        return true;
    }
}
