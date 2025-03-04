using UnityEngine;
using UnityEngine.InputSystem;

public class Fly : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction jump;

    [SerializeField] private float velocity = 0f;
    [SerializeField] private float rotationSpeed = 10f;
    private Rigidbody2D rb;

    void Awake()
    {
        GameManager.OnSetVelocity += SetVelocity;
    }

    void Start()
    {
        jump = inputActions.FindActionMap("Player").FindAction("Jump");
        jump.Enable();
        jump.performed += Jump;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0,0,rb.linearVelocity.y * rotationSpeed);
    }

    void Jump(InputAction.CallbackContext context)
    {
        rb.linearVelocity = Vector2.up * velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.instance.GameOver();
        jump.performed -= Jump;
    }

    private void SetVelocity(float _velocity)
    {
        velocity = _velocity;
        rb.gravityScale = 0.65f;
    }

    void OnDestroy()
    {
        GameManager.OnSetVelocity -= SetVelocity;
    }
}
