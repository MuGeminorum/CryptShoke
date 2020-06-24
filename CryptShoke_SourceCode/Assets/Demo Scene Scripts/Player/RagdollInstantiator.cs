using UnityEngine;
using System.Collections;

public class RagdollInstantiator : MonoBehaviour {
	
	public GameObject deadReplacement;
	public string cameraTargetPath;
	
	void Die () {
	
		// Replace ourselves with the dead body
		GameObject dead = null;
		if (deadReplacement) {
			// Create the dead body
			dead = (GameObject)Instantiate (deadReplacement, transform.position, transform.rotation);
			
			Vector3 vel = Vector3.zero;
			if (rigidbody) {
				vel = rigidbody.velocity;
			}
			else {
				CharacterController cc = GetComponent<CharacterController> ();
				vel = cc.velocity;
			}
			
			// Copy position & rotation from the old hierarchy into the dead replacement
			CopyTransformsRecurse (transform, dead.transform, vel);
			
			gameObject.SetActiveRecursively(false);
			
			ShooterGameCamera cam = Camera.mainCamera.gameObject.GetComponent<ShooterGameCamera>();
			cam.player = dead.transform.FindChild(cameraTargetPath);
		}
	}
	
	void CopyTransformsRecurse (Transform src, Transform dst, Vector3 velocity) {
		
		Rigidbody body = dst.rigidbody;
		if (body != null) {
			body.velocity = velocity;
			body.useGravity = true;
		}
		
		dst.position = src.position;
		dst.rotation = src.rotation;
		
		foreach (Transform child in dst) {
			// Match the transform with the same name
			Transform curSrc = src.Find (child.name);
			if (curSrc)
				CopyTransformsRecurse (curSrc, child, velocity);
		}
	}
}
