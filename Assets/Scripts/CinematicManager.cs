using UnityEngine;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public Transform playerTarget;
    public Vector3 zoomOffset = new Vector3(0, 2.5f, -2.5f);
    public float zoomDuration = 3f;
    public float pauseDuration = 2f;

    [Header("Dialogue")]
    public AudioClip swordPhaseLine;
    public AudioClip throwPhaseLine;
    public AudioClip ticklePhaseLine;



    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip introMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    public AudioClip spotlightSFX;

    [Header("References")]
    public PlayerController player;
    public DragonController dragon;
    public EnemyHealth dragonhp;
    public FireballSpawner spawner;

    [Header("Throw Objects")]
    public Transform sword;
    public Transform shield;
    public Transform swordThrowTarget;   // punt naast draak → mis
    public Transform shieldThrowTarget;  // punt op draak → hit

    public bool IsSwordHitActive() => qteSwordHits;
    public bool IsThrowActive() => qteThrow;
    public bool IsTickleActive() => qteTickle;

    [Header("Audio – Dragon")]
    public AudioClip dragonLaughLoop;
    public AudioSource dragonAudioSource;



    // STATES
    private int hitCount = 0;
    private bool qteSwordHits = false;
    private bool qteThrow = false;
    private bool qteTickle = false;

    private Vector3 startCamPos;
    private Quaternion startCamRot;
    private Vector3 zoomPos;

    void Start()
    {
        StartCoroutine(StartCinematic());
    }

    IEnumerator StartCinematic()
    {
        yield return new WaitForSeconds(0.1f);

        player.SetCanMove(false);
        dragon.enabled = false;
            if (spawner != null) spawner.Pause();   // 🔥 alleen pauzeren, niet uitschakelen


        startCamPos = cameraTransform.position;
        startCamRot = cameraTransform.rotation;

        zoomPos = playerTarget.position - cameraTransform.forward * 2.5f + Vector3.up;
        cameraTransform.position = zoomPos;
        cameraTransform.LookAt(playerTarget.position + Vector3.up);

        // play intro
        if (introMusic && musicSource)
        {
            musicSource.clip = introMusic;
            musicSource.loop = false;
            musicSource.Play();
        }

        if (spotlightSFX)
            AudioSource.PlayClipAtPoint(spotlightSFX, playerTarget.position);

        yield return new WaitForSeconds(pauseDuration);

        // zoom out
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / zoomDuration;
            cameraTransform.position = Vector3.Lerp(zoomPos, startCamPos, t);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, startCamRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // start boss music
        if (musicSource && bossMusic)
        {
            musicSource.clip = bossMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        player.SetCanMove(true);
        dragon.enabled = true;
        dragon.forceGround = false;
        spawner.Resume();


        StartCoroutine(BossFightFlow());
    }

    // ——————————————————————————
    //          BOSS FIGHT
    // ——————————————————————————
    IEnumerator BossFightFlow()
    {
        yield return new WaitForSeconds(13f);

        // STOP FIREBALLS
        dragon.forceGround = true;
        spawner.Pause();

        // Phase 1 — 3 sword hits
        qteSwordHits = true;
        hitCount = 0;
        Debug.Log("QTE 1: Sword hits");

        StartCoroutine(AutoWalkToDragon());


        while (hitCount < 3)
            yield return null;

        // mini pause
        yield return new WaitForSeconds(13f);

        // Phase 2 — Throw sword + shield
        qteThrow = true;
        StartCoroutine(ThrowSequence());

        while (qteThrow)
            yield return null;

        // mini pause
        yield return new WaitForSeconds(13f);

        // Phase 3 — Tickle
        qteTickle = true;
        if (dragonLaughLoop && dragonAudioSource)
{
    dragonAudioSource.clip = dragonLaughLoop;
    dragonAudioSource.loop = true;
    dragonAudioSource.Play();
}


        while (qteTickle)
            yield return null;

        // DONE → Dragon dies
        DragonDefeated();
    }

    // ——————————————————————————
    //       REGISTER EVENTS
    // ——————————————————————————

    public void RegisterSwordHit()
    {
        if (!qteSwordHits) return;
        hitCount++;
        dragonhp.TakeQTEHit(5f); // low damage
        if (hitCount >= 3)
        {
            qteSwordHits = false;
            dragon.OnHitByPlayer();
        } 
            

    }

    public void RegisterShieldHit()
    {
        if (!qteThrow) return;
        dragonhp.TakeQTEHit(40f); // major hit
        qteThrow = false;
    }

    public void RegisterTickle()
    {
        if (!qteTickle) return;

        Debug.Log("🪶 Tickle success!");
        if (dragonAudioSource)
        {
            dragonAudioSource.Stop();
            dragonAudioSource.loop = false;
        }
    }

    // ——————————————————————————
    //          THROW LOGIC
    // ——————————————————————————

    IEnumerator ThrowSequence()
    {
        // detach sword and shield
        sword.parent = null;
        shield.parent = null;

        Rigidbody s = sword.gameObject.AddComponent<Rigidbody>();
        Rigidbody sh = shield.gameObject.AddComponent<Rigidbody>();

        s.useGravity = true;
        sh.useGravity = true;

        // zwaard MISST (lerp)
        StartCoroutine(ThrowObject(s, swordThrowTarget.position, false));

        // wacht halve seconde
        yield return new WaitForSeconds(0.6f);

        // schild TREFT
        StartCoroutine(ThrowObject(sh, shieldThrowTarget.position, true));
    }

    IEnumerator ThrowObject(Rigidbody rb, Vector3 targetPos, bool isHit)
    {
        float t = 0;
        Vector3 start = rb.transform.position;

        while (t < 1)
        {
            t += Time.deltaTime * 1.6f;
            rb.transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        if (isHit)
            RegisterShieldHit();
    }

    // ——————————————————————————
    //        DRAGON DEFEAT
    // ——————————————————————————
    void DragonDefeated()
    {
        spawner.enabled = false;

        if (victoryMusic)
        {
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }

IEnumerator AutoWalkToDragon()
{
    while (qteSwordHits)
    {
        float dist = Vector3.Distance(player.transform.position, dragon.transform.position);

        // Stop het lopen 100% en beëindig de coroutine
        if (dist <= 1.5f)
        {
            player.rb.linearVelocity = Vector3.zero;
            yield break;  // <-- STOPT HIER VOORGOED
        }

        // Wandel verder naar de draak
        Vector3 dir = (dragon.transform.position - player.transform.position).normalized;
        player.rb.linearVelocity = dir * 3f;

        yield return null;
    }
}







}
