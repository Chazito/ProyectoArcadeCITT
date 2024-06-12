using Kryz.CharacterStats;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Perks
{
    [CreateAssetMenu(menuName = "Data/Perks/StatPerk")]
    public class StatPerk : PerkSO
    {
        public enum PlayerStat
        {
            HEALTH = 0,
            SPEED = 1,
            ROTATION_SPEED = 2,
            FIRE_RATE = 3,
            BULLET_DAMAGE = 4,
            BULLET_SIZE = 5,
            BULLET_COUNT = 6,
            BULLET_ARC = 7,
        }

        public PlayerStat stat;
        public float value;
        public StatModType modType;

        public override void ApplyPerk(PlayerController player)
        {
            StatModifier mod = new StatModifier(value, modType, this.perkID);
            WeaponInstance weapon = player.GetWeaponInstance();
            switch (stat)
            {
                case PlayerStat.HEALTH:
                    player.maxHealth.RemoveAllModifiersFromSource(perkID);
                    player.maxHealth.AddModifier(mod);
                    break;
                case PlayerStat.SPEED:
                    player.movementSpeed.RemoveAllModifiersFromSource(perkID);
                    player.movementSpeed.AddModifier(mod);
                    break;
                case PlayerStat.ROTATION_SPEED:
                    player.rotationSpeed.RemoveAllModifiersFromSource(perkID);
                    player.rotationSpeed.AddModifier(mod);
                    break;
                case PlayerStat.FIRE_RATE:
                    weapon.fireRate.RemoveAllModifiersFromSource(perkID);
                    weapon.fireRate.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_DAMAGE:
                    weapon.damage.RemoveAllModifiersFromSource(perkID);
                    weapon.damage.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_SIZE:
                    weapon.projectileSize.RemoveAllModifiersFromSource(perkID);
                    weapon.projectileSize.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_COUNT:
                    weapon.projectileCount.RemoveAllModifiersFromSource(perkID);
                    weapon.projectileCount.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_ARC:
                    weapon.shootingArcAngle.RemoveAllModifiersFromSource(perkID);
                    weapon.shootingArcAngle.AddModifier(mod);
                    break;
            }
        }

        public override void ApplyPerk(PlayerController player, int stacks)
        {
            float finalValue = value;
            switch (modType)
            {
                case StatModType.Flat:
                    finalValue = value * stacks;
                    break;
                case StatModType.PercentAdd:
                    finalValue = value * stacks;
                    break;
                case StatModType.PercentMult:
                    finalValue = Mathf.Pow(1+value, stacks);
                    break;
            }
            StatModifier mod = new StatModifier(finalValue, modType, this.perkID);
            WeaponInstance weapon = player.GetWeaponInstance();
            switch (stat)
            {
                case PlayerStat.HEALTH:
                    player.maxHealth.RemoveAllModifiersFromSource(perkID);
                    player.maxHealth.AddModifier(mod);
                    break;
                case PlayerStat.SPEED:
                    player.movementSpeed.RemoveAllModifiersFromSource(perkID);
                    player.movementSpeed.AddModifier(mod);
                    break;
                case PlayerStat.ROTATION_SPEED:
                    player.rotationSpeed.RemoveAllModifiersFromSource(perkID);
                    player.rotationSpeed.AddModifier(mod);
                    break;
                case PlayerStat.FIRE_RATE:
                    weapon.fireRate.RemoveAllModifiersFromSource(perkID);
                    weapon.fireRate.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_DAMAGE:
                    weapon.damage.RemoveAllModifiersFromSource(perkID);
                    weapon.damage.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_SIZE:
                    weapon.projectileSize.RemoveAllModifiersFromSource(perkID);
                    weapon.projectileSize.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_COUNT:
                    weapon.projectileCount.RemoveAllModifiersFromSource(perkID);
                    weapon.projectileCount.AddModifier(mod);
                    break;
                case PlayerStat.BULLET_ARC:
                    weapon.shootingArcAngle.RemoveAllModifiersFromSource(perkID);
                    weapon.shootingArcAngle.AddModifier(mod);
                    break;
            }
        }

        public override void RemovePerk(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
}