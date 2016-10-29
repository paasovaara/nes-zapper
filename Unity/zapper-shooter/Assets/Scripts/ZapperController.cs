using UnityEngine;
using System.Collections;

public class ZapperController : MonoBehaviour {
    [SerializeField]
    GameObject m_plane;

    [SerializeField]
    Transform m_targetPos;

    [SerializeField]
    GameObject m_targetPrefab;

    private GameObject m_targetObj;

    private int m_planeFrameCounter = 0;

    enum DisplayState {
        IDLE,
        BLANK_FRAME,
        TARGET
    }
    private DisplayState m_state;

    private int getFrameCountForState(DisplayState state) {
        switch(state) {
        case DisplayState.BLANK_FRAME: return 2;
        case DisplayState.TARGET: return 2;
        default: return -1;
        }
    }


	// Use this for initialization
	void Start () {

        m_state = DisplayState.IDLE;
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

        m_plane.SetActive(m_state != DisplayState.IDLE);
        m_targetObj.SetActive(m_state == DisplayState.TARGET);
        m_targetPos.gameObject.SetActive(m_state == DisplayState.IDLE);//TODO set only visibility? This also disables movement
    }

	void Update () {
        bool pressed = Input.GetButtonDown("trigger");
        if (pressed)
        {
            Debug.LogFormat ("Trigger pressed");
            showPlane();
        }

        handleState();
    }
}
