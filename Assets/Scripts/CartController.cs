using UnityEngine;

public class CartController : MonoBehaviour
 {
//     private AudioSource wheels;
//     private AudioSource arrival;

    void Start()
    {
        // AudioSource[] sources = GetComponents<AudioSource>();
        // wheels = sources[0];
        // arrival = sources[1];
    }


    // public void PlayWheels()
    // {
    //     if (!wheels.isPlaying)
    //         wheels.Play();
    // }

    // // Timeline can call this when the cart reaches the end
    // public void PlayArrival()
    // {
    //     arrival.Play();
    //     wheels.Stop();
    // }

    public void ResetCart()
    {
        transform.position = new Vector3(9.4f,-2.52f,-2.5f);
    }
}
