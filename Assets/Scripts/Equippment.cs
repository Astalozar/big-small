using UnityEngine;
using System.Collections;

public class Equippment : MonoBehaviour {
	
	public Item firstItem;
	CharacterBody body;
	
	// Use this for initialization
	void Start () {
		body = GetComponent<CharacterBody>();
		firstItem.applyAttributes(body);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
