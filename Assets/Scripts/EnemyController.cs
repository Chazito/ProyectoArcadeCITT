using UnityEngine;
using Lean.Pool;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float _tokensRequired;
    public float tokensRequired { 
        get { return _tokensRequired; } 
        private set { _tokensRequired = value; }
    }
    private float health;
    private Transform player; // Reference to the player transform

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        player = GameDirector.instance.GetPlayer().transform;
        health = maxHealth;
    }

    private void Update()
    {
        if(player != null)
        {
            direction = (player.position - transform.position).normalized;
        }
        if(transform.position.sqrMagnitude > 900)
        {
            if (OnEnemyDeathEvent != null)
            {
                OnEnemyDeathEvent.Invoke();
                OnEnemyDeathEvent = null;
            }
            LeanPool.Despawn(this);
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Set velocity based on direction and speed
            rb.velocity = direction * speed * Time.fixedDeltaTime;

            // Rotate towards player
            RotateTowardsTarget(direction);
        }
        else
        {
            rb.velocity = direction * speed * Time.fixedDeltaTime;
        }
    }

    private void RotateTowardsTarget(Vector2 targetDirection)
    {
        // Calculate target angle
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards target angle
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle-90, rotationSpeed * Time.fixedDeltaTime);

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player
        if (collision.gameObject == player.gameObject)
        {
            // Call function for handling player collision (optional)
            OnPlayerCollision();
        }
    }

    public delegate void OnEnemyDeath();
    public OnEnemyDeath OnEnemyDeathEvent;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if(OnEnemyDeathEvent != null)
            {
                OnEnemyDeathEvent.Invoke();
                OnEnemyDeathEvent = null;
            }
            LeanPool.Despawn(this);
        }
    }

    private void OnPlayerCollision()
    {
        // Implement logic for what happens when the enemy collides with the player (e.g., damage player, destroy enemy)
        Debug.Log("Enemy collided with player!");  // Example log for now
        player.gameObject.GetComponent<PlayerController>().TakeDamage(10f);
    }
}
