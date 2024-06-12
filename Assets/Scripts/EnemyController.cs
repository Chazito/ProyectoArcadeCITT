using Lean.Pool;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected const float MIN_COLLISION_DAMAGE = 5f;
    protected const float IMPACT_FORCE_MULT = 3f;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float _tokensRequired;
    [SerializeField] private ExperienceOrb experiencePrefab;
    [SerializeField] private float experienceDrop;
    public float tokensRequired
    {
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
        if (GameDirector.instance.paused) return;
        if (player != null)
        {
            direction = (player.position - transform.position).normalized;
        }
        if (transform.position.sqrMagnitude > 900)
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
        if (GameDirector.instance.paused) return;
        //TODO Change Enemy patterns
        rb.AddForce(direction * speed * Time.fixedDeltaTime);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 5f);
        RotateTowardsTarget(direction);
    }

    private void RotateTowardsTarget(Vector2 targetDirection)
    {
        // Calculate target angle
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        // Smoothly rotate towards target angle
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle - 90, rotationSpeed * Time.fixedDeltaTime);
        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherRigidBody = collision.gameObject.GetComponent<Rigidbody2D>();
        if (otherRigidBody != null)
        {
            TakeDamage(CalculateCollisionForce(collision));
        }
        // Check if collided with player
        if (collision.gameObject == player.gameObject)
        {
            // Call function for handling player collision (optional)
            OnPlayerCollision();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        // Check if collided with player
        if (collision.gameObject == player.gameObject)
        {
            // Call function for handling player collision (optional)
            OnPlayerCollision();
            TakeDamage(MIN_COLLISION_DAMAGE);
        }
    }

    public float CalculateCollisionForce(Collision2D collision)
    {
        Rigidbody2D otherRB = collision.collider.GetComponent<Rigidbody2D>();

        if (otherRB != null)
        {
            Vector2 normal = collision.GetContact(0).normal; // Collision normal
            Vector2 relativeVelocity = collision.relativeVelocity; // Relative velocity

            // Bounce direction calculation
            Vector2 bounceDirection = Vector2.Reflect(relativeVelocity, normal).normalized;
            // Apply force to objects based on bounce direction and impact force
            rb.AddForce(-bounceDirection* IMPACT_FORCE_MULT * (otherRB.mass/rb.mass), ForceMode2D.Impulse);
            otherRB.AddForce(bounceDirection * IMPACT_FORCE_MULT * (rb.mass / otherRB.mass), ForceMode2D.Impulse);

            // Store impact force for damage calculation later
            // You can use this value as needed for your damage calculation
            float storedImpactForce = relativeVelocity.magnitude;
            Debug.Log(storedImpactForce);
            return storedImpactForce > MIN_COLLISION_DAMAGE ? storedImpactForce : MIN_COLLISION_DAMAGE;
        }

        return MIN_COLLISION_DAMAGE;
    }

    public delegate void OnEnemyDeath();
    public OnEnemyDeath OnEnemyDeathEvent;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (OnEnemyDeathEvent != null)
            {
                OnEnemyDeathEvent.Invoke();
                OnEnemyDeathEvent = null;
            }
            ExperienceOrb orb = LeanPool.Spawn(experiencePrefab, transform.position, Quaternion.identity);
            orb.Setup(experienceDrop);
            LeanPool.Despawn(this.gameObject);
        }
    }

    private void OnPlayerCollision()
    {
        // Implement logic for what happens when the enemy collides with the player (e.g., damage player, destroy enemy)
        //Debug.Log("Enemy collided with player!");  // Example log for now
        player.gameObject.GetComponent<PlayerController>().TakeDamage(10f);
    }
}
