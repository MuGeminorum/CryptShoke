using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float    MaxHealth=100;  //最大血量
	public float    CurrentHealth;  //当前血量
    public bool     Invincible;     //是否不可战胜..无敌
	public bool     Dead;           //死亡标识
	
	
	// Use this for initialization
	void Start () 
    {
		//当前生命值
	    CurrentHealth=MaxHealth;
	}
	
	// Update is called once per frame
	void Update () 
    {	
		//是否不可战胜..无敌
		if(Invincible)
        {
		    CurrentHealth=MaxHealth;	
		}
		else
        {
		    if(CurrentHealth<=0)
            {
			    CurrentHealth=0;
			    Dead=true;
		    }	
			
		    //血量校准
			if(CurrentHealth>=MaxHealth)CurrentHealth=MaxHealth;
			
			//死亡处理
		    if(Dead)
            {				
			    FreeAI AI=(FreeAI)GetComponent("FreeAI");
				if(AI)
                {
                    AI.IsDead=true;
		        }
                AICount aiCount = (AICount)GetComponent("AICount");
                if (aiCount)
                {
                    aiCount.IsDead = true;
                }
		    }
		}
	
	}
}
