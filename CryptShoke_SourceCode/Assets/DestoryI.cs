using UnityEngine;
using System.Collections;

public class DestoryI : MonoBehaviour 
{
	public float disappearTime;

	void Update()
	{
		if(gameObject.GetComponent<FreeAI>().IsDead)
		{
			Destroy(gameObject,4f);
		}
	}

}
