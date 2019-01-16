using UnityEngine;
using System.Collections;

enum GunStatus
{
	None,
	Frozen,
	Poison
};

public class HumanStatus : MonoBehaviour 
{
	public int keyNumber;
	public int defenceLevel;
	public int damageLevel;
	//public int speedLevel;
	public GUIText keyNumText;
	public GUIText AbilityText;
	public  int  gun = -1;
	public int whole = -1;

	private HealthController healthController;
	private HealthController enermyHC;
//	private CharacterMotor characterMotor;
	public GameObject[] enermies;
	//private float speed;

	public void UHumanStatus()
	{
		enermies = GameObject.FindGameObjectsWithTag("Enermies");

		// defence up
		float rate = healthController.health / healthController.maxHealth;

		switch (defenceLevel)
		{
		case 0:
			healthController.maxHealth = 70;
			break;
		case 1:
			healthController.maxHealth = 100;
			break;
		case 2:
			healthController.maxHealth = 130;
			break;
		case 3:
			healthController.maxHealth = 160;
			break;
		default:
			healthController.maxHealth = 100;
			break;
		}
		healthController.health = rate * healthController.maxHealth;


		// attack up
		foreach (GameObject enermy in enermies)
		{
			enermyHC = enermy.GetComponent<HealthController>();
			switch(damageLevel)
			{
			case 0:
				enermyHC.hitDamage = enermyHC.normalHitDamage *0.7f;
				break;
			case 1:
				enermyHC.hitDamage = enermyHC.normalHitDamage *1.0f;
				break;
			case 2:
				enermyHC.hitDamage = enermyHC.normalHitDamage *1.3f;
				break;
			case 3:
				enermyHC.hitDamage = enermyHC.normalHitDamage *1.6f;
				break;
			default:
				enermyHC.hitDamage = enermyHC.normalHitDamage *1f;
				break;
			}
		}

		/*switch (damageLevel)
		{
		case 0:
			healthController.hitDamage = 2.5f;
			break;
		case 1:
			healthController.hitDamage = 3f;
			break;
		case 2:
			healthController.hitDamage = 4f;
			break;
		case 3:
			healthController.hitDamage = 5;
			break;
		default:
			healthController.hitDamage = 3;
			break;
		}

		switch(speedLevel)
		{
		case 0:
			speed = 0.7f;
			break;
		case 1:
			speed = 1.0f;
			break;
		case 2:
			speed = 1.5f;
			break;
		case 3:
			speed = 5.7f;
			break;
		default:
			speed = 1.0f;
			break;
		}*/

		//characterMotor.maxBackwardsSpeed *= speed;
		//characterMotor.maxForwardSpeed *= speed;
		//characterMotor.maxSidewaysSpeed *= speed;
		//characterMotor.maxVelocityChange *= speed;
	}

	void Awake()
	{
		keyNumber = 0;
		defenceLevel = 1;
		damageLevel = 1;
		//speedLevel = 3;

		healthController = gameObject.GetComponent<HealthController>();
//		characterMotor = gameObject.GetComponent<CharacterMotor>();

		UHumanStatus();

	}

	void Update()
	{
		keyNumText.text = "Keys: " + keyNumber.ToString();
		switch(whole)
		{
		case -1:
			AbilityText.text = "Ablility:" +"None";
			break;
		case 0:
			AbilityText.text = "Ablility:" +"Attack and Defence Up";
			break;
		case 1: 
			AbilityText.text = "Ablility:" +"Healing";
			break;
		case 2:
			AbilityText.text = "Ablility:" +"Poisonous Bullet";
			break;
		case 3:
			AbilityText.text = "Ablility:" +"Freezing Bullet";
			break;
		default:
			AbilityText.text = "Ablility:" +"None";
			break;
		}
	}
}
