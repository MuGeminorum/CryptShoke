using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float    MaxHealth=100;  //���Ѫ��
	public float    CurrentHealth;  //��ǰѪ��
    public bool     Invincible;     //�Ƿ񲻿�սʤ..�޵�
	public bool     Dead;           //������ʶ
	
	
	// Use this for initialization
	void Start () 
    {
		//��ǰ����ֵ
	    CurrentHealth=MaxHealth;
	}
	
	// Update is called once per frame
	void Update () 
    {	
		//�Ƿ񲻿�սʤ..�޵�
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
			
		    //Ѫ��У׼
			if(CurrentHealth>=MaxHealth)CurrentHealth=MaxHealth;
			
			//��������
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
