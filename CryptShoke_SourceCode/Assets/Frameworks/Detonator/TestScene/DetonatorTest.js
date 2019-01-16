public var currentDetonator:GameObject;
private var _currentExpIdx = -1;
private var buttonClicked = false;
var detonatorPrefabs : GameObject[];
var explosionLife : float = 10;
var timeScale : float = 1.0;
var detailLevel : float = 1.0;

var wall : GameObject;
private var _currentWall : GameObject;
private var _spawnWallTime = -1000;
private var _guiRect;

function Start() 
{
	SpawnWall();
	if (!currentDetonator) NextExplosion();
	else _currentExpIdx = 0;
}

private var toggleBool : boolean = false;
function OnGUI()
{
	_guiRect = Rect (7,Screen.height-180,250,200);
	GUILayout.BeginArea (_guiRect);
	
	GUILayout.BeginVertical();
	var expName : String = currentDetonator.name;
	if (GUILayout.Button (expName+" (Click For Next)"))
	{
		NextExplosion();
	}
	if (GUILayout.Button ("Rebuild Wall")) 
	{
        SpawnWall();
    }
	if (GUILayout.Button ("Camera Far")) 
	{
        Camera.main.transform.position = Vector3(0,0,-7);
		Camera.main.transform.eulerAngles = Vector3(13.5,0,0);
    }
	if (GUILayout.Button ("Camera Near")) 
	{
        Camera.main.transform.position = Vector3(0,-8.664466,31.38269);
		Camera.main.transform.eulerAngles = Vector3(1.213462,0,0);
    }
	
	GUILayout.Label("Time Scale");
	timeScale = GUILayout.HorizontalSlider (timeScale, 0.0, 1.0);
	
	GUILayout.Label("Detail Level (re-explode after change)");
	detailLevel = GUILayout.HorizontalSlider (detailLevel, 0.0, 1.0);
	
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function NextExplosion()
{
	if (_currentExpIdx >= detonatorPrefabs.Length-1) _currentExpIdx = 0;
	else _currentExpIdx++;
	currentDetonator = detonatorPrefabs[_currentExpIdx];
}

function SpawnWall()
{
	if (_currentWall) Destroy(_currentWall);
	_currentWall = Instantiate (wall, Vector3(-7,-12,48), Quaternion.identity);
	_spawnWallTime = Time.time;
}

//is this a bug? We can't use the same rect for placing the GUI as for checking if the mouse contains it...
private var checkRect: Rect = Rect (0,0,260,180);
function Update() 
{
		//keeps the UI in the corner in case of resize... 
		_guiRect = Rect (7,Screen.height-150,250,200);

		//keeps the play button from making an explosion
		if ((Time.time + _spawnWallTime) > 0.5)
		{
			//don't spawn an explosion if we're using the UI
			if (!checkRect.Contains(Input.mousePosition))
			{
				if(Input.GetMouseButtonDown(0))
				{
					SpawnExplosion();
				}
			}
			Time.timeScale = timeScale;
		}
}


function SpawnExplosion()
{
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

var hit : RaycastHit;
			if (Physics.Raycast (ray, hit, 1000)) 
			{
				var offsetSize = currentDetonator.GetComponent("Detonator").size / 3;
				var hitPoint = hit.point + ((Vector3.Scale(hit.normal, Vector3(offsetSize,offsetSize,offsetSize))));
				var exp : GameObject = Instantiate (currentDetonator, hitPoint, Quaternion.identity);
				exp.GetComponent("Detonator").detail = detailLevel;
			}
			Destroy(exp, explosionLife); 

}