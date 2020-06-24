using UnityEngine;
using System.Collections;

public class GunAimer : ExecutionOrderBehaviour {
	
	public Transform aimPivot;
	public Transform aimWeapon;
	public Transform aimTarget;
	public float effect = 1;
	
	private Vector3 aimDirection = Vector3.zero;
	private LayerMask mask;
	
	void Start () {
		// Add player's own layer to mask
		mask = 1 << gameObject.layer;
		// Add Igbore Raycast layer to mask
		mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
		// Invert mask
		mask = ~mask;
	}
	
	// Update is called once per frame
	public override void LateUpdateCustom () {
		
		if (effect <= 0)
			return;
		
		Vector3 origPos = aimWeapon.position;
		Quaternion origRot = aimWeapon.rotation;
		
		// Find pivot
		Vector3 pivot = aimPivot.position;
		
		Transform aimSpace = transform;
		
		// Find current aim direction in character space, prior to adjustment
		Vector3 pivotWeaponDirection = Quaternion.Inverse(aimSpace.rotation) * (aimWeapon.position - pivot);
		
		// Find desired aim direction in character space
		Vector3 pivotTargetDirection = Quaternion.Inverse(aimSpace.rotation) * (aimTarget.position - pivot);
		// Move direction smoothly
		pivotTargetDirection = aimDirection = Vector3.Slerp(aimDirection, pivotTargetDirection, 15*Time.deltaTime);
		
		// Get aiming rotation needed
		Quaternion rotation = Quaternion.FromToRotation(pivotWeaponDirection, pivotTargetDirection);
		
		RotateTransformAroundPointInOtherTransformSpace (aimWeapon, rotation, pivot, aimSpace);
		
		float distFraction = 0;
		
		// Calculates horizontal angle by projecting pivotWeaponDirection and
		// pivotTargetDirection on XZ plane and taking angle between them
		Vector3 weaponDir = pivotWeaponDirection;
		Vector3 targetDir = pivotTargetDirection;
		weaponDir.y = 0;
		targetDir.y = 0;
		float rotY = Vector3.Angle(weaponDir, targetDir);
		
		// Calculate distFraction based on horizontal (XZ) rotation angle
		distFraction = 1 - (rotY / 400);
		
		aimWeapon.position = pivot + (aimWeapon.position - pivot) * distFraction;
				
		if (effect <= 1) {
			aimWeapon.position = Vector3.Lerp(origPos, aimWeapon.position, effect);
			aimWeapon.rotation = Quaternion.Slerp(origRot, aimWeapon.rotation, effect);
		}
	}
	
	void RotateTransformAroundPointInOtherTransformSpace (Transform toRotate, Quaternion rotation, Vector3 pivot, Transform space) {
		Vector3 pivotToWeapon = toRotate.position - pivot;
		
		Vector3 globalPositionDelta = - pivotToWeapon + space.rotation * (rotation * (Quaternion.Inverse(space.rotation) * pivotToWeapon));
		
		toRotate.position += globalPositionDelta;
		toRotate.rotation = space.rotation * rotation * Quaternion.Inverse(space.rotation) * toRotate.rotation;
	}
	
	void OnFire () {
		Vector3 dir = aimTarget.position-aimPivot.position;
		dir.Normalize();
		Ray ray = new Ray(aimPivot.position, dir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000, mask)) {
			hit.transform.root.SendMessage("OnHit", new RayAndHit(ray, hit), SendMessageOptions.DontRequireReceiver);
		}
	}
}
