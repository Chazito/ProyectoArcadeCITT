using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class WeaponInstance : MonoBehaviour
{
    private WeaponSO defaultWeaponTemplate;
    private WeaponSO template;
    private CharacterStat damage;
    private CharacterStat fireRate;
    private CharacterStat projectileCount;
    private CharacterStat projectileSpeed;
    private CharacterStat projectileSize;
    private CharacterStat projectileWrapCount;
    private CharacterStat shootingArcAngle;
    private CharacterStat projectileDeviation;
    private bool initialized;
    private bool shooting;
    private float nextFire;

    void Awake()
    {
        damage = new CharacterStat(0);
        fireRate = new CharacterStat(0);
        projectileCount = new CharacterStat(0);
        projectileSize = new CharacterStat(0);
        projectileSpeed = new CharacterStat(0);
        projectileWrapCount = new CharacterStat(0);
        shootingArcAngle = new CharacterStat(0);
        projectileDeviation = new CharacterStat(0);
        LoadTemplate(Resources.Load<WeaponSO>("ScriptableObjects/DefaultWeapon"));
    }

    public void LoadTemplate(WeaponSO template)
    {
        this.template = template;
        damage.BaseValue = template.damage;
        fireRate.BaseValue = template.fireRate;
        projectileCount.BaseValue = template.projectileCount;
        projectileSize.BaseValue = template.projectileSize;
        projectileSpeed.BaseValue = template.projectileSpeed;
        projectileWrapCount.BaseValue = template.projectileWrapCount;
        shootingArcAngle.BaseValue = template.shootingArcAngle;
        projectileDeviation.BaseValue = template.projectileDeviation;
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
        //Debug.Log("Click");
        if(CanFire())
        {
            shooting = true;
            //Debug.Log("Pew");
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
        float angleIncrement = shootingArcAngle.Value/(projectileCount.Value-1);

        for(int i = 0; i < projectileCount.Value; i++)
        {
            float zRotation = -shootingArcAngle.Value / 2 + angleIncrement * i;
            Quaternion finalRotation = Quaternion.Euler(0, 0, zRotation);
            Quaternion forward = Quaternion.Euler(forwardDirection);
            GameObject bullet = LeanPool.Spawn(template.bulletPrefab, origin, transform.rotation * finalRotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            bulletScript.SetBulletProperties(this.projectileSpeed.Value, this.projectileSize.Value, (int)this.projectileWrapCount.Value, bullet.transform.right, this.damage.Value);

            // Set the owner of the bullet
            bulletScript.owner = gameObject;
        }
        shooting = false;
        nextFire = fireRate.Value;
        yield return null;
    }
}
