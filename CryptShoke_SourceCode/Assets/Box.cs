using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Box : MonoBehaviour 
{
	public AnimationClip boxOpen;
	public AnimationClip boxClose;
	public GameObject player;
	public GameObject style1;
	public GameObject style2;
	public GUISkin skin;
	public bool hasOpen = false;
	public bool gui = false;

	private HumanStatus humanStatus;
	private List<Animation> animationComponents;
	private List<AudioSource> audioSourceComponents;
	private float menuOn = 0;
	private float lastTime = 0;
	private HealthController healthController;

	void Awake()
	{
		humanStatus = player.GetComponent<HumanStatus>();
		healthController = player.GetComponent<HealthController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(humanStatus.keyNumber >= 2)
		{
			transform.animation.CrossFade(boxClose.name, 0.12f);
			if(!hasOpen)
			{
				gui = true;
				StartCoroutine(Pause(true));
				hasOpen = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(humanStatus.keyNumber >= 2)
		{
			transform.animation.CrossFade(boxClose.name, 0.12f);


		}
	}

	void OnGUI()
	{
		if(gui)
		{
			GUI.skin = skin;
			float w = Screen.width*2 /3;
			float h = Screen.height /4 * 7/10;
			Rect rect = new Rect(0,0,w,h);
			rect.x = w /4;
			rect.y = h /5;
			if (GUI.Button(rect, "You feel full of strength."))
			{
				//StartCoroutine(ChangeAbility(0));
				ChangeAbility(0);
				StartCoroutine(Pause(false));
				gui = false;
			}

			rect.y += h *7/6;
			if (GUI.Button(rect, "You have an eternal life."))
			{
				//StartCoroutine(ChangeAbility(0));
				ChangeAbility(1);
				StartCoroutine(Pause(false));
				gui = false;
			}

			rect.y += h *7/6;
			if (GUI.Button(rect, "You make a deal with devil."))
			{
				//StartCoroutine(ChangeAbility(0));
				ChangeAbility(2);
				StartCoroutine(Pause(false));
				gui = false;
			}

			rect.y += h *7/6;
			if (GUI.Button(rect, "You are blessed by the snow queen."))
			{
				//StartCoroutine(ChangeAbility(0));
				ChangeAbility(3);
				StartCoroutine(Pause(false));
				gui = false;
			}
		}
	}

	void ChangeAbility(int i)
	{
		switch(i)
		{
		case 0:
			humanStatus.damageLevel++;
			humanStatus.defenceLevel++;
			humanStatus.whole = 0;
			style1.SetActive(true);
			break;
		case 1:
			healthController.healingSpeed = 4;
			humanStatus.whole = 1;
			style1.SetActive(true);
			break;
		case 2:
			humanStatus.gun = 0;
			humanStatus.whole = 2;
			style2.SetActive(true);
			break;
		case 3:
			humanStatus.gun = 1;
			humanStatus.whole = 3;
			style2.SetActive(true);
			break;
		default:
			break;
		}
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
}
