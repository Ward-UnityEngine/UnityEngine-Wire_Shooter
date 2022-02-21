using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireLeaderPysicsScript : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 9);
    }
}
