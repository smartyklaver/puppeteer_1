using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Bewegingsinstellingen")]
    public float moveSpeed = 3f; // snelheid van het mannetje

    [Header("Input toetsen")]
    public KeyCode moveLeftKey = KeyCode.LeftArrow;
    public KeyCode moveRightKey = KeyCode.RightArrow;
    public float minX = -4f;
    public float maxX = 4f;

    private Vector3 startPosition;

    void Start()
    {
        // Onthoud de startpositie zodat we straks kunnen resetten of beperken
        startPosition = transform.position;
    }

void Update()
{
    float moveX = 0f;

    if (Input.GetKey(moveLeftKey))
        moveX = -1f;
    else if (Input.GetKey(moveRightKey))
        moveX = 1f;

    Vector3 newPos = transform.position + new Vector3(moveX * moveSpeed * Time.deltaTime, 0, 0);
    newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
    transform.position = newPos;


}

}
