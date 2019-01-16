using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour 
{
	public AnimationClip openDoor1;
	public AnimationClip openDoor2;
	public AnimationClip closeDoor1;
	public AnimationClip closeDoor2;

	private GameObject player;
	private BoxCollider box;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		box = gameObject.GetComponent<BoxCollider>();
	}

	void OnTriggerEnter (Collider other)
	{

		if (other.gameObject == player)
		{
			if (player.transform.position.x < 7.261238)
			{
				transform.animation.CrossFade(openDoor1.name, 0.12f);
				box.isTrigger = true;
				box.enabled = false;
			}
			else
			{
				transform.animation.CrossFade(openDoor2.name, 0.12f);
				box.isTrigger = true;
				box.enabled = false;
			}
		}
	}

	void OnTriggerExit (Collider other)
	{

		if (other.gameObject == player)
		{
			if (player.transform.position.x > 7.261238)
			{
				transform.animation.CrossFade(closeDoor2.name, 0.12f);
				box.isTrigger = false;
				box.enabled = true;
			}
			else
			{
				transform.animation.CrossFade(closeDoor1.name, 0.12f);
				box.isTrigger = false;
				box.enabled = true;
			}
		}
	}
}
