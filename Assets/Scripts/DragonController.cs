using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour, IDamageable
{

    public Transform body; // hoofdlichaam van de draak (bijv. de romp)

    [Header("Audio")]
    public AudioSource roarSound;
    public AudioSource wingFlapSound;

    private bool playerCanMove = true;

    [Header("Movement")]
    public float walkSpeed = 1f;
    public float flyHeight = 3f;
    public float flySpeed = 1.5f;
    public float flyDuration = 15f;
    public float groundDuration = 8f;
    public float descendSpeed = 2f;

    [Header("Head & Body Tracking")]
    public Transform head;
    public Transform mouth;
    public Transform player;
    public float headTrackSpeed = 3f;
    public float headPitchLimit = 40f;
    public float bodyPitchFollow = 0.5f;   // Hoeveel van hoofdrotatie overgenomen wordt door lichaam
    public float bodyPitchReturnSpeed = 2f;
    public float bodyTurnSpeed = 1.5f;
    public float bodyAssistThreshold = 35f;

    [Header("Head Movement")]
    public float headBobAmount = 0.03f;
    public float headBobSpeed = 1.5f;

    [Header("Leg Animation (Transform-based)")]
    public Transform leftLeg;
    public Transform rightLeg;
    public float legSwingAngle = 15f;
    public float legSwingSpeed = 3f;
    public float bodyBobAmount = 0.05f;

    [Header("Meteor Shower Settings")]
    public GameObject fireballPrefab;
    public int fireballsPerWave = 15;     // aantal vallende vuurballen
    public float spawnAreaWidth = 20f;    // breedte van het gebied
    public float spawnAreaDepth = 10f;    // diepte van het gebied
    public float spawnHeight = 12f;       // hoogte boven arena
    public float fallDelay = 0.2f;        // tijd tussen elke spawn
    public float fireballFallSpeed = 15f; // val-snelheid
    public float fireRainChance = 0.25f;  // 25% kans

    [Header("Reaction")]
    public float roarDuration = 1.2f;
    public float cameraShakeIntensity = 0.2f;
    public float cameraShakeDuration = 0.4f;
    public float knockbackForce = 20f;

    [Header("References")]
    public PlayerController playerController;

    public Camera mainCamera;

    private bool isFlying = false;
    private bool isRoaring = false;
    private float flyTimer = 0f;
    private float groundTimer = 0f;
    private Vector3 groundPos;
    private float legTimer = 0f;
    private Vector3 headBaseLocalPos;
    private Quaternion headBaseLocalRot;
    private Quaternion leftLegBaseRot;
    private Quaternion rightLegBaseRot;
    private Quaternion bodyBaseRot;
    private bool wingSoundPlaying = false;

    void Start()
    {

        //Debug.DrawRay(head.position, head.forward * 2f, Color.blue, 10f);
        //Debug.DrawRay(head.position, head.up * 2f, Color.green, 10f);
        //Debug.DrawRay(head.position, head.right * 2f, Color.red, 10f);


        groundPos = transform.position;
        bodyBaseRot = transform.rotation;

        if (head != null)
        {
            headBaseLocalPos = head.localPosition;
            headBaseLocalRot = head.localRotation;
        }
        if (leftLeg != null) leftLegBaseRot = leftLeg.localRotation;
        if (rightLeg != null) rightLegBaseRot = rightLeg.localRotation;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (isRoaring) return; 
        HandleFlightCycle();
        HandleLegsAndBodyBob();
        HandleHeadAndBodyTracking();
    }

    // ✈️ Beweging met zachte stijging en landing
    void HandleFlightCycle()
{
    if (isFlying)
    {
        flyTimer += Time.deltaTime;

        // start looping wing sound when we enter flying state
        if (!wingSoundPlaying && wingFlapSound != null)
        {
            wingFlapSound.loop = true;
            wingFlapSound.Play();
            wingSoundPlaying = true;
        }

        // zachte opstijging
        float targetY = groundPos.y + flyHeight;
        Vector3 desired = new Vector3(transform.position.x + walkSpeed * Time.deltaTime, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * flySpeed);

        if (flyTimer >= flyDuration)
        {
            flyTimer = 0f;
            StartCoroutine(FlyDownSmoothly());
        }
    }
    else
    {
        // stop wing-loop if we're on ground
        if (wingSoundPlaying && wingFlapSound != null)
        {
            wingFlapSound.loop = false;
            wingFlapSound.Stop();
            wingSoundPlaying = false;
        }

        groundTimer += Time.deltaTime;
        Vector3 desired = new Vector3(transform.position.x + walkSpeed * Time.deltaTime, groundPos.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * flySpeed);

        if (groundTimer >= groundDuration)
        {
            groundTimer = 0f;
            isFlying = true;
            flyTimer = 0f;
        }
    }
}

    // 🕊️ Zachte landing
    IEnumerator FlyDownSmoothly()
    {
        isFlying = false;
        float targetY = groundPos.y;
        float startY = transform.position.y;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (descendSpeed / Mathf.Max(0.1f, flyHeight));
            float newY = Mathf.Lerp(startY, targetY, Mathf.SmoothStep(0, 1, t));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // terwijl hij daalt → geleidelijk lichaam terug rechtzetten
            Quaternion upright = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, upright, Time.deltaTime * bodyPitchReturnSpeed);

            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // STOP wing loop (if still playing) and play landing impact
        if (wingSoundPlaying && wingFlapSound != null)
        {
            wingFlapSound.loop = false;
            wingFlapSound.Stop();
            wingSoundPlaying = false;
        }


        // APPLY KNOCKBACK WHEN DRAGON LANDS
        if (playerController != null)
        {
            // direction from dragon to player (away from dragon)
            Vector3 dir = (playerController.transform.position - transform.position).normalized;
            dir.y = 0.5f; // give a little upward push
            playerController.ApplyKnockback(dir, knockbackForce);
            // optionally briefly disable player movement during impact
            playerController.SetCanMove(false);
            // re-enable after a short delay
            StartCoroutine(ReenablePlayerAfter(0.6f)); // 0.6s stun from landing
        }

    }
    IEnumerator ReenablePlayerAfter(float seconds)
{
    yield return new WaitForSeconds(seconds);
    if (playerController != null)
        playerController.SetCanMove(true);
}

    // 🦵 Beenanimatie op grond
    void HandleLegsAndBodyBob()
    {
        if (isFlying)
        {
            ResetLeg(leftLeg, leftLegBaseRot);
            ResetLeg(rightLeg, rightLegBaseRot);
            return;
        }

        legTimer += Time.deltaTime * legSwingSpeed;
        float swing = Mathf.Sin(legTimer);
        if (leftLeg != null)
            leftLeg.localRotation = leftLegBaseRot * Quaternion.Euler(0, swing * legSwingAngle, 0);
        if (rightLeg != null)
            rightLeg.localRotation = rightLegBaseRot * Quaternion.Euler(0, -swing * legSwingAngle, 0);

        float bob = Mathf.Abs(swing) * bodyBobAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, groundPos.y + bob, transform.localPosition.z);
    }

    void ResetLeg(Transform leg, Quaternion baseRot)
    {
        if (leg == null) return;
        leg.localRotation = Quaternion.Slerp(leg.localRotation, baseRot, Time.deltaTime * 5f);
    }

    // 🧠 Hoofd en lichaam volgen speler, inclusief X-rotatie van lichaam
    // 🧠 Hoofd en lichaam volgen speler + constante op/neer rotatie
