using UnityEngine;
using System.Collections;

public class DistanceActive : MonoBehaviour {
	//NEEDS PLAYERS LOCATION TO DETECT DISTANCE
	public Transform Player;
	//THE DISTANCE FROM PLAYER THE AI WILL BECOME ACTIVE
	public float DistanceToActivateAI;
	private AnimationClip idle;
	public int checkdistevery=10;
	private int chcount;
	public bool PlayIdleAnimationWhenDeactive=true;
	
	// Use this for initialization
	void Start () {
	FreeAI AI=(FreeAI)GetComponent("FreeAI");
	idle=AI.IdleAnimation;	
	}
	
	// Update is called once per frame
	void Update () {
		chcount=chcount+1;
		if(chcount>=checkdistevery){
	if(Player){
			//GET DISTANCE
		float dist=Vector3.Distance(transform.position, Player.transform.position);
			//GET AI COMPONENT
			 FreeAI AI=(FreeAI)GetComponent("FreeAI");
			
			
			//WHEN DISTANCE BECOMES LESS THE ACTIVATE DISTANCE
			if(dist<=DistanceToActivateAI){
				
				
			if(AI.enabled){}
				else AI.enabled=true;
				if(rigidbody.isKinematic)rigidbody.isKinematic=false;
			}
			else{
				if(rigidbody.isKinematic){}
				else rigidbody.isKinematic=true;
				
				if(AI.enabled)AI.enabled=false;
				if(PlayIdleAnimationWhenDeactive){
					if(AI.IsDead){}
					else{
					if(AI.IdleAnimation){
				AI.AICharacter.animation.CrossFade( idle.name, 0.12f);
					}
					}
			}
			}
			
		}
			chcount=0;
		}
		
	}
}
