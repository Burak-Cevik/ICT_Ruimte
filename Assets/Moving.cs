using UnityEngine;
using UnityEngine.InputSystem;

public class Moving : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool jumpRequested = false;
    private bool isGrounded = false;
    public float groundCheckRadius = 0.3f;
    public float groundCheckOffset = 0.5f;
    public Vector3 respawnPosition = new Vector3(0, 2, 0);
    public float fallThreshold = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Grond-check met CheckSphere net onder de speler
        Vector3 checkPos = transform.position + Vector3.down * groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, LayerMask.GetMask("Default"));
        Debug.DrawRay(transform.position, Vector3.down * groundCheckOffset, Color.red);

        var kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame && isGrounded)
            jumpRequested = true;

        // Reset player if fallen out of map
        if (transform.position.y < fallThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = respawnPosition;
        }
    }

    void FixedUpdate()
    {
        float horizontal = 0f;
        float vertical = 0f;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    vertical   += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  vertical   -= 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  horizontal -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;
        }

        Vector3 moveDir = (transform.right * horizontal + transform.forward * vertical).normalized;
        Vector3 velocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = velocity;

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y voor consistente sprong
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }
}
