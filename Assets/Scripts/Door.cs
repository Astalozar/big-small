using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour {


	public GameObject doorPrefab;
	public GameObject sensorPrefab;
	public LayerMask ground;
	
	public float defaultSpeed = 6;
	public float lockdownSpeed = 1;
	
	float sectionCount;
	float size;
	
	ArrayList sections;
	
	// Use this for initialization
	void Start () {
		size = ((BoxCollider)(collider)).size.y;
		
		RaycastHit hit;
		sections = new ArrayList();
		
		if(Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 10 * size, ground)) {
			float dist = hit.distance + size / 2;
			int numberOfSegments = (int)(dist / size);
	
			// Init all sections
			for(int i = 0; i < numberOfSegments; i++) {
				Vector3 sectionClosedPosition = new Vector3(transform.position.x, transform.position.y - size * i);
				Vector3 sectionOpenedPosition = new Vector3(transform.position.x, transform.position.y - size * i + size * numberOfSegments);
				GameObject section = (GameObject)Instantiate(doorPrefab, sectionClosedPosition, transform.rotation);
				DoorSection script = (DoorSection)section.GetComponent<DoorSection>();
				script.closedPosition = sectionClosedPosition;
				script.openedPosition = sectionOpenedPosition;
				script.defaultSpeed = defaultSpeed;
				script.lockdownSpeed = lockdownSpeed;
				script.gameObject.tag = gameObject.tag;
				script.gameObject.layer = LayerMask.NameToLayer("Doors");//gameObject.layer;
				sections.Add(script);
			}
			
			// Init sensor
			Vector3 sensorPosition = new Vector3(transform.position.x, transform.position.y + size / 2 - dist / 2, transform.position.z);
			GameObject sensor = (GameObject)Instantiate(sensorPrefab, sensorPosition, transform.rotation);
			((BoxCollider)sensor.collider).size = new Vector3(size * 5, size * numberOfSegments, 0.5f);
			DoorSensor sensorScript = (DoorSensor)sensor.GetComponent<DoorSensor>();
			sensorScript.door = this;
		}
		
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.O)) {
			open();
		} 
		if(Input.GetKeyDown(KeyCode.C)) {
			close(false);
		} 
		if(Input.GetKeyDown(KeyCode.L)) {
			close(true);
		} 
	}
	
	public void open() {
		foreach(DoorSection section in sections) {
			section.open();
		}
	}
	
	public void close(bool lockdown) {
		foreach(DoorSection section in sections) {
			section.close(lockdown);
		}
	}
	
}
