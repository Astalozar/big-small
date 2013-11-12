using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	public Ammo ammo;
	public GameObject firingPoint;
	public GameObject aimingPoint;
	
	
	// Use this for initialization
	void Start () {
		ammo = GetComponent<Ammo>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			fire();
		}
	}
			
	public void fire() {
		Vector2 origin = firingPoint.transform.position;
		Vector2 direction = (aimingPoint.transform.position - firingPoint.transform.position).normalized;
		ammo.fire(origin, direction);
	}
}

