using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireDropper : MonoBehaviour
{
    private Vector2 pos1;
    private Vector2 pos3;
    private Rigidbody2D rbWireLeader;
    private Rigidbody2D rbPlayer;
    public float dropDistance;
    public float ropeWidth;
    
    public float haulInSpeed;
    private float haulInCounter = 0;

    public float ropeEnduranceTime;
    private float ropeEnduranceCounter = 0;

    public GameObject wireSegmentPrefab;
    private GameObject wireSeg1;
    private GameObject wireSeg3;

    private LineRenderer lineRenderer;

    private List<GameObject> ropeSegmentsLeader = new List<GameObject>();
    private List<GameObject> ropeSegmentsPlayer = new List<GameObject>();
    private GameObject[] segments;

    private bool ropeSolidified = false;

    private void Awake()
    {
        rbWireLeader = GetComponent<Rigidbody2D>();
        rbPlayer = GameObject.FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody2D>() ;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        AddRopeSegmentLeader();
        pos3 = pos1;
    }

    private void Update()
    {
        if (!ropeSolidified)
        {
            bool changed = false;
            //Check for wireLeader
            Vector2 pos2 = rbWireLeader.transform.position;
            if (Vector2.Distance(pos1, pos2) > dropDistance)
            {
                AddRopeSegmentLeader();
                changed = true;
            }

            //Check for player
            Vector2 pos4 = rbPlayer.transform.position;
            if (Vector2.Distance(pos3, pos4) > dropDistance)
            {
                AddRopeSegmentPlayer();
                changed = true;
            }

            if (changed)
                DrawRope();
        }
        else
        {
            SimulateRope();
            DrawRope();

            //Check if rope should be removed
            if (ropeEnduranceCounter> ropeEnduranceTime)
            {
                OnDestroy();
                Destroy(this.gameObject);
            }
            

            
            else
            {
                ropeEnduranceCounter += Time.deltaTime;
                ///Haul in rope
                if (haulInSpeed < haulInCounter)
                {
                    HaulInRope();
                    haulInCounter = 0;
                }
                else
                {
                    haulInCounter += Time.deltaTime;
                }

            }
        }
    }

    private void AddRopeSegmentLeader()
    {
        wireSeg1 = Instantiate(wireSegmentPrefab, this.transform.position, Quaternion.identity);
        ropeSegmentsLeader.Add(wireSeg1);
        pos1 = wireSeg1.transform.position;

    }

    private void AddRopeSegmentPlayer()
    {
        wireSeg3 = Instantiate(wireSegmentPrefab, rbPlayer.transform.position, Quaternion.identity);
        ropeSegmentsPlayer.Add(wireSeg3);
        pos3 = wireSeg3.transform.position;
    }

    private void DrawRope()
    {
        
        
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        //ropeSegmentsPlayer.Reverse();

        Vector3[] pos = new Vector3[ropeSegmentsLeader.Count + ropeSegmentsPlayer.Count];



        
        for (int i = 0; i<ropeSegmentsPlayer.Count ; i++)
        {
            pos[i] = ropeSegmentsPlayer[ropeSegmentsPlayer.Count-i-1].transform.position;
            
        }
        int index = ropeSegmentsPlayer.Count;
        foreach (GameObject s in ropeSegmentsLeader)
        {
            pos[index] = s.transform.position;
            index++;
        }


        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);
    }

    public void SolidifyRope()
    {
        ropeSolidified = true;

        
    }

   

    private void SimulateRope()
    {


        /*Old simultion
         * 
         * Vector2 pos1 = rbWireLeader.position;
                
        for (int i = ropeSegmentsPlayer.Count-1; i >= 0; i--)
        {
            Vector2 pos2 = ropeSegmentsPlayer[i].transform.position;
            float distance = Vector2.Distance(pos1, pos2);
            if (distance > dropDistance)
            {
                Vector2 targetPos = new Vector2((pos2.x - pos1.x) * dropDistance / distance, (pos2.y - pos1.y) * dropDistance / distance) + pos1;
                //Debug.Log(targetPos);

                ropeSegmentsPlayer[i].GetComponent<Rigidbody2D>().MovePosition(targetPos);
            }
            pos1 = pos2;
        }
        pos1 = rbWireLeader.position;
        for (int i = ropeSegmentsLeader.Count - 1; i >= 0; i--)
        {
           Vector2 pos2 = ropeSegmentsLeader[i].transform.position;
           float distance = Vector2.Distance(pos1, pos2);
           
           if (distance > dropDistance)
           {
                Vector2 targetPos = new Vector2((pos2.x - pos1.x) * dropDistance / distance, (pos2.y - pos1.y) * dropDistance / distance) + pos1;
                //Debug.Log(targetPos);
                ropeSegmentsLeader[i].GetComponent<Rigidbody2D>().MovePosition(targetPos);
           }
           pos1 = pos2;
           }*/



        // Make it into one list in the right order
        segments = new GameObject[ropeSegmentsLeader.Count + ropeSegmentsPlayer.Count];
        for (int i = 0; i < ropeSegmentsPlayer.Count; i++)
        {
            segments[i] = ropeSegmentsPlayer[ropeSegmentsPlayer.Count - i - 1];

        }
        int index = ropeSegmentsPlayer.Count;
        foreach (GameObject s in ropeSegmentsLeader)
        {
            segments[index] = s;
            index++;
        }

        int half = Mathf.RoundToInt(segments.Length / 2);

        //Simulate
        Vector2 pos1 = rbWireLeader.position;

        for (int i = 0; i <= half; i++)
        {
            Vector2 pos2 = segments[i].transform.position;
            float distance = Vector2.Distance(pos1, pos2);
            if (distance > dropDistance)
            {
                Vector2 targetPos = new Vector2((pos2.x - pos1.x) * dropDistance / distance, (pos2.y - pos1.y) * dropDistance / distance) + pos1;
                //Debug.Log(targetPos);

                segments[i].GetComponent<Rigidbody2D>().MovePosition(targetPos);
            }
            pos1 = pos2;
        }
        pos1 = rbWireLeader.position;
        for (int i = segments.Length - 1; i > half; i--)
        {
            Vector2 pos2 = segments[i].transform.position;
            float distance = Vector2.Distance(pos1, pos2);

            if (distance > dropDistance)
            {
                Vector2 targetPos = new Vector2((pos2.x - pos1.x) * dropDistance / distance, (pos2.y - pos1.y) * dropDistance / distance) + pos1;
                //Debug.Log(targetPos);
                segments[i].GetComponent<Rigidbody2D>().MovePosition(targetPos);
            }
            pos1 = pos2;
        }

        //Insert segments where it splits up
/*
        int amountOfInserts = 0;
        bool ropeFixed = false;
        //List<GameObject> seg = new List<GameObject>(segments);

        while (!ropeFixed && amountOfInserts < 100 && ropeSegmentsLeader.Count > 0 && ropeSegmentsPlayer.Count > 0)
        {
            GameObject seg1 = ropeSegmentsPlayer[ropeSegmentsPlayer.Count - 1];
            GameObject seg2 = ropeSegmentsLeader[ropeSegmentsLeader.Count - 1];
            seg1.GetComponent<SpriteRenderer>().color = Color.red;
            seg2.GetComponent<SpriteRenderer>().color = Color.red;
            float dist = Vector2.Distance(seg1.transform.position, seg2.transform.position);
            if (dist > dropDistance)
            {
                Vector2 target = ((seg1.transform.position + seg2.transform.position) / 2);
                //Debug.Log(seg.Count);
                //Debug.Log(ropeSegmentsPlayer.Count - 1 + " index");
                ropeSegmentsLeader.Insert(0, Instantiate(wireSegmentPrefab, target, Quaternion.identity));
                amountOfInserts++;

            }
            else
            {
                ropeFixed = true;
            }
            Debug.Log("amountOfInsterts: " + amountOfInserts);
            for (int x = 0; x < amountOfInserts; x++)
            {
                if (ropeSegmentsPlayer.Count > 0)
                {
                    ropeSegmentsPlayer.RemoveAt(ropeSegmentsPlayer.Count - 1);
                }
                else if (ropeSegmentsLeader.Count > 0)
                {
                    ropeSegmentsLeader.RemoveAt(ropeSegmentsLeader.Count - 1);
                }
                else
                {
                    break;
                }
            }

        }*/
    }

    private void HaulInRope()
    {
        if(ropeSegmentsPlayer.Count > 0)
            ropeSegmentsPlayer.RemoveAt(ropeSegmentsPlayer.Count - 1);
        if(ropeSegmentsLeader.Count>0)
            ropeSegmentsLeader.RemoveAt(0);
    }
    public void OnDestroy()
    {
       /*foreach (GameObject s in ropeSegmentsLeader)
        {
            Destroy(s);
        }
        foreach (GameObject s in ropeSegmentsPlayer)
        {
            Destroy(s);
        }
        foreach (GameObject s in segments)
        {
            Destroy(s);
        }*/

        GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("RopeSegment");
        foreach (GameObject g in toDestroy)
        {
            if(g != wireSegmentPrefab)
            Destroy(g);
        }
        
    }
}
