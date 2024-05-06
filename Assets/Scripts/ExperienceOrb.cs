using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class ExperienceOrb : MonoBehaviour
{
    private float experienceValue;
    [SerializeField] private float speed;
    private bool bInitialized = false;
    [HideInInspector] public PlayerController player;
    private Rigidbody2D rb2d;
    private Vector2 screenBounds;
    [SerializeField] private TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
    }

    public void Setup(float experience)
    {
        experienceValue = experience;
        bInitialized = true;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), 90 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (bInitialized && player)
        {
            Vector2 dir = player.transform.position - rb2d.transform.position;
            //rb2d.AddForce(dir.normalized * speed * Time.fixedDeltaTime, ForceMode2D.Force);
            rb2d.velocity = dir.normalized * speed * Time.fixedDeltaTime;
        }

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
        if (horizontalWrap)
        {
            transform.position = new Vector2(newPos, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x, newPos);
        }
        trailRenderer.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bInitialized && player)
        {
            if(collision.gameObject == player.gameObject)
            {
                player.AddExperience(experienceValue);
                bInitialized = false;
                LeanPool.Despawn(this);
            }
        }
    }
}
