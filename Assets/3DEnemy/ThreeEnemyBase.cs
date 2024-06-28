using IndieMarc.EnemyVision;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeEnemyBase : MonoBehaviour
{
    public GameObject Player;
    public bool inPursuitBool;
    public float delayTime;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (inPursuitBool)
        {
            float distance = Vector3.Distance(gameObject.transform.position, Player.transform.position);
            if (distance < 2f)
            {
                Debug.Log("Hits Player");
            }
        }
    }

    IEnumerator TimerStart(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Turn the bool off
        if (!gameObject.GetComponent<EnemyFOV>().canSeePlayer)
        {
            backToPatrol();
        }
        else
        {
            StartCoroutine(TimerStart(delayTime));
        }
    }


    // Update is called once per frame
    public void inPursuit(Transform target)
    {
        Debug.Log(gameObject.name + " In Pursuit");
        inPursuitBool = true;
        gameObject.GetComponent<Patrol>().enabled = false;
        gameObject.GetComponent<AIDestinationSetter>().target = target;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<EnemyFOV>().radius = 12;
        StartCoroutine(TimerStart(delayTime));
    }

    public void backToPatrol()
    {
        Debug.Log(gameObject.name + " Back to Patro;");
        inPursuitBool = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        gameObject.GetComponent<Patrol>().enabled = true;
        gameObject.GetComponent<EnemyFOV>().radius = 6;
    }
}
