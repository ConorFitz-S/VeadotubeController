using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object mLock = new object();
    private static T mInstance;
    public static T Instance
    {
        get
        {
            lock (mLock)
            {
                if (mInstance == null)
                {
                    // try to find it
                    T[] instances = FindObjectsOfType(typeof(T)) as T[];

                    // couldn't find any object
                    if (instances == null || instances.Length == 0)
                    {
                        Debug.LogWarning($"Could not find singleton {typeof(T)}!");
                    }
                    else
                    {
                        // see if there's more than one, if so, do something about it
                        if (instances.Length > 1)
                        {
                            Debug.LogError($"Found more than 1 instance of the singleton {typeof(T)} so destorying the others!");
                            /*
                            for (int i = 1, len = instances.Length; i < len; i++)
                            {
                                DestroyImmediate(instances[i]);
                            }
                            */
                        }
                        else if (instances.Length == 1)
                        {
                        }
                        mInstance = instances[0];
                    }
                }
                return mInstance;
            }
        }
    }
}
