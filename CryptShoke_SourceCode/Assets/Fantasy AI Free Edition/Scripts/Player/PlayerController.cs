using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public  float   movespeed=4;    //移动速度
	public  float   Damage=60;      //伤害值
    public int CharacterCollisionLayer;    //图层(设置图层，防止移动抖动,攻击和被攻击角色，应该是属于不同图层)，当然可以用命名来规范
	public  Transform Cam;          //相机
	public  float   CamHeight=10;          //相机相对角色Y+方向高度
    public  float   CamHeightPushback = 5; //相机相对角色X+方向高度
	public  AnimationClip Run;
	public  AnimationClip Idle;
	public  AnimationClip Attack;
	public  AnimationClip die;
	public  float   AttackSpeed=0.7f;//攻击速度
    public  bool    dead;           //角色死亡标识
    public  int     TotalAICount;   //总敌人数(AI攻击目标是角色)

	private bool    kill;       //开始攻击标识	
	private bool    playd;      //动作播放标识
	private float   atime;      //攻击间隔时间
	private bool    dealdamage; //处理本次攻击指令标识	
    private bool    YouWon;     //胜利标识
	private bool    m_bGameStart;       //游戏开始标识
	public List<Transform> KillList;    //攻击波及对象(全体攻击模式-_-)

    /// <summary>
    /// 检查胜利
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
    /// 角色AI
    /// </summary>
    void PlayerAI()
    {
        Health php = (Health)GetComponent("Health");
        if (php)
        {
            if (php.CurrentHealth <= 0) dead = true;

        }

        //死亡
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
            //摄像机位置调整
            Vector3 ch = transform.position;
            ch.y = transform.position.y + CamHeight;
            ch.x = transform.position.x + CamHeightPushback;

            Cam.transform.position = ch;

            //角色攻击
            if (Input.GetKeyUp(KeyCode.Space))
            {
                dealdamage = true;
                kill = true;
            }

            //攻击处理
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
                //播放对应动画
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                {
                    animation.CrossFade(Run.name, 0.15f);
                }
                else
                {
                    animation.CrossFade(Idle.name, 0.15f);
                }
            }

            //移动角色
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
        //相机脱离玩家
		if(Cam)Cam.parent=null;
        m_bGameStart = true;
		playd   =true;

        //设置图层，攻击和被攻击角色，应该是属于不同图层
        Component[] ArrObj = gameObject.GetComponentsInChildren(typeof(Transform));
        for (int i = 0; i < ArrObj.Length; i++)
        {
            ArrObj[i].gameObject.layer = CharacterCollisionLayer;
        }
	}
	
	// Update is called once per frame
	void Update () {
		//检查是否胜利
        CheckWin();

        //角色AI
        PlayerAI();
	}
	
	void OnTriggerEnter(Collider other)
    {
		//敌人进入攻击范围
		Health AI=(Health)other.transform.GetComponent("Health");
		if(AI)
        {
		    KillList.Add(other.transform);			
		}
	}
	
	void OnTriggerExit(Collider other)
    {
		//敌人退出攻击范围
		Health AI=(Health)other.transform.GetComponent("Health");
		if(AI)
        {
		    KillList.Remove(other.transform);			
		}
	}
	void OnGUI()
    {
		//提示角色生命值，敌人数目
		Health php=(Health)GetComponent("Health");
		if(php)
        {
		    float hpp=php.CurrentHealth;
		    GUI.Button(new Rect(0, 30, 300, 26), "Health: "+hpp);
		}
		GUI.Button(new Rect(0, 60, 300, 26), "Evil Skellies Left: "+TotalAICount);

        GUI.Label(new Rect(0, 100, 300, 50), "MOVE   : [W、S、A、D]\nATTACK: [SPACE]");
		//胜利提示
		if(YouWon)
        {
		    GUI.Box(new Rect(200, 200, 360, 50), "Congratulations!  You have defeated all the Evil Skellies!");
			if(GUI.Button(new Rect(310, 225, 120, 25), "Continue Playing"))YouWon=false;
		}		
	}
	
}
