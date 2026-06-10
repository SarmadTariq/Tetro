using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCull : MonoBehaviour
{
    private void Start() {
		GetComponent<MeshFilter>().mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
	}
}
