using UnityEngine;
using System.Collections;

public class AICount : MonoBehaviour 
{
	public Transform Player;    //(���ú�,�ᱻ��ɫ������Ҫ��������˶�����,������ʾ��ǰ������Ŀ��)
    public bool IsDead = false;
	
	void Start ()
    {
        //��ʼ��ʱ����ӵ���
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
