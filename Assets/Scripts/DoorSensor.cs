using UnityEngine;
using System.Collections;

public class DoorSensor : MonoBehaviour {
	
	public Door door;
	
	public void openDoor() {
		door.open();
	}
	
	public void closeDoor() {
		door.close(false);
	}
}
