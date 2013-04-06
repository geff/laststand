#pragma strict

public var Tank:GameObject;
public var CameraHeight:float = 0;

function Start () 
{

}

function Update () 
{
	
	if (Input.GetKey("o") && transform.position.y < 100)
	{
		CameraHeight += 20*Time.deltaTime;
		if (CameraHeight > 100) {CameraHeight = 100;}
	}
	
	if (Input.GetKey("p") && transform.position.y > 15)
	{
		CameraHeight -= 20*Time.deltaTime;
		if (CameraHeight < -15) {CameraHeight = -15;}
	}
	
	
	transform.position = Tank.transform.position;
	transform.position.y += 30+CameraHeight;
	transform.LookAt(Tank.transform);
}