using UnityEngine;
using System.Collections;

public class EnemyCountController : MonoBehaviour 
{
	public GameObject  enemy;
	
	public int numleft;	//还有多少怪没有刷出	
	public int numone;		//至少场景中有多少怪
	public int numfirst=4;		//第一次刷出怪的数量
	public int nownum;		//现在场景中怪的数量
	
	public Vector3 []  positionStart;	//第一次刷出怪的位置 单独设置，数量与numfirst相同
	public GameObject controller;
	
	private GameObject player;
	private GameObject [] clone;
	private float nextFire;
	private float fireRate=1.0f;
	private int passnum;		//共刷出了多少怪
	void Start()
	{
		nownum=positionStart.Length;
		player=GameObject.FindGameObjectWithTag("Player");
		clone=new GameObject[numleft+numfirst];
		for(int i=0;i<numfirst;i++)
		{

			clone[i]= Instantiate(enemy, positionStart[i], transform.rotation)as GameObject;
			clone[i].gameObject.GetComponent<FreeAI>().Target=player.transform;
			clone[i].gameObject.GetComponent<AICount>().Player=player.transform;
			clone[i].GetComponent<HealthController>().player=player;
			clone[i].GetComponent<HealthController>().controller=controller;
		}
		passnum=numfirst;
	}
	
	void Update()
	{
		if(nownum<numone&&numleft>0)
		{
			clone[passnum]=Instantiate(enemy, transform.position, transform.rotation)as GameObject;
			clone[passnum].GetComponent<FreeAI>().Target=player.transform;
			clone[passnum].GetComponent<AICount>().Player=player.transform;
			clone[passnum].GetComponent<HealthController>().player=player;
			clone[passnum].GetComponent<HealthController>().controller=controller;
			passnum++;
			numleft--;
		}
		nownum=0;
		for(int i=0;i<clone.Length;i++)
		{
			if(clone[i]!=null&&clone[i].GetComponent<FreeAI>().IsDead)
			{
				
				Destroy(clone[i],2f);
			}
			if(clone[i]!=null)
			{
				if(clone[i].GetComponent<FreeAI>().IsDead)
				{
					
				}
				else{
					nownum++;
				}
			}
		}
	}

}
