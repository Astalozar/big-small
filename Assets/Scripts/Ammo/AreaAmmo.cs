using UnityEngine;
using System.Collections;

public class AreaAmmo : Ammo {
	
	public float colliderHeight;
	public float colliderWidth;
	
	bool isActive;
	BoxCollider damageDealer;
	
	// Use this for initialization
	void Start () {
		GameObject aim = transform.FindChild("aimingPoint").gameObject;
	
		gameObject.AddComponent(typeof(BoxCollider));
		
		damageDealer = GetComponent<BoxCollider>();
		damageDealer.size = new Vector3(colliderWidth, colliderHeight, 1);
		damageDealer.center = aim.transform.localPosition;
		
		gameObject.AddComponent("Rigidbody");
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
	
	void OnTriggerEnter(Collider other) {
        if(other.tag == "Enemy") {
			other.GetComponent<CharacterBody>().damage(damage);
		}
    }
}
