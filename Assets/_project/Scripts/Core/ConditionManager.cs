using UnityEngine;
using System.Collections.Generic;

public class ConditionManager : MonoBehaviour
{
    [Header("Règles Globales (Terrain)")]
    [SerializeField] private List<SeatRuleSO> globalRules = new List<SeatRuleSO>();

    [Header("Règles par Siège (Optionnel)")]
    [SerializeField] private List<SeatProfileSO> seatProfiles = new List<SeatProfileSO>();

    [Header("Feedback Config")] 
    public Color validColor = Color.green; 
    public Color invalidColor = Color.red;

    private Dictionary<Seat, List<SeatRuleSO>> seatRuleCache = new Dictionary<Seat, List<SeatRuleSO>>();

    private void Awake() { BuildCache(); }

    private void BuildCache()
    {
        seatRuleCache.Clear();
        foreach (var profile in seatProfiles)
        {
            if (profile.seat != null)
            {
                var rules = new List<SeatRuleSO>(globalRules);
                rules.AddRange(profile.rules);
                seatRuleCache[profile.seat] = rules;
            }
        }
    }

    public bool CanPlace(Dino dino, Seat seat)
    {
        if (seat == null || seat.occupant != null) return false;

        if (dino != null && dino.profile != null)
        {
            foreach (var rule in dino.profile.myRules)
            {
                if (!rule.IsSatisfied(dino, seat)) return false;
            }
        }

        var envRules = GetEnvironmentRules(seat);
        foreach (var rule in envRules)
        {
            if (!rule.IsSatisfied(dino, seat)) return false;
        }

        return true;
    }

    public Color GetHighlightColor(Dino dino, Seat seat) => CanPlace(dino, seat) ? validColor : invalidColor;

    public bool DropDino(Dino dino, Seat seat)
    {
        if (seat == null || seat.occupant != null) return false;

        seat.occupant = dino;
        return true;
    }

    public Dino PickupDino(Seat seat)
    {
        var dino = seat.occupant;
        seat.occupant = null;
        return dino;
    }

    public void ClearSeat(Seat seat) => seat.occupant = null;

    private List<SeatRuleSO> GetEnvironmentRules(Seat seat)
    {
        return seatRuleCache.ContainsKey(seat) ? seatRuleCache[seat] : globalRules;
    }
}