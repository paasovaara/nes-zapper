using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class ZombieZapperController : MonoBehaviour, IArduinoMessageHandler {

    /*
     * TODO inherit from same base class with ZapperController, if possible
     * */
    private const int TARGET_LAYER_MASK = 1 << 8;

    private bool m_zapperConnectionOk = false;

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
        WAIT_FOR_RESULTS,
        RELOAD
    }
    private DisplayState m_state;

    private enum SignalProcState {
        LOOKING_BLANK,
        LOOKING_HIGH,
        MISSED,
        HIT
    }

    private int getFrameCountForState(DisplayState state) {
        switch (state) {
            case DisplayState.BLANK_FRAME: return 1;
            case DisplayState.TARGET: return 1;
            case DisplayState.WAIT_FOR_RESULTS: return 5;
            case DisplayState.RELOAD: return 120;
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
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.TARGET;
                m_planeFrameCounter = 0;

                swapMaterials(MaterialController.MaterialState.TARGET);
            }
        }
        else if (m_state == DisplayState.TARGET) {
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.WAIT_FOR_RESULTS;
                m_planeFrameCounter = 0;

                swapMaterials(MaterialController.MaterialState.NORMAL);
                swapCamera(true);            
            }
        }
        else if (m_state == DisplayState.WAIT_FOR_RESULTS) {
            bool hit = determineHitFromZapperData(m_latestBurst);
            if (hit) {
                killTarget();
            }
            if (hit || m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.RELOAD;
                m_planeFrameCounter = 0;
                AudioManager.Instance.playClip(AudioManager.AppAudioClip.PistolReload);
            }
        }
        else if (m_state == DisplayState.RELOAD) {
            if (m_planeFrameCounter > getFrameCountForState(m_state)) {
                m_state = DisplayState.IDLE;
                m_planeFrameCounter = 0;
            }
        }
        else if (m_state == DisplayState.IDLE) {
            m_planeFrameCounter = 0;
        }

        if (m_state != DisplayState.IDLE) {
            m_planeFrameCounter++;
            //Debug.LogFormat("Last frame time {0} ms", Time.deltaTime * 1000);
        }
    }

    void startZapperLogic() {
        if (m_state == DisplayState.IDLE) {
            m_planeFrameCounter = 0;
            m_state = DisplayState.BLANK_FRAME;

            swapMaterials(MaterialController.MaterialState.BLANK);
            swapCamera(false);

            AudioManager.Instance.playClip(AudioManager.AppAudioClip.PistolShot);
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
                    List<int> values = messageQueueValues(m_latestBurst);
                    writeToFile(printValues(values));

                    List<int> normalized = removeAverage(values);
                    writeToFile(printValues(normalized));
                    
                    List<int> normalizedUsingStart = removeAverageOfStartSamples(values, 4);
                    writeToFile(printValues(normalizedUsingStart));

                    writeToFile("");
                    //string messages = printMessages(m_latestBurst);
                    //string diff = printDiff(m_latestBurst);
                    //writeToFile(messages + "\n" + diff);
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

    /*
    void FixedUpdate() {
        if (m_state != DisplayState.IDLE) {
            return;
        }
        float speed = 5.0f;

        List<GameObject> zombies = m_spawner.getZombies();
        GameObject zombieInSpotlight = whichGotHit(zombies);
        foreach(GameObject zombie in zombies) {
            Rigidbody rb = zombie.GetComponent<Rigidbody>();
            if (zombie == zombieInSpotlight) {
                rb.velocity *= 0f;
            }
            else {
                Vector3 towardsFlashligh = m_flashlightDirection.position - rb.transform.position;
                Vector3 direction = Vector3.Scale(new Vector3(1f, 0f, 1f), towardsFlashligh);
                rb.velocity = speed * Time.deltaTime * direction.normalized;
            }
            //Debug.Log(rb.velocity + " direction : " + direction.normalized);
        }
    }*/
    

    private void killTarget() {
        GameObject toDie = whichGotHit(m_spawner.getZombies());
        if (toDie != null) {
            Debug.Log("Dead zombie is the best zombie! DIE!");
            m_spawner.KillZombie(toDie);
        }
    }

    private GameObject whichGotHit(List<GameObject> zombies) {
        //use Raycast to find out which zombie (if any) got hit.
        GameObject toDie = null;
        RaycastHit raycastHit;

        Vector3 origin = m_flashlightDirection.transform.position;
        float maxDistance = 10f;
        
        if (Physics.SphereCast(origin, m_flashlightHitRadius, m_flashlightDirection.forward, out raycastHit, maxDistance, TARGET_LAYER_MASK)) {
            float distanceToObstacle = raycastHit.distance;
            GameObject raycastObject = raycastHit.collider.gameObject;
            Transform raycastObjParentTransform = raycastObject.transform.parent;

           ///Debug.Log("Raycast is a hit to zombie at distance: " + distanceToObstacle);
            foreach (GameObject zombie in zombies) {
                bool gotHit = false;
                gotHit = (raycastObject == zombie);
                //check also parent, since we haven't yet decided where we have the colliders, parent or the parts.
                //TODO remove useless code later
                if (raycastObjParentTransform != null) {
                    gotHit |= (raycastObjParentTransform.gameObject == zombie);
                }
                if (gotHit) {
                    Debug.Assert(toDie == null, "We hit two zombies at the same round, impossible..");
                    toDie = zombie;    
                }
            }
        }
        else {
            //Debug.Log("Raycast Did not hit any zombies");
        }

        return toDie;
    }


    private bool determineHitFromZapperData(List<ArduinoMessage> msgs) {
        if (!m_zapperConnectionOk) {
            //we are possibly debugging without the gun
            return true;
        }


        if (msgs.Count < 1)
            return false;
        
        float displayFps = 60f;
        float zapperFps = 1.0f / 8.0f;
        int samplesPerFrame = Mathf.RoundToInt(displayFps / zapperFps);
        int maxStateLength = 2 * samplesPerFrame;//let's duplicate just in case
        //Debug.LogFormat("displayFPS: {0}, zapperFPS: {1}, samplesPerFrame: {3}", displayFps, zapperFps, samplesPerFrame); 

        List<int> values = messageQueueValues(msgs);
        //delay for the displays can easily be 60ms, but let's go with 30ms just to be sure => about 4 samples
        List<int> normalized = removeAverageOfStartSamples(values, 4);

        //These thresholds are very much affected by the display contrast and in-game-flashlight intensity.
        //TODO add calibration mode where we define the flashlight intensity and these thresholds.
        int blankThreshold = -25;
        int highThreshold = 50;
        int sampleCounter = 0;
        SignalProcState state = SignalProcState.LOOKING_BLANK;
        foreach (int sample in normalized) {
            if (state == SignalProcState.LOOKING_BLANK) {
                if (sample <= blankThreshold) {
                    Debug.LogFormat("Blank found {0}, let's look for hit", sample);
                    state = SignalProcState.LOOKING_HIGH;             
                }
            }
            else if (state == SignalProcState.LOOKING_HIGH) {
                if (sampleCounter > maxStateLength) {
                    state = SignalProcState.MISSED;
                    Debug.LogFormat("Waited for high value for {0} samples, considering this as a miss.", maxStateLength);
                    break;
                }
                if (sample >= highThreshold) {
                    Debug.LogFormat("Found HIT after {0} frames, value was {1}", sampleCounter, sample);

                    state = SignalProcState.HIT;
                    break;
                }
                sampleCounter++;

            }
        }
        return state == SignalProcState.HIT;
    }

    private List<int> messageQueueValues(List<ArduinoMessage> msgs) {
        List<int> values = new List<int>();
        int val = 0;

        foreach (ArduinoMessage msg in msgs) {
            if (int.TryParse(msg.Message, out val)) {
                values.Add(val);
            }
            else {
                Debug.Assert(false, "Failed to parse message value: " + msg);
            }
        }
        return values;
    }
    
    private List<int> removeAverage(List<int> values) {
        int average = Mathf.RoundToInt((float)values.Average());
        return values.Select(n => n - average).ToList();
    }

    private List<int> removeAverageOfStartSamples(List<int> values, int sampleCount) {
        int average = Mathf.RoundToInt((float)values.GetRange(0, sampleCount).Average());
        //Debug.Log("average of " + sampleCount + " first samples is " + average);
        return values.Select(n => n - average).ToList();
    }

    private string printValues(List<int> values) {
        StringBuilder builder = new StringBuilder();
        foreach (int val in values) {
            builder.AppendFormat("{0},\t", val);
        }
        return builder.ToString();

    }
    /*
    private string printDiff(List<ArduinoMessage> msgs) {
        StringBuilder builder = new StringBuilder();
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
        return builder.ToString();
    }*/


    public void messageReceived(ArduinoMessage msg) {
    }

    public void statusChanged(ArduinoStatus newStatus) {
        Debug.Log("Arduino status changed to " + newStatus);
        if (newStatus == ArduinoStatus.INITIALIZED_AND_RUNNING) {
            m_zapperConnectionOk = true;
        }
        else if (newStatus == ArduinoStatus.INIT_FAILED) {
            m_zapperConnectionOk = false;
        }
    }

    private void writeToFile(string msg) {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"./zapper-zombie.txt", true)) {
            file.WriteLine(msg);
        }
    }
}
