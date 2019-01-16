
var delayTime:float=0;
var delayPlusTime:float=0;
var rampDownTime:float=1;
var origMinEmission:float;
var origMaxEmission:float;

function Start () {

origMinEmission=particleEmitter.minEmission;
origMaxEmission=particleEmitter.maxEmission;
particleEmitter.emit=false;

}

function Update () {
if((delayTime+delayPlusTime)>0) delayTime-=Time.deltaTime;


if(delayTime<=0 && particleEmitter.emit==false) particleEmitter.emit=true;


if((delayTime+delayPlusTime)<=0){
particleEmitter.minEmission=origMinEmission*rampDownTime;
particleEmitter.maxEmission=origMaxEmission*rampDownTime;
rampDownTime-=Time.deltaTime;
if(rampDownTime<0){ rampDownTime=0;}
}

}