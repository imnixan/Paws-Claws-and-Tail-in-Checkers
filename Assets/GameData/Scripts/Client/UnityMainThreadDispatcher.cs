using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private static UnityMainThreadDispatcher _instance = null;

    public static bool Exists
    {
        get { return _instance != null; }
    }

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (!Exists)
            {
                throw new Exception(
                    "UnityMainThreadDispatcher doesn't exist in the scene. Make sure to add the UnityMainThreadDispatcher prefab to your scene."
                );
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        while (_executionQueue.Count > 0)
        {
            Action action = null;
            lock (_executionQueue)
            {
                action = _executionQueue.Dequeue();
            }

            action?.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}
