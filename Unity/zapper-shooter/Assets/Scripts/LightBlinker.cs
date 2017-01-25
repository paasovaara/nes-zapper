using UnityEngine;
using System.Collections;

public class LightBlinker : MonoBehaviour {

    float m_timer = 0.0f;
    float m_nextBlink = 0.0f;
    
    float m_intensity = 0.0f;
    [SerializeField]
    Light m_light;

    // Use this for initialization
    void Start () {
        if (m_light == null)
            m_light = GetComponent<Light>();

        initNextBlink();
	}
	
    private void initNextBlink() {
        float blinkLength = Random.Range(0.025f, 0.1f);
        float nextTime = 0.0f;

        //likelihood of short burst:
        bool shortBurst = Random.Range(0.0f, 1.0f) > 0.4f;
        if (shortBurst) {
            nextTime = Random.Range(blinkLength + 0.1f, 0.4f);
        }
        else {
            nextTime = Random.Range(blinkLength + 3.0f, 5.0f);
        }

        m_nextBlink = nextTime;
        m_timer = -blinkLength;

    }

    // Update is called once per frame
    void Update () {
        m_timer += Time.deltaTime;

        if (m_timer > m_nextBlink) {
            initNextBlink();

            m_intensity = m_light.intensity;
            m_light.intensity = 0.0f;

        }
        else if (m_timer >= 0.0f && m_intensity > 0.0f) {
            //TODO using a state would be nicer..
            m_light.intensity = m_intensity;
        }
    }
}
