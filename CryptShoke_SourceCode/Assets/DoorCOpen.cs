using UnityEngine;
using System.Collections;

// 惩罚房操作
public class DoorCOpen : MonoBehaviour 
{
	public AnimationClip openDoor1;
	public AnimationClip openDoor2;
	public AnimationClip closeDoor1;
	public AnimationClip closeDoor2;
	public bool isOpenout;
	public GameObject player;					// 玩家操作
	public GameObject enermiesInRoomA;			// 这个屋子中的敌人
	public GameObject keyBox;					// 钥匙箱
	
	private BoxCollider box;
	public bool hasOpened = false;
	private EnemyCountController[] enermyCountControllers;
	private HumanStatus humanStatus;
	private KeyBox keyB;
	
	void Awake()
	{
		box = gameObject.GetComponent<BoxCollider>();
		enermyCountControllers  = enermiesInRoomA.GetComponents<EnemyCountController>();
		keyB = keyBox.GetComponent<KeyBox>();
		humanStatus = player.GetComponent<HumanStatus>();
		isOpenout = true;
		hasOpened = false;
		enermiesInRoomA.SetActive(false);
	}
	
	void OnTriggerEnter (Collider other)
	{
		
		if (other.gameObject == player)
		{
			if (player.transform.position.x > -0.3657951)
			{
				isOpenout = false;
				if (hasOpened)
				{
					transform.animation.CrossFade(openDoor1.name, 0.12f);
					box.enabled = false;
					//isOpenout = false;
				}
			}
			else
			{
				// 在屋子外的操作
				transform.animation.CrossFade(openDoor2.name, 0.12f);
				//box.isTrigger = true;
				box.enabled = false;
				isOpenout = true;

			}
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		
		if (other.gameObject == player)
		{
			if (isOpenout)
			{
				transform.animation.CrossFade(closeDoor2.name, 0.12f);
				//box.isTrigger = false;
				box.enabled = true;

				if ((!hasOpened) && (player.transform.position.x > -0.3657951))
				{
					enermiesInRoomA.SetActive(true);
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
		// 设置敌人清光的的条件
		if(!hasOpened)
		{
			int sum = 0;
			
			// 这里有些问题2015.6.3
			foreach(EnemyCountController ecc in enermyCountControllers)
			{
				sum += ecc.nownum + ecc.numleft;
			}
			
			// 钥匙出现，门可以从内部打开
			if (sum == 0)
			{
				keyB.canOpen = true;
				hasOpened = true;
			}
		}
	}
}
