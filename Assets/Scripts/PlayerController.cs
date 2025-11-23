// PlayerController.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bool inputEnabled = true;

    [Header("References")]
    public Rigidbody rb;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!inputEnabled) return;

    }

    // External API ------------------------------------------------
    public void SetCanMove(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled && rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    // Deterministic knockback (used by cinematic)
    public void ApplyKnockback(Vector3 direction, float force, float stunSeconds = 0.6f)
    {
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        StartCoroutine(ForceGroundedThenEnable(stunSeconds));
    }

    IEnumerator ForceGroundedThenEnable(float wait)
    {
        // disable input while in knockback
        inputEnabled = false;
        yield return new WaitForSeconds(wait);

        // ensure player is on ground (zero vertical velocity) to avoid hovering
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            rb.linearVelocity = new Vector3(v.x, 0f, v.z);
            rb.useGravity = true;
        }

        inputEnabled = true;
    }
}
