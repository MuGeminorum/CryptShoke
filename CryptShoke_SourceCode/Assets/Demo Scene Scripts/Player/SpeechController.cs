using UnityEngine;
using System.Collections;

public class SpeechController : MonoBehaviour {
	
	public bool speak = true;
	public AudioSource source;
	public AudioClip[] hitSounds;
	public AudioClip[] fireSounds;
	public AudioClip[] reloadSounds;
	public AudioClip[] panicSounds;
	public AudioClip[] runFastSounds;
	public AudioClip[] safeSounds;
	public AudioClip[] painSounds;
	public AudioClip gonnaBlowSound;
	public AudioClip winSound;
	public AudioClip dieSound;
	
	// For testing purposes
	/*void Start () {
		StartCoroutine(PlayAll());
	}*/
	
	IEnumerator PlayAll () {
		Debug.Log("Playing all sounds.");
		yield return StartCoroutine(PlayAll(hitSounds));
		yield return StartCoroutine(PlayAll(fireSounds));
		yield return StartCoroutine(PlayAll(reloadSounds));
		yield return StartCoroutine(PlayAll(panicSounds));
		yield return StartCoroutine(PlayAll(runFastSounds));
		yield return StartCoroutine(PlayAll(safeSounds));
		yield return StartCoroutine(PlayAll(painSounds));
		yield return StartCoroutine(PlayAll(new AudioClip[]{gonnaBlowSound}));
		yield return StartCoroutine(PlayAll(new AudioClip[]{winSound}));
		yield return StartCoroutine(PlayAll(new AudioClip[]{dieSound}));
	}
	
	IEnumerator PlayAll (AudioClip[] sounds) {
		for (int i=0; i<sounds.Length; i++) {
			while (source.isPlaying)
				yield return 0;
			yield return new WaitForSeconds(0.3f);
			Debug.Log("Playing "+sounds[i].name);
			source.clip = sounds[i];
			source.pitch = 1;
			source.Play();
		}
	}
	
	void MaySpeak (AudioClip[] sounds) {
		if (!speak || source.isPlaying)
			return;
		source.clip = sounds[Random.Range(0, sounds.Length)];
		source.pitch = Random.Range(0.95f, 1.00f);
		source.Play();
	}
	
	IEnumerator QueueSpeak (AudioClip[] sounds) {
		if (!speak)
			yield return 1;
		while (source.isPlaying)
			yield return 0;
		source.clip = sounds[Random.Range(0, sounds.Length)];
		source.pitch = Random.Range(0.95f, 1.00f);
		source.Play();
	}
	
	void PrioritySpeak (AudioClip[] sounds) {
		if (!speak)
			return;
		source.Stop();
		source.clip = sounds[Random.Range(0, sounds.Length)];
		source.pitch = Random.Range(0.95f, 1.00f);
		source.Play();
	}
	
	void OnHit () {
		PrioritySpeak(hitSounds);
	}
	
	void OnFire () {
		if (Random.value < 0.3f)
			MaySpeak(fireSounds);
	}
	
	void OnReload () {
		if (Random.value < 0.6f)
			StartCoroutine(QueueSpeak(reloadSounds));
	}
	
	void OnPanic () {
		StartCoroutine(QueueSpeak(panicSounds));
	}
	
	void OnRunFast () {
		if (Random.value < 0.5f)
			MaySpeak(runFastSounds);
	}
	
	void OnSafe () {
		StartCoroutine(QueueSpeak(safeSounds));
	}
	
	void OnPain () {
		StartCoroutine(QueueSpeak(painSounds));
	}
	
	IEnumerator OnGonnaBlow () {
		yield return new WaitForSeconds(2);
		PrioritySpeak(new AudioClip[] {gonnaBlowSound});
	}
	
	IEnumerator OnWin () {
		yield return new WaitForSeconds(1);
		PrioritySpeak(new AudioClip[] {winSound});
	}
	
	void Die () {
		source.Stop();
		AudioSource.PlayClipAtPoint(dieSound, source.transform.position);
	}
	
}
