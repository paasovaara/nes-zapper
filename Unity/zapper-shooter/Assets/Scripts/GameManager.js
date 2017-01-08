#pragma strict
var lights : GameObject[];
var zombie : GameObject;

function Start () {

}

function Update () {

	if(Input.GetKeyDown("1")) {
		lights[0].GetComponent.<Light>().color = Color.blue;
		lights[0].GetComponent.<Light>().intensity = 8;
		lights[1].GetComponent.<Light>().intensity = 3.2;
		lights[2].GetComponent.<Light>().intensity = 0;
	}
	if(Input.GetKeyDown("2")) {
		lights[1].GetComponent.<Light>().intensity = 0;
	}
	if(Input.GetKeyDown("3")) {
		lights[2].GetComponent.<Light>().intensity = 8;
		lights[0].GetComponent.<Light>().intensity = 0;

	}
	if(Input.GetKeyDown("4")) {
		Instantiate(zombie);

	}


}