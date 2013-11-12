using UnityEngine;
using System.Collections;


public class DoorSection : MonoBehaviour {
	
	[HideInInspector] 
	public Vector3 openedPosition;
	[HideInInspector]
	public Vector3 closedPosition;
	[HideInInspector]
	public float defaultSpeed;
	[HideInInspector]
	public float lockdownSpeed;
	
	[HideInInspector]
	public bool isActive = true;
	[HideInInspector]
	public bool isOpening = false;
	[HideInInspector]
	public bool isClosing = false;
	
	float moveSpeed;
// Update is called once per frame
	void Update () {		
		if(isActive) {
			if(isOpening) {
				if(transform.position.y + moveSpeed * Time.deltaTime > openedPosition.y) {
					transform.position = openedPosition;
					isOpening = false;
				} else {
					transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime, transform.position.z);
				}
			} else {
				if(isClosing) {
					if(transform.position.y - moveSpeed * Time.deltaTime < closedPosition.y) {
						transform.position = closedPosition;
						isClosing = false;
					} else {
						transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed * Time.deltaTime, transform.position.z);
					}
				}
			}
		}
	}
	
	public void open() {
		isClosing = false;
		isOpening = true;
		moveSpeed = defaultSpeed;
	}
	
	public void close(bool lockdown) {
		isOpening = false;
		isClosing = true;
		if(lockdown) {
			moveSpeed = lockdownSpeed;
		} else {
			moveSpeed = defaultSpeed;
		}
	}
	
	
}
