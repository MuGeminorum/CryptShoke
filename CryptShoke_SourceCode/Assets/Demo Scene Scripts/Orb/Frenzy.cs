using UnityEngine;
using System.Collections;

public class Frenzy : MonoBehaviour {
	
	public GameObject player;
	public GameObject deadReplacement;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator Die () {
		animation.Stop();
		
		OrbController oc = GetComponent<OrbController>();
		
		// Disable this component so Update and LateUpdate is no longer called
		oc.enabled = false;
		
		StartCoroutine(RandomSparkAllOver());
		
		// Fold turret out from whatever point it was in the animation (unless it's already fully out)
		if (oc.IsUnfolded() == false || animation["Take 001"].enabled) {
			animation["Take 001"].enabled = true;
			animation["Take 001"].speed = 1;
		}
		
		player.SendMessage("OnGonnaBlow");
		
		// Give the orb a rigidbody so it can move physically
		Rigidbody rigid = rigidbody;
		
		// First make the orb rotate wildly in air for 1.5 seconds
		// Then make it fall to the ground and continue rotating wildly
		// Make it explode after 5 seconds
		float fallAfterSeconds = 1.5f;
		float explodeAfterSeconds = 5.0f;
		float rotateSpeed = 2000f;
		float axisChange = 20f;
		
		float time = Time.time;
		Vector3 axis = Vector3.up;
		while (Time.time < time + explodeAfterSeconds) {
			if (Time.deltaTime > 0 && Time.timeScale > 0) {
				
				// Value that starts at 0 and is 1 after fallAfterSeconds time
				float fallLerp = Mathf.InverseLerp(time, time+fallAfterSeconds, Time.time);
				
				// Value that starts at 0 and is 1 after explodeAfterSeconds time
				float explodeLerp = Mathf.InverseLerp(time, time+explodeAfterSeconds, Time.time);
				
				// Rotate the axis to create unpredictable rotation
				float deltaRot = axisChange * Time.deltaTime;
				axis = Quaternion.Euler(deltaRot, deltaRot, deltaRot) * axis;
				
				// Rotate around the axis.
				rigidbody.angularVelocity = axis * fallLerp * rotateSpeed * Mathf.Deg2Rad;
				
				// Make the pitch increasingly higher until the explosion
				audio.pitch = Mathf.Max(audio.pitch, 1 + Mathf.Pow(explodeLerp, 2) * 4.0f);
				
				// Make it fall to the ground after fallAfterSeconds time
				if (Time.time - time > fallAfterSeconds && rigid.useGravity == false) {
					rigid.useGravity = true;
					axisChange = 90f;
					StartCoroutine(PanicShooting());
				}
			}
			yield return 0;
		}
		gameObject.SetActiveRecursively(false);
		Instantiate(deadReplacement, transform.position, transform.rotation);
		
		// Make player die if too close
		if (Vector3.Distance(player.transform.position, transform.position) < 6) {
			// Make player die
			HealthController hc = player.GetComponent<HealthController>();
			hc.health = 0;
			
			// Make player fly into the air
			CharacterController cc = player.GetComponent<CharacterController>();
			cc.Move(((player.transform.position - transform.position).normalized * 15 + Vector3.up * 5) * Time.deltaTime);
		}
		// Otherwise, player wins the game
		else {
			player.SendMessage("OnWin");
		}
		
	}
	
	IEnumerator RandomSparkAllOver () {
		HealthController hc = GetComponent(typeof(HealthController)) as HealthController;
		if (hc && hc.hitParticles) {
			while (true) {
				yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
				Vector3 dir = Random.onUnitSphere;
				GameObject particles = Instantiate(
					hc.hitParticles,
					transform.position + dir * 0.9f,
					Quaternion.LookRotation(dir)
				) as GameObject;
				particles.transform.parent = transform;
			}
		}
	}
	
	IEnumerator PanicShooting () {
		while (true) {
			SendMessage("Shoot");
			yield return new WaitForSeconds(0.1f);
		}
	}
}
