using UnityEngine;

[CreateAssetMenu(menuName = "Data/VoidPerk")]
public class VoidPerkPerk : PerkSO
{
    //TODO Add to score
    public override void ApplyPerk(PlayerController player)
    {
        return;
    }

    public override void ApplyPerk(PlayerController player, int stacks)
    {
        //SHOULDN'T APPLY
        return;
    }

    public override void RemovePerk(PlayerController player)
    {
        //SHOULDN'T APPLY AS THIS IS OVERWRITTEN BY OTHER WEAPON PERKS
        return;
    }
}