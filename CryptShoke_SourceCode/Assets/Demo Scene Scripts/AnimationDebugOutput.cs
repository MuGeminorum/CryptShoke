using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class for printing various animation debug output
public class AnimationDebugOutput : MonoBehaviour {
	public GUIText text;
	// the list of animations to output
	public string[] animationNames;
	public bool show = true;
	
	private ArmIK armIK;
	private GunAimer gunAimer;
	private HeadLookController lookController;

	// Use this for initialization
	void Start () 
	{
		armIK = GetComponent<ArmIK>();
		gunAimer = GetComponent<GunAimer>();
		lookController = GetComponent<HeadLookController>();
	}
	
	// converts float (from 0 to 1) into text-form progressbar
	private static string FloatToProgressbar(float value)
	{
		string str = "";
		int pos = (int)(value * 10);
		pos = pos > 9 ? 9 : pos;		
		for (int i = 0; i < 10; ++i)
			str += (i == pos ? "|" : " ");
		return str;
	}
	 
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.L)) {
			animation["LocomotionSystem"].weight = (animation["LocomotionSystem"].weight == 1 ? 0 : 1);
			Debug.Log("LocomotionSystem weight = "+animation["LocomotionSystem"].weight);
		}
		
		if (text == null)
			return;
					
		if (!show)
		{
			// output is disabled
			text.text = "";
		}
		else
		{
			string str = "";
			
			// output for armIK
			if (armIK)
			{
				str += string.Format("IK: H: {0:0.00} <{1}> {2:0.00} <{3}> W: {4:0.00} <{5}>\n", 
					armIK.arm0adjustment, FloatToProgressbar(armIK.arm0adjustment),
					armIK.arm1adjustment, FloatToProgressbar(armIK.arm1adjustment),
					armIK.weaponAdjustment, FloatToProgressbar(armIK.weaponAdjustment));
			}
			
			if (gunAimer || lookController)
				str += "Aim: ";
			
			// output for gun aiming
			if (gunAimer)
			{
				str += string.Format("W: {0:0.00} <{1}> {2}", 
					gunAimer.effect, FloatToProgressbar(gunAimer.effect), lookController ? "" : "\n");
			}
			
			// output for head aiming
			if (lookController)
			{
				str += string.Format("H: {0:0.00} <{1}>\n", 
					lookController.effect, FloatToProgressbar(lookController.effect));
			}

			// output for animations
			if (animationNames != null)
			{
				str += "E Wght Time Time           L A Name\n";
			
				foreach (string name in animationNames)
				{
					AnimationState s = animation[name];
					
					float time = s.normalizedTime % 2f;
					if (time < 0)
						time += 2;
					
					switch (s.wrapMode)
					{
						case WrapMode.Once: 			time = (s.normalizedTime > 1 ? 0 : time); break;
						case WrapMode.ClampForever: 	time = (s.normalizedTime >= 1 ? 1 : time); break;
						case WrapMode.PingPong: 		time = (time > 1 ? 2 - time : time); break;					
						case WrapMode.Default: // I don't know what to do with Default
						case WrapMode.Loop: 
						default: 						time = time % 1; break;
					}
				
					str += string.Format("{0} {1:0.00} {2:0.00} <{5}> {3,3:G} {6} {4}\n", 
						(s.enabled ? "X" : " "), s.weight, time, s.layer, s.name, FloatToProgressbar(time), 
						s.blendMode == AnimationBlendMode.Additive ? "X" : " ");
				}
			}
			
			/*
			// output for active joystick buttons (quite useful for figuring out xbox controller button mappings)
			string buttonName = "joystick button ";
			for (int i = 0; i < 20; ++i)
			{
				string name = buttonName + i.ToString();
				if (Input.GetKey(name))
				{
					str += name + "\n";
				}
			}*/
						
			text.text = str;
		}
	}
}