void HandleHeadAndBodyTracking()
{
    if (player == null || head == null || body == null) return;

    // === Direction to player ===
    Vector3 toPlayer = (player.position - head.position).normalized;

    // Flatten direction for yaw (rotation around Y)
    Vector3 toPlayerFlat = new Vector3(toPlayer.x, 0f, toPlayer.z).normalized;

    float yaw = Mathf.Atan2(toPlayerFlat.x, toPlayerFlat.z) * Mathf.Rad2Deg;
    float pitch = -Mathf.Atan2(toPlayer.y, toPlayerFlat.magnitude) * Mathf.Rad2Deg;

    // === Breathing / idle nodding ===
    float breathing = Mathf.Sin(Time.time * 2f) * 15f; // ±15°

    // Clamp pitch
    float targetPitch = Mathf.Clamp(pitch + breathing, -40f, 40f);

    // 🧭 Apply only X-rotation (pitch), no sideways roll
    // Use headBaseLocalRot to keep its natural orientation (so we don’t flip axes)
    Quaternion targetHeadRot = headBaseLocalRot * Quaternion.Euler(targetPitch, 0f, 0f);

    // Smooth rotation
    head.localRotation = Quaternion.Slerp(head.localRotation, targetHeadRot, Time.deltaTime * headTrackSpeed);

    // 🦴 Lichaam volgt licht mee (alleen X-rotatie)
    if (isFlying)
    {
        float bodyTilt = targetPitch * bodyPitchFollow;
        Quaternion targetBodyRot = bodyBaseRot * Quaternion.Euler(bodyTilt, 0f, 0f);
        body.localRotation = Quaternion.Slerp(body.localRotation, targetBodyRot, Time.deltaTime * bodyTurnSpeed);
    }
    else
    {
        // Terug naar neutraal bij landing
        body.localRotation = Quaternion.Slerp(body.localRotation, bodyBaseRot, Time.deltaTime * bodyPitchReturnSpeed);
    }

    // Debug info
    //Debug.Log($"pitch={pitch:F1}, targetPitch={targetPitch:F1}, headLocalRot={head.localEulerAngles}");
}



    public void OnHitByPlayer(){
        Debug.Log("⚔️ Draak is geraakt door speler!");

        if (isRoaring) return;

        if (Random.value < fireRainChance)
        {
            StartCoroutine(FireRain());
        }
    
    StartCoroutine(RoarAndReact());
}

