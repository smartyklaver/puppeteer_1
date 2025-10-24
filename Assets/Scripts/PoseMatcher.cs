using UnityEngine;

public class PoseMatcher : MonoBehaviour
{
    [Header("Link je real player en ghost hier")]
    public Transform playerRoot;
    public Transform ghostRoot;

    [Header("Gewrichten die vergeleken worden")]
    public Transform[] playerJoints;
    public Transform[] ghostJoints;

    [Header("Instellingen")]
    public float maxAllowedDifference = 10f; // graden
    public float successHoldTime = 1.5f; // hoelang je pose moet aanhouden

    [HideInInspector] public bool IsPoseMatched = false;


    private float timer = 0f;

    void Update()
    {
        if (playerRoot == null || ghostRoot == null || playerJoints.Length != ghostJoints.Length)
            return;

        float totalDifference = 0f;

        for (int i = 0; i < playerJoints.Length; i++)
        {
            float angleDiff = Quaternion.Angle(playerJoints[i].rotation, ghostJoints[i].rotation);
            totalDifference += angleDiff;
        }

        float averageDifference = totalDifference / playerJoints.Length;

        if (averageDifference < maxAllowedDifference)
        {
            timer += Time.deltaTime;
            if (timer >= successHoldTime)
            {
                Debug.Log("✅ Pose matched!");
                IsPoseMatched = true;
            }
        }
        else
        {
            timer = 0f;
            IsPoseMatched = false;
        }
    }
}
