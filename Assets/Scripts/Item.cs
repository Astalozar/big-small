using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	
	public float walkSpeed;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void applyAttributes(CharacterBody body) {
		body.walkSpeed += walkSpeed;
	}
}
