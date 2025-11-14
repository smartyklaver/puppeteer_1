using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Knockback")]
    public bool canMove = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!canMove) return; // ⛔ Geen input tijdens cinematic

        Vector3 move = Vector3.zero;

    // Enkel links en rechts
    if (Input.GetKey(KeyCode.LeftArrow)) move += Vector3.left;
    if (Input.GetKey(KeyCode.RightArrow)) move += Vector3.right;

    move.Normalize();

    Vector3 vel = rb.linearVelocity;
    Vector3 targetVel = move * moveSpeed;

    rb.linearVelocity = new Vector3(targetVel.x, vel.y, 0f);
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
