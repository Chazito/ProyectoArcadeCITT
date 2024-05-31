using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

[CreateAssetMenu(menuName ="Data/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public float projectileSize;
    public float projectileCount;
    public int projectileWrapCount;
    public float shootingArcAngle;
    public float projectileDeviation;

    public GameObject bulletPrefab;
}
