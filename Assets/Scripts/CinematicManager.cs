using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class CinematicManager : MonoBehaviour
{
    public ArduinoButtonReader arduinoButton;
    public bool lampOn;
    public AudioOutputSwitcher audioSwitcher;

    public TextToggle duckcue;
    public TextToggle attackcue;
    
    public TextToggle throwswordcue;
    public TextToggle throwshieldcue;
    public TextToggle ticklecue;
 
    public GameObject thankYouText;

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
    public Transform sword;              
    public Transform shield;             
    public Transform swordMissTarget;    
    public Transform shieldHitTarget;    

    [Header("Shield Block")]
    public ShieldBlockZone shieldZone;
    public float shieldCheckDelay = 1f;

    [Header("FMOD Music")]
    public StudioEventEmitter introEmitter;
    public StudioEventEmitter bossEmitter;
    public StudioEventEmitter victoryEmitter;
    public StudioEventEmitter begin;
    public StudioEventEmitter duck;
    public StudioEventEmitter attack;
    public StudioEventEmitter zone;
    public StudioEventEmitter swordthrow;
    public StudioEventEmitter shieldthrow;
    public StudioEventEmitter tickle;
    public StudioEventEmitter hero;

    public SpineController1 spinecontroller;
    public ShoulderController1 shouldercontroller;

    // --- Recorded space presses ---
    public List<float> spaceTimestamps = new List<float>();
    public int replayIndex = 0;
    public bool isReplayingInput = false;
    public float replayStartTime;

    float recordingStartTime = 0f;

    // -----------------------------
    // Throw detection
    // -----------------------------
    [Header("Throw Detection")]
    public Transform rightHand;     
    public Transform leftHand;      

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
        playerStartPos = player.transform.position;
        playerStartRot = player.transform.rotation;
        camStartPos = cameraTransform.position;
        camStartRot = cameraTransform.rotation;
        dragonStartPos = dragon.transform.position;
        dragonStartRot = dragon.transform.rotation;
        swordStartParent = sword.parent;
        swordStartLocalPos = sword.localPosition;
        swordStartLocalRot = sword.localRotation;
        
        leftCurtain.localPosition = leftClosedPos;
        rightCurtain.localPosition = rightClosedPos;

        shieldStartParent = shield.parent;
        shieldStartLocalPos = shield.localPosition;
        shieldStartLocalRot = shield.localRotation;

        spaceTimestamps.Clear();
        thankYouText.SetActive(false);
        
        StartCoroutine(RunCinematicSequence());
    }

    public bool IsSwordHitActive() => qteSword;
    public bool IsThrowActive() => qteThrow;
    public bool IsTickleActive() => qteTickle;
    private bool end = false;
    private bool replayed= false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)||end)
        {
            if(replayed == false){
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

    public void RegisterSwordHit()
    {
        if (!qteSword) return;
        swordHits++;
        if (dragonHealth != null) dragonHealth.TakeQTEHit(5f);

        if (swordHits >= 4)
        {
            qteSword = false;
            var col = sword.GetComponent<Collider>();
            if (col) col.enabled = false;
        }
    }

    public void RegisterShieldHit()
    {
        if (!qteThrow) return;
        if (dragonHealth != null) dragonHealth.TakeQTEHit(40f);
        qteThrow = false;
    }

    public void RegisterTickle()
    {
        if (!qteTickle) return;
        qteTickle = false;
    }

    IEnumerator RunCinematicSequence()
    {
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

        Vector3 startCamPos = cameraTransform.position;
        Quaternion startCamRot = cameraTransform.rotation;
        Vector3 zoomPos = playerTarget.position - cameraTransform.forward * 2.5f + Vector3.up * 1f;

        cameraTransform.position = zoomPos;

        StartCoroutine(OpenCurtains());

        cameraTransform.LookAt(playerTarget.position + Vector3.up * 0.8f);

        if (introEmitter != null)
        {
            introEmitter.Play();
            begin.Play();
        }

        yield return new WaitForSeconds(pauseDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(zoomPos, startCamPos, t);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, startCamRot, t);
            yield return null;
        }

        if (introEmitter != null)
        {
            introEmitter.Stop();
            bossEmitter.Play();
        }

        leanZone?.ShowZone();
        if (dragon != null) dragon.enabled = true;
        yield return new WaitForSeconds(1);
        duck.Play();
        
        // <--- FIX: Don't show text if replaying
        if(!isReplayingInput) duckcue.ShowText();

        yield return new WaitForSeconds(leanCheckDelay);

        Debug.Log("Waiting for player to duck...");
        yield return new WaitUntil(() => leanZone.PlayerIsLowEnough());
        
        duckcue.RemoveText();
        Debug.Log("Player ducked low → FIREBALL!");

        dragon.FireballOverPlayer();
        leanZone.gameObject.SetActive(false);

        yield return new WaitForSeconds(6f);

        yield return StartCoroutine(AdvanceAndSwordAttackSequence());

        zone.Play();
        if (shieldZone != null) shieldZone.ShowZone();
        yield return new WaitForSeconds(shieldCheckDelay);
        yield return new WaitUntil(() => shieldZone.shieldLocked);
        Debug.Log("🛡 Shield placed correctly – FIREBALL!");
        dragon.FireballOverPlayer();
        shieldZone.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        qteThrow = true;

        yield return new WaitForSeconds(5);

        StartCoroutine(ThrowSequence());

        yield return new WaitUntil(() => !qteThrow);
        yield return StartCoroutine(MovePlayer(playerAttackPos.position));

        yield return new WaitForSeconds(0.8f);
        
        // <--- FIX: Don't show text if replaying
        if(ticklecue != null && !isReplayingInput)
        {
             ticklecue.ShowText();
        }
        
        tickle.Play();

        qteTickle = true;
        Debug.Log("Tickle active = " + qteTickle);

        yield return new WaitUntil(() => !qteTickle);
        
        // <--- FIX: Use RemoveText() for consistency and ensure it runs
        if(ticklecue != null)
        {
             ticklecue.RemoveText(); 
        }

        if (isReplayingInput)
        {
            // <--- FIX: We do NOT turn off isReplayingInput here yet. 
            // We let the OnVictoryRoutine handle the logic using the boolean state,
            // or simply wait a tiny bit to ensure the Victory routine catches the flag.
            
            // Actually, best to just log here and let OnVictoryRoutine handle cleanup
            Debug.Log("[Replay] Sequence finished. Waiting for Victory logic.");
        }
    }

    IEnumerator AdvanceAndSwordAttackSequence()
    {
        yield return StartCoroutine(MovePlayer(playerAttackPos.position));
        attack.Play();
        
        // <--- FIX: Don't show text if replaying
        if(!isReplayingInput) attackcue.ShowText();

        qteSword = true;
        swordHits = 0;

        yield return new WaitUntil(() => !qteSword);
        attackcue.RemoveText();

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

    IEnumerator ThrowSequence()
    {
        Debug.Log("Waiting for SPACE to throw sword...");
        swordthrow.Play();
        
        // <--- FIX: Don't show text if replaying
        if(!isReplayingInput) throwswordcue.ShowText();

        SendLampStateForced(true);
        yield return new WaitUntil(() => IsSpacePressed());
        Debug.Log("SPACE pressed → Throwing sword!");
        throwswordcue.RemoveText();
        SendLampStateForced(false);

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

        yield return new WaitForSeconds(0.4f);

        Debug.Log("Waiting for SPACE again to throw shield...");
        shieldthrow.Play();
        
        // <--- FIX: Don't show text if replaying
        if(!isReplayingInput) throwshieldcue.ShowText();
        
        SendLampStateForced(true);
        yield return new WaitUntil(() => IsSpacePressed());
        throwshieldcue.RemoveText();
        Debug.Log("SPACE pressed → Throwing shield!");
        SendLampStateForced(false);

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
        yield break;
    }

    IEnumerator LerpObjectTo(Transform obj, Vector3 targetPos, float duration, bool isHit)
    {
        if (!obj) yield break;

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

    public void OnVictory()
    {
        StartCoroutine(OnVictoryRoutine());
    }

    private IEnumerator OnVictoryRoutine()
    {
        // <--- FIX: Capture the Replay state NOW, before we wait 5 seconds.
        // If we wait first, the main loop might finish and set 'isReplayingInput' to false.
        bool wasReplaying = isReplayingInput;
        if(ticklecue != null)
        {
             ticklecue.RemoveText(); 
        }

        if (bossEmitter != null)
        {
            bossEmitter.Stop();
            hero.Play();
            victoryEmitter.Play();
        }

        yield return StartCoroutine(CloseCurtains());

        Debug.Log("Cinematic: victory sequence complete.");

        // Wait 5 seconds
        yield return new WaitForSeconds(5f);

        // <--- FIX: Check the CAPTURED variable, not the live one
        if (wasReplaying && thankYouText != null)
        {
            thankYouText.SetActive(true);
            
            // Reset the live variable now that we are done showing the text
            isReplayingInput = false; 
        }

        end = true;
    }

    public void RestartCinematic()
    {
        Debug.Log("🔄 FULL CINEMATIC RESET");

        isReplayingInput = true;
        replayStartTime = Time.time;
        replayIndex = 0;

        if (audioSwitcher != null)
            audioSwitcher.SwitchToSecondary(); 

        StopAllCoroutines();
        ResetObjects();
        StartCoroutine(RunCinematicSequence());
    }

    void ResetObjects()
    {
        player.transform.position = playerStartPos;
        player.transform.rotation = playerStartRot;

        cameraTransform.position = camStartPos;
        cameraTransform.rotation = camStartRot;

        dragon.transform.position = dragonStartPos;
        dragon.transform.rotation = dragonStartRot;
        dragon.enabled = false;

        if (!dragon.gameObject.activeSelf)
            dragon.gameObject.SetActive(true);

        if (dragonHealth != null)
        {
            dragonHealth.ResetHealth();
        }

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

        if (sword != null)
        {
            sword.SetParent(swordStartParent);
            sword.localPosition = swordStartLocalPos;
            sword.localRotation = swordStartLocalRot;
            sword.gameObject.SetActive(true);
            var col = sword.GetComponent<Collider>();
            if (col) col.enabled = true;
        }

        if (shield != null)
        {
            shield.SetParent(shieldStartParent);
            shield.localPosition = shieldStartLocalPos;
            shield.localRotation = shieldStartLocalRot;
            shield.gameObject.SetActive(true);
            var col = shield.GetComponent<Collider>();
            if (col) col.enabled = true;
        }

        if (shieldZone != null)
        {
            shieldZone.gameObject.SetActive(false);
            shieldZone.shieldLocked = false;
        }

        if (leanZone != null)
        {
            leanZone.gameObject.SetActive(false);
        }

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
        yield return null;
        // <--- FIX: We do NOT set isReplayingInput = false here anymore.
        // We let the Victory routine handle the end state to ensure text shows up.
    }
    
    public void RegisterTickleHit()
    {
        if (!qteTickle) return; 

        if (dragonHealth != null)
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
        while (t < 1f)
        {
            t += Time.deltaTime / curtainOpenDuration;
            leftCurtain.localPosition  = Vector3.Lerp(leftOpenPos,  leftClosedPos2,  t);
            rightCurtain.localPosition = Vector3.Lerp(rightOpenPos, rightClosedPos2, t);
            yield return null;
        }
    }

    public void SendLampStateForced(bool on)
    {
        lampOn = on;
        if (arduinoButton != null)
            arduinoButton.SendToArduino(on ? "L" : "l");
    }
}