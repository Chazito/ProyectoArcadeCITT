using UnityEngine;
using Lean.Pool;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float size;
    private int wrapCount;
    private Vector2 direction;
    private Rigidbody2D rb;
    public GameObject owner;

    private void Awake()
        => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()
    {
        transform.localScale = Vector2.one * size; // Set size
    }

    public void SetBulletProperties(float speed, float size, int wrapCount, Vector2 direction)
    {
        this.speed = speed;
        this.size = size;
        this.wrapCount = wrapCount;
        this.direction = direction.normalized; // Normalize for consistent speed
        rb.velocity = direction * speed; // Set initial velocity
    }

    private void FixedUpdate()
    {
        // Check for screen wrap or despawn
        HandleScreenWrap();
    }

    private void HandleScreenWrap()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        if (transform.position.x > screenBounds.x && direction.x > 0)
        {
            WrapAround(screenBounds.x, -screenBounds.x);
        }
        else if (transform.position.x < -screenBounds.x && direction.x < 0)
        {
            WrapAround(-screenBounds.x, screenBounds.x);
        }

        if (transform.position.y > screenBounds.y && direction.y > 0)
        {
            WrapAround(screenBounds.y, -screenBounds.y);
        }
        else if (transform.position.y < -screenBounds.y && direction.y < 0)
        {
            WrapAround(-screenBounds.y, screenBounds.y);
        }
    }

    private void WrapAround(float positiveBound, float negativeBound)
    {
        if (wrapCount > 0)
        {
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, negativeBound, positiveBound), transform.position.y);
            wrapCount--;
        }
        else
        {
            LeanPool.Despawn(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for collision with anything except owner, bullets, and itself
        if (collision.gameObject != owner && !collision.gameObject.CompareTag("Bullet") && collision.gameObject != gameObject)
        {
            OnBulletHit(collision.gameObject);  // Call function for handling the hit
            LeanPool.Despawn(this);
        }
    }

    public virtual void OnBulletHit(GameObject other) { } // Override this for specific hit behaviour 
}
