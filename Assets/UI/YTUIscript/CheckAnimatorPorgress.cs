using UnityEngine;

public class CheckAnimationProgress : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Canvas parentCanvas = animator.GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            Debug.Log(parentCanvas);
            parentCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Parent Canvas not found.");
        }
    }
    public void start() {
    
    }

}

