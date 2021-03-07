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
    public float turnSpeed;

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
    private Quaternion targetRotation;






    void Start()
    {
        initialPosition = gameObject.GetComponent<Transform>().position;
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
                WanderRadiusCheck();
                break;
            case AnimalState.WANDER:
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                if (Time.time-lastActTime >actRestTime)
                {
                    RandomAction();
                }
                WanderRadiusCheck();
                break;

        }
    }

    void WanderRadiusCheck()
    {
        distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        if(distanceToInitial > wanderRadius)
        {
            targetRotation = Quaternion.LookRotation(initialPosition);
        }
    }
}
