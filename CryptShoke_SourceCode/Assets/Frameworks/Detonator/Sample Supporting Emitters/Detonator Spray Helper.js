var startTimeMin : float = 0;
var startTimeMax : float = 0;
var stopTimeMin : float = 10;
var stopTimeMax : float = 10;

var firstMaterial : Material;
var secondMaterial : Material;

private var startTime : float;
private var stopTime : float;

//the time at which this came into existence
private var spawnTime : float;
private var isReallyOn : boolean;
function Start ()
{
	isReallyOn = this.particleEmitter.emit;
	
	//this kind of emitter should always start off
	this.particleEmitter.emit = false;
	
	spawnTime = Time.time;
	
	//get a random number between startTimeMin and Max
	startTime = (Random.value * (startTimeMax - startTimeMin)) + startTimeMin + Time.time;
	stopTime = (Random.value * (stopTimeMax - stopTimeMin)) + stopTimeMin + Time.time;
	
	//assign a random material
	if (Random.value > 0.5)
	{
		this.renderer.material = firstMaterial;
	}
	else
	{
		this.renderer.material = secondMaterial;
	}
}

function FixedUpdate () 
{
	//is the start time passed? turn emit on
	if (Time.time > startTime)
	{
		this.particleEmitter.emit = isReallyOn;
	}
	
	if (Time.time > stopTime)
	{
		this.particleEmitter.emit = false;
	}
}