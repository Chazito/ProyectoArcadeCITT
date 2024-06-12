using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName ="Data/PlayerTemplate")]
    public class PlayerTemplates : ScriptableObject
    {
        public float movementSpeed;
        public float rotationSpeed;
        public float maxHealth;
        public float healthRegen;
    }
}