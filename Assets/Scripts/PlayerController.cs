using Lean.Pool;
using UnityEngine;
using Kryz.CharacterStats;
using Assets.Scripts;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public CharacterStat movementSpeed;
    [HideInInspector] public CharacterStat rotationSpeed;
    [HideInInspector] public CharacterStat maxHealth;
    [HideInInspector] public CharacterStat healthRegen;
    private WeaponInstance weapon;
    [SerializeField] private Transform firepoint;
    private float currentExperience;
    private float nextLevel;
    private int currentLevel;
    private float currentHealth;

    [SerializeField] private float invulnerabilityTime = .1f;
    private float invulTime;

    public float CurrentExperience
    {
        get { return currentExperience; }
        private set { currentExperience = value; }
    }
    public float NextLevel
    {
        get { return nextLevel; }
        private set { nextLevel = value; }
    }
    public int CurrentLevel
    {
        get { return currentLevel; }
        private set { currentLevel = value; }
    }
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set { currentHealth = value; }
    }
    public float MaxHealth
    {
        get { return maxHealth.Value; }
        private set { maxHealth.BaseValue = value; }
    }

    private float nextFireTime = 0f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLevel = 1;
        nextLevel = 120;
        weapon = GetComponent<WeaponInstance>();
    }

    public void SetupTemplate(PlayerTemplates template)
    {
        this.movementSpeed = new CharacterStat(template.movementSpeed);
        this.rotationSpeed = new CharacterStat(template.rotationSpeed);
        this.maxHealth = new CharacterStat(template.maxHealth);
        this.healthRegen = new CharacterStat(template.healthRegen);

        this.currentHealth = maxHealth.Value;
    }

    private void Update()
    {
        if (GameDirector.instance.paused) return;
        if(currentHealth < maxHealth.Value)
        {
            currentHealth += healthRegen.Value * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0 , maxHealth.Value);
        }
        if (invulTime > 0) invulTime -= Time.deltaTime;
        weapon.Tick(Time.deltaTime);
        // Check for shooting input
        if (Time.time > nextFireTime && Input.GetButton("Fire1"))
        {
            weapon.Shoot(firepoint.position);
            /*
            ShootBullet();
            nextFireTime = Time.time + fireRate;
            */
        }
    }

    private void FixedUpdate()
    {
        if (GameDirector.instance.paused) return;
        // Get input for both horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement vector based on input and speed
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply force to the Rigidbody2D for movement with momentum
        rb.AddForce(movement.normalized * movementSpeed.Value * Time.fixedDeltaTime);

        // Wrap player position around screen edges
        WrapAroundScreen();

        // Rotate towards mouse position
        RotateTowardsMouse();


    }

    public WeaponInstance GetWeaponInstance()
    {
        return weapon;
    }

    private void WrapAroundScreen()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        // Check if player position goes past left/right screen bounds
        if (transform.position.x > screenBounds.x)
        {
            transform.position = new Vector2(-screenBounds.x, transform.position.y);
        }
        else if (transform.position.x < -screenBounds.x)
        {
            transform.position = new Vector2(screenBounds.x, transform.position.y);
        }

        // Check if player position goes past top/bottom screen bounds
        if (transform.position.y > screenBounds.y)
        {
            transform.position = new Vector2(transform.position.x, -screenBounds.y);
        }
        else if (transform.position.y < -screenBounds.y)
        {
            transform.position = new Vector2(transform.position.x, screenBounds.y);
        }
    }

    private void RotateTowardsMouse()
    {
        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get direction vector from player to mouse
        Vector2 direction = mousePosition - transform.position;

        // Calculate target angle
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards target angle
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed.Value * Time.deltaTime);

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    public void Heal(float amount)
    {
        this.currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);
    }

    public void TakeDamage(float damage)
    {
        if (invulTime > 0) return;
        this.currentHealth -= damage;
        invulTime = invulnerabilityTime;
        if (currentHealth <= 0)
        {
            GameDirector.instance.GameOver();
        }
    }

    private void LevelUp()
    {
        currentExperience -= nextLevel;
        currentLevel++;
        nextLevel = 100 + ((currentLevel - 1) * 50);
        Heal(9999999);
        GameDirector.instance.PlayerLevelUp();
        Debug.Log("Level up!");
    }

    public void AddExperience(float experience)
    {
        currentExperience += experience;
        while (currentExperience > nextLevel)
        {
            LevelUp();
        }
    }

    public void SetWeapon(WeaponSO weapon)
    {
        this.weapon.LoadTemplate(weapon);
    }
}
