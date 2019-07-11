using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPlayerController : MonoBehaviour {

	public Vector3 basePos;
	public Vector3 offset = Vector3.zero;
	public float radiusBounds;
	public GameObject holdPoint;
	public Camera playerCamera;


	public GameObject HeldObject = null;

	public float interactionDistance = 1.0f;
	public Vector2 pitchBounds = new Vector2(-45, 45);
	private float internalRotation = 180.0f;
	public float mouseSensitivity = 0.50f;
	public float rotationSpeed = 1.0f;
	public float pitchSpeed = 1.0f;
	private float cameraPitch = 0.0f;

	private PositionTracker posTracker;

	public float speed = 1.0f;
	// Use this for initialization
	void Start () {
		basePos = this.transform.position;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		posTracker = new PositionTracker(2);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Fire1")) {
			if (HeldObject == null) {
				//Cast a ray to see if we are clicking on a pickupable
				RaycastHit rayInfo;
				//Ray, ray info out, max distance
				Physics.Raycast(playerCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)), out rayInfo, interactionDistance);
				if (rayInfo.collider != null && rayInfo.collider.gameObject.tag == GameData.objectTag) {
					HeldObject = rayInfo.collider.gameObject;
					HeldObject.GetComponent<Rigidbody>().useGravity = false;
					FMODUnity.RuntimeManager.PlayOneShot("event:/Pickup");
				}
				else if (rayInfo.collider != null && rayInfo.collider.gameObject.GetComponent<VRPhysicalButton>() != null) {
					rayInfo.collider.gameObject.GetComponent<VRPhysicalButton>().action.Invoke();
				}
			}
		}

		//Update the object if we are holding one
		if (HeldObject != null) {
			//HeldObject.transform.position = holdPoint.transform.position;
			HeldObject.GetComponent<Rigidbody>().MovePosition(holdPoint.transform.position);
			//HeldObject.transform.rotation = playerCamera.transform.rotation;
			HeldObject.GetComponent<Rigidbody>().MoveRotation(holdPoint.transform.rotation);
			posTracker.Add(holdPoint.transform.position);

		}

		if (Input.GetButtonUp("Fire2")) {
			if (HeldObject != null) {
				//Fake log two positions
				Vector3 v = (holdPoint.transform.position - playerCamera.gameObject.transform.position).normalized * 0.22f;
				posTracker.Add(holdPoint.transform.position - v + (Vector3.down * 0.12f));
				posTracker.Add(holdPoint.transform.position);
				ReleaseObject();
			}
		}
		else if (Input.GetButtonUp("Fire1")) {
			if (HeldObject != null) {
				ReleaseObject();
			}
		}



		handleMovement();
		handleCamera();
	}

	private void ReleaseObject() {
		if (HeldObject == null)
			return;
		FMODUnity.RuntimeManager.PlayOneShot("event:/Drop");
		HeldObject.GetComponent<Rigidbody>().useGravity = true;
		HeldObject.GetComponent<Rigidbody>().AddForce(posTracker.GetVelocity(), ForceMode.Impulse);
		HeldObject = null;
		posTracker.Clear();
	}


	private void handleMovement() {
		float anteralMovement = ((Input.GetKey(KeyCode.W)) ? 1 : 0) + ((Input.GetKey(KeyCode.S)) ? -1 : 0);
		float lateralMovement = ((Input.GetKey(KeyCode.A)) ? -1 : 0) + ((Input.GetKey(KeyCode.D)) ? 1 : 0);

		float sum = Mathf.Abs(anteralMovement) + Mathf.Abs(lateralMovement);
		//Normalize the movement vector
		if (sum >= 1) {
			sum = 1.0f / sum;
			anteralMovement *= sum;
			lateralMovement *= sum;
		}
		//Translate this local oriented movement direction into one that makes sense in world coordinates
		Vector3 moveDir = transform.TransformDirection(new Vector3(lateralMovement, 0.0f, anteralMovement));

		//Modify the offset
		offset += moveDir * Time.deltaTime * speed;

		if (offset.magnitude > radiusBounds)
			offset = offset.normalized * radiusBounds;

		//Apply the movement
		this.gameObject.transform.position = basePos + offset;
	}

	/// <summary>
	/// Handles camera orientation change based on player input
	/// </summary>
	private void handleCamera() {
		/*
		 	else if (axis == Axis.RightHorizontal) {
				ret = Input.GetAxis("Mouse X") * instance.mouseSensitivity;
			}
			else if (axis == Axis.RightVertical) {
				ret = Input.GetAxis("Mouse Y") * instance.mouseSensitivity;
				if (instance.invertedMouse == false) ret *= -1; 
			}
		 */


		//Camera is handled by rotating our player model left and right
		internalRotation += Input.GetAxis("Mouse X") * mouseSensitivity * rotationSpeed * Time.fixedDeltaTime;
		this.transform.localRotation = Quaternion.Euler(0.0f, internalRotation, 0.0f);

		//Camera is Handled by rotation our camera up and down.
		cameraPitch += Input.GetAxis("Mouse Y") * mouseSensitivity * pitchSpeed * Time.fixedDeltaTime * -1.0f;
		if (cameraPitch < pitchBounds.x) cameraPitch = pitchBounds.x;
		if (cameraPitch > pitchBounds.y) cameraPitch = pitchBounds.y;

		playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);
	}
}
