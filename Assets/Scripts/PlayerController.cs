using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Knockback")]
    public bool canMove = true;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

void Update()
{
    if (!canMove) return;

    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    Vector3 move = new Vector3(h, 0, v).normalized;

    if (move.magnitude > 0.1f)
    {
        rb.linearVelocity = move * moveSpeed;
    }
}


    // 🎯 Externe knockback (aangeroepen door draak)
    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero; // reset huidige beweging
        rb.AddForce(direction * force, ForceMode.VelocityChange);
        StartCoroutine(ForceGroundedAfter(0.3f)); // wacht een fractie seconde

    }

    IEnumerator ForceGroundedAfter(float delay)
{
    yield return new WaitForSeconds(delay);

    if (rb != null)
    {
        Vector3 v = rb.linearVelocity;
        if (v.y > 0f)
        rb.linearVelocity = new Vector3(v.x, 0f, v.z); // verwijder "zweef"
        rb.useGravity = true;
    }
}


    // ⏸️ Speler tijdelijk vastzetten
    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
            rb.linearVelocity = Vector3.zero;
    }
}
