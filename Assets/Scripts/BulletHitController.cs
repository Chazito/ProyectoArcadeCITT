using Lean.Pool;
using UnityEngine;

public class BulletHitController : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        _particleSystem.Play();
    }

    private void Update()
    {
        if (!_particleSystem.IsAlive())
        {
            LeanPool.Despawn(this);
        }
    }

}
