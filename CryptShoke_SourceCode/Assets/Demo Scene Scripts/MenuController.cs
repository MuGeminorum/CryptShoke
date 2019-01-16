using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {
	
	static int difficulty = 1;
	public GUISkin skin;
	public GameObject b;
	private List<Animation> animationComponents;
	private List<AudioSource> audioSourceComponents;
	private float menuOn = 0;
	private float lastTime = 0;
	private Box box;
	
	int[] playerHitDamage = new int[5] {  3,  4,  6, 10, 20 };
	int[] playerHeal      = new int[5] {  5,  4,  3,  2,  0 };
	int[] enemyHitDamage  = new int[5] { 10,  5,  2,  2,  2 };
	int[] enemyHeal       = new int[5] {  0,  0,  0,  0,  0 };
	
	// Use this for initialization
	IEnumerator Start () {
		box = b.GetComponent<Box>();

//		UpdateDifficulty();
		AudioListener.volume = 0;
		yield return 0;
		AudioListener.volume = 1;
	}
	
	// Update is called once per frame
	void Update () {
		// Don't pause in first frame - allow scripts to settle in first
		if (Time.timeSinceLevelLoad == 0)
			return;
		
		float realDeltaTime = (Time.realtimeSinceStartup - lastTime);
		lastTime = Time.realtimeSinceStartup;
		menuOn = Mathf.Clamp01(menuOn + (Time.timeScale == 0 ? 1 : -1) * realDeltaTime * 5);
		
		if (!Screen.lockCursor && Time.timeScale != 0) {
			StartCoroutine(Pause(true));
		}
	}
	
	void OnGUI () {
		if (menuOn == 0)
			return;
		if(!box.gui)
		{
		GUI.skin = skin;
		
		// PLAY button
		Rect rect = new Rect(0, 0, 150, 75);
		rect.x = (Screen.width  - rect.width ) / 2 + (1 - menuOn) * Screen.width;
		rect.y = (Screen.height - rect.height) / 2;
		if (GUI.Button(rect, "PLAY")) {
			StartCoroutine(Pause(false));
		}
		}
		// Difficulty buttons
//		rect = new Rect(rect.x - 200, rect.y + 150, rect.width + 400, 40);
//		string[] difficulties = new string[] {"NO FAIL", "EASY", "MEDIUM", "HARD", "INSANE"};
//		int newDifficulty = GUI.SelectionGrid(rect, difficulty, difficulties, difficulties.Length);
//		if (newDifficulty != difficulty) {
//			difficulty = newDifficulty;
//			UpdateDifficulty();
//		}
	}
	
	IEnumerator Pause (bool pause) {
		// Pause/unpause time
		Time.timeScale = (pause ? 0 : 1);
		// Unlock/Lock cursor
		Screen.lockCursor = !pause;
		
		if (pause == true) {
			Object[] objects = FindObjectsOfType(typeof(Animation));
			animationComponents = new List<Animation>();
			foreach (Object obj in objects) {
				Animation anim = (Animation)obj;
				if (anim != null && anim.enabled) {
					animationComponents.Add(anim);
					anim.enabled = false;
				}
			}
			objects = FindObjectsOfType(typeof(AudioSource));
			audioSourceComponents = new List<AudioSource>();
			foreach (Object obj in objects) {
				AudioSource source = (AudioSource)obj;
				if (source != null && source.enabled /*&& source.isPlaying*/) {
					audioSourceComponents.Add(source);
					source.Pause();
				}
			}
		}
		else {
			// If unpausing, wait one frame before we enable animation component.
			// Procedural adjustments are one frame delayed because first frame
			// after being paused has deltaTime of 0.
			yield return 0;
			foreach (Animation anim in animationComponents)
				anim.enabled = true;
			foreach (AudioSource source in audioSourceComponents)
				source.Play();
			animationComponents = null;
		}
	}
	
	void UpdateDifficulty () {
		Object[] objects = FindObjectsOfType(typeof(HealthController));
		foreach (Object obj in objects) {
			HealthController health = (HealthController)obj;
			if (health.gameObject.tag == "Player") {
				health.healingSpeed = playerHeal[difficulty];
				health.hitDamage = playerHitDamage[difficulty];
			}
			else {
				health.healingSpeed = enemyHeal[difficulty];
				health.hitDamage = enemyHitDamage[difficulty];
			}
		}
	}
}
