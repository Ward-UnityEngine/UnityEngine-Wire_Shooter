using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireLeaderFollowPlayer : MonoBehaviour
{
    private Transform playerTR;

    private void Start()
    {
        playerTR = FindObjectOfType<PlayerMovement>().GetComponent<Transform>();
    }
    private void Update()
    {
        this.transform.position = playerTR.position;
    }
}
