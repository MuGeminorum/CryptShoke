using UnityEngine;
using System.Collections;

public class GhostOriginal : MonoBehaviour {
	
	public GameObject character;
	public Vector3 offset;
	
	public void Synch() {
		foreach (AnimationState state in character.animation) {
			AnimationState ownState = animation[state.name];
			if (ownState==null) {
				animation.AddClip(state.clip, state.name);
				ownState = animation[state.name];
			}
			if (ownState.enabled != state.enabled) {
				ownState.wrapMode = state.wrapMode;
				ownState.enabled = state.enabled;
				ownState.speed = state.speed;
			}
			ownState.weight = state.weight;
			ownState.time = state.time;
		}
	}
	
	void LateUpdate() {
		transform.position = character.transform.position+offset;
		transform.rotation = character.transform.rotation;
	}
}
