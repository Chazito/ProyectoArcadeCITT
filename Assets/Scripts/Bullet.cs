using UnityEngine;
using Lean.Pool;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public GameObject owner; // GameObject that fired the bullet
    public float damage;

    private float speed;
    private float size;
    private int wrapCount;
    private Vector2 direction;
    private Rigidbody2D rb;
    private Vector2 screenBounds;
    [SerializeField] private GameObject hitEffect;

    private void Awake()
        => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()
    {
        transform.localScale = Vector2.one * size; // Set size
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
    }

    public void SetBulletProperties(float speed, float size, int wrapCount, Vector2 direction, float bulletDamage)
    {
        this.speed = speed;
        this.size = size;
        this.wrapCount = wrapCount;
        this.direction = direction.normalized; // Normalize for consistent speed
        this.damage = bulletDamage;
        rb.velocity = direction * speed; // Set initial velocity
    }

    private void FixedUpdate()
    {
        // Check for screen wrap or despawn
        HandleScreenWrap();
    }

    private void HandleScreenWrap()
    {
        // Check for wrap on all sides
        if (transform.position.x > screenBounds.x)
        {
            WrapAround(-screenBounds.x, true);
        }
        else if (transform.position.x < -screenBounds.x)
        {
            WrapAround(screenBounds.x, true);
        }

        if (transform.position.y > screenBounds.y)
        {
            WrapAround(-screenBounds.y, false);
        }
        else if (transform.position.y < -screenBounds.y)
        {
            WrapAround(screenBounds.y, false);
        }
    }

    private void WrapAround(float newPos, bool horizontalWrap)
    {
        if (wrapCount > 0)
        {
            if(horizontalWrap)
            {
                transform.position = new Vector2(newPos, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(transform.position.x, newPos);
            }
            wrapCount--;
        }
        else
        {
            LeanPool.Despawn(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // Check for collision with anything except owner, bullets, and itself
        if (collision.gameObject != owner && collision.gameObject != gameObject)
        {
            OnBulletHit(collision.gameObject);  // Call function for handling the hit
            LeanPool.Despawn(this);
        }
    }

    public virtual void OnBulletHit(GameObject other) 
    {
        LeanPool.Spawn(hitEffect, transform.position, transform.rotation);
        EnemyController ec = other.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.TakeDamage(damage);
        }

    } // Override this for specific hit behaviour 
}
