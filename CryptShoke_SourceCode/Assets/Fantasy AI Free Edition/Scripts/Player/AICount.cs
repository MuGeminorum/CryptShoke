using UnityEngine;
using System.Collections;

public class AICount : MonoBehaviour 
{
	public Transform Player;    //(设置后,会被角色列入需要解决掉敌人队列中,仅仅显示当前敌人数目用)
    public bool IsDead = false;
	
	void Start ()
    {
        //初始化时候添加敌人
		if(Player)
        {
	        PlayerController p=(PlayerController)Player.GetComponent("PlayerController");
		    if(p)
            {
		        p.TotalAICount=p.TotalAICount+1;
		    }
		}
	}

    void Update()
    {
        if (IsDead && Player)
        {
            PlayerController p = (PlayerController)Player.GetComponent("PlayerController");
            if (p)
            {
                p.TotalAICount = p.TotalAICount - 1;                
            }
            if (gameObject.GetComponent(typeof(Health)))
            {
                Destroy(gameObject.GetComponent(typeof(Health)));
            }
            if (gameObject.GetComponent(typeof(FreeAI)))
            {
                Destroy(gameObject.GetComponent(typeof(FreeAI)));
            }            
            Destroy(gameObject.GetComponent(typeof(AICount)));
        }
    }
}
