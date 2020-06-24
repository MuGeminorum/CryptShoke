using UnityEngine;
using System.Collections;

[System.Serializable]
public class AimArm {
	public Transform shoulder;
	public Transform elbow;
	public Transform wrist;
	[System.NonSerialized] public Matrix4x4 handRelativeToGun;
}

public class ArmIK : ExecutionOrderBehaviour {
	
	public AnimationClip aimPose;
	public AnimationClip legSnapPose;
	public Transform aimWeapon;
	public Transform legSnap;
	public AimArm[] arms;
	public float arm0adjustment = 1;
	public float arm1adjustment = 1;
	//[System.NonSerialized] 
	private Matrix4x4 weaponRelativeToLeg;
	public float weaponAdjustment = 0;
	
	private IK1JointAnalytic ikSolver = new IK1JointAnalytic();
	
	// Use this for initialization
	void Start () 
	{
		// Get relative transform of weapon relative to a leg
		gameObject.SampleAnimation(legSnapPose, 0);
		weaponRelativeToLeg = Util.RelativeMatrix(aimWeapon, legSnap);
		
		// Get relative transforms of hands relative to gun
		gameObject.SampleAnimation(aimPose, 0);
		for (int arm=0; arm<arms.Length; arm++) {
			arms[arm].handRelativeToGun = Util.RelativeMatrix(arms[arm].wrist, aimWeapon);
		}
	}
	
	// LateUpdate is called once per frame, after animation sampling
	public override void LateUpdateCustom () 
	{
		// it's important to do weapon snapping first, because we do hand snapping to weapon after that
		if (weaponAdjustment > 0)
		{
			Matrix4x4 weaponMatrix = legSnap.localToWorldMatrix * weaponRelativeToLeg;
			aimWeapon.rotation = Quaternion.Lerp(aimWeapon.rotation, Util.QuaternionFromMatrix(weaponMatrix), weaponAdjustment);
			aimWeapon.position = Vector3.Lerp(aimWeapon.position, weaponMatrix.GetColumn(3), weaponAdjustment);
		}
		
		for (int arm=0; arm<arms.Length; arm++) {
			// Find out how much IK adjustment to use
			float adjustment = 1;
			if (arm == 0)
				adjustment = arm0adjustment;
			if (arm == 1)
				adjustment = arm1adjustment;
			
			// Don't adjust if no adjustment
			if (adjustment <= 0)
				continue;
			
			// Remember original arm bone rotations
			Quaternion origShoulder = arms[arm].shoulder.rotation;
			Quaternion origElbow = arms[arm].elbow.rotation;
			Quaternion origWrist = arms[arm].wrist.rotation;
			
			// IK to make wrist go into desired position
			Vector3 desiredWristPosition = (aimWeapon.localToWorldMatrix * arms[arm].handRelativeToGun).MultiplyPoint3x4(Vector3.zero);
			ikSolver.Solve( new Transform[]{ arms[arm].shoulder, arms[arm].elbow, arms[arm].wrist }, desiredWristPosition );
			
			// Get adjusted wrist rotation
			arms[arm].wrist.rotation = Util.QuaternionFromMatrix(aimWeapon.localToWorldMatrix * arms[arm].handRelativeToGun);
			
			// Lerp between orig and adjusted rotations iff less than full adjustment
			if (adjustment < 1) {
				arms[arm].shoulder.rotation = Quaternion.Lerp(origShoulder, arms[arm].shoulder.rotation, adjustment);
				arms[arm].elbow.rotation = Quaternion.Lerp(origElbow, arms[arm].elbow.rotation, adjustment);
				arms[arm].wrist.rotation = Quaternion.Lerp(origWrist, arms[arm].wrist.rotation, adjustment);
			}
		}
	}
}
