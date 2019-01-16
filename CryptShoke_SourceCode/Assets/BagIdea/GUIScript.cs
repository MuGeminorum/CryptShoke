using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour
{
	public string animateText;
	public Animation animate;
	private Transform golem;
	public Transform floor;
	private Transform parentCamera;

	private Quaternion tempRotation;
	private GUIStyle style;

	void Start ()
	{
		golem = animate.transform;
		parentCamera = transform.parent;
		tempRotation = golem.rotation;
	}

	void Update ()
	{
		if(animateText == "Walk")
		{
			floor.Rotate(0f,-0.05f,0f);
			golem.transform.rotation = tempRotation;
		}
		else if(animateText == "Run")
		{
			floor.Rotate(0f,-0.2f,0f);
			golem.transform.rotation = tempRotation;
		}
		else if(animateText == "Turn Right")
		{
			golem.Rotate(0f,-1f,0f);
		}
		else if(animateText == "Turn Left")
		{
			golem.Rotate(0f,1f,0f);
		}else{
			golem.transform.rotation = tempRotation;
		}

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			parentCamera.Rotate(0f,1f,0f);
		}
		else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			parentCamera.Rotate(0f,-1f,0f);
		}
	}

	void AnimateShow(int id)
	{
		style = GUI.skin.GetStyle("Label");
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.contentColor = Color.green;
		GUILayout.Label(animateText, style);
	}

	void OnGUI ()
	{
		GUI.color = Color.yellow;
		GUI.Window(0,new Rect((Screen.width/2f)-125,Screen.height-60f,250f,50f),AnimateShow,":: AnimationClip ::");

		checkButton("Idle");
		if(GUI.Button(new Rect(20f,10f,150f,30f), "Idle"))
		{
			animateText = "Idle";
		}

		checkButton("Attack1");
		if(GUI.Button(new Rect(175f,10f,150f,30f), "Attack1"))
		{
			animateText = "Attack1";
		}

		checkButton("Attack2");
		if(GUI.Button(new Rect(330f,10f,150f,30f), "Attack2"))
		{
			animateText = "Attack2";
		}

		checkButton("Walk");
		if(GUI.Button(new Rect(485f,10f,150f,30f), "Walk"))
		{
			animateText = "Walk";
		}

		checkButton("Run");
		if(GUI.Button(new Rect(640f,10f,150f,30f), "Run"))
		{
			animateText = "Run";
		}

		checkButton("Jump");
		if(GUI.Button(new Rect(795f,10f,150f,30f), "Jump"))
		{
			animateText = "Jump";
		}

		checkButton("Take Damage1");
		if(GUI.Button(new Rect(20f,45f,150f,30f), "Take Damage1"))
		{
			animateText = "Take Damage1";
		}

		checkButton("Take Damage2");
		if(GUI.Button(new Rect(175f,45f,150f,30f), "Take Damage2"))
		{
			animateText = "Take Damage2";
		}

		checkButton("Attack Stun");
		if(GUI.Button(new Rect(330f,45f,150f,30f), "Attack Stun"))
		{
			animateText = "Attack Stun";
		}

		checkButton("Turn Right");
		if(GUI.Button(new Rect(485f,45f,150f,30f), "Turn Right"))
		{
			animateText = "Turn Right";
		}

		checkButton("Turn Left");
		if(GUI.Button(new Rect(640f,45f,150f,30f), "Turn Left"))
		{
			animateText = "Turn Left";
		}

		checkButton("Death");
		if(GUI.Button(new Rect(795f,45f,150f,30f), "Death"))
		{
			animateText = "Death";
		}

		checkButton("Action");
		if(GUI.Button(new Rect(20f,80f,925f,25f), "Action"))
		{
			animateText = "Action";
		}

		animate.Play(animateText);

		style = GUI.skin.GetStyle("Label");
		style.fontSize = 12;
		style.alignment = TextAnchor.MiddleLeft;
		GUI.color = Color.white;
		GUI.Label(new Rect(20f,Screen.height-50f,200f,40f),"Rotate Press [ A or D ]", style);
	}

	void checkButton(string ani)
	{
		if(animateText == ani)
		{
			GUI.color = Color.green;
			GUI.contentColor = Color.green;
		}else{
			GUI.color = Color.gray;
			GUI.contentColor = Color.white;
		}
	}
}
