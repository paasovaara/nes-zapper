using UnityEngine;
using System.Collections;

public class FxPlayer : MonoBehaviour {
    
    private bool m_playItAgain = false;
    
    void Start() {
        StartCoroutine(playAndWait(5f, 15f));
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
        AudioManager.Instance.playClip(AudioManager.AppAudioClip.RandomFx);
        yield return new WaitForSeconds(waitDelay);
        m_playItAgain = true;
    }
}
