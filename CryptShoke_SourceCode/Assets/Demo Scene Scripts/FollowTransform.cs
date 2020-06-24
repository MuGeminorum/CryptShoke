using UnityEngine;
using System.Collections;

public class FollowTransform : ExecutionOrderBehaviour {
	
	public Transform follower;
	public Transform target;
			
	// Update is called once per frame
	public override void LateUpdateCustom () {
		follower.position = target.position;
	}
}
