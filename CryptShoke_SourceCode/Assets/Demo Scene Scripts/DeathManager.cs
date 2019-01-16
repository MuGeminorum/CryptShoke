using UnityEngine;
using System.Collections;

public class DeathManager : MonoBehaviour {
	
	public HealthController[] entities;
	public ProgressBar[] progressBars;
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<entities.Length; i++) {
			HealthController hc = entities[i];
			if (hc == null)
				continue;
			
			if (hc.gameObject.active && hc.health <= 0) {
				entities[i] = null;
				hc.gameObject.SendMessage("Die");
				if (hc.tag == "Player")
					StartCoroutine(PrepareRestart(false));
				else {
					StartCoroutine(PrepareRestart(true));
				}
			}
		}
	}
	
	IEnumerator PrepareRestart (bool won) {
		if (won) {
			yield return new WaitForSeconds (11);
			Screen.lockCursor = false;
		}
		else {
			yield return new WaitForSeconds (5);
		}
		Application.LoadLevel(0);
	}
}
