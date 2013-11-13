using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterBody))]

public class PlayerController : MonoBehaviour {
	
	// Player Handling
	
	
	public Camera mainCamera;
	
	public GameObject arm;
	public GameObject armPivot;
	
	private CharacterBody body;
	private tk2dSprite sprite;
	
	void Start () {
		body = GetComponent<CharacterBody>();
		sprite = GetComponent<tk2dSprite>();
	}
	
	void Update () {
		
		// Update arm position
		Vector2 armPoint = new Vector2(1, 0);
		arm.transform.localScale =  new Vector3(body.facing, 1, 1);
		Vector2 mousePoint = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float angle = Vector2.Angle(armPoint, mousePoint);
		if(body.facing < 0) {
			angle += 180;
		}
		if(mousePoint.y < armPoint.y) {
			angle = 360 - angle;
		}
		sprite.scale = new Vector3(body.facing * Mathf.Abs(sprite.scale.x), sprite.scale.y, sprite.scale.z);
		arm.transform.Rotate(new Vector3(0, 0, angle - arm.transform.eulerAngles.z));// armPivot.transform.position, Vector3.forward, angle - arm.transform.eulerAngles.z);
		
		if(Input.GetButtonDown("Jump")) {
			body.startJump(false);
		}

		if(Input.GetButtonUp("Jump")) {
			body.stopJump(false);
		}
		
		if(Input.GetKeyDown(KeyCode.Q)) {
			body.hook();
		}
		
		if(Input.GetKeyDown(KeyCode.R)) {
			body.rail();
		}

		
		
		body.moveBy(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), false);
		/*
		if(Input.GetKeyDown(KeyCode.Q)) {
			Collider[] foundHooks = Physics.OverlapSphere(transform.position, hookRadius, hooks);
			RaycastHit hit;
			float dist = 10000;
			int i = 0;
			int closestHook = -1;
	
			foreach(Collider hook in foundHooks) {
				float newDist = Vector3.Distance(transform.position, hook.gameObject.transform.position);
				if(Physics.Raycast(new Ray(transform.position, (hook.gameObject.transform.position - transform.position).normalized), out hit, newDist, constrainer.obstacles)) {

				} else {
					if(newDist < dist) {
						dist = newDist;
						closestHook = i;
					}
				}
				i++;
			}
			if(closestHook >= 0) {
				constrainer.isHooking = true;
				hookDestination = new Vector3(foundHooks[closestHook].gameObject.transform.position.x, foundHooks[closestHook].gameObject.transform.position.y, transform.position.z);
			}
		}
		
		
		body.moveBy(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
		
		if(Input.GetKeyDown(KeyCode.P)) {
			body.flyTo(new Vector2(8, 20));
		}
		*/
		
		
		
		
		
		/*	
		// Aplly jump
		if (constrainer.isGrounded) {
			amountToMove.y = 0;
			if(Input.GetButtonDown("Jump")) {
				amountToMove.y = jumpHeight;	
				extraJump = extraJumpHeight;
			}
		} else {
			if(Input.GetButton("Jump") && extraJump > 0) {
				float extraJumpAmount = extraJumpHeight * Time.deltaTime;
				amountToMove.y += extraJumpAmount;
				extraJump -= extraJumpAmount;
			}
		}
			
		if (constrainer.isHooked) {
			amountToMove.y = 0;	
		}
		// Apply horizontal movement and gravity
		amountToMove.x = speed * Time.smoothDeltaTime * Input.GetAxisRaw("Horizontal");	amountToMove.x = speed * Time.smoothDeltaTime * Input.GetAxisRaw("Horizontal");
		amountToMove.y -= gravity * Time.smoothDeltaTime;

		if(constrainer.isClimbing) {
			amountToMove.y = climbSpeed * Input.GetAxisRaw("Vertical") * Time.smoothDeltaTime;
		}
		
		// Try to get the nearest hook
		if(Input.GetKeyDown(KeyCode.Q)) {
			if(!constrainer.isHooked) {
				if(!constrainer.isHooking) {
					Collider[] foundHooks = Physics.OverlapSphere(transform.position, hookRadius, hooks);
					RaycastHit hit;
					float dist = 10000;
					int i = 0;
					int closestHook = -1;
			
					foreach(Collider hook in foundHooks) {
						float newDist = Vector3.Distance(transform.position, hook.gameObject.transform.position);
						if(Physics.Raycast(new Ray(transform.position, (hook.gameObject.transform.position - transform.position).normalized), out hit, newDist, constrainer.obstacles)) {
		
						} else {
							if(newDist < dist) {
								dist = newDist;
								closestHook = i;
							}
						}
						i++;
					}
					if(closestHook >= 0) {
						constrainer.isHooking = true;
						hookDestination = new Vector3(foundHooks[closestHook].gameObject.transform.position.x, foundHooks[closestHook].gameObject.transform.position.y, transform.position.z);
					}
				} else {
					constrainer.isHooking = false;
				}
			} else {
				constrainer.isHooked = false;
			}
		}
		
		bool drop = false;
		
		// If the player is moving towards the hook
		
		if(constrainer.isHooking) {
			amountToMove = (hookDestination - transform.position).normalized * hookSpeed * Time.deltaTime;
			drop = true;
		}
		
		// Define whether we should drop from platform
	
		if(!constrainer.isClimbing) {
			if(Input.GetAxisRaw("Vertical") < 0) {
				drop = true;
			}
		}
		
		constrainer.applyConstraints(amountToMove, drop);
		*/
	}
	
	// Increase n towards target by speed
	private float IncrementTowards(float n, float target, float a) {
		if (n == target) {
			return n;	
		}
		else {
			float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target-n))? n: target; // if n has now passed target then return target, otherwise return n
		}
	}
	
}
