// PlayerController.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bool inputEnabled = true;

    [Header("References")]
    public Rigidbody rb;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!inputEnabled) return;

    }


}
