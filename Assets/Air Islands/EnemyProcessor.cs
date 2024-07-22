using Fungus;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class EnemyProcessor : MonoBehaviour
{
    public Animator animator;

    // Animation states
    protected readonly string walkingAnimation = "walking";
    protected readonly string runningAnimation = "running";

    public Seeker seeker;
    public AstarPath aStarPath;
    // Start is called before the first frame update
    void Start()
    {

    }

    public virtual void AnimationSwitchTo(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    public void SwitchToWalking()
    {
        AnimationSwitchTo(walkingAnimation);
    }

    public void SwitchToRunning()
    {
        AnimationSwitchTo(runningAnimation);
    }
}
