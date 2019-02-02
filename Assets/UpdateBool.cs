using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBool : StateMachineBehaviour
{

    public bool status;
    public string targetBool;
    public bool resetOnExit;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(targetBool, status);
    }

    

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (resetOnExit)
        {
            Debug.Log("ONStateExitCalled");
            animator.SetBool(targetBool, !status);
        }
    }

    
}
