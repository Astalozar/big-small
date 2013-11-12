using UnityEngine;
using System.Collections;

public class Exp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)) {
			Debug.Log("Boom");
			rigidbody.AddExplosionForce(1000, new Vector3(22, 2, 0), 100);
		}
	}
}
