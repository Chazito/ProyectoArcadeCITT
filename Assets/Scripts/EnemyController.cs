using UnityEngine;
using Lean.Pool;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxHealth;
    private float health;
    [SerializeField] private Transform player; // Reference to the player transform

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        health = maxHealth;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate direction towards player
            Vector2 direction = (player.position - transform.position).normalized;

            // Set velocity based on direction and speed
            rb.velocity = direction * speed;

            // Rotate towards player
            RotateTowardsTarget(direction);
        }
    }

    private void RotateTowardsTarget(Vector2 targetDirection)
    {
        // Calculate target angle
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards target angle
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle-90, rotationSpeed * Time.deltaTime);

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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            LeanPool.Despawn(this);
        }
    }

    private void OnPlayerCollision()
    {
        // Implement logic for what happens when the enemy collides with the player (e.g., damage player, destroy enemy)
        Debug.Log("Enemy collided with player!");  // Example log for now
    }
}
