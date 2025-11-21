using UnityEngine;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public Transform playerTarget;
    public float zoomDuration = 1.2f;
    public float pauseDuration = 1.2f;

    [Header("References")]
    public PlayerController player;
    public DragonController dragon;
    public EnemyHealth dragonHealth;
    public FireballSpawner spawner;

    [Header("Lean Zone Trigger")]
    public LeanTriggerZone leanZone;
    public float leanCheckDelay = 1f;

    [Header("Sword Auto Phase")]
    public Transform playerStartPos;
    public Transform playerAttackPos;
    public float walkSpeed = 2f;

    [Header("Throw References (scene objects)")]
    public Transform sword;              // attached to player initially
    public Transform shield;             // attached to player initially
    public Transform swordMissTarget;    // world target where sword should miss
    public Transform shieldHitTarget;    // world target where shield should hit

    [Header("Shield Block")]
    public ShieldBlockZone shieldZone;
    public float shieldCheckDelay = 1f;

    [Header("Audio / Lines")]
    public AudioSource musicSource;
    public AudioClip introMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    public AudioClip spotlightSFX;
    public AudioClip swordPhaseLine;
    public AudioClip throwPhaseLine;
    public AudioClip ticklePhaseLine;
    public AudioSource sfxSource;

    // -----------------------------
    // Throw detection (embedded)
    // -----------------------------
   
[Header("Throw Detection")]
public Transform rightHand;     // Hand met zwaard
public Transform leftHand;      // Hand met schild

public float pullBackDistance = 0.15f;
public float minForwardSpeed = 1.2f;
public float maxWaitForThrow = 5f;

