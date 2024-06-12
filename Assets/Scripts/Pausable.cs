using UnityEngine;

public class Pausable : MonoBehaviour
{
    private Rigidbody2D rb2d;

    private float angularVelocity;
    private Vector2 velocity;
    private Vector3 position;
    private Quaternion rotation;

    private bool _paused = false;
    public bool paused
    {
        get { return _paused; }
        private set { _paused = value; }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_paused)
        {
            if (!GameDirector.instance.paused) Resume();
        }
        else
        {
            if (GameDirector.instance.paused) Pause();
        }
    }

    public void Pause()
    {
        _paused = true;
        velocity = rb2d.velocity;
        angularVelocity = rb2d.angularVelocity;
        position = transform.position;
        rotation = transform.rotation;
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0;
        rb2d.isKinematic = true;
    }

    public void Resume()
    {
        _paused = false;
        rb2d.isKinematic = false;
        rb2d.velocity = velocity;
        rb2d.angularVelocity = angularVelocity;
        transform.position = position;
        transform.rotation = rotation;
    }

}