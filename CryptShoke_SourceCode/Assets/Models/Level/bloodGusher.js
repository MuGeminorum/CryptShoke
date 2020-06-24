var enemyTarget : Transform;

var bloodSpatter : GameObject;
var bloodLine : LineRenderer;

var textures : Texture[];

//var dir = new Vector3;

function Update () 
{
	if (Input.GetMouseButtonDown (0))
	{
		dir = transform.TransformDirection (transform.position - enemyTarget.position);
			
		dir.Normalize ();
			
		//if (Random.Range (0,5) == 2)
		//{
			
			
			bloodLine.SetVertexCount (2);
			bloodLine.SetPosition (0, transform.position);
			bloodLine.SetPosition (1, transform.position + (dir * (Random.value * 2.0 + 4.0)));
			
			bloodLine.SetWidth (1, 1);
			
			bloodLine.material.mainTexture = textures[Random.Range (0, textures.length)];
			
			thisBloodLine = Instantiate (bloodLine.gameObject, transform.position, Quaternion.identity);
		//}
		
		Instantiate (bloodSpatter, transform.position, Quaternion.LookRotation(dir));
		
		Destroy (thisBloodLine, 0.1);
	}
}