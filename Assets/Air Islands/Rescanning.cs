using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rescanning : MonoBehaviour
{
    private void Start()
    {
        // Start the coroutine to rescan the A* graph every 2 seconds
        StartCoroutine(RescanGraphEveryTwoSeconds());
    }

    private IEnumerator RescanGraphEveryTwoSeconds()
    {
        while (true)
        {
            // Call the rescan method
            AstarPath.active.Scan();

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);
        }
    }
}
