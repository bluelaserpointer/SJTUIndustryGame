
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TibetanAI : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator thisAnimator;
    private Vector3 initialPosition;
    public float wanderRadius;

    public float walkSpeed=0.5f;
    public float turnSpeed = 0.1f;
    public int cellCountX = 24;
    public int cellCountZ = 18;
    public enum AnimalState
    {
        EAT,
        WANDER
    }
    private AnimalState currentState = AnimalState.EAT;

    public float[] actionWeight = { 30000, 3000 };
    public float actRestTime = 2;
    public float lastActTime;
    public HexCell CurrentCell{
        get { return currentCell; }
        set { currentCell = value; }
    }
    public HexCell InitialCell
    {
        get { return initialCell; }
        set { initialCell = value; }
    }
    public HexCell TargetCell
    {
        get { return targetCell; }
        set { targetCell = value; }
    }


    public float distanceToInitial;
    private Vector3 targetPosition;
    public HexCell currentCell;
    public HexCell targetCell;
    private HexCell initialCell;





    void Start()
    {
        thisAnimator = GetComponent<Animator>();
        initialPosition = gameObject.GetComponent<Transform>().position;
        RefreshTargetPosition();
        this.transform.position = targetPosition;
        RefreshTargetPosition();
        RandomAction();

    }

    void RandomAction()
    {
        lastActTime = Time.time;
        float randNum = Random.Range(0, actionWeight[0] + actionWeight[1]);
        if (randNum <= actionWeight[0])
        {
            currentState = AnimalState.WANDER;
            thisAnimator.SetInteger("AnimalState", 0);
        }
        else if (randNum < actionWeight[0] + actionWeight[1])
        {
            currentState = AnimalState.EAT;
            thisAnimator.SetInteger("AnimalState", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationVector3;
        Quaternion rotation;
        switch (currentState)
        {
            case AnimalState.EAT:
                if(Time.time - lastActTime > thisAnimator.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();
                }
                ReachCheck();
                break;
            case AnimalState.WANDER:
                Vector3 currentDirection =new Vector3(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y, targetPosition.z - transform.position.z);
                
                transform.LookAt(targetPosition);
                rotationVector3 = new Vector3(0, -85, 0) + transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(rotationVector3);
                //transform.Translate(Vector3.forward * walkSpeed*10 * Time.deltaTime);
                transform.position = transform.position + currentDirection.normalized*walkSpeed * Time.deltaTime;
                
                if (Time.time-lastActTime >actRestTime)
                {
                    RandomAction();
                }
                ReachCheck();
                break;

        }
    }

    void ReachCheck()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if(distanceToTarget < 0.01)
        {
            currentCell = targetCell;
            RefreshTargetPosition();
        }
        
    }

    void RefreshTargetPosition()
    {
        List<HexCell> possibleTargetCells = GetTargetCells();
        targetCell = possibleTargetCells[Random.Range(0, possibleTargetCells.Count)];
        targetPosition = targetCell.transform.position + new Vector3(Random.Range(-HexMetrics.innerRadius+2,HexMetrics.innerRadius-2),0,Random.Range(-HexMetrics.innerRadius+2,HexMetrics.innerRadius-2));
    }

    List<HexCell> GetTargetCells()
    {
        List<HexCell> t = new List<HexCell>();
        for (int i = 0; i <= 5; i++)
        {
            if(initialCell.GetNeighbor((HexDirection)i) == currentCell)
            {
                t.Add(initialCell);
                HexCell c = initialCell.GetNeighbor(((HexDirection)i).Next());
                if(!c.IsUnderwater && Mathf.Abs(c.Elevation - currentCell.Elevation) <=1)
                    t.Add(c);
                c = initialCell.GetNeighbor(((HexDirection)i).Previous());
                if (!c.IsUnderwater && Mathf.Abs(c.Elevation - currentCell.Elevation) <= 1)
                    t.Add(c);
                return t;
            }
        }

        for(int i = 0; i <= 5; i++)
        {
            HexCell c = initialCell.GetNeighbor((HexDirection)i);
            if(!c.IsUnderwater && Mathf.Abs(c.Elevation - currentCell.Elevation) <=1)
                t.Add(c);
        }
        return t;
       
    }

    void SetNeighborIndex()
    {
        
    }
}
