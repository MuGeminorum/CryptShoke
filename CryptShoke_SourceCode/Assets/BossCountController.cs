using UnityEngine;
using System.Collections;

public class BossCountController : MonoBehaviour 
{
	public GameObject  enemy;
	
	public int numleft;	//还有多少怪没有刷出	
	public int numone;		//至少场景中有多少怪
	public int numfirst=4;		//第一次刷出怪的数量
	public int nownum;		//现在场景中怪的数量
	
	public Vector3 []  positionStart;	//第一次刷出怪的位置 单独设置，数量与numfirst相同
	
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
			clone[i].gameObject.GetComponent<OrbController>().player=player;
			clone[i].gameObject.GetComponent<Frenzy>().player=player;
		}
		passnum=numfirst;
	}
	
	void Update()
	{
		if(nownum<numone&&numleft>0)
		{
			clone[passnum]=Instantiate(enemy, transform.position, transform.rotation)as GameObject;
			clone[passnum].GetComponent<OrbController>().player=player;
			clone[passnum].GetComponent<Frenzy>().player=player;
			passnum++;
			numleft--;
		}
		nownum=0;
		for(int i=0;i<clone.Length;i++)
		{
			if(clone[i]!=null&&clone[i].GetComponent<HealthController>().health<=0)
			{
				
				//Destroy(clone[i],2f);
			}
			if(clone[i]!=null)
			{
				if(clone[i].GetComponent<HealthController>().health<=0)
				{
					
				}
				else{
					nownum++;
				}
			}
		}
	}
	
}
