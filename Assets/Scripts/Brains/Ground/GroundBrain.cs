using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterBody))]

public class GroundBrain : MonoBehaviour {
	
	public LayerMask targets;
	public float searchRadius;
	
	private CharacterBody body;
	private GameObject target;
	
	private Vector2 destination;
	private bool lockedOn = false;
	
	// Use this for initialization
	void Start () {
		body = GetComponent<CharacterBody>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(body.facing, 1, 1);
		
		if(!lockedOn) {
			Collider[] foundTargets = Physics.OverlapSphere(transform.position, searchRadius, targets);
			if(foundTargets.Length > 0) {
				target = foundTargets[0].gameObject;
				lockedOn = true;
			}
		} else {
			body.moveTo(new Vector2(target.transform.position.x, transform.position.y));
		}
		/*
		float dist = target.transform.position.x - transform.position.x;
		if(Mathf.Abs(dist) > eps) {
			int direction = (int)Mathf.Sign(dist);
			body.moveBy(new Vector2(direction, 0), false);
		}
		*/
		
		
		/*
		if(refreshTime <= 0) {
			Collider[] foundTargets = Physics.OverlapSphere(transform.position, searchRadius, targets);
			if(foundTargets.Length > 0) {
				destination = new Vector2(foundTargets[0].gameObject.transform.position.x, transform.position.y);
				body.moveTo(destination);
			}
			refreshTime = refreshRate;
		} else {
			refreshTime -= Time.deltaTime;
			body.moveBy(Vector2.zero, false);
		}
		*/
	}
}
