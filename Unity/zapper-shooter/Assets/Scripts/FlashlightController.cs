using UnityEngine;
using System.Collections;

public class FlashlightController : MonoBehaviour {

    const float DEG_PER_SEC = 25.0f;
	
	void Update () {
        keyboardInput();
        joystickInput();

        //clamp z always to 0
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
    }

    private void keyboardInput() {
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
    }

    private void joystickInput() {
        float horizontal = Input.GetAxis("js-horizontal");
        float vertical = Input.GetAxis("js-vertical");
        //Debug.LogFormat("joystick hor: {0}, ver {1}", horizontal, vertical);
        Vector3 rotation = new Vector3(vertical, horizontal, 0.0f) * Time.deltaTime * DEG_PER_SEC;
        transform.Rotate(rotation);

    }
}
