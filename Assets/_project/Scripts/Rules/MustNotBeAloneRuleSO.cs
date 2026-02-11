using UnityEngine;

[CreateAssetMenu(menuName = "Rules/Must Not Be Alone")]
public class MustNotBeAloneRuleSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (seat.neighbors != null)
        {
            foreach (var neighbor in seat.neighbors)
            {
                if (neighbor != null && neighbor.occupant != null)
                {
                    return true; 
                }
            }
        }
        
        return false; 
    }
}