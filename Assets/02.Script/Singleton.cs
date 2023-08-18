using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                if(obj != null)
                {
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    public void InitSingleTon(T singletonObject)
    {
        if (instance == null)
        {
            instance = singletonObject;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
