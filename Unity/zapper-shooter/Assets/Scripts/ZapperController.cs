using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZapperController : MonoBehaviour, IArduinoMessageHandler {
    [SerializeField]
    GameObject m_plane;

    [SerializeField]
    Transform m_targetPos;

    [SerializeField]
    GameObject m_targetPrefab;

    private GameObject m_targetObj;

    private int m_planeFrameCounter = 0;

    private ArduinoListener m_arduino;

    enum DisplayState {
        IDLE,
        BLANK_FRAME,
        TARGET,
        WAIT_FOR_RESULTS
    }
    private DisplayState m_state;

    private int getFrameCountForState(DisplayState state) {
        switch(state) {
        case DisplayState.BLANK_FRAME: return 1;
        case DisplayState.TARGET: return 1;
        case DisplayState.WAIT_FOR_RESULTS: return 5;
        default: return -1;
        }
    }


	// Use this for initialization
	void Start () {

        m_state = DisplayState.IDLE;
        m_arduino = GetComponent<ArduinoListener>();
        m_arduino.addMessageHandler(this);

        createTarget();
        handleState();

        m_plane.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
	}
	
    void showPlane() {
        if (m_state == DisplayState.IDLE) {
            m_planeFrameCounter = 0;
            m_plane.SetActive(true);
            m_state = DisplayState.BLANK_FRAME;

        }
        else {
            Debug.LogError("Wow, this guy is trigger happy!");
        }
    }

    void createTarget() {
        GameObject target = Instantiate(m_targetPrefab) as GameObject;
        target.GetComponent<Renderer>().material.color = new Color(255, 255, 255); 
        m_targetObj = target;
        moveTarget();
    }

    void moveTarget() {
        //TODO project the size properly
        Vector3 newPos = new Vector3(m_targetPos.position.x, m_targetPos.position.y, m_targetPos.position.z);
        m_targetObj.transform.position = newPos;

    }

    void handleState() {
        if (m_state == DisplayState.BLANK_FRAME) {
            //TODO refactor to separate function
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.TARGET;
                moveTarget();
                //createTarget();//Is this slow as hell? Move to constructor and only activate here
                m_planeFrameCounter = 0;

                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

        }
        else if (m_state == DisplayState.TARGET) {
            //TODO refactor to separate function
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.WAIT_FOR_RESULTS;
                m_planeFrameCounter = 0;

                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

        }
        else if (m_state == DisplayState.WAIT_FOR_RESULTS) {
            //TODO refactor to separate function
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.IDLE;
                m_planeFrameCounter = 0;

                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

        }
        else if (m_state == DisplayState.IDLE) {
            /*if (m_targetObj != null) {
                Destroy(m_targetObj);
                m_targetObj = null;
            }*/
            m_planeFrameCounter = 0;
        }

        m_plane.SetActive(m_state == DisplayState.BLANK_FRAME || m_state == DisplayState.TARGET);
        m_targetObj.SetActive(m_state == DisplayState.TARGET);
        m_targetPos.gameObject.SetActive(m_state == DisplayState.IDLE || m_state == DisplayState.WAIT_FOR_RESULTS);//TODO set only visibility? This also disables movement
    }

	void Update () {
        bool pressed = Input.GetButtonDown("trigger");

        List<ArduinoMessage> msgs = m_arduino.getAndClearQueue();
        if (msgs != null) {
            foreach(ArduinoMessage msg in msgs) {
                //TODO define the interface better
                if (msg.Message.Contains("TRIG")) {
                    pressed = true;
                }
            }
        }

        if (pressed)
        {
            Debug.LogFormat ("Trigger pressed");
            showPlane();
        }

        handleState();
    }

    public void messageReceived(ArduinoMessage msg) {
        if (m_state != DisplayState.IDLE)
            Debug.Log(string.Format("[Zapper at {0}]: {1}", m_state, msg));
    }
}
