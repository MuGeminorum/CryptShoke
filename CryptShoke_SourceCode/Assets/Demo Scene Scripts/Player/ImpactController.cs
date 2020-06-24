using UnityEngine;
using System.Collections;

// Controller for playing "bullet" impact animations
public class ImpactController : MonoBehaviour {
	
	public AnimationClip[] impactFront;
	public AnimationClip[] impactBack;
	public AnimationClip[] impactLeft;
	public AnimationClip[] impactRight;
	public float impactForce = 1;
	private CharacterController cc;
	
	// sets up array of impact animation
	private void SetupAnimations(AnimationClip[] impactAnimations) {
		foreach (AnimationClip anim in impactAnimations)
			SetupAnimation(anim);
	}
	
	// sets up single impact animation
	private void SetupAnimation(AnimationClip impactAnimation) {
		animation[impactAnimation.name].wrapMode = WrapMode.Once;
		animation[impactAnimation.name].blendMode = AnimationBlendMode.Additive;	
		animation[impactAnimation.name].layer = 100;
	}

	// Use this for initialization
	void Start () {
		// if any of animation is not present, then we disable the controller
		if (impactBack==null || impactLeft == null || impactRight == null)
			impactFront = null;
			
		if (impactFront != null) {
			// initialize animations
			SetupAnimations(impactFront);
			SetupAnimations(impactBack);
			SetupAnimations(impactLeft);
			SetupAnimations(impactRight);
		}
		
		cc = GetComponent<CharacterController>();
	}
	
	private void StartAnimation(AnimationClip impactAnimation, float weight) {
		if (weight > 0) {
			AnimationState a = animation[impactAnimation.name];
			
			a.enabled = true;
			// we do instant blend in - no crossfade
			a.weight = weight;
			// skip to first frame (in order to make impact look more instant)
			a.time = 1.0F / impactAnimation.frameRate;
		}
	}
	
	// Activates impact animations
	void OnHit (RayAndHit rayAndHit) {
		Vector3 dir = -rayAndHit.ray.direction;
		
		// Push character slightly
		cc.Move(cc.velocity * Time.deltaTime - dir * impactForce);
		
		// Find horizontal direction in character space, with a slight randomness
		dir = transform.InverseTransformDirection(dir);
		dir += Random.insideUnitSphere * 0.2f;
		dir.y = 0;
		dir = dir.normalized;
		
		if (impactFront != null) {
			float axisz = dir.z;
			float axisx = dir.x;
			
			// Start animations
			// For each of four directions, choose a random animation from the array
			// and play it with the weight multiplier for that direction times a random value
			float rand = Random.Range(0.6f, 1.0f);
			if (axisz > 0)
				StartAnimation(impactFront[Random.Range(0, impactFront.Length)], axisz * rand);
			if (axisz < 0)
				StartAnimation(impactBack[Random.Range(0, impactBack.Length)], -axisz * rand);
			if (axisx < 0)
				StartAnimation(impactLeft[Random.Range(0, impactLeft.Length)], -axisx * rand);
			if (axisx > 0)
				StartAnimation(impactRight[Random.Range(0, impactRight.Length)], axisx * rand);
		}
	}
}



















