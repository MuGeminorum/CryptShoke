using UnityEngine;
using System.Collections;

public class AimLookCharacterController : MonoBehaviour {
	
	private CharacterMotor motor;
	
	// Use this for initialization
	void Start () {
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		if (motor==null) Debug.Log("Motor is null!!");
		
		//originalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input vector from kayboard or analog stick and make it length 1 at most
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0);
		if (directionVector.magnitude>1) directionVector = directionVector.normalized;
		
		// Rotate input vector into camera space so up is camera's up and right is camera's right
		directionVector = Camera.main.transform.rotation * directionVector;
		
		// Rotate input vector to be perpendicular to character's up vector
		Quaternion camToCharacterSpace = Quaternion.FromToRotation(Camera.main.transform.forward*-1, transform.up);
		directionVector = (camToCharacterSpace * directionVector);
		
		// Apply direction
		motor.desiredFacingDirection = directionVector;
	}
}
