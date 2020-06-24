using UnityEngine;
using System.Collections;

// Orb controller (position and fire)
public class OrbController : MonoBehaviour 
{
	public Transform turret;
	public Transform muzzleFlash;
	public Transform fireTrail;
	public GameObject player;
	
	// How fast the turret can turn and aim towards the player
	public float aimSpeed = 50;
	
	public AudioClip fireSound;
	public AudioClip foldoutSound;
	public AudioClip foldinSound;
	
	// Variable to be animated
	public float flyLerp = 0;
	
	// Is the turret folded out?
	private bool unfolded = false;
	public bool IsUnfolded () { return unfolded; }
	
	// Position A and B and how to fly between them
	private Vector3 oldFlyTarget = Vector3.zero;
	private Vector3 flyTarget = Vector3.zero;
	
	private bool gotBehindYet = false;
	private LayerMask mask;
	private AudioSource gunAudioSource;
	
	private int hitsThisRound = 0;
	
	private Vector3 aimTarget { get { return player.transform.position + new Vector3(0, 1.4f, 0); } }
	
	// Use this for initialization
	void Start () {
		// Add orb's own layer to mask
		mask = 1 << gameObject.layer;
		// Add Igbore Raycast layer to mask
		mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
		// Invert mask
		mask = ~mask;
		
		flyTarget = transform.position;
		oldFlyTarget = flyTarget;
		animation["FightSequence"].layer = -1;
		animation.Play("FightSequence");
		
		//muzzleFlash.enabled = false;
		muzzleFlash.gameObject.SetActiveRecursively(false);
		
		gunAudioSource = audio;
	}
	
	void FixedUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		
		// Variables for positional and rotational stabilization
		float prediction = 0.35f;
		float force = 300.0f;
		float torque = 100.0f;
		
		// Move orb target position from old position to new
		Vector3 position = oldFlyTarget * (1-flyLerp) + flyTarget * flyLerp;
		// Add slight up and down motion
		position += Mathf.Sin(Time.time * 2) * Vector3.up * 0.05f;
		
		// Add force to control position
		Vector3 forceVector = (position - (transform.position + rigidbody.velocity * prediction ));
		rigidbody.AddForce(forceVector * force);
		Debug.DrawLine(position, transform.position);
		
		// Calculate what the transform.up direction will be in a little while,
		// given the current angular velocity
		Vector3 predictedUp = Quaternion.AngleAxis(
			rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * prediction,
			rigidbody.angularVelocity.normalized
		) * transform.up;
		
		// Apply torque to seek towards target up direction in a stable manner
		Vector3 bankedUp = (Vector3.up + forceVector * 0.3f).normalized;
        rigidbody.AddTorque(Vector3.Cross(predictedUp, bankedUp) * torque);
        
        Vector3 targetForwardDir = (aimTarget - turret.position);
        targetForwardDir.y = 0;
        targetForwardDir.Normalize();
		
		// Calculate what the transform.forward direction will be in a little while,
		// given the current angular velocity
		Vector3 predictedForward = Quaternion.AngleAxis(
			rigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * prediction,
			rigidbody.angularVelocity.normalized
		) * transform.forward;
		
		// Apply torque to seek towards target forward direction in a stable manner
		rigidbody.AddTorque(Vector3.Cross(predictedForward, targetForwardDir) * torque);
		
