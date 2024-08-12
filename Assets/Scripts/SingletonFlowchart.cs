using UnityEngine;
using Fungus;

public class SingletonFlowchart : Flowchart
{
    public static SingletonFlowchart Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
