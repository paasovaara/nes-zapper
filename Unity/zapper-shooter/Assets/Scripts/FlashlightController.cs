using UnityEngine;
using System.Collections;

public class FlashlightController : MonoBehaviour {

    const float DEG_PER_SEC = 25.0f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 localRotation = transform.localEulerAngles;
        
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        if (left && !right) {
            transform.Rotate(Time.deltaTime * new Vector3(0.0f, -DEG_PER_SEC, 0.0f));
        }
        else if (right && !left) {
            transform.Rotate(Time.deltaTime * new Vector3(0.0f, DEG_PER_SEC, 0.0f));
        }

        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);

        if (up && !down) {
            transform.Rotate(Time.deltaTime * new Vector3(-DEG_PER_SEC, 0.0f, 0.0f));
        }
        else if (down && !up) {
            transform.Rotate(Time.deltaTime * new Vector3(DEG_PER_SEC, 0.0f, 0.0f));
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
    }
}
