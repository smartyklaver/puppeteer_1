using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class CinematicManager : MonoBehaviour
{
    public ArduinoButtonReader arduinoButton;
    enum ActionPhase { None, Duck, ShieldBlock, SwordThrow, ShieldThrow }
ActionPhase currentPhase = ActionPhase.None;
bool lampOn = false;


    [Header("Camera")]
    public Transform cameraTransform;
    public Transform playerTarget;
    public float zoomDuration = 1.2f;
    public float pauseDuration = 1.2f;
    public Camera cameraA; 
    public Camera cameraB;

    [Header("Curtains")]
public Transform leftCurtain;
public Transform rightCurtain;
public Vector3 leftClosedPos;
public Vector3 rightClosedPos;
public Vector3 leftOpenPos;
public Vector3 rightOpenPos;
public float curtainOpenDuration = 1.5f;

[Header("Curtains close")]

public Vector3 leftClosedPos2;
public Vector3 rightClosedPos2;




    [Header("References")]
    public PlayerController player;
    public DragonController dragon;
    public EnemyHealth dragonHealth;
    public FireballSpawner spawner;

    [Header("Lean Zone Trigger")]
    public LeanTriggerZone leanZone;
    public float leanCheckDelay = 1f;

    [Header("Sword Auto Phase")]
    public Transform playerStartPosi;
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
    public SpineController1 spinecontroller;
    public ShoulderController1 shouldercontroller;

    // --- Recorded space presses ---
    public List<float> spaceTimestamps = new List<float>();
    public int replayIndex = 0;
    public bool isReplayingInput = false;
    public float replayStartTime;

    // internal: when did we start recording (relative timestamps)
    float recordingStartTime = 0f;

    // -----------------------------
    // Throw detection (embedded)
    // -----------------------------
    [Header("Throw Detection")]
    public Transform rightHand;     // Hand met zwaard
    public Transform leftHand;      // Hand met schild

    // QTE states
    bool qteSword = false;
    bool qteThrow = false;
    bool qteTickle = false;
    int swordHits = 0;

    [Header("Reset Cache")]
    Vector3 playerStartPos;
    Quaternion playerStartRot;

    Vector3 camStartPos;
    Quaternion camStartRot;

    Transform swordStartParent;
    Vector3 swordStartLocalPos;
    Quaternion swordStartLocalRot;

    Transform shieldStartParent;
    Vector3 shieldStartLocalPos;
    Quaternion shieldStartLocalRot;

    Quaternion dragonStartRot;
    Vector3 dragonStartPos;

    void Start()
    {
        // Save player
        playerStartPos = player.transform.position;
        playerStartRot = player.transform.rotation;

        // Save camera
        camStartPos = cameraTransform.position;
        camStartRot = cameraTransform.rotation;

        // Save dragon
        dragonStartPos = dragon.transform.position;
        dragonStartRot = dragon.transform.rotation;

        // Save sword
        swordStartParent = sword.parent;
        swordStartLocalPos = sword.localPosition;
        swordStartLocalRot = sword.localRotation;

        leftCurtain.localPosition = leftClosedPos;
        rightCurtain.localPosition = rightClosedPos;


        // Save shield
        shieldStartParent = shield.parent;
        shieldStartLocalPos = shield.localPosition;
        shieldStartLocalRot = shield.localRotation;
        Debug.Log("Sword parent at start: " + swordStartParent);

        // ensure list cleared at cold start
        spaceTimestamps.Clear();
        

        // Start cinematic
        StartCoroutine(RunCinematicSequence());
    }

    public bool IsSwordHitActive() => qteSword;
    public bool IsThrowActive() => qteThrow;
    public bool IsTickleActive() => qteTickle;
    private bool end = false;
    private bool replayed= false;

    void Update()
    {
        UpdateArduinoActionLamp();
        if (Input.GetKeyDown(KeyCode.M)||end)
        {
            if(replayed ==false){
            end = false;
            replayed = true;
            Debug.Log("🔄 Restarting Cinematic...");
            RestartCinematic();

            cameraA.targetDisplay = 1;
            cameraB.targetDisplay = 0;

            spinecontroller?.ReplayPuppetSpine();
            shouldercontroller?.ReplayPuppetShoulders();
            }
        }
    }

    // Called by sword collider trigger
    public void RegisterSwordHit()
    {
        if (!qteSword) return;

        swordHits++;
        Debug.Log($"Sword Hit Count: {swordHits}");

        if (dragonHealth != null)
        {
            Debug.Log("🔥 in qte");
            dragonHealth.TakeQTEHit(5f);
        }

        if (swordHits >= 4)
        {
            Debug.Log("🔥 QTE FINISHED! Sword hits reached 4");
            qteSword = false;

            // disable sword collider if still present
            var col = sword.GetComponent<Collider>();
            if (col) col.enabled = false;
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
        // Start recording if NOT replaying
        if (!isReplayingInput)
        {
            spaceTimestamps.Clear();
            recordingStartTime = Time.time;
            Debug.Log("[Record] Starting recording at: " + recordingStartTime);
        }
        else
        {
            Debug.Log("[Replay] Starting replay at: " + replayStartTime);
        }

        if (dragon != null) dragon.enabled = false;

        // camera intro — start zoomed in then out
        Vector3 startCamPos = cameraTransform.position;
        Quaternion startCamRot = cameraTransform.rotation;
        Vector3 zoomPos = playerTarget.position - cameraTransform.forward * 2.5f + Vector3.up * 1f;

        cameraTransform.position = zoomPos;

StartCoroutine(OpenCurtains());


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
        if (dragon != null) dragon.enabled = true;

        yield return new WaitForSeconds(leanCheckDelay);

        Debug.Log("Waiting for player to duck...");
        yield return new WaitUntil(() => leanZone.PlayerIsLowEnough());
        StartPhase(ActionPhase.Duck);
        EndPhase();   // speler drukte → lamp uit

        Debug.Log("Player ducked low → FIREBALL!");

        dragon.FireballOverPlayer();
        leanZone.gameObject.SetActive(false);

        // small gap to let the first fireball pass
        yield return new WaitForSeconds(6f);

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

        qteThrow = true;
        if (sfxSource && throwPhaseLine) sfxSource.PlayOneShot(throwPhaseLine);

        yield return new WaitForSeconds(5);

        // ❗ NOW start listening for throw detection
        StartCoroutine(ThrowSequence());

        // And wait until this QTE is done
        yield return new WaitUntil(() => !qteThrow);
        yield return StartCoroutine(MovePlayer(playerAttackPos.position));


        yield return new WaitForSeconds(0.8f);

        // Tickle QTE
        qteTickle = true;
        Debug.Log("Tickle active = " + qteTickle);

        if (sfxSource && ticklePhaseLine) sfxSource.PlayOneShot(ticklePhaseLine);
        yield return new WaitUntil(() => !qteTickle);

        // If we were replaying input, stop replay mode when done
        if (isReplayingInput)
        {
            isReplayingInput = false;
            Debug.Log("[Replay] Finished replaying input.");
        }
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
        yield return StartCoroutine(MovePlayer(playerStartPosi.position));
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
        Debug.Log("Waiting for SPACE to throw sword...");

        // ===============================
        // 1️⃣ WAIT FOR FIRST SPACE (SWORD)
        // ===============================
        yield return new WaitUntil(() => IsSpacePressed());
        Debug.Log("SPACE pressed → Throwing sword!");

        if (sword != null)
        {
            sword.SetParent(null);

            Rigidbody rb = sword.gameObject.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = true;


            StartCoroutine(LerpObjectTo(
                sword,
                swordMissTarget.position,
                2f,
                false
            ));
        }

        // Small delay so the sword starts flying
        yield return new WaitForSeconds(0.4f);

        // ===============================
        // 2️⃣ WAIT FOR SECOND SPACE (SHIELD)
        // ===============================
        Debug.Log("Waiting for SPACE again to throw shield...");
        yield return new WaitUntil(() => IsSpacePressed());
        Debug.Log("SPACE pressed → Throwing shield!");

        if (shield != null)
        {
            shield.SetParent(null);

            Rigidbody rs = shield.gameObject.AddComponent<Rigidbody>();

            rs.useGravity = false;
            rs.isKinematic = true;



            StartCoroutine(LerpObjectTo(
                shield,
                shieldHitTarget.position,
                2f,
                true
            ));
        }

        // QTE finishes when shield hit lands and LerpObjectTo calls RegisterShieldHit()
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

        var rb = obj.GetComponent<Rigidbody>();
        if (rb) Destroy(rb);

        obj.gameObject.SetActive(false);
    }

    // -----------------------------
    // Victory
    // -----------------------------
public void OnVictory()
{
    StartCoroutine(OnVictoryRoutine());
}

private IEnumerator OnVictoryRoutine()
{
    // 🎵 Speel victory muziek
    if (musicSource && victoryMusic)
    {
        musicSource.Stop();
        musicSource.clip = victoryMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    // 🎬 Gordijnen dicht doen
    yield return StartCoroutine(CloseCurtains());

    Debug.Log("Cinematic: victory sequence complete.");

    // ⏳ Wacht 5 seconden *echt*
    yield return new WaitForSeconds(5f);

    end = true;
}

    public void RestartCinematic()
    {
        Debug.Log("🔄 FULL CINEMATIC RESET");

        // Enable replay mode and mark start time
        isReplayingInput = true;
        replayStartTime = Time.time;
        replayIndex = 0;

        StopAllCoroutines();
        ResetObjects();
        StartCoroutine(RunCinematicSequence());
    }

    void ResetObjects()
    {
        // RESET PLAYER
        player.transform.position = playerStartPos;
        player.transform.rotation = playerStartRot;

        // RESET CAMERA
        cameraTransform.position = camStartPos;
        cameraTransform.rotation = camStartRot;

        // RESET DRAGON
        dragon.transform.position = dragonStartPos;
        dragon.transform.rotation = dragonStartRot;
        dragon.enabled = false;

        if (!dragon.gameObject.activeSelf)
        dragon.gameObject.SetActive(true);

        // RESET DRAGON HEALTH + UI
        if (dragonHealth != null)
        {
            dragonHealth.ResetHealth();
        }

        // RESET PLAYER HEALTH + UI
        var healthField = player.GetType().GetField("health");
        if (healthField != null)
        {
            var healthObj = healthField.GetValue(player);
            if (healthObj != null)
            {
                var cur = healthObj.GetType().GetField("currentHealth");
                var max = healthObj.GetType().GetField("maxHealth");
                var ui = healthObj.GetType().GetMethod("UpdateHealthUI");

                if (max != null && cur != null)
                    cur.SetValue(healthObj, max.GetValue(healthObj));

                ui?.Invoke(healthObj, null);
            }
        }

        // RESET SWORD
        if (sword != null)
        {
            sword.SetParent(swordStartParent);
            sword.localPosition = swordStartLocalPos;
            sword.localRotation = swordStartLocalRot;
            sword.gameObject.SetActive(true);

            var col = sword.GetComponent<Collider>();
            if (col) col.enabled = true;
        }

        // RESET SHIELD
        if (shield != null)
        {
            shield.SetParent(shieldStartParent);
            shield.localPosition = shieldStartLocalPos;
            shield.localRotation = shieldStartLocalRot;
            shield.gameObject.SetActive(true);

            var col = shield.GetComponent<Collider>();
            if (col) col.enabled = true;
        }

        // RESET ZONES
        if (shieldZone != null)
        {
            shieldZone.gameObject.SetActive(false);
            shieldZone.shieldLocked = false;
        }

        if (leanZone != null)
        {
            leanZone.gameObject.SetActive(false);
        }

        // RESET QTE STATES
        qteSword = false;
        qteThrow = false;
        qteTickle = false;
        swordHits = 0;
    }

    public bool IsSpacePressed()
{
    bool pressed =
        Input.GetKeyDown(KeyCode.Space) ||
        (arduinoButton != null && arduinoButton.WasButtonPressedThisFrame());

    // ---- RECORDING MODE ----
    if (!isReplayingInput)
    {
        if (pressed)
        {
            float relative = Time.time - recordingStartTime;
            spaceTimestamps.Add(relative);
            Debug.Log($"[Record] SPACE/Arduino at {relative:F3}s");
            return true;
        }
        return false;
    }

    // ---- REPLAY MODE ----
    if (replayIndex < spaceTimestamps.Count)
    {
        float nextRelative = spaceTimestamps[replayIndex];
        float elapsed = Time.time - replayStartTime;

        if (elapsed >= nextRelative)
        {
            replayIndex++;
            Debug.Log($"[Replay] Simulated SPACE at {elapsed:F3}s");

            if (replayIndex >= spaceTimestamps.Count)
                StartCoroutine(EndReplayNextFrame());

            return true;
        }
    }

    return false;
}


    IEnumerator EndReplayNextFrame()
    {
        // small delay so callers can consume the last simulated press
        yield return null;
        isReplayingInput = false;
        Debug.Log("[Replay] Will stop replay mode now.");
    }
    public void RegisterTickleHit()
{
    Debug.Log("erin ");

    if (!qteTickle) return;   // enkel tijdens tickle-QTE tellen

    if (dragonHealth != null)
        Debug.Log("damage");

        dragonHealth.TakeQTEHit(5f);

    Debug.Log("🤣 Tickle hit! Dragon takes 5 damage.");
}
void HandleDragonDeath()
{
    Debug.Log("CinematicManager: Dragon has died → Triggering victory.");
    OnVictory();
}

IEnumerator OpenCurtains()
{
    float t = 0f;

    Vector3 lStart = leftCurtain.localPosition;
    Vector3 rStart = rightCurtain.localPosition;

    while (t < 1f)
    {
        t += Time.deltaTime / curtainOpenDuration;

        leftCurtain.localPosition  = Vector3.Lerp(leftClosedPos,  leftOpenPos,  t);
        rightCurtain.localPosition = Vector3.Lerp(rightClosedPos, rightOpenPos, t);

        yield return null;
    }
}

IEnumerator CloseCurtains()
{
    float t = 0f;

    Vector3 lStart = leftCurtain.localPosition;
    Vector3 rStart = rightCurtain.localPosition;

    while (t < 1f)
    {
        t += Time.deltaTime / curtainOpenDuration;

        leftCurtain.localPosition  = Vector3.Lerp(leftOpenPos,  leftClosedPos2,  t);
        rightCurtain.localPosition = Vector3.Lerp(rightOpenPos, rightClosedPos2, t);

        yield return null;
    }
}

void UpdateArduinoActionLamp()
{
    // Als er geen fase actief is → lamp uit
    if (currentPhase == ActionPhase.None)
    {
        lampOn = false;
        SendLampState();
        return;
    }

    // Indien LED aan staat → wacht op input
    if (lampOn)
    {
        if (IsSpacePressed())      // speler drukte
        {
            EndPhase();            // LED uit + fase klaar
        }
        return;
    }

    // LED uit maar fase nog actief = fout → force LED aan
    if (currentPhase != ActionPhase.None)
    {
        lampOn = true;
        SendLampState();
    }
}

void StartPhase(ActionPhase phase)
{
    currentPhase = phase;
    lampOn = true;                // LED AAN
    SendLampState();
}

void EndPhase()
{
    lampOn = false;               // LED UIT
    SendLampState();
    currentPhase = ActionPhase.None;
}

void SendLampState()
{
    if (arduinoButton != null)
        arduinoButton.SendToArduino(lampOn ? "L" : "l");
}


}