private bool pulledBack = false;
private bool throwDetected = false;
private Vector3 lastThrowPos;


    // QTE states
    bool qteSword = false;
    bool qteThrow = false;
    bool qteTickle = false;
    int swordHits = 0;

    void Start()
    {
     
        StartCoroutine(RunCinematicSequence());
    }

    void Update()
    {

    }

   /* void SampleThrowDetection()
    {
        // use local X rotation as simple shoulder-back / forward axis (works for many rigs).
        // convert to signed angle -180..180
        float raw = handTransform.localEulerAngles.x;
        float angle = raw > 180f ? raw - 360f : raw;
        float dt = Mathf.Max(0.0001f, Time.time - lastSampleTime);
        float dAngle = angle - lastSampleAngle;
        // unwrap large jumps
        if (dAngle > 180f) dAngle -= 360f;
        if (dAngle < -180f) dAngle += 360f;
        float angSpeed = Mathf.Abs(dAngle) / dt; // deg/sec

        // Detect "pull back" (hand rotates backward beyond threshold)
        if (!pulledBack && angle < -detectBackAngleDeg) // negative means backward in many rigs
        {
            pulledBack = true;
            // reset throw flag - waiting for forward snap
            throwDetected = false;
            //Debug.Log("Throw: pulled back");
        }

        // If pulled back, detect forward snap: angle moves forward (towards positive) with enough delta & speed
        if (pulledBack && !throwDetected)
        {
            // forward snap: angle increases by at least detectForwardSnapDeg (from its minimum)
            float forwardDelta = angle - lastSampleAngle; // positive if moved forward
            if (forwardDelta > 0f && angSpeed >= detectAngularSpeed && angle > detectForwardSnapDeg * 0.5f)
            {
                throwDetected = true;
                pulledBack = false;
                Debug.Log("Throw detected (shoulder snap)");
            }
        }

        lastSampleAngle = angle;
        lastSampleTime = Time.time;
    }*/

    // -----------------------------
    // Exposed helpers for other scripts
    // -----------------------------
    public bool IsSwordHitActive() => qteSword;
    public bool IsThrowActive() => qteThrow;
    public bool IsTickleActive() => qteTickle;

    // Called by sword collider trigger
    public void RegisterSwordHit()
    {
        if (!qteSword) return;

        swordHits++;
        Debug.Log($"Sword Hit Count: {swordHits}");

        if (dragonHealth != null)
            dragonHealth.TakeQTEHit(5f);

        if (swordHits >= 4)
        {
            Debug.Log("🔥 QTE FINISHED! Sword hits reached 4");
            qteSword = false;

            // disable sword collider if still present
            if (sword != null)
            {
                var col = sword.GetComponent<Collider>();
                if (col) col.enabled = false;
            }
        }
    }

    public void RegisterShieldHit()
    {
        if (!qteThrow) return;
        if (dragonHealth != null)
            dragonHealth.TakeQTEHit(40f);
        qteThrow = false;
    }

    public void RegisterTickle()
    {
        if (!qteTickle) return;
        qteTickle = false;
    }

    // -----------------------------
    // Main cinematic flow
    // -----------------------------
    IEnumerator RunCinematicSequence()
    {
        if (player != null) player.SetCanMove(false);
        if (dragon != null) dragon.enabled = false;

        // camera intro — start zoomed in then out
        Vector3 startCamPos = cameraTransform.position;
        Quaternion startCamRot = cameraTransform.rotation;
        Vector3 zoomPos = playerTarget.position - cameraTransform.forward * 2.5f + Vector3.up * 1f;

        cameraTransform.position = zoomPos;
        cameraTransform.LookAt(playerTarget.position + Vector3.up * 0.8f);

        if (musicSource && introMusic)
        {
            musicSource.clip = introMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        if (spotlightSFX && sfxSource) sfxSource.PlayOneShot(spotlightSFX);

        yield return new WaitForSeconds(pauseDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(zoomPos, startCamPos, t);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, startCamRot, t);
            yield return null;
        }

        if (musicSource && bossMusic)
        {
            musicSource.Stop();
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // Show lean zone and enable player/dragon
        leanZone?.ShowZone();
        if (player != null) player.SetCanMove(true);
        if (dragon != null) dragon.enabled = true;

        yield return new WaitForSeconds(leanCheckDelay);

        Debug.Log("Waiting for player to duck...");
        yield return new WaitUntil(() => leanZone.PlayerIsLowEnough());
        Debug.Log("Player ducked low → FIREBALL!");

        dragon.FireballOverPlayer();
        leanZone.gameObject.SetActive(false);

        // small gap to let the first fireball pass
        yield return new WaitForSeconds(5f);

        // Advance and sword combo
        yield return StartCoroutine(AdvanceAndSwordAttackSequence());

        // Shield block sequence (green zone)
        if (shieldZone != null) shieldZone.ShowZone();
        yield return new WaitForSeconds(shieldCheckDelay);
        yield return new WaitUntil(() => shieldZone.shieldLocked);
        Debug.Log("🛡 Shield placed correctly – FIREBALL!");
        dragon.FireballOverPlayer();
        shieldZone.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        // Throw phase: wait for player's throw motion (detected by our embedded detector)
        qteThrow = true;
        if (sfxSource && throwPhaseLine) sfxSource.PlayOneShot(throwPhaseLine);
        yield return StartCoroutine(ThrowSequence());
        yield return new WaitUntil(() => !qteThrow);
        yield return new WaitForSeconds(0.8f);

        // Tickle QTE
        qteTickle = true;
        if (sfxSource && ticklePhaseLine) sfxSource.PlayOneShot(ticklePhaseLine);
        yield return new WaitUntil(() => !qteTickle);

        // Victory
        OnVictory();
    }

    // -----------------------------
    // Advance, hit, retreat
    // -----------------------------
    IEnumerator AdvanceAndSwordAttackSequence()
    {
        // move player to attack pos (direct MoveTowards)
        yield return StartCoroutine(MovePlayer(playerAttackPos.position));

        // start sword QTE
        qteSword = true;
        swordHits = 0;
        if (sfxSource && swordPhaseLine) sfxSource.PlayOneShot(swordPhaseLine);

        // wait until QTE ends (RegisterSwordHit will set qteSword=false)
        yield return new WaitUntil(() => !qteSword);

        // walk back to start
        yield return StartCoroutine(MovePlayer(playerStartPos.position));
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator MovePlayer(Vector3 targetPos)
    {
        while (Vector3.Distance(player.transform.position, targetPos) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // -----------------------------
    // Throw sequence (sword first, then shield) — uses embedded throw detection
    // -----------------------------
    IEnumerator ThrowSequence()
{
    throwDetected = false;
    pulledBack = false;

    Debug.Log("Waiting for real throwing motion...");

    lastThrowPos = rightHand.position;
    float startTime = Time.time;

    // === Detect THROW motion with right hand ===
    while (!throwDetected && Time.time - startTime < maxWaitForThrow)
    {
        Vector3 current = rightHand.position;

        // Step 1 – hand moet eerst naar achter
        float backwardDist = lastThrowPos.z - current.z;
        if (!pulledBack && backwardDist > pullBackDistance)
        {
            pulledBack = true;
            Debug.Log("Right arm pulled back – now waiting for forward throw.");
        }

        // Step 2 – daarna een snelle beweging vooruit
        if (pulledBack)
        {
            float forwardSpeed = current.z - lastThrowPos.z;
            if (forwardSpeed > minForwardSpeed)
            {
                throwDetected = true;
                Debug.Log("THROW DETECTED!");
                break;
            }
        }

        lastThrowPos = current;
        yield return null;
    }

    if (!throwDetected)
        Debug.LogWarning("No valid throw detected – continuing anyway.");

    // -----------------------------------------------------
    // 1️⃣ ZWAARD WEG SMETEN (miss)
    // -----------------------------------------------------

    if (sword != null)
    {
        sword.SetParent(null);

        Rigidbody rs = sword.gameObject.AddComponent<Rigidbody>();
        rs.useGravity = false;
        rs.isKinematic = true;

        StartCoroutine(LerpObjectTo(
            sword,
            swordMissTarget.position,
            0.8f,
            false
        ));
    }

    yield return new WaitForSeconds(0.6f);

    // -----------------------------------------------------
    // 2️⃣ SCHILD VAN LINKERHAND LOSSEN EN NAAR DRAAK
    // -----------------------------------------------------

    if (shield != null)
    {
        shield.SetParent(null);

        Rigidbody rh = shield.gameObject.AddComponent<Rigidbody>();
        rh.useGravity = false;
        rh.isKinematic = true;

        StartCoroutine(LerpObjectTo(
            shield,
            shieldHitTarget.position,
            0.8f,
            true
        ));
    }

    // QTE eindigt wanneer LerpObjectTo → RegisterShieldHit
    yield break;
}


IEnumerator LerpObjectTo(Transform obj, Vector3 targetPos, float duration, bool isHit)
{
    if (!obj)
        yield break;

    Vector3 startPos = obj.position;
    float t = 0f;

    while (t < 1f)
    {
        t += Time.deltaTime / duration;
        obj.position = Vector3.Lerp(startPos, targetPos, t);
        yield return null;
    }

    if (isHit)
    {
        RegisterShieldHit();
        qteThrow = false;
    }

    Destroy(obj.gameObject, 0.1f);
}


    // -----------------------------
    // Victory
    // -----------------------------
    void OnVictory()
    {
        if (musicSource && victoryMusic)
        {
            musicSource.Stop();
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        Debug.Log("Cinematic: victory sequence complete.");
    }
}
