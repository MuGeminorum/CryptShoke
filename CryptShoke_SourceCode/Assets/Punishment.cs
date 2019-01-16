using UnityEngine;
using System.Collections;

public class Punishment : MonoBehaviour 
{
	private GameObject player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void OnTriggerEnter (Collider other)
	{
		
		if (other.gameObject == player)
		{
			HealthController hpI =player.GetComponent("HealthController") as HealthController;
			hpI.health = 0.0f;
		}
	}
}
