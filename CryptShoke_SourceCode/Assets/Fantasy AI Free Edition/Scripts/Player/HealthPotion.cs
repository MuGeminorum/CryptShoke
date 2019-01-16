using UnityEngine;
using System.Collections;

public class HealthPotion : MonoBehaviour {
	public Transform Player;
	public float HpBoost=100;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	void OnTriggerEnter(Collider other){
		if(other.transform==Player){
		Health hp=(Health)other.transform.GetComponent("Health");
		if(hp){
		if(hp.CurrentHealth<hp.MaxHealth){		
		hp.CurrentHealth=hp.CurrentHealth+HpBoost;
			Destroy(gameObject);
				}
		}
	}
	}
}
