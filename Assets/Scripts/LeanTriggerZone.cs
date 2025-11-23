using UnityEngine;
using System.Collections.Generic;

public class LeanTriggerZone : MonoBehaviour
{
    public AudioSource instructionAudio;

    // All player colliders currently touching
    private HashSet<Collider> touchingColliders = new HashSet<Collider>();

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowZone()
    {
        gameObject.SetActive(true);

        touchingColliders.Clear();

        if (instructionAudio != null)
            instructionAudio.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            touchingColliders.Add(other);
            // Debug.Log($"Player collider entered: {other.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            touchingColliders.Remove(other);
            // Debug.Log($"Player collider exited: {other.name}");
        }
    }

    /// <summary>
    /// TRUE if NO player collider is touching the zone
    /// </summary>
    public bool PlayerIsLowEnough()
    {
        return touchingColliders.Count == 0;
    }
}
