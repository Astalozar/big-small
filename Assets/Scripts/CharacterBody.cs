using UnityEngine;
using System.Collections;

// Processes movement and collisions

public class CharacterBody : MonoBehaviour {
public bool lerp;
	
	public bool doPrintState = false;
	
	public LayerMask surfaces;
	public LayerMask obstacles;
	public LayerMask hooks;
	public LayerMask rails;
	
	// State variables
	private States state;
	
	public enum States
	{
	    grounded,
	    falling,
		dropping,
		climbing,
		jumping,
		hooking,
		railing,
		knocked,
		hovering,
	}
	
	private bool forcedMovement = false;
	private bool railing = false;	
	
	// Abilities
	//[HideInInspector]
	public bool canHook = false;
	//[HideInInspector]
	public bool canRail = false;
	//[HideInInspector]
	public bool attachToHook = true;
	//[HideInInspector]
	public bool infinteJump = false;
	//[HideInInspector]
	public bool airJump = false;
	
	// Parameters
	public float walkSpeed = 2;
	public float runSpeed = 2;
	
	public float flySpeed = 2;
	public float hookSpeed = 2;
	public float railSpeed = 2;
	
	public float climbSpeed = 2;
	
	public float jumpSpeed = 2;	// Initial jump height
	public float jumpHeight = 2;// Continued jump height
	
	public float hookRadius = 10;
	string lastHookName = "";
	
	// Health
	public int maxHealth = 100;
	private int currentHealth = 100;
	
	// Gravity
	public float gravity = 2;
	private float currentGravity;
	public float knockbackMultiplier = 1;
	public float knockbackSpeed = 12;
	
	// Momentum
	private float currentMomentum;
	private float momentumSpeed;
	private int momentumDirection;
	
	// Jump handling
	private float jumpResource;

	// Collider parameters
	private BoxCollider boxCollider;
	private Vector2 center;
	private Vector2 size;
	private float skin = 0.0005f;
	
	// Facing 
	[HideInInspector]
	public int facing = 1;
	
	// Temporary variables
	private Vector3 origin;
	private Ray ray;
	private RaycastHit hit;
	private Vector2 dir;
	private LayerMask mask;
	
	// Movement variables
	private Vector3 amountToMove;
	private Vector3 destination;
	private Vector3 prevPosition;
	
	// Amount of ladder blocks character contacts with
	private int ladderContacts = 0;
	
	// Accuracy when defining if we have reached destination point
	float eps = 0.05f;
	
	void Start () {
		boxCollider = GetComponent<BoxCollider>();
		center = boxCollider.center;
		size = boxCollider.size;
		origin.z = transform.position.z;
		
		currentGravity = gravity;
		state = States.grounded;
		
		currentHealth = maxHealth;
	}
	
	
	public void damage(int damage) {
		currentHealth -= damage;
		knockBack(new Vector2(1, 0.25f));
		if(currentHealth <= 0) {
			DestroyObject(gameObject);
		}
	}
	// Jumping
	
	public void startJump(bool forced) {
		if((forcedMovement && forced) || !forcedMovement) {
			if(state == States.grounded || airJump) {
				state = States.jumping;
				amountToMove.y = 0;
				jumpResource = jumpHeight;
				printState();
			}
		}
	}
	
	public void stopJump(bool forced) {
		if((forcedMovement && forced) || !forcedMovement) {
			if(state == States.jumping) {
				state = States.falling;
				printState();
			}
		}
	}
	
	// Hooking and railing
	
	public void hook() {
		if(canHook) {
			Collider[] foundHooks = Physics.OverlapSphere(transform.position, hookRadius, hooks);
			RaycastHit hit;
			float dist = 10000;
			int i = 0;
			int closestHook = -1;
	
			foreach(Collider hook in foundHooks) {
				Vector3 origin = new Vector3(transform.position.x + facing, transform.position.y + 1, transform.position.z);
				
				float newDist = Vector3.Distance(origin, hook.gameObject.transform.position);
				if(!Physics.Raycast(new Ray(origin, (hook.gameObject.transform.position - transform.position).normalized), out hit, newDist, obstacles)) {
					if(newDist < dist && hook.gameObject.name != lastHookName) {
						dist = newDist;
						closestHook = i;
					}
				}
				i++;
			}
			if(closestHook >= 0) {
				lastHookName = foundHooks[closestHook].gameObject.name;
				hookTo(new Vector2(foundHooks[closestHook].gameObject.transform.position.x, foundHooks[closestHook].gameObject.transform.position.y));
			}
		}
	}
	
