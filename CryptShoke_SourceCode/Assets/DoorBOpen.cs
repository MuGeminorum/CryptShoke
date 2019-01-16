using UnityEngine;
using System.Collections;

// boss房操作
public class DoorBOpen : MonoBehaviour 
{
	public AnimationClip openDoor1;
	public AnimationClip openDoor2;
	public AnimationClip closeDoor1;
	public AnimationClip closeDoor2;
	public bool isOpenout;
	public GameObject player;
	public GameObject enermiesInRoomB;
	public GameObject boss;

	private BoxCollider box;
	public bool hasOpened;
	private bool keyPickedUp;
	private EnemyCountController[] enermyCountControllers;
	private HumanStatus humanStatus;

	
	void Awake()
	{
		humanStatus = player.GetComponent<HumanStatus>();
		box = gameObject.GetComponent<BoxCollider>();
		enermyCountControllers  = enermiesInRoomB.GetComponents<EnemyCountController>();
		isOpenout = true;
		hasOpened = false;
		keyPickedUp = false;
		enermiesInRoomB.SetActive(false);
		boss.SetActive(false);
	}
	
	void OnTriggerEnter (Collider other)
	{
		
		if ((other.gameObject == player) && (humanStatus.keyNumber >= 3))
		{
			if (player.transform.position.x + player.transform.position.z > 21.25643)
			{
				isOpenout = false;
				if(hasOpened)
				{
					transform.animation.CrossFade(openDoor1.name, 0.12f);
					//box.isTrigger = true;
					box.enabled = false;
					//isOpenout = false;
				}
			}
			else
			{
				transform.animation.CrossFade(openDoor2.name, 0.12f);
				//box.isTrigger = true;
				box.enabled = false;
				isOpenout = true;

			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		
		if ((other.gameObject == player) && (humanStatus.keyNumber >= 3))
		{
			if (isOpenout)
			{
				transform.animation.CrossFade(closeDoor2.name, 0.12f);
				//box.isTrigger = false;
				box.enabled = true;

				if (player.transform.position.x + player.transform.position.z > 21.25643)
				{
					enermiesInRoomB.SetActive(true);
					boss.SetActive(true);
					humanStatus.UHumanStatus();
				}
			}
			else
			{
				if (hasOpened)
				{
					transform.animation.CrossFade(closeDoor1.name, 0.12f);
					//box.isTrigger = false;
					box.enabled = true;
				}
			}
		}
	}

	void Update()
	{
		if(!hasOpened)
		{
			int sum = 0;
			foreach(EnemyCountController ecc in enermyCountControllers)
			{
				sum += ecc.numleft + ecc.nownum;
			}

			// 设置胜利界面
			if (sum == 0)
			{

			}
		}
	}
}
