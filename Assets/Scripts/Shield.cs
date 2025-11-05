using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Shield : MonoBehaviour
{
    [Header("Shield Settings")]
    public string shieldOwnerTag = "ShieldOwner"; // optional, not usually needed
    public AudioClip deflectSound;
    public ParticleSystem deflectEffect;

    void OnCollisionEnter(Collision collision)
    {
        // If fireball hits, we can add a small effect (the actual reflection is handled in Fireball)
        var fb = collision.gameObject.GetComponent<Fireball>();
        if (fb != null)
        {
            if (deflectEffect != null)
                Instantiate(deflectEffect, collision.contacts[0].point, Quaternion.identity);
            if (deflectSound != null)
                AudioSource.PlayClipAtPoint(deflectSound, transform.position);
        }
    }
}
