using UnityEngine;
using System.Collections;

public class HealthSlide : MonoBehaviour {
	
	private Camera camera;
	private HealthController healthController;
	
	float npcHeight;
	public Texture2D blood_red;
	public Texture2D blood_black;
	public Texture2D blood_green;
	public float h=45;
	public string name;
	public bool isPoison = false;

	
	private float HP;
	private float max;
	
	// Use this for initialization
	void Start () {
		camera = Camera.main;
		
		float size_y = collider.bounds.size.y;
		float scal_y = transform.localScale.y;
		
		npcHeight = (size_y * scal_y);
		healthController = gameObject.GetComponent<HealthController>();

	}
	
	void OnGUI(){
		HP=healthController.health;
		max = healthController.maxHealth;


		Vector3 worldPosition = new Vector3 (transform.position.x, transform.position.y + npcHeight, transform.position.z);
		Vector2 position = camera.WorldToScreenPoint (worldPosition);
		position = new Vector2 (position.x, Screen.height - position.y);
		
		Vector2 bloodSize = GUI.skin.label.CalcSize (new GUIContent (blood_red));
		float blood_width = blood_red.width * HP / max;
		if (!isPoison)
		{
		GUI.DrawTexture (new Rect (position.x - (bloodSize.x / 3)+bloodSize.x/6, position.y - bloodSize.y-h, bloodSize.x/3, bloodSize.y/3), blood_black);
		
		GUI.DrawTexture (new Rect (position.x - (bloodSize.x / 3)+bloodSize.x/6, position.y - bloodSize.y-h, blood_width/3, bloodSize.y/3), blood_red);
		}
		else
		{
			GUI.DrawTexture (new Rect (position.x - (bloodSize.x / 3)+bloodSize.x/6, position.y - bloodSize.y-h, blood_width/3, bloodSize.y/3), blood_green);
		}

		Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent (name));
		GUI.color = Color.yellow;
		
		GUI.Label (new Rect (position.x - (nameSize.x / 2), position.y - nameSize.y - bloodSize.y-h, nameSize.x, nameSize.y), name);



	}


}
