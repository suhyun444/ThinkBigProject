using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentChanger : MonoBehaviour
{
    [SerializeField] Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = parent;   
    }
}
