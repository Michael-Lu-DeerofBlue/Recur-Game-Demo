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


    // Update is called once per frame
    public void inPursuit(Transform target)
    {
        if (!inStool)
        {
            //Debug.Log(gameObject.name + " In Pursuit");
            inPursuitBool = true;
            gameObject.GetComponent<Patrol>().enabled = false;
            gameObject.GetComponent<AIDestinationSetter>().target = target;
            gameObject.GetComponent<AIDestinationSetter>().enabled = true;
            gameObject.GetComponent<AIDestinationSetter>().enabled = true;
            gameObject.GetComponent<EnemyFOV>().radius = 12;
            StartCoroutine(TimerStart(inPursuitDelayTime));
        }
    }

    public void backToPatrol()
    {
        //Debug.Log(gameObject.name + " Back to Patrol");
        inPursuitBool = false;
        gameObject.GetComponent<AIDestinationSetter>().enabled = false;
        gameObject.GetComponent<Patrol>().enabled = true;
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
