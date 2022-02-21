using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Transform playerTR;
    public Rigidbody2D playerRB;
    public GameObject wireLeaderPrefab;
    public float wireLeaderSpeed;
    public float retractForce;
    public bool retractable = false;
    private Rigidbody2D rbWireLeader;
    public float distanceBeforeCatch;

    //Starting values
    GameObject wireLeader = null;
    private bool caught = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        AngleMouse(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        FollowPlayer();

        //MouseInput
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetKey(KeyCode.Mouse1) && wireLeader != null)
        {
            Retract();
        }

    }
    private void FixedUpdate()
    {

    }

    private void FollowPlayer()
    {
        this.transform.position = playerTR.position;
    }

    private void AngleMouse(Vector3 mouseCor)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2((mouseCor.x - playerTR.position.x), (mouseCor.y - playerTR.position.y)) * Mathf.Rad2Deg);
    }

    private void Shoot()
    {

        if (wireLeader != null)
        {
            //wireLeader.GetComponent<WireDropper>().OnDestroy();
            DestroyImmediate(wireLeader);
            retractable = false;
        }
        wireLeader = Instantiate(wireLeaderPrefab, this.transform.position, Quaternion.identity);
        float xSpeed = this.transform.up.x * wireLeaderSpeed + playerRB.velocity.x;
        float ySpeed = this.transform.up.y * wireLeaderSpeed + playerRB.velocity.y;
        rbWireLeader = wireLeader.GetComponent<Rigidbody2D>();
        rbWireLeader.velocity = new Vector2(xSpeed, ySpeed);

        Invoke("retractableTrue", 0.5f);
        caught = false;


    }

    private void Retract()
    {
        if (retractable)
        {
            float distance = (Mathf.Sqrt(Mathf.Pow(rbWireLeader.transform.position.x - this.transform.position.x, 2) + (Mathf.Pow(rbWireLeader.position.y - this.transform.position.y, 2))));
            rbWireLeader.AddForce((-Vector2.ClampMagnitude(new Vector2(rbWireLeader.transform.position.x - this.transform.position.x, rbWireLeader.position.y - this.transform.position.y), 1f)) * retractForce * Time.deltaTime   /*   /Mathf.Pow(distance,1)   */   );

            //Debug.Log("Distance is " + distance);
            if (distance <= distanceBeforeCatch && !caught)
            {
                wireLeader.GetComponent<WireLeaderFollowPlayer>().enabled = true;
                wireLeader.GetComponent<SpriteRenderer>().enabled = false;
                caught = true;
                wireLeader.GetComponent<WireDropper>().SolidifyRope();
            }
        }
    }

    private void retractableTrue()
    {
        retractable = true;
    }


}
