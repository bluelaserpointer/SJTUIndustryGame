using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimationControl : MonoBehaviour
{
    private Animator animator;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Random.Range(0.0f, 5.0f);

        // Sets the animator off
        animator = GetComponent<Animator>();
        if(animator != null)
            StartCoroutine(AnimationStart());
    }

    IEnumerator AnimationStart()
    {
        animator.speed = 0;
        animator.enabled = false;
        
        yield return new WaitForSeconds(startTime);
        
        animator.enabled = true;
        animator.speed = 1;
    }
}
