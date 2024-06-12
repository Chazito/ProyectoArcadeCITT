using System.Collections.Generic;
using UnityEngine;

public abstract class PerkSO : ScriptableObject
{
    public string perkID;
    public string perkName;
    public string perkDescription;
    public bool canStack;
    public bool canBeRetaken;
    public List<string> requirements;

    public abstract void ApplyPerk(PlayerController player);
    public abstract void ApplyPerk(PlayerController player, int stacks);
    public abstract void RemovePerk(PlayerController player);
}
