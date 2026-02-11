using UnityEngine;

[CreateAssetMenu(menuName = "Rules/No One Behind")]
public class NoOneBehindRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (seat.back != null)
        {
            if (seat.back.occupant != null)
            {
                return false; 
            }
        }
        
        return true; 
    }
}