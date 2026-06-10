using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	[SerializeField]
	private GameObject[] tetrisObjects;
	// Use this for initialization
	
	void Start () {
		SpawnRandom();
	}

	// Update is called once per frame
	public void SpawnRandom () {
		int index = Random.Range (0, tetrisObjects.Length);
		GameObject tObj = Instantiate (tetrisObjects[index], transform.position, Quaternion.identity);
		GameObject p = GameObject.FindGameObjectWithTag("tetro");
		tObj.transform.SetParent(p.transform);
	}
	private void Update() {
		if(transform.position.x > 13 || transform.position.x < 0)
		{
			float X = (14+transform.position.x)%14;
			transform.position = new Vector3(X, transform.position.y, transform.position.z);
		}
	}

}