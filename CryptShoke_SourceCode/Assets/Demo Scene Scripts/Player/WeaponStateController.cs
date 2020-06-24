using UnityEngine;
using System.Collections;

// Class for controlling holstering, reload, fire and muzzle flash
public class WeaponStateController : MonoBehaviour {
	public bool gunOn = true;
	
	// name of locomotion group with aim
	public string aimLocomotionGroup;
	// name of locomotion group without aim
	public string noAimLocomontionGroup;
	
	public Transform spine;
	public Transform rightGun;
	public Transform aimTarget;
	public float maxHorizontalAimAngle = 50f;
	
	public AnimationClip gunOnAnimation;
	public AnimationClip gunOffAnimation;
	public AnimationClip reloadAnimation;
	
	public AnimationClip fireAnimation;
	
	public Renderer muzzleFlash;
	
	private float maxSpeedWithGun    = 2.79f;
	private float maxSpeedWithoutGun = 5.64f;
	
	public int clipSize = 7;
	private int bullets = 7;
	
	private float lastFireTime = 0;
	
	private CharacterMotor motor;
	
	bool inReload { get { return animation[reloadAnimation.name].enabled; } }
	bool inTransition { get { return animation[gunOffAnimation.name].enabled || animation[gunOnAnimation.name].enabled;	} }
	bool inFire { get { return animation[fireAnimation.name].enabled; } }
	
	// Set up upper body animation
	private void SetupGunAnimation (AnimationClip animationClip) {
		AnimationState a = animation[animationClip.name];
		a.wrapMode = WrapMode.Once;
		a.layer = 1;
		a.AddMixingTransform(spine);
		a.AddMixingTransform(rightGun);
	}

	// Use this for initialization
	void Start () {
		if (gunOnAnimation == null || gunOffAnimation == null || reloadAnimation == null || fireAnimation == null || muzzleFlash == null)
			enabled = false;
			
		SetupGunAnimation(gunOnAnimation);
		SetupGunAnimation(gunOffAnimation);
		SetupGunAnimation(reloadAnimation);
		
		// setup additive fire animation
		AnimationState fireAS = animation[fireAnimation.name];
		fireAS.wrapMode = WrapMode.Once;
		fireAS.layer = 101; // we put it in a separate layer than impact animations
		fireAS.blendMode = AnimationBlendMode.Additive;
		
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
		
		LegController legC = GetComponent(typeof(LegController)) as LegController;
		IMotionAnalyzer[] motions = legC.motions;
		foreach (IMotionAnalyzer motion in motions) {
			if (motion.name == "RunForward")
				maxSpeedWithGun = motion.cycleSpeed;
			if (motion.name == "RunForwardNoGun")
				maxSpeedWithoutGun = motion.cycleSpeed;
			//if (motion.name == "RunForwardFastNoGun")
			//	maxSpeedWithoutGun = motion.cycleSpeed;
		}
		
		muzzleFlash.enabled = false;
	}
	
	// Put gun in holster
	void HolsterGun () {
		if (gunOn && !inReload && !inFire) {
			animation.CrossFade(noAimLocomontionGroup); 
			animation.CrossFade(gunOffAnimation.name);
			gunOn = false;
			StartCoroutine (CrossFadeMaxSpeed (maxSpeedWithoutGun, 0.5f));
		}
	}
	
	// Take gun up from holster
	void UnHolsterGun () {
		if (!gunOn && !inReload && !inFire) {
			animation.CrossFade(aimLocomotionGroup); 
			animation.CrossFade(gunOnAnimation.name); 
			gunOn = true;
			StartCoroutine (CrossFadeMaxSpeed (maxSpeedWithGun, 0.5f));
		}
	}
	
	// Reload the gun
	void ReloadGun () {
		bullets = 0;
		if (!gunOn) {
			UnHolsterGun();
		}
		else if (!inTransition && !inFire) {
			animation.CrossFade(reloadAnimation.name);
			bullets = clipSize;
			
			SendMessage("OnReload");
		}
	}
	
	void Fire () {
		lastFireTime = Time.time;
		if (!gunOn)
			UnHolsterGun();
		// Else play fire if it's not in other state
		else if (!inFire && !inTransition && !inReload && bullets > 0) {
			AnimationState fireAS = animation[fireAnimation.name];
			fireAS.wrapMode = WrapMode.Once;
			fireAS.enabled = true;
			// we do instant blend in - no crossfade
			fireAS.weight = 1;
			// skip to first frame (in order to make recoil look more instant)
			//fireAS.time = 1.0F / fireAnimation.frameRate;
			bullets--;
			
			SendMessage("OnFire");
			
			StartCoroutine(ShowMuzzleFlash());
		}
	}
	
	// Update is called once per frame
	void Update () {			
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		
		// Play holser/unholster
		if (Input.GetButtonDown("Holster")) {
			if (gunOn)
				HolsterGun();
			else
				UnHolsterGun();
		}
		
		// Play reload
		if (Input.GetButtonDown("Reload") || bullets == 0)
			ReloadGun();
		
		// Play fire
		if (Input.GetAxis("Fire1") > 0.5f || Input.GetMouseButton(0)) {
			//Debug.Log("Fire1 axis: "+Input.GetAxis("Fire1"));
			Fire();
		}
		
		// Holster gun if running and haven't fired gun for two seconds
		if (gunOn && Time.time - lastFireTime > 2 && motor.desiredMovementDirection.sqrMagnitude > 0.5f) {
			HolsterGun();
			SendMessage("OnRunFast");
		}
		
		if (gunOn) {
			// Find vector towards aimTarget
			Vector3 aimTargetVector = aimTarget.position - transform.position;
			aimTargetVector.y = 0;
			// Always apply direction right away when moving.
			// When standing still, apply direction when it's more than given treshold,
			// so character at first aims horizontaly and only when the aim target is
			// out of reach it will turn whole body.
			if (motor.desiredMovementDirection.sqrMagnitude > 0.1f ||
				Vector3.Angle(transform.forward, aimTargetVector) > maxHorizontalAimAngle)
				motor.desiredFacingDirection = aimTargetVector;
		}
		else {
			// When not aiming, set desired facing direction to zero vector,
			// which will make character face the desired direction of movement
			motor.desiredFacingDirection = Vector3.zero;
		}
	}
	
	// Cross fade movement speed
	IEnumerator CrossFadeMaxSpeed (float goalValue, float fadeLength) {
		float startTime = Time.time;
		float startValue = motor.maxForwardSpeed;
		while (Time.time < startTime + fadeLength) {
			motor.maxForwardSpeed = Mathf.Lerp (
				startValue,
				goalValue,
				Mathf.InverseLerp (startTime, startTime + fadeLength, Time.time)
			);
			yield return 0;
		}
		motor.maxForwardSpeed = goalValue;
	}
	
	// Show muzzle flash for a brief moment
	IEnumerator ShowMuzzleFlash () {
		// Show muzzle flash when firing
		muzzleFlash.transform.localRotation *= Quaternion.Euler(0, 0, Random.Range(-360, 360));
		muzzleFlash.enabled = true;
		yield return new WaitForSeconds(0.05f);
		muzzleFlash.enabled = false;
	}
}
