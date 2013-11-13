using UnityEngine;
using System.Collections;

public class BulletAmmo : Ammo {
	public void fire(Vector2 origin, Vector2 direction) {
		Debug.Log("FIRE");
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
