using UnityEngine;
using System.Collections;

public class Key1 : MonoBehaviour 
{
	public AudioClip keyGrab;

	private GameObject player;
	private HumanStatus humanStatus;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		humanStatus = player.GetComponent<HumanStatus>(); 
	}

	void OnTriggerStay(Collider other)
	{
		if ((other.gameObject == player) && Input.GetKeyDown(KeyCode.C))
		{
			//AudioSource.PlayClipAtPoint (keyGrab, transform.position);
			humanStatus.keyNumber++;
			Destroy (gameObject);
		}
	}

	void Update()
	{
		transform.Rotate(new Vector3(0,1,0) * Time.deltaTime * 0.5f);
	}
}