		// Update aiming
		// Set orb turret transform while it is folded out and not controlled by animation
		if (unfolded && animation["Take 001"].enabled == false) {
			// Find the global direction towards the target and convert it into local space
			Vector3 aimDirInLocalSpace = transform.InverseTransformDirection(aimTarget - turret.position);
			
			// Find a rotation that points at that target
			Quaternion aimRot = Quaternion.LookRotation(aimDirInLocalSpace);
			
			// Smoothly rotate the turret towards the target rotation
			float newEulerX = TurnTowards(turret.transform.localEulerAngles.x, aimRot.eulerAngles.x, aimSpeed * Time.deltaTime);
			turret.transform.localEulerAngles = new Vector3(newEulerX, 0, 0);
		}
	}
	
	public bool TestPositionCloseToPlayer (float maxDist) {
		// Get random vector
		flyTarget = Quaternion.Euler(0, Random.Range(0f, 360f), 0)
			* new Vector3(0, Random.Range(1f, 5f), Random.Range(3f, maxDist));
		
		// Set position to be the vector relative to the player position
		flyTarget += player.transform.position;
		
		// Ensure a minimum height above the ground
		RaycastHit hit;
		float minHeightAbove = 1.5f;
		if (Physics.Raycast(flyTarget+5*Vector3.up, -Vector3.up, out hit, 10)) {
			if (hit.point.y > flyTarget.y - minHeightAbove)
				flyTarget = hit.point + Vector3.up * minHeightAbove;
		}
		
		LayerMask mask = new LayerMask();
		mask.value = 1;
		if (Physics.CheckCapsule(oldFlyTarget, flyTarget, 1.2f, mask))
			return false;
		
		return true;
	}
	
	public IEnumerator FlyToRandomPosition () {
		oldFlyTarget = flyTarget;
		
		// Try up to 100 random positions close to player
		bool success = false;
		for (int i=0; i<100; i++) {
			// Make a position and test if there's a clear path towards it
			if (TestPositionCloseToPlayer(5 + 0.1f*i)) {
				// We have a clear path
				success = true;
				// If we can also shoot the player from the found position,
				// we don't need to test any more positions.
				if (CanShootTargetFromPosition(flyTarget))
					break;
			}
			// yield a frame for every 10 tests
			if ((i % 10) == 0)
				yield return 0;
		}
		
		// If we can't find a clear path, just stay at the same place for now.
		// The player will probably move to a better location later
		if (!success) {
			Debug.LogWarning("Couldn't find clear path");
			flyTarget = oldFlyTarget;
		}
		
		flyLerp = 0;
	}
	
	public void FoldoutTurret () {
		animation["Take 001"].normalizedTime = 0;
		animation["Take 001"].speed = 1;
		animation.Play("Take 001");
		gunAudioSource.PlayOneShot(foldoutSound);
		
		unfolded = true;
		
		if (TargetIsInFront() && CanShootTarget()) {
			player.SendMessage("OnPanic");
			gotBehindYet = false;
			hitsThisRound = 0;
			StartCoroutine(MonitorPlayer());
		}
	}
	
	public IEnumerator FoldinTurret () {
		unfolded = false;
		
		// Smoothly set turret to neutral position before starting animation
		float eulerX = turret.transform.localEulerAngles.x;
		while (eulerX != 0) {
			if (Time.deltaTime > 0 && Time.timeScale > 0) {
				eulerX = TurnTowards(eulerX, 0, aimSpeed * Time.deltaTime);
				turret.transform.localEulerAngles = new Vector3(eulerX, 0, 0);
			}
			yield return 0;
		}
		
		animation["Take 001"].normalizedTime = 1;
		animation["Take 001"].speed = -1;
		animation.Play("Take 001");
		gunAudioSource.PlayOneShot(foldinSound);
		
		if (hitsThisRound > 8)
			player.SendMessage("OnPain", SendMessageOptions.DontRequireReceiver);
		hitsThisRound = 0;
	}
	
	public bool TargetIsInFront () {
		Vector3 playerDir = player.transform.position-transform.position;
		playerDir.y = 0;
		playerDir.Normalize();
		return (Vector3.Dot(transform.forward, playerDir) > 0.4f);
	}
	
	public bool TargetIsBehind () {
		Vector3 playerDir = player.transform.position-transform.position;
		playerDir.y = 0;
		playerDir.Normalize();
		return (Vector3.Dot(transform.forward, playerDir) < 0.0f);
	}
	
	public bool CanShootTarget () {
		return CanShootTargetFromPosition(turret.position);
	}
	
	public bool CanShootTargetFromPosition (Vector3 fromPosition) {
		Vector3 target = player.transform.position + new Vector3(0, 1.4f, 0);
		Vector3 shootDir = (target - fromPosition).normalized;
		Ray ray = new Ray(fromPosition, shootDir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000, mask)) {
			if (hit.transform.root.gameObject == player)
				return true;
		}
		return false;
	}
	
	public void Shoot () {
		StartCoroutine(ShowMuzzleFlash());
		gunAudioSource.PlayOneShot(fireSound);
		
		Vector3 shootDir = turret.up;
		shootDir += Random.insideUnitSphere * 0.01f;
		Ray ray = new Ray(turret.position, shootDir);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000, mask)) {
			hit.transform.root.SendMessage("OnHit", new RayAndHit(ray, hit), SendMessageOptions.DontRequireReceiver);
			muzzleFlash.localRotation = Quaternion.Euler(0, 0, Random.Range(-360, 360));
			muzzleFlash.forward = shootDir;
			fireTrail.localScale = new Vector3(0.2f, 0.2f, hit.distance*2.4f);
			
			// If player got shot, he obviously isn't in a safe spot anymore
			if (hit.transform.gameObject == player) {
				gotBehindYet = false;
				hitsThisRound++;
				StartCoroutine(MonitorPlayer());
			}
		}
	}
	
	void OnHit (RayAndHit rayAndHit) {
		// Add a big force impact from the bullet hit
		rigidbody.AddForce(rayAndHit.ray.direction * 200, ForceMode.Impulse);
		
		// Sometimes, also AddForceAtPosition - this adds some rotation as well.
		// We don't want to allow this too often, otherwise the orb, when being hit constantly,
		// will aim so bad that it's too easy to win.
		if (Random.value < 0.2f)
			rigidbody.AddForceAtPosition(rayAndHit.ray.direction * 15, rayAndHit.hit.point, ForceMode.Impulse);
		
	}
	
	// Function to monitor if player is in a safe spot yet
	IEnumerator MonitorPlayer () {
		while (unfolded && !gotBehindYet) {
			if (TargetIsBehind() || !CanShootTarget()) {
				if (hitsThisRound > 8)
					player.SendMessage("OnPain", SendMessageOptions.DontRequireReceiver);
				else
					player.SendMessage("OnSafe", SendMessageOptions.DontRequireReceiver);
				gotBehindYet = true;
				hitsThisRound = 0;
				yield return 1;
			}
			yield return new WaitForSeconds(1.0f/6);
		}
	}
	
	// Make an angle go maxDegreesDelta closer to a target angle
	public static float TurnTowards (float current, float target, float maxDegreesDelta) {
		float angleDifference = Mathf.Repeat (target - current + 180, 360) - 180;
		float turnAngle = maxDegreesDelta * Mathf.Sign (angleDifference);
		if (Mathf.Abs (turnAngle) > Mathf.Abs (angleDifference)) {
			return target;
		}
		return current + turnAngle;
	}
	
	// Show muzzle flash for a brief moment
	IEnumerator ShowMuzzleFlash () {
		// Show muzzle flash when firing
		muzzleFlash.gameObject.SetActiveRecursively(true);
		yield return new WaitForSeconds(0.05f);
		muzzleFlash.gameObject.SetActiveRecursively(false);
	}
}
