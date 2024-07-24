using Fungus;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool inPursuit;
    public bool inStool;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
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

    public void SwitchToNone()
    {
        AnimationSwitchTo("None");
    }

    public virtual void Update()
    {
        //Debug.Log(inPursuit);
        if (inPursuit)
        {
            Player = GameObject.Find("Player");
            float distance = Vector3.Distance(gameObject.transform.position, Player.transform.position);
            if (distance < 2f)
            {
                //Debug.Log("hit");
                inPursuit = false;
                inStool = true;
                Stool();
                Player.GetComponent<ThreeDPlayerBase>().gotHitByEnemy();
            }
        }
    }


    public void Stool()
    {
        SwitchToNone();
        StartCoroutine(StoolTimerStart(3f));
    }

    IEnumerator StoolTimerStart(float delay)
    {
        //Debug.Log("turning off");
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        inPursuit = true;
        inStool = false;
       // Debug.Log("turning on");
       SwitchToRunning();

    }
}
