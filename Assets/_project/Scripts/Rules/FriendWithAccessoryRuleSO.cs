using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Friend With Accessory")]
public class FriendWithAccessoryRuleSO : SeatRuleSO
{
    [Header("Tag de l'accessoire recherché")]
    public string targetAccessoryTag; 

    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        if (string.IsNullOrEmpty(targetAccessoryTag)) return true;

        foreach (var neighbor in seat.neighbors)
        {
            if (neighbor != null && neighbor.occupant != null)
            {
                // Check Dino Profile for accessory tag
                // Requires adding 'public string accessoryTag;' to DinoProfileSO
                if (neighbor.occupant.profile != null && neighbor.occupant.profile.accessoryTag == targetAccessoryTag)
                    return true;
            }
        }
        return false;
    }
}
