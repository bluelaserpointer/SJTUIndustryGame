using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator thisAnimator;
    private Vector3 initialPosition;
    public float wanderRadius;

    public float walkSpeed;
    public float turnSpeed=0.1f;
    public int cellCountX = 24;
    public int cellCountZ = 18;
    public enum AnimalState
    {
        EAT,
        WANDER
    }
    private AnimalState currentState = AnimalState.EAT;

    public float[] actionWeight = { 3000, 3000 };
    public float actRestTime;
    public float lastActTime;

    
    public float distanceToInitial;
    private Vector3 targetPosition;






    void Start()
    {
        initialPosition = gameObject.GetComponent<Transform>().position;
        this.transform.position = new Vector3(initialPosition.x + Random.Range(0, wanderRadius), initialPosition.y, initialPosition.z + Random.Range(-wanderRadius, wanderRadius));
        RefreshTargetPosition();
        thisAnimator = GetComponent<Animator>();
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
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                transform.LookAt(targetPosition);
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
            RefreshTargetPosition();
        }
        
    }

    void RefreshTargetPosition()
    {
        Vector3 tmpPos = new Vector3(initialPosition.x + Random.Range(0, wanderRadius), initialPosition.y, initialPosition.z + Random.Range(-wanderRadius, wanderRadius));
        while(Vector3.Distance(tmpPos,this.transform.position) < 1) { 
            tmpPos = new Vector3(initialPosition.x + Random.Range(0, wanderRadius), initialPosition.y, initialPosition.z + Random.Range(-wanderRadius, wanderRadius));
            
        }
        targetPosition = tmpPos;
    }
}