	public void rail() {
		if(canRail) {
			RaycastHit hit;
			if(Physics.Raycast(new Ray(transform.position, Vector3.up), out hit, 200, rails)) {
				if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Rails")) {
					float d = ((BoxCollider)hit.collider).size.y / 2 + ((BoxCollider)collider).size.y / 2;
					railTo(new Vector2(transform.position.x, hit.collider.gameObject.transform.position.y - d));
				}
			}
		}
	}
	
	// Movement orders
	
	public void moveTo(Vector2 position) {
		if(state != States.knocked) {
			forcedMovement = true;
			destination = position;
		}
	}
	
	public void flyTo(Vector2 position) {
		forcedMovement = true;
		destination = position;
		state = States.hooking;
	}
	
	public void hookTo(Vector2 position) {
		if(state != States.railing) {
			forcedMovement = true;
			destination = position;
			state = States.hooking;
		}
	}
	
	public void railTo(Vector2 position) {
		forcedMovement = true;
		railing = true;
		hookTo(position);
	}
	
	public void knockBack(Vector2 power) {
		if(state != States.knocked) {	
			forcedMovement = true;
			destination = new Vector3(transform.position.x - facing * power.x, transform.position.y + power.y);			
			state = States.knocked;
			printState();
		}
	}
	
	// Change position according to the input and current state
	public void moveBy(Vector2 direction, bool forced) {
		
		if(state == States.railing && direction.y < 0 ) {
			state = States.falling;
			printState();
			forcedMovement = false;
		}
		
		if((forcedMovement && forced) || !forcedMovement) {
			// If we are near a ladder, are in an appropriate state and are attempting to move up or down
			if(ladderContacts > 0) {
				if(direction.y != 0) {
					if(state != States.climbing && /*(state != States.dropping || direction.y > 0) &&*/ state != States.hooking && state != States.railing && state != States.knocked) {
						state = States.climbing;
						printState();
					}
				}
			} else {
				// If there are no ladders nearby and we are climbing - stop climbing
				if(state == States.climbing) {
					state = States.falling;
					printState();
				} else {
					// If we are grounded
					if(state == States.grounded && direction.y < 0) {
						state = States.dropping;
						printState();
					} else {
						if(state == States.dropping && direction.y >= 0) {
							state = States.falling;
							printState();
						}
					}
				}
			}
			
			// If we are on a hook and press down, start falling 
			if(!forcedMovement && state == States.hooking && (direction.y != 0 || direction.x != 0)) {
				state = States.falling;
				printState();
			}
			
			
			switch (state) {
				case States.grounded:
					amountToMove.x = direction.x * walkSpeed * Time.smoothDeltaTime;
					applyMomentum();
					amountToMove.y = -currentGravity * Time.deltaTime;
					break;
				case States.falling:
					amountToMove.x = direction.x * walkSpeed * Time.smoothDeltaTime;
					applyMomentum();
					amountToMove.y -= currentGravity * Time.deltaTime;
					break;
				case States.dropping:
					amountToMove.x = direction.x * walkSpeed * Time.smoothDeltaTime;
					applyMomentum();
					amountToMove.y -= currentGravity * Time.deltaTime;
					break;
				case States.climbing:
					amountToMove.x = direction.x * walkSpeed * Time.smoothDeltaTime;
					amountToMove.y = direction.y * climbSpeed * Time.smoothDeltaTime;
					break;
				case States.jumping:
					amountToMove.x = direction.x * walkSpeed * Time.smoothDeltaTime;
					applyMomentum();
					if(jumpResource > 0 || infinteJump) {
						float toJump = jumpSpeed * Time.deltaTime;
						amountToMove.y = toJump - currentGravity * Time.smoothDeltaTime;
						jumpResource -= toJump;
						//jumpResource = 0;
					} else {
						stopJump(true);
					}
					break;	
				case States.hooking:
					amountToMove.x = (destination - transform.position).normalized.x * hookSpeed * Time.smoothDeltaTime;
					amountToMove.y = (destination - transform.position).normalized.y * hookSpeed * Time.smoothDeltaTime;
					break;
				case States.railing:
					amountToMove.x = (destination - transform.position).normalized.x * railSpeed * Time.smoothDeltaTime;
					amountToMove.y = (destination - transform.position).normalized.y * railSpeed * Time.smoothDeltaTime;
					currentMomentum += Mathf.Abs(amountToMove.x / 2);
					break;
				case States.knocked:
					amountToMove.x = (destination - transform.position).normalized.x * knockbackSpeed * Time.smoothDeltaTime;
					amountToMove.y = (destination - transform.position).normalized.y * knockbackSpeed * Time.smoothDeltaTime;
					break;
				default:
					amountToMove.x = 0;
					amountToMove.y = 0;
					break;
			}
			applyConstraints(amountToMove, false);
		}
	}
	
	// If we were issued a move order, execute it here
	void Update() {
		if(forcedMovement) {
			prevPosition = transform.position;
			moveBy((destination - transform.position).normalized, true);
			
			switch(state) {
			case States.railing:
				destination = new Vector3(transform.position.x + railSpeed * facing, transform.position.y, transform.position.z);
				break;
			case States.hooking:
				if(Mathf.Abs(Vector2.Distance(transform.position, destination)) < eps) {
					if(railing) {
						railing = false;
						currentMomentum = 0;
						momentumDirection = facing;
						state = States.railing;
						printState();
					} else {
						if(!attachToHook) {
							state = States.falling;
							printState();
						}
						forcedMovement = false;
					}

				} 
				break;
			default:
				if(Mathf.Abs((transform.position.x - destination.x)) < eps) {
					forcedMovement = false;
					state = States.falling;
					printState();
					
				} else {
					if(Mathf.Abs(transform.position.x - prevPosition.x) == 0) {
						startJump(true);
					}
				}
				break;
			}
			
			
		}
	}
		
	// Make sure we don't pass through objects we shouldn't
	public void applyConstraints(Vector3 moveBy, bool drop) {
			
		dir.x = 0;//Mathf.Sign(moveBy.x);
		dir.y = Mathf.Sign(moveBy.y);
		
		float dist;
		
		// Vertical movement
		
		switch (state) {
		case States.grounded:
			dir.y = -1;
			mask = obstacles + surfaces;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Nothing beneath our feet
			if(dist == 0) {
				state = States.falling;
				printState();
			}
			break;
		case States.falling:
			dir.y = -1;
			mask = obstacles + surfaces;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit the ground
			if(dist != 0) {
				state = States.grounded;
				printState();
			}
			break;
		case States.dropping:
			dir.y = -1;
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit the ground
			if(dist != 0) {
				state = States.grounded;
				printState();
			}
			break;
		case States.climbing:
			dir.y = Mathf.Sign(moveBy.y);
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit something below us
			if(dist != 0 && dir.y < 0) {
				state = States.grounded;
				printState();
			}
			break;
		case States.jumping:
			dir.y = 1;
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit something above us
			if(dist != 0) {
				state = States.falling;
				printState();
			}
			break;	
		case States.hooking:
			dir.y = Mathf.Sign(moveBy.y);
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit something 
			if(dist != 0) {
				state = States.falling;
				printState();
				forcedMovement = false;
			}
			break;
		case States.railing:
			dir.y = Mathf.Sign(moveBy.y);
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			break;
		case States.knocked:
			dir.y = 1;
			mask = obstacles;
			dist = castRays(dir, Mathf.Abs(moveBy.y), mask);
			// Hit something above us
			if(dist != 0) {
				state = States.falling;
				printState();
			}
			break;
		default:
			dist = 0;
			break;
		}
		
		// Apply vertical constraints
		if(dist != 0) {
			if(dist > skin) {
				moveBy.y = (dist - skin) * dir.y;
			} else {
				moveBy.y = 0;
			}
		}
		
		dir.x = Mathf.Sign(moveBy.x);
		
		// Horizontal movement
		if(moveBy.x != 0) {	
			if(state != States.knocked) {
				facing = (int)Mathf.Sign(moveBy.x);
			}
			dist = castRays(new Vector2(dir.x, 0), moveBy.x, obstacles);
			if(dist != 0) {
				if(state == States.hooking || state == States.railing || state == States.knocked) {
					state = States.falling;
					printState();
					forcedMovement = false;
				}
				if(dist > skin) {
					moveBy.x = (dist - skin) * dir.x;
				} else {
					moveBy.x = 0;
				}
				currentMomentum = 0;
			}	
		}
		
		transform.Translate(moveBy);
	}
	
	// Helper method for raycasting
	float castRays(Vector2 direction, float length, LayerMask mask) {
		for(int i = 0; i <3; i++) {
			// Point of origin
			origin.x = transform.position.x + center.x + size.x / 2 * direction.x + (-1 + i) * size.x / 2 * direction.y;
			origin.y = transform.position.y + center.y + size.y / 2 * direction.y + (-1 + i) * size.y / 2 * direction.x;

			ray = new Ray(origin, direction);
			Debug.DrawRay(ray.origin, ray.direction, Color.green);
			// If we hit something
			if(Physics.Raycast(ray, out hit, Mathf.Abs(length) + skin, mask)) {
				return hit.distance;
			}
		}
	
		return 0;
	}
	
	// Actions when player enters specific areas
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Ladder") {
			ladderContacts++;
		}
		if(other.tag == "Door") {
			knockBack(new Vector2(1, 0.25f));
		}
		if(other.tag == "DoorSensor") {
			((DoorSensor)(other.GetComponent<DoorSensor>())).openDoor();
		}
		if(other.tag == "Enemy" && state != States.knocked) {
			knockBack(new Vector2(1, 0.25f));
		}
    }
	
	void OnTriggerExit(Collider other) {
        if(other.tag == "Ladder") {
			ladderContacts--;
		}
		if(other.tag == "DoorSensor") {
			((DoorSensor)(other.GetComponent<DoorSensor>())).closeDoor();
		}

    }
	
	
	void printState() {
		if(doPrintState) {
			Debug.Log(state);
		}
	}
	
	
	void applyMomentum() {
		float momentum;
		if(currentMomentum > 0) {
			momentum = momentumDirection * currentMomentum * Time.smoothDeltaTime;
			Debug.Log(momentum + " " + amountToMove.x);
			if(Mathf.Abs(momentum) > eps && Mathf.Abs(momentum) > Mathf.Abs(amountToMove.x)) {
				amountToMove.x += momentum;
				currentMomentum -= Mathf.Abs(momentum);
			} else {
				amountToMove.x += momentum;
				currentMomentum = 0;
			}
		}
	}
}
