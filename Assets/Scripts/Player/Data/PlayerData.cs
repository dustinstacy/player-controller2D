using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float moveVelocity = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.35f;
    public LayerMask whatIsGround;
}
