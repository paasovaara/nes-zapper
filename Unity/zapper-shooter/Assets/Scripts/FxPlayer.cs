using UnityEngine;
using System.Collections;

public class FxPlayer : MonoBehaviour {
    [SerializeField]
    private AudioManager m_audioManager;

    private bool m_playItAgain = false;
    
    void Start() {
        StartCoroutine(playAndWait(5f, 5f));
    }

    // Update is called once per frame
    void Update () {
	    if (m_playItAgain) {
            StartCoroutine(playAndWait(5f, 15f));
        }
	}

    private IEnumerator playAndWait(float preDelay, float waitDelay) {
        m_playItAgain = false;
        yield return new WaitForSeconds(preDelay);
        m_audioManager.playClip(AudioManager.AppAudioClip.RandomFx);
        yield return new WaitForSeconds(waitDelay);
        m_playItAgain = true;
    }
}
