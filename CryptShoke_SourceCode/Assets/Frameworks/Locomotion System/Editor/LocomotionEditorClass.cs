/*
Copyright (c) 2008, Rune Skovbo Johansen & Unity Technologies ApS

See the document "TERMS OF USE" included in the project folder for licencing details.
*/
using UnityEditor;
using UnityEngine;

class LocomotionEditorClass {
	
	[DrawGizmo (GizmoType.SelectedOrChild)]
	static void RenderGizmo (LegController legC, GizmoType gizmoType) {
		if (Application.isPlaying || AnimationUtility.InAnimationMode())
			return;
		
		Vector3 up = legC.transform.up;
		Vector3 forward = legC.transform.forward;
		Vector3 right = legC.transform.right;
		
		// Draw cross signifying the Ground Plane Height
		Vector3 groundCenter = (
			legC.transform.position
				+ legC.groundPlaneHeight * up * legC.transform.lossyScale.y
		);
		Gizmos.color = (Color.green+Color.white)/2;
		Gizmos.DrawLine(groundCenter-forward, groundCenter+forward);
		Gizmos.DrawLine(groundCenter-right, groundCenter+right);
		
		// Draw rect showing foot boundaries
		if (legC.groundedPose==null) return;
		float scale = legC.transform.lossyScale.z;
		for (int leg=0; leg<legC.legs.Length; leg++) {
			if (legC.legs[leg].ankle==null) continue;
			if (legC.legs[leg].toe==null) continue;
			if (legC.legs[leg].footLength+legC.legs[leg].footWidth==0) continue;
			legC.InitFootData(leg);
			Vector3 heel = legC.legs[leg].ankle.TransformPoint(legC.legs[leg].ankleHeelVector);
			Vector3 toetip = legC.legs[leg].toe.TransformPoint(legC.legs[leg].toeToetipVector);
			Vector3 side = (Quaternion.AngleAxis(90,up) * (toetip-heel)).normalized * legC.legs[leg].footWidth * scale;
			Gizmos.DrawLine(heel+side/2, toetip+side/2);
			Gizmos.DrawLine(heel-side/2, toetip-side/2);
			Gizmos.DrawLine(heel-side/2, heel+side/2);
			Gizmos.DrawLine(toetip-side/2, toetip+side/2);
		}
	}
	
	private static bool SanityCheckAnimationCurves(LegController legC, AnimationClip animation) {
		AnimationClipCurveData[] curveData = AnimationUtility.GetAllCurves(animation,false);
		
		bool hasRootPosition = false;
		bool hasRootRotation = false;
		
		// Check each joint from hip to ankle in each leg
		bool[][] hasJointRotation = new bool[legC.legs.Length][];
		for (int i=0; i<legC.legs.Length; i++) {
			hasJointRotation[i] = new bool[legC.legs[i].legChain.Length];
		}
		
		foreach (AnimationClipCurveData data in curveData) {
			Transform bone = legC.transform.Find(data.path);
			if (bone==legC.root && data.propertyName=="m_LocalPosition.x") hasRootPosition = true;
			if (bone==legC.root && data.propertyName=="m_LocalRotation.x") hasRootRotation = true;
			for (int i=0; i<legC.legs.Length; i++) {
				for (int j=0; j<legC.legs[i].legChain.Length; j++) {
					if (bone==legC.legs[i].legChain[j] &&  data.propertyName=="m_LocalRotation.x") {
						hasJointRotation[i][j] = true;
					}
				}
			}
		}
		
		bool success = true;
		
		if (!hasRootPosition) {
			Debug.LogError("AnimationClip \""+animation.name+"\" is missing animation curve for the position of the root bone \""+legC.root.name+"\".");
			success = false;
		}
		if (!hasRootRotation) {
			Debug.LogError("AnimationClip \""+animation.name+"\" is missing animation curve for the rotation of the root bone \""+legC.root.name+"\".");
			success = false;
		}
		for (int i=0; i<legC.legs.Length; i++) {
			for (int j=0; j<legC.legs[i].legChain.Length; j++) {
				if (!hasJointRotation[i][j]) {
					Debug.LogError("AnimationClip \""+animation.name+"\" is missing animation curve for the rotation of the joint \""+legC.legs[i].legChain[j].name+"\" in leg "+i+".");
					success = false;
				}
			}
		}
		
		return success;
	}
	
	[MenuItem ("Custom/Locomotion Initialization")]
    static void DoToggle()
    {
        Debug.Log("Menu item selected");
        GameObject activeGO = Selection.activeGameObject;
        LegController legC = activeGO.GetComponent(typeof(LegController)) as LegController;
        
        legC.Init();
        
        bool success = true;
        foreach (MotionAnalyzer analyzer in legC.sourceAnimations) {
        	if (!SanityCheckAnimationCurves(legC,analyzer.animation)) success = false;
        }
        if (!success) return;
        
        legC.Init2();
    }
	
}
