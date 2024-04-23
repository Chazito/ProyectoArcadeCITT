using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float screenWidth;
    [SerializeField] private float screenHeight;
    [SerializeField] private float rotationSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Get input for both horizontal and vertical movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement vector based on input and speed
        Vector2 movement = new Vector2(horizontalInput * movementSpeed, verticalInput * movementSpeed);

        // Apply force to the Rigidbody2D for movement with momentum
        rb.AddForce(movement);

        // Wrap player position around screen edges
        WrapAroundScreen();

        // Rotate towards mouse position
        RotateTowardsMouse();
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
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
