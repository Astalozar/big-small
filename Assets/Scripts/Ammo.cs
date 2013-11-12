using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour {
	
	
	public LayerMask hittableObjects;
	public float maxDistance = 20;
	public int damage = 20;
	
	public void fire(Vector2 origin, Vector2 direction) {
		RaycastHit hit;
		if(Physics.Raycast(origin, direction, out hit, maxDistance, hittableObjects)) {
			CharacterBody target = hit.collider.gameObject.GetComponent<CharacterBody>();
			if(target != null) {
				Debug.Log("hit!");
				target.damage(damage);
			}
		}
	}
}
