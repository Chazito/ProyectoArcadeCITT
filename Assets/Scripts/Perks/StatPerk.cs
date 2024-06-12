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
            SPEED = 1
        }

        public override void ApplyPerk(PlayerController player)
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyPerk(PlayerController player, int stacks)
        {
            throw new System.NotImplementedException();
        }

        public override void RemovePerk(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
}