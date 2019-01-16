
var particleScale:float=1;  


function Start () {

particleEmitter.minSize*=particleScale;
particleEmitter.maxSize*=particleScale;
particleEmitter.worldVelocity*=particleScale;
particleEmitter.localVelocity*=particleScale;
particleEmitter.rndVelocity*=particleScale;
particleEmitter.angularVelocity*=particleScale;
particleEmitter.rndAngularVelocity*=particleScale;	


}

function Update () {


}