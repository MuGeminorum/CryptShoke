using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public  float   movespeed=4;    //�ƶ��ٶ�
	public  float   Damage=60;      //�˺�ֵ
    public int CharacterCollisionLayer;    //ͼ��(����ͼ�㣬��ֹ�ƶ�����,�����ͱ�������ɫ��Ӧ�������ڲ�ͬͼ��)����Ȼ�������������淶
	public  Transform Cam;          //���
	public  float   CamHeight=10;          //�����Խ�ɫY+����߶�
    public  float   CamHeightPushback = 5; //�����Խ�ɫX+����߶�
	public  AnimationClip Run;
	public  AnimationClip Idle;
	public  AnimationClip Attack;
	public  AnimationClip die;
	public  float   AttackSpeed=0.7f;//�����ٶ�
    public  bool    dead;           //��ɫ������ʶ
    public  int     TotalAICount;   //�ܵ�����(AI����Ŀ���ǽ�ɫ)

	private bool    kill;       //��ʼ������ʶ	
	private bool    playd;      //�������ű�ʶ
	private float   atime;      //�������ʱ��
	private bool    dealdamage; //�����ι���ָ���ʶ	
    private bool    YouWon;     //ʤ����ʶ
	private bool    m_bGameStart;       //��Ϸ��ʼ��ʶ
	public List<Transform> KillList;    //������������(ȫ�幥��ģʽ-_-)

    /// <summary>
    /// ���ʤ��
    /// </summary>
    void CheckWin()
    {
        if (m_bGameStart)
        {
            if (TotalAICount <= 0)
            {
                YouWon = true;
                m_bGameStart = false;
            }
        }
    }

    /// <summary>
    /// ��ɫAI
    /// </summary>
    void PlayerAI()
    {
        Health php = (Health)GetComponent("Health");
        if (php)
        {
            if (php.CurrentHealth <= 0) dead = true;

        }

        //����
        if (dead)
        {
            if (playd)
            {
                animation.CrossFade(die.name, 0.15f);
            }
            playd = false;
        }
        else
        {
            //�����λ�õ���
            Vector3 ch = transform.position;
            ch.y = transform.position.y + CamHeight;
            ch.x = transform.position.x + CamHeightPushback;

            Cam.transform.position = ch;

            //��ɫ����
            if (Input.GetKeyUp(KeyCode.Space))
            {
                dealdamage = true;
                kill = true;
            }

            //��������
            if (kill)
            {
                atime += Time.deltaTime;
                animation[Attack.name].speed = animation[Attack.name].length / AttackSpeed;
                animation.CrossFade(Attack.name, 0.15f);

                if (atime >= AttackSpeed * 0.35f & atime <= AttackSpeed * 0.48f)
                {
                    if (KillList.Count > 0 & dealdamage)
                    {
                        int ls = KillList.Count;
                        for (int i = 0; i < ls; i++)
                        {
                            Health hp = (Health)KillList[i].transform.GetComponent("Health");
                            if (hp)
                            {
                                hp.CurrentHealth = hp.CurrentHealth - Damage;
                            }
                        }
                        dealdamage = false;
                    }
                }

                if (atime >= AttackSpeed)
                {
                    kill = false;
                    atime = 0;
                }
            }
            else
            {
                //���Ŷ�Ӧ����
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                {
                    animation.CrossFade(Run.name, 0.15f);
                }
                else
                {
                    animation.CrossFade(Idle.name, 0.15f);
                }
            }

            //�ƶ���ɫ
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += transform.forward * +movespeed * Time.deltaTime;
                Cam.transform.position += transform.forward * +(movespeed) * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                transform.position += transform.forward * +movespeed * Time.deltaTime;
                Cam.transform.position += transform.forward * +(movespeed) * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.position += transform.forward * +movespeed * Time.deltaTime;
                Cam.transform.position += transform.forward * +(movespeed) * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
                transform.position += transform.forward * +movespeed * Time.deltaTime;
                Cam.transform.position += transform.forward * +(movespeed) * Time.deltaTime;
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
        //����������
		if(Cam)Cam.parent=null;
        m_bGameStart = true;
		playd   =true;

        //����ͼ�㣬�����ͱ�������ɫ��Ӧ�������ڲ�ͬͼ��
        Component[] ArrObj = gameObject.GetComponentsInChildren(typeof(Transform));
        for (int i = 0; i < ArrObj.Length; i++)
        {
            ArrObj[i].gameObject.layer = CharacterCollisionLayer;
        }
	}
	
	// Update is called once per frame
	void Update () {
		//����Ƿ�ʤ��
        CheckWin();

        //��ɫAI
        PlayerAI();
	}
	
	void OnTriggerEnter(Collider other)
    {
		//���˽��빥����Χ
		Health AI=(Health)other.transform.GetComponent("Health");
		if(AI)
        {
		    KillList.Add(other.transform);			
		}
	}
	
	void OnTriggerExit(Collider other)
    {
		//�����˳�������Χ
		Health AI=(Health)other.transform.GetComponent("Health");
		if(AI)
        {
		    KillList.Remove(other.transform);			
		}
	}
	void OnGUI()
    {
		//��ʾ��ɫ����ֵ��������Ŀ
		Health php=(Health)GetComponent("Health");
		if(php)
        {
		    float hpp=php.CurrentHealth;
		    GUI.Button(new Rect(0, 30, 300, 26), "Health: "+hpp);
		}
		GUI.Button(new Rect(0, 60, 300, 26), "Evil Skellies Left: "+TotalAICount);

        GUI.Label(new Rect(0, 100, 300, 50), "MOVE   : [W��S��A��D]\nATTACK: [SPACE]");
		//ʤ����ʾ
		if(YouWon)
        {
		    GUI.Box(new Rect(200, 200, 360, 50), "Congratulations!  You have defeated all the Evil Skellies!");
			if(GUI.Button(new Rect(310, 225, 120, 25), "Continue Playing"))YouWon=false;
		}		
	}
	
}
