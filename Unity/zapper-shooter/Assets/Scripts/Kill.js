#pragma strict
var explodingZombie : GameObject;

function Start () {

}

function Update () {

	if(Input.GetKeyDown("space")) {
		var zombie = Instantiate(explodingZombie, transform.position, Quaternion.identity);

		for(var zombiePart: Transform in zombie.transform) {
			var rb : Rigidbody = zombiePart.gameObject.GetComponent.<Rigidbody>();
			rb.AddExplosionForce(200.0, zombie.transform.position, 15.0, 3.0);
		}


		Destroy(gameObject);

	}
	

}