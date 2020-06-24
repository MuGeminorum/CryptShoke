using UnityEngine;
using System.Collections;

public class DefenceUp : MonoBehaviour 
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
			humanStatus.defenceLevel ++;
			if (humanStatus.defenceLevel > 3)
				humanStatus.defenceLevel = 3;
			humanStatus.UHumanStatus();
			
			Destroy (gameObject);
		}
	}
	
	void Update()
	{
		transform.Rotate(new Vector3(0,30,0) * Time.deltaTime);
	}

}
