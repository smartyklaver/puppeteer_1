using UnityEngine;

public class CartController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool startmoving = false;
    public float speed = 0.1f;
    public float xstop = 2.43f;
    private AudioSource wheels;
    private AudioSource arrival;
    void Start()

    {
        AudioSource[] sources = GetComponents<AudioSource>();
        wheels = sources[0]; 
        arrival = sources[1];   
    }
    

    public void StartMoving(){
        startmoving = true;
    }

    // Update is called once per frame

    void Update()
    {
        if (startmoving)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (!wheels.isPlaying)
                wheels.Play();

            if (transform.position.x <= xstop)
            {
                startmoving = false;
                wheels.Stop();
                arrival.Play();
            }
        }
    }
}
