using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class WeaponSO : ScriptableObject
{
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public float projectileSize;
    public float projectileCount;

    public GameObject bulletPrefab;
}
