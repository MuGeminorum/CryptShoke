using UnityEngine;
using System.Collections;

public class SoundEffectController : MonoBehaviour {
	
	public AudioSource gunAudioSource;
	public AudioSource footAudioSource;
	public AudioClip fire;
	public AudioClip reload;
	public AudioClip[] footsteps;
	
	void OnFire () {
		gunAudioSource.PlayOneShot(fire);
	}
	
	void OnReload () {
		gunAudioSource.PlayOneShot(reload);
	}
	
	void OnFootStrike () {
		CharacterController cc = GetComponent<CharacterController>();
		float volume = Mathf.Clamp01(0.3f + cc.velocity.magnitude);
		footAudioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], volume);
	}
}
