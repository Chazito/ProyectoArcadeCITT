using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class WeaponInstance : MonoBehaviour
{
    private WeaponSO template;
    private CharacterStat damage;
    private CharacterStat fireRate;
    private CharacterStat projectileCount;
    private CharacterStat projectileSpeed;
    private CharacterStat projectileSize;
    private bool initialized;
    private bool shooting;
    private float nextFire;
    private GameObject owner;

    public void LoadTemplate(WeaponSO template)
    {
        this.template = template;
        damage = new CharacterStat(template.damage);
        fireRate = new CharacterStat(template.fireRate);
        projectileCount = new CharacterStat(template.projectileCount);
        projectileSize = new CharacterStat(template.projectileSize);
        projectileSpeed = new CharacterStat(template.projectileSpeed);

        initialized = true;
    }

    public void Tick(float deltaTime)
    {
        if(nextFire > 0) nextFire -= deltaTime;
    }

    private bool CanFire()
    {
        return initialized && nextFire <= 0 && !shooting && projectileCount.Value > 0;
    }

    public bool Shoot(Vector3 firepoint, Vector3 forwardDirection)
    {
        if(CanFire())
        {
            shooting = true;
            StartCoroutine(FireBullets(firepoint, forwardDirection));
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator FireBullets(Vector3 origin, Vector3 forwardDirection)
    {
        float angleIncrement = 180/projectileCount.Value;
        Vector3 startDirection = Quaternion.AngleAxis(-angleIncrement * (projectileCount.Value - 1) / 2, Vector3.up) * forwardDirection;

        for(int i = 0; i < projectileCount.Value; i++)
        {
            Vector3 spawnDirection = Quaternion.AngleAxis(angleIncrement * i, Vector3.up) * startDirection;
            Quaternion spawnRotation = Quaternion.LookRotation(spawnDirection, Vector3.up);
            LeanPool.Spawn(template.bulletPrefab, origin, spawnRotation);
        }
        //TODO TEST
        shooting = false;
        nextFire = fireRate.Value;
        yield return null;
    }
}
