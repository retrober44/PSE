using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Superclass inherited by all the Managers
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }

            return instance;
        }
    }
}
