using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetPosition : MonoBehaviour
{
    public Vector3[] petPositions = new Vector3[4];
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
}
