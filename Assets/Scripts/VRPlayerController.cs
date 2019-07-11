using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRPlayerController : MonoBehaviour {

	public Vector3 indicatorOffset;
	public GameHand leftHand;
	public GameHand rightHand;

	private PrimitiveType _primitiveType = PrimitiveType.Sphere;

	public GameHand[] handSet;


	private void ControllerMove(GameHand hand, SteamVR_Behaviour_Pose a, SteamVR_Input_Sources b) {
		hand.indicator.transform.position = a.transform.position + indicatorOffset;
		if (hand.grabbedObject != null) {
			hand.UpdateGrabbedObject();
		}
	}



	public void Start() {
		handSet = new GameHand[] { leftHand, rightHand };
		leftHand.controller.onTransformChangedEvent += (SteamVR_Behaviour_Pose a, SteamVR_Input_Sources b) => { ControllerMove(leftHand, a, b); };
		rightHand.controller.onTransformChangedEvent += (SteamVR_Behaviour_Pose a, SteamVR_Input_Sources b) => { ControllerMove(rightHand, a, b); };
	}


	public void Update() {

		foreach (GameHand hand in handSet) {
			if (hand.OnGrabDown() && hand.grabbedObject == null) {
				Debug.Log("Grab down");
				IndicatorCollisionTracker ICT = hand.indicator.GetComponent<IndicatorCollisionTracker>();
				if (ICT.objInRange.Count > 0)
					hand.GrabObject(ICT.objInRange[0]);

			}
			if (hand.OnGrabUp()) {
				Debug.Log("Grab Up");

				if (hand.grabbedObject != null)
					hand.ReleaseObject();
			}
			if (hand.OnGrabStay()) {
				//Debug.Log("Grab stay");
				if (hand.grabbedObject != null)
					hand.UpdateGrabbedObject();
			}
		}
	}

	[System.Serializable]
	public class GameHand {

		public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

		public SteamVR_Behaviour_Pose controller;
		public GameObject grabbedObject;
		public GameObject indicator;
		public string handName = "Unnamed Hand";
		private PositionTracker posTracker = new PositionTracker(2);

		public bool OnGrabDown() {
			return grab.GetStateDown(controller.inputSource);
		}

		public bool OnGrabUp() {
			return grab.GetStateUp(controller.inputSource);
		}

		public bool OnGrabStay() {
			return grab.GetState(controller.inputSource) && !OnGrabDown();
		}


		public void GrabObject(GameObject obj) {
			FMODUnity.RuntimeManager.PlayOneShot("event:/Pickup", indicator.transform.position);
			grabbedObject = obj;
			//Set its physics settings to be controlled by us.
			Rigidbody rbg = obj.GetComponent<Rigidbody>();
			rbg.isKinematic = true;
			posTracker.Add(indicator.gameObject.transform.position);
		}

		public void ReleaseObject() {
			if (grabbedObject == null)
				return;
			FMODUnity.RuntimeManager.PlayOneShot("event:/Drop", indicator.transform.position);
			//Return its physics settings to not be controlled by us
			Rigidbody rbg = grabbedObject.GetComponent<Rigidbody>();
			rbg.isKinematic = false;
			rbg.AddForce(posTracker.GetVelocity() * 5.0f, ForceMode.Impulse);
			grabbedObject = null;
			posTracker.Clear();
		}

		public void UpdateGrabbedObject() {
			if (grabbedObject == null)
				return;

			//Update the position and orientation of the grabbed object to match our hand.
			posTracker.Add(indicator.gameObject.transform.position);

			grabbedObject.transform.position = indicator.transform.position - (grabbedObject.transform.up * 0.5f * 0.25f); // *half of its up, modified by scale (scale is not dynmically calculated)
			grabbedObject.transform.rotation = controller.transform.rotation;
		}
	}
}
