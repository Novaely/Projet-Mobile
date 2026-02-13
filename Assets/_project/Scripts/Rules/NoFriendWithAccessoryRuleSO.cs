using UnityEngine;

[CreateAssetMenu(menuName = "DinoSystem/Rules/No Friend With Accessory")]
public class NoFriendWithAccessoryRuleSO : SeatRuleSO
{
    [Tooltip("Le tag de l'accessoire que le voisin ne doit PAS porter")]
    public string forbiddenAccessory;

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                if (neighbor.occupant.profile != null && 
                    neighbor.occupant.profile.accessoryTag == forbiddenAccessory)
                {
                    return false; 
                }
            }
        }
        return true;
    }
}