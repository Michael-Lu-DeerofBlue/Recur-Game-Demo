using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves an object randomly around the screen
public class MoveRandomly : MonoBehaviour
{
    IEnumerator Start()
    {
        while(true)
        {
            transform.position = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), transform.position.z);
            yield return new WaitForSeconds(3f);
        }
    }
}
