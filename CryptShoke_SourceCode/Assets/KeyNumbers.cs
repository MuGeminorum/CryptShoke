using UnityEngine;
using System.Collections;

public class healthSlide : MonoBehaviour {
	
	private Camera camera;
	private string name = "Enemy";
	
	float npcHeight;
	public Texture2D blood_red;
	public Texture2D blood_black;
	
	private int HP = 100;
	
	// Use this for initialization
	void Start () {
		camera = Camera.main;
		
		float size_y = collider.bounds.size.y;
		float scal_y = transform.localScale.y;
		
		npcHeight = (size_y * scal_y);
		
	}
	
	void OnGUI(){
		Vector3 worldPosition = new Vector3 (transform.position.x, transform.position.y + npcHeight, transform.position.z);
		Vector2 position = camera.WorldToScreenPoint (worldPosition);
		position = new Vector2 (position.x, Screen.height - position.y);
		
		Vector2 bloodSize = GUI.skin.label.CalcSize (new GUIContent (blood_red));
		int blood_width = blood_red.width * HP / 100;
		
		GUI.DrawTexture (new Rect (position.x - (bloodSize.x / 2), position.y - bloodSize.y, bloodSize.x, bloodSize.y), blood_black);
		
		GUI.DrawTexture (new Rect (position.x - (bloodSize.x / 2), position.y - bloodSize.y, blood_width, bloodSize.y), blood_red);
		
		Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent (name));
		GUI.color = Color.yellow;
		
		GUI.Label (new Rect (position.x - (nameSize.x / 2), position.y - nameSize.y - bloodSize.y, nameSize.x, nameSize.y), name);
	}
	
	// Update is called once per frame
	void Update () {
		//HP = HealthController.print;
	}
}
