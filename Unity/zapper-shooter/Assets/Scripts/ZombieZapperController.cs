using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ZombieZapperController : MonoBehaviour, IArduinoMessageHandler {

    private const bool DEBUG_KILLING = true;
    /*
     * TODO inherit from same base class with ZapperController, if possible
     * */
    private const int TARGET_LAYER_MASK = 1 << 8;

    private int m_planeFrameCounter = 0;
    private ArduinoListener m_arduino;
    private List<ArduinoMessage> m_latestBurst = new List<ArduinoMessage>();

    [SerializeField]
    Camera m_mainCamera;
    [SerializeField]
    Camera m_zapperCamera;
    [SerializeField]
    Transform m_flashlightDirection;
    [SerializeField]
    float m_flashlightHitRadius = 0.5f; //this should at max be the lens radius. Can be also smaller to make hitting harder

    [SerializeField]
    private EnemySpawner m_spawner;
    
    
    enum DisplayState {
        IDLE,
        BLANK_FRAME,
        TARGET,
        WAIT_FOR_RESULTS
    }
    private DisplayState m_state;

    private enum SignalProcState {
        INIT,
        NOTHING_FOUND,
        BLANK_FOUND,
        HIT_FOUND
    }

    private int getFrameCountForState(DisplayState state) {
        switch (state) {
            case DisplayState.BLANK_FRAME: return 1;
            case DisplayState.TARGET: return 1;
            case DisplayState.WAIT_FOR_RESULTS: return 5;
            default: return -1;
        }
    }
    
    // Use this for initialization
    void Start() {

        m_state = DisplayState.IDLE;
        m_arduino = GetComponent<ArduinoListener>();
        m_arduino.addMessageHandler(this);

        swapCamera(true);
        
        handleState();
    }

    void swapCamera(bool useMain) {
        m_mainCamera.gameObject.SetActive(useMain);
        m_zapperCamera.gameObject.SetActive(!useMain);
    }

    void swapMaterials(MaterialController.MaterialState newState) {
        foreach (GameObject o in m_spawner.getZombies()) {
            o.GetComponent<MaterialController>().setState(newState);
        }
    }

    void handleState() {
        if (m_state == DisplayState.BLANK_FRAME) {
            //TODO refactor to separate function
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.TARGET;
                m_planeFrameCounter = 0;

                swapMaterials(MaterialController.MaterialState.TARGET);
                
                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

        }
        else if (m_state == DisplayState.TARGET) {
            //TODO refactor to separate function
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.WAIT_FOR_RESULTS;
                m_planeFrameCounter = 0;

                swapMaterials(MaterialController.MaterialState.NORMAL);
                swapCamera(true);

                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

        }
        else if (m_state == DisplayState.WAIT_FOR_RESULTS) {
            //TODO refactor to separate function
            bool hit = determineHit(m_latestBurst);
            if (hit) {
                m_state = DisplayState.IDLE;
                m_planeFrameCounter = 0;

                killTarget();
            }
            else if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.IDLE;
                m_planeFrameCounter = 0;

                Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
            }
            m_planeFrameCounter++;

            
        }
        else if (m_state == DisplayState.IDLE) {
            m_planeFrameCounter = 0;
        }

    }

    void startZapperLogic() {
        if (m_state == DisplayState.IDLE) {
            m_planeFrameCounter = 0;
            m_state = DisplayState.BLANK_FRAME;

            swapMaterials(MaterialController.MaterialState.BLANK);
            swapCamera(false);
        }
        else {
            Debug.LogError("Wow, this guy is trigger happy!");
        }
    }

    void Update() {
        bool pressed = Input.GetButtonDown("trigger");

        List<ArduinoMessage> msgs = m_arduino.getAndClearQueue();
        if (msgs != null) {
            foreach (ArduinoMessage msg in msgs) {
                //TODO define the interface better
                if (msg.Message.Contains("TRIG")) {
                    pressed = true;
                    m_latestBurst.Clear();
                }
                else if (msg.Message.Contains("END")) {
                    string messages = printMessages(m_latestBurst);
                    string diff = printDiff(m_latestBurst);
                    writeToFile(messages + "\n" + diff);
                }
                else {
                    m_latestBurst.Add(msg);
                }
            }
        }

        if (pressed) {
            Debug.Log("Trigger pressed");
            startZapperLogic();
        }

        handleState();
    }

    private void killTarget() {
        //TODO somehow figure out which zombie got hit!?
        List<GameObject> zombies = m_spawner.getZombies();
        GameObject toDie = null;
        foreach(GameObject zombie in zombies) {
            if (amIHit(zombie)) {
                Debug.Log("Dead zombie is the best zombie! DIE!");

                Debug.Assert(toDie == null, "We hit two zombies at the same round..");
                toDie = zombie;
            }
        }
        if (toDie != null) {
            m_spawner.KillZombie(toDie);
        }
    }

    private bool amIHit(GameObject zombie) {
        bool hit = false;
        RaycastHit raycastHit;

        Vector3 origin = m_flashlightDirection.transform.position;
        float distanceToObstacle = 0;
        float maxDistance = 10f;
        
        // Cast a sphere wrapping character controller forward from camera
        // to see if it is about to hit anything.
        if (Physics.SphereCast(origin, m_flashlightHitRadius, m_flashlightDirection.forward, out raycastHit, maxDistance, TARGET_LAYER_MASK)) {
            distanceToObstacle = raycastHit.distance;
            Debug.Log("Raycast is a hit to zombie at distance: " + distanceToObstacle);
            hit = true;
        }
        else {
            Debug.Log("Raycast Did not hit zombie");
        }

        return hit;
    }


    private bool determineHit(List<ArduinoMessage> msgs) {
        if (DEBUG_KILLING)
            return true;

        if (msgs.Count == 0)
            return false;
        SignalProcState state = SignalProcState.INIT;
        //int startValue = 0;
        foreach (ArduinoMessage msg in msgs) {

        }
        
        return state == SignalProcState.HIT_FOUND;
    }

    private string printMessages(List<ArduinoMessage> msgs) {
        StringBuilder builder = new StringBuilder();
        builder.Append("Buffer:\t[");
        foreach (ArduinoMessage msg in msgs) {
            builder.AppendFormat("{0},\t", msg);
        }
        builder.Append("]");
        return builder.ToString();

    }

    private string printDiff(List<ArduinoMessage> msgs) {
        StringBuilder builder = new StringBuilder();
        builder.Append("Diff:\t[");
        int prev = -1;
        int val = -1;

        foreach (ArduinoMessage msg in msgs) {
            if (int.TryParse(msg.Message, out val)) {
                if (prev < 0) {
                    builder.AppendFormat("{0},\t", val);
                }
                else {
                    builder.AppendFormat("{0},\t", val - prev);
                }
                prev = val;
            }

        }
        builder.Append("]");
        return builder.ToString();
    }


    public void messageReceived(ArduinoMessage msg) {
        //if (m_state != DisplayState.IDLE)
        //    Debug.Log(string.Format("[Zapper at {0}]: {1}", m_state, msg));
    }

    private void writeToFile(string msg) {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"./zapper-zombie.txt", true)) {
            file.WriteLine(msg);
        }
    }
}
