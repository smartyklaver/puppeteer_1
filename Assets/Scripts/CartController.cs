using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine.SceneManagement;


public class CartController : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool startmoving = false;
    public float speed = 0.1f;
    public float xstop = 2.43f;
    private AudioSource wheels;
    private AudioSource arrival;

    //var allFrames = FindObjectOfType<UdpReceiver>().GetRecordedData();

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        wheels = sources[0]; 
        arrival = sources[1];   
    }
    

    public void StartMoving(){
        startmoving = true;
    }
    public void Reset(){
        transform.position = new Vector3(9.4f,-2.52f,-2.5f);
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
