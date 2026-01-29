using UnityEngine;

[CreateAssetMenu(menuName = "SeatRules/Not Between Two Dinosaurs")]
public class NotBetweenTwoDinosaursSO : SeatRuleSO
{
    public override bool IsSatisfied(Dino dino, Seat seat)
    {
        // Check horizontal neighbors (Left + Right)
        bool hasLeft = seat.left != null && seat.left.occupant != null;
        bool hasRight = seat.right != null && seat.right.occupant != null;

        // If both left and right are occupied, rule fails
        if (hasLeft && hasRight) return false;

        // Check vertical neighbors (Front + Back) - Optional depending on game logic
        bool hasFront = seat.front != null && seat.front.occupant != null;
        bool hasBack = seat.back != null && seat.back.occupant != null;

        if (hasFront && hasBack) return false;

        return true;
    }
}