IEnumerator RoarAndReact()
{
    isRoaring = true;
    playerCanMove = false; // ⛔ speler kan niet bewegen
    float roarTime = 8f;   // duur van brul

    // 🎵 Vleugelslaggeluid
    if (wingFlapSound != null)
        wingFlapSound.Play();

    if (playerController != null)
{
    Vector3 dir = (playerController.transform.position - transform.position).normalized;
    dir.y = 0.4f; // kleine boog omhoog
    playerController.ApplyKnockback(dir, knockbackForce);
    playerController.SetCanMove(false); // zet speler vast
}

    // 🎥 Camera shake voor impact
    yield return StartCoroutine(CameraShake(cameraShakeDuration, cameraShakeIntensity));

    // 🦴 Hoofd omhoog richten (cinematic houding)
    Quaternion lookUpRot = headBaseLocalRot * Quaternion.Euler(-70f, 0f, 0f); // hoofd naar boven
    float t = 0f;
    while (t < 1f)
    {
        t += Time.deltaTime * 1.5f;
        if (head != null)
            head.localRotation = Quaternion.Slerp(head.localRotation, lookUpRot, t);
        yield return null;
    }

    // 🎵 Speel brulg geluid (duurt ±8 sec)
    if (roarSound != null)
        roarSound.Play();

    // 🕒 wacht 8 seconden brultijd
    yield return new WaitForSeconds(roarTime);

    // 🎯 Klaar met brullen → vliegmodus activeren
    isRoaring = false;
    
    playerController.SetCanMove(true);
    isFlying = true;
    flyTimer = 0f;
    groundTimer = 0f; // reset zodat het niet blokkeert

    // Reset hoofdpositie terug naar normaal
    if (head != null)
        head.localRotation = Quaternion.Slerp(head.localRotation, headBaseLocalRot, 0.2f);

    // (Later hier: trigger vallende vuurballen)
}

    IEnumerator CameraShake(float duration, float intensity)
    {
        if (mainCamera == null) yield break;
        Vector3 original = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            mainCamera.transform.localPosition = original + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = original;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"🐲 DragonController received {amount} damage!");
        OnHitByPlayer();
    }

    IEnumerator FireRain()
    {
        Debug.Log("🌧️ FIRE RAIN activated (line fall)!");

        int count = fireballsPerWave;
        float startX = -6f;
        float endX = 6f;
        float zPos = -2.33f;
        float yStart = 4f;

        for (int i = 0; i < count; i++)
        {
            // bereken positie op de lijn (x loopt geleidelijk van -6 naar 6)
            float t = (float)i / (count - 1);
            float xPos = Mathf.Lerp(startX, endX, t);

            // spawn de fireball
            Vector3 spawnPos = new Vector3(xPos, yStart, zPos);
            GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

            Rigidbody rb = fb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // geen zwaartekracht
                rb.isKinematic = false;

                // willekeurige trage snelheid naar beneden
                float fallSpeed = Random.Range(1.2f, 2.4f);

                // laat de vuurbal traag naar beneden bewegen
                StartCoroutine(SlowFall(fb, fallSpeed));
            }

            // kleine vertraging tussen spawns (mooi visueel effect)
            yield return new WaitForSeconds(Random.Range(0.08f, 0.15f));
        }

        Debug.Log("🔥 Fire rain finished!");
    }

IEnumerator SlowFall(GameObject fireball, float speed)
{
    Transform t = fireball.transform;

    // blijf bewegen tot hij de grond (y=0) raakt of vernietigd wordt
    while (fireball != null && t.position.y > 0.1f)
    {
        t.position += Vector3.down * speed * Time.deltaTime;
        yield return null;
    }

    if (fireball != null)
    {
        // optioneel: spawn effect bij impact
        Fireball fb = fireball.GetComponent<Fireball>();
        if (fb != null && fb.hitEffect != null)
            Instantiate(fb.hitEffect, t.position, Quaternion.identity);

        Destroy(fireball);
    }
}




}
