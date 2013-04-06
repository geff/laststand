#pragma strict

function Start () 
{
	
}

function Awake ()
{
	transform.position = Vector3(100, 20, 100);
}

function FixedUpdate () 
{
	if (Input.GetKey("z"))
	{
		rigidbody.AddRelativeForce(100*Vector3.forward);
	}
	
	if (Input.GetKey("s"))
	{
		rigidbody.AddRelativeForce(-100*Vector3.forward);
	}
	
	if (Input.GetKey("q"))
	{
		transform.Rotate(-Vector3.up*90*Time.deltaTime);
	}
	
	if (Input.GetKey("d"))
	{
		transform.Rotate(Vector3.up*90*Time.deltaTime);
	}
}