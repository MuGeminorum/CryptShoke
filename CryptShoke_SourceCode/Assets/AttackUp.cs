using UnityEngine;
using System.Collections;

public class AttackUp : MonoBehaviour 
{
	public AudioClip keyGrab;
	
	private GameObject player;
	private HumanStatus humanStatus;
	
	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		humanStatus = player.GetComponent<HumanStatus>(); 
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == player)
		{
			//AudioSource.PlayClipAtPoint (keyGrab, transform.position);
			humanStatus.damageLevel ++;
			if (humanStatus.damageLevel > 3)
				humanStatus.damageLevel = 3;
			humanStatus.UHumanStatus();

			Destroy (gameObject);
		}
	}
	
	void Update()
	{
		transform.Rotate(new Vector3(0,30,0) * Time.deltaTime);
	}
}
