using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreeAI : MonoBehaviour {

    public Transform    AICharacter;        //��ײ��
    public Transform    Target;             //����Ŀ��(��Ҫ���ò����ж�)
    public bool         EnableCombat = true;        //���ɹ���
    public int          CharacterCollisionLayer;    //ͼ��(����ͼ�㣬��ֹ�ƶ�����,�����ͱ�������ɫ��Ӧ�������ڲ�ͬͼ��)����Ȼ�������������淶    
    public float        turnspeed = 5;      //ת���ٶ�
    public float        runspeed = 4;       //�ƶ��ٶ�
    public float        Damage = 10;        //�˺�ֵ
    public float        AttackSpeed = 1;    //�����ٶ�
    public float        AttackRange = 5;    //��������
    public float        MoveRange   = 60;   //�ƶ�����
	public bool			isPoisonous = false;
	public bool			isFrozen =false;
	public int			a;
	public GameObject	frozenEffect;

    //����
    public AnimationClip RunAnimation;
    public AnimationClip IdleAnimation;
    public AnimationClip AttackAnimation;
    public AnimationClip DeathAnimation;

    public float DistanceNodeChange = 1.5f; 
    public bool IsDead;                     //������ʶ
    public bool DebugShowPath = true;       //�滭��·����ʶ(���ڵ���)

    //�����ƶ�
    private bool    bAdd    = false;
    private int     nPosNum = 0;
    private Vector3 vctFollow;
    private List<Vector3> vctLTestFollow = new List<Vector3>();
    

    private Vector3 CurrentTarget;  //����Ŀ��λ��
    private bool    TargetVisible;  //�Ƿ�ɼ�Ŀ���ʶ
    private bool    MoveToTarget;   //����Ŀ���ʶ
    private bool    damdealt;       //��ʼ����˺���ʶ
    private bool    stop;           //ֹͣ���
    private bool    DeadPlayed;     //������������
    private bool    startfollow;    //��ʼ����
    public  bool    EnableFollowNodePathFinding;    //����Ѱ·
    private float   Atimer;	        //��ʱ

    RaycastHit hit = new RaycastHit();
    LayerMask lay;
	
	
	// Use this for initialization
	void Start () 
    {
        //����ͼ�㣬��ֹ�ƶ�����
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
    /// ��������
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
    /// ����AI����
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
        else if (Target)    //���ڸ������
        {
            //������Ŀ�����룬�ж��Ƿ�Ӧ��ͣ�½��й���
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

            //������볬����󹥻�����,ֹͣ׷��
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
                //����Ͷ��,�鿴��Ŀ����Ƿ����ϰ���		    
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

                //���û�ϰ�����Կ�������Ŀ��
                if (TargetVisible)
                {
                    CurrentTarget = Target.position;
                    MoveToTarget = true;
                    vctLTestFollow.Clear();
                    nPosNum = 0;
                    bAdd = false;
                }

                //�ƶ���Ŀ��
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
                            //�ɹ���
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
                                            //����˺�
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

                //����ֱ�ӿ���Ŀ��
                if (!TargetVisible)
                {
                    //��������Ƶ�,�����ѱ�����
                    if (EnableFollowNodePathFinding & startfollow)
                    {
                        //������һ�ο���ֱ�ӿ�����Ŀ���,���ƶ�����
                        if (vctLTestFollow.Count <= 0)
                        {
                            vctLTestFollow.Add(CurrentTarget);
                            nPosNum = 0;
                            vctFollow = CurrentTarget;
                        }
                        //��ӹ���Ͷ���,���ƶ�����
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
                //��ת
                if (MoveToTarget)
                {
                    if (vctLTestFollow.Count > 0)
                    {
                        //��ת�����ƶ���
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vctLTestFollow[nPosNum] - transform.position), turnspeed * Time.deltaTime);
                    }
                    else
                    {
                        //��ת����Ŀ���
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
