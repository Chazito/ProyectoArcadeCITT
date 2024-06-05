using UnityEngine;

[CreateAssetMenu(menuName ="Data/WeaponPerk")]
public class WeaponPerk : PerkSO
{
    public WeaponSO weapon;

    public override void ApplyPerk(PlayerController player)
    {
        player.SetWeapon(weapon);
    }

    public override void ApplyPerk(PlayerController player, int stacks)
    {
        //SHOULDN'T APPLY
        throw new System.NotImplementedException();
    }

    public override void RemovePerk(PlayerController player)
    {
        //SHOULDN'T APPLY AS THIS IS OVERWRITTEN BY OTHER WEAPON PERKS
        throw new System.NotImplementedException();
    }
}