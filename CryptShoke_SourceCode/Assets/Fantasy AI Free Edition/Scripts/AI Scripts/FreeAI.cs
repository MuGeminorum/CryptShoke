using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreeAI : MonoBehaviour {

    public Transform    AICharacter;        //碰撞器
    public Transform    Target;             //攻击目标(需要设置才能行动)
    public bool         EnableCombat = true;        //不可攻击
    public int          CharacterCollisionLayer;    //图层(设置图层，防止移动抖动,攻击和被攻击角色，应该是属于不同图层)，当然可以用命名来规范    
    public float        turnspeed = 5;      //转向速度
    public float        runspeed = 4;       //移动速度
    public float        Damage = 10;        //伤害值
    public float        AttackSpeed = 1;    //攻击速度
    public float        AttackRange = 5;    //攻击区域
    public float        MoveRange   = 60;   //移动区域
	public bool			isPoisonous = false;
	public bool			isFrozen =false;
	public int			a;
	public GameObject	frozenEffect;

    //动画
    public AnimationClip RunAnimation;
    public AnimationClip IdleAnimation;
    public AnimationClip AttackAnimation;
    public AnimationClip DeathAnimation;

    public float DistanceNodeChange = 1.5f; 
    public bool IsDead;                     //死亡标识
    public bool DebugShowPath = true;       //绘画出路径标识(用于调试)

    //跟随移动
    private bool    bAdd    = false;
    private int     nPosNum = 0;
    private Vector3 vctFollow;
    private List<Vector3> vctLTestFollow = new List<Vector3>();
    

    private Vector3 CurrentTarget;  //保存目标位置
    private bool    TargetVisible;  //是否可见目标标识
    private bool    MoveToTarget;   //移至目标标识
    private bool    damdealt;       //开始造成伤害标识
    private bool    stop;           //停止随机
    private bool    DeadPlayed;     //播放死亡动画
    private bool    startfollow;    //开始跟随
    public  bool    EnableFollowNodePathFinding;    //允许寻路
    private float   Atimer;	        //计时

    RaycastHit hit = new RaycastHit();
    LayerMask lay;
	
	
	// Use this for initialization
	void Start () 
    {
        //设置图层，防止移动抖动
        Component[] ArrObj = gameObject.GetComponentsInChildren(typeof(Transform));
        for (int i = 0; i < ArrObj.Length; i++)
        {
            ArrObj[i].gameObject.layer = CharacterCollisionLayer;
        }
        if (AICharacter == null)
        {
            AICharacter = transform;
        }
        lay = CharacterCollisionLayer; 

		//Shader.SetGlobalColor("Stone Frog", Color.blue);
	}

    /// <summary>
    /// 死亡处理
    /// </summary>
    void DeadDeal()
    {
        if (DeathAnimation)
        {
            if (!DeadPlayed)
            {
                AICharacter.animation.CrossFade(DeathAnimation.name, 0.1f);
            }
            DeadPlayed = true;
        }
    }

    /// <summary>
    /// 敌人AI控制
    /// </summary>
    void AIControl()
    {
		HealthController hpI = (HealthController)transform.GetComponent("HealthController");

		if(hpI.health <= 0)
		{
			IsDead = true;
		}

        if (IsDead)
        {
            DeadDeal();
        }
        else if (Target)    //存在跟随对象
        {
            //根据与目标间距离，判断是否应该停下进行攻击
            float Tdist = Vector3.Distance(Target.position, transform.position);
            //Debug.Log(Tdist);
            if (Tdist <= AttackRange)
            {
                if (TargetVisible) stop = true;
            }
            else
            {
                stop = false;
            }

            //如果距离超过最大攻击距离,停止追击
            if (Tdist >= MoveRange)
            {
                MoveToTarget = false;
                TargetVisible = false;
                if (IdleAnimation)
                {
                    AICharacter.animation.CrossFade(IdleAnimation.name, 0.12f);
                }
                vctLTestFollow.Clear();
                nPosNum = 0;
            }
            else
            {
                //光线投射,查看与目标间是否有障碍物		    
                if (Physics.Linecast(transform.position, Target.position, out hit, lay))
                {
                    TargetVisible = false;
                }
                else
                {
                    startfollow = true;
                    TargetVisible = true;
                }

                //Debug.DrawLine(Target.transform.position, transform.position, Color.green);

                //如果没障碍物，可以看到跟随目标
                if (TargetVisible)
                {
                    CurrentTarget = Target.position;
                    MoveToTarget = true;
                    vctLTestFollow.Clear();
                    nPosNum = 0;
                    bAdd = false;
                }

                //移动至目标
                if (MoveToTarget)
                {
                    if (!stop)
                    {
                        transform.position += transform.forward * runspeed * Time.deltaTime;
                    }
                    if (RunAnimation)
                    {
                        if (stop)
                        {
                            //可攻击
                            if (EnableCombat)
                            {
                                HealthController hp = (HealthController)Target.transform.GetComponent("HealthController");
                                if (hp && hp.health > 0)
                                {
                                    Atimer += Time.deltaTime;
                                    AICharacter.animation[AttackAnimation.name].speed = AICharacter.animation[AttackAnimation.name].length / AttackSpeed;
                                    AICharacter.animation.CrossFade(AttackAnimation.name, 0.12f);
                                    if (!damdealt)
                                    {
                                        if (Atimer >= AttackSpeed * 0.35 & Atimer <= AttackSpeed * 0.45)
                                        {
                                            //造成伤害
                                            if (hp)
                                            {
                                                hp.health = hp.health - Damage;
                                                damdealt = true;
												if (isPoisonous)
												{
													float m  = Random.Range (0, 8);
													if (m >= 7)
													{
														hp.isPoison = true;
													}
												}
                                            }
                                        }
                                    }

                                    if (Atimer >= AttackSpeed)
                                    {
                                        damdealt = false;
                                        Atimer = 0;
                                    }
                                }
                                else
                                {
                                    AICharacter.animation.CrossFade(IdleAnimation.name, 0.12f);
                                }

                            }
                            else
                            {
                                AICharacter.animation.CrossFade(IdleAnimation.name, 0.12f);
                            }
                        }
                        else
                        {
                            Atimer = 0;
                            AICharacter.animation.CrossFade(RunAnimation.name, 0.12f);
                        }
                    }
                }
                else
                {
                    if (IdleAnimation)
                    {
                        AICharacter.animation.CrossFade(IdleAnimation.name, 0.12f);
                    }
                }

                //不能直接看到目标
                if (!TargetVisible)
                {
                    //物体可以绕道,并且已被激活
                    if (EnableFollowNodePathFinding & startfollow)
                    {
                        //添加最后一次可以直接看到的目标点,入移动队列
                        if (vctLTestFollow.Count <= 0)
                        {
                            vctLTestFollow.Add(CurrentTarget);
                            nPosNum = 0;
                            vctFollow = CurrentTarget;
                        }
                        //添加光线投射点,入移动队列
                        if (Physics.Linecast(Target.position, vctLTestFollow[vctLTestFollow.Count - 1], out hit, lay))
                        {
                            if (!bAdd)
                            {
                                vctLTestFollow.Add(vctFollow);
                                bAdd = true;
                            }
                        }
                        else
                        {
                            bAdd = false;
                            vctFollow = Target.position;
                        }

                        float dist = Vector3.Distance(transform.position, vctLTestFollow[nPosNum]);
                        if (dist < DistanceNodeChange)
                        {
                            if (vctLTestFollow.Count > nPosNum + 1)
                            {
                                nPosNum++;
                            }
                        }
                    }
                }
                if (DebugShowPath)
                {
                    if (vctLTestFollow.Count > 0)
                    {
                        for (int i = 0; i < vctLTestFollow.Count - 1; i++)
                        {
                            Debug.DrawLine(vctLTestFollow[i], vctLTestFollow[i + 1], Color.black);
                        }
                    }
                }
                //旋转
                if (MoveToTarget)
                {
                    if (vctLTestFollow.Count > 0)
                    {
                        //旋转朝向移动点
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vctLTestFollow[nPosNum] - transform.position), turnspeed * Time.deltaTime);
                    }
                    else
                    {
                        //旋转朝向目标点
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(CurrentTarget - transform.position), turnspeed * Time.deltaTime);
                    }
                }
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }

	// Update is called once per frame
	void Update () 
    {
        AIControl();
	}

	public void FixedUpdate()
	{
		if(isFrozen)
		{
			a++;
			if(a==1)
			{
				runspeed = runspeed/2;
				frozenEffect.SetActive(true);
			}
			
			if(a>=351)
			{
				isFrozen = false;
				//gameObject.transform.position.y += 0.2;
				runspeed = runspeed*2;
				frozenEffect.SetActive (false);
				a = 0;
			}
		}
	}
}
