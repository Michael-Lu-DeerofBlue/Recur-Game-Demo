using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.EnemyVision
{
    /// <summary>
    /// Editor only object (invisible at runtime)
    /// </summary>
    
    public class Invisible : MonoBehaviour
    {
        void Start()
        {
            MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
            if(mesh != null)
                mesh.enabled = false;
        }

    }
}
