using UnityEngine;
using System.Collections;

public class Curing : MonoBehaviour 
{
	public AudioClip keyGrab;
	public GameObject[] curingEffect;
	public GameObject player;
	public int a = 0;
	private bool isCuring = false;
	private HealthController healthController;
	
	void Awake()
	{
		healthController = player.GetComponent<HealthController>(); 
	}

	void OnTriggerStay(Collider other)
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			if(a<3)
			{
				healthController.health = healthController.maxHealth;
				curingEffect[a].SetActive(true);
				a++;
			}
		}
	}

	void Update () 
	{
		transform.Rotate(new Vector3(0,30,0) * Time.deltaTime);
	}
}
