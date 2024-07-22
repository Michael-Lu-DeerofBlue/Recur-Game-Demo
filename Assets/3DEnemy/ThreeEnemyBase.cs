using IndieMarc.EnemyVision;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Unity.VisualScripting;

public class ThreeEnemyBase : MonoBehaviour
{
    public GameObject Player;
    public bool inPursuitBool;
    public float inPursuitDelayTime;
    public float stoolDelayTime;
    public float materialDelayTime;
    public bool inStool;
    public Transform[] enemySpots;
    public Material originalMaterial; 
    public Material newMaterial;
    public Transform model;
    public bool inChange;
    public bool hasKey;
    public float patrolSpeed = 3;
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
                inPursuitBool = false;
                Player.GetComponent<ThreeDPlayerBase>().gotHitByEnemy();
                Stool();
            }
        }
    }

    void FixedUpdate()
    {

    }

    IEnumerator TimerStart(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Turn the bool off
        if (!gameObject.GetComponent<EnemyFOV>().canSeePlayer && inPursuitBool)
        {
            backToPatrol();
        }
        else
        {
            StartCoroutine(TimerStart(inPursuitDelayTime));
        }
    }

    IEnumerator StoolTimerStart(float delay)
    {
        gameObject.GetComponent<Patrol>().enabled = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        inStool = true;
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);
        backToPatrol();
        inStool = false;
    }
    IEnumerator MaterialTimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        Renderer renderer = model.GetComponent<Renderer>();
        renderer.material = originalMaterial;
        inChange = false;
    }

    IEnumerator BackToPatroTimerStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        gameObject.GetComponent<Patrol>().enabled = true;
    }


    // Update is called once per frame
    public void inPursuit(Transform target)
    {
        if (!inStool)
        {
            //Debug.Log(gameObject.name + " In Pursuit at " + Time.time);
            inPursuitBool = true;
            gameObject.GetComponent<Patrol>().enabled = false;
            gameObject.GetComponent<AIDestinationSetter>().target = Player.transform;
            gameObject.GetComponent<AIDestinationSetter>().enabled = true;
            gameObject.GetComponent<EnemyFOV>().radius = 12;
            StartCoroutine(TimerStart(inPursuitDelayTime));
        }
    }

    public void inBell(Transform target)
    {
        inStool = false;
        Debug.Log(gameObject.name + " In Bell at " + Time.time);
        inPursuitBool = true;
        gameObject.GetComponent<Patrol>().enabled = false;
        gameObject.GetComponent<AIDestinationSetter>().target = target;
        gameObject.GetComponent<AIDestinationSetter>().enabled = true;
        gameObject.GetComponent<EnemyFOV>().radius = 12;
        StartCoroutine(TimerStart(10));
    }

    public void backToPatrol()
    {
        //Debug.Log(gameObject.name + " Back to Patrol at " + Time.time);
        inPursuitBool = false;
        gameObject.GetComponent<AIPath>().maxSpeed = patrolSpeed;
        int i = gameObject.GetComponent<Patrol>().targets.Length;
        int r = Random.Range(0, i);
        gameObject.GetComponent<AIDestinationSetter>().target = gameObject.GetComponent<Patrol>().targets[r];
        StartCoroutine(BackToPatroTimerStart(1));
        gameObject.GetComponent<EnemyFOV>().radius = 6;
    }

    public void Stool()
    {
        StartCoroutine(StoolTimerStart(stoolDelayTime));
    }

    public void ChangeMaterial()
    {
        if (!inChange && hasKey)
        {
            inChange = true;
            Renderer renderer = model.GetComponent<Renderer>();
            renderer.material = newMaterial;
            StartCoroutine(MaterialTimerStart(materialDelayTime));
        }
    }
}
