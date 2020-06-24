using UnityEngine;
using System.Collections;

public class KeyBox : MonoBehaviour 
{
	public AnimationClip boxOpen;
	public AnimationClip boxClose;
	public GameObject player;
	public GameObject key;
	public GameObject addition;
	public bool canOpen = false;
	public bool hasOpened = false;
	public bool isOpen = false;
	public bool additionalItems = false;

	void Awake()
	{
		key.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if(canOpen)
		{
			transform.animation.CrossFade(boxOpen.name, 0.12f);
			isOpen = true;
		
			if(!hasOpened)
			{
				key.SetActive(true);
				hasOpened = true;
				if(additionalItems)
					addition.SetActive(true);
			}
		}
	}

	void onTriggerExit(Collider other)
	{
		if(canOpen)
		{
			transform.animation.CrossFade(boxClose.name, 0.12f);
			isOpen = false;
		}
	}
}
