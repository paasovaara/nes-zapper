using UnityEngine;
using System.Collections;

public class DelayedCleanup : MonoBehaviour {

    [SerializeField]
    private float delay = 1.0f;

	// Use this for initialization
	void Start () {
        StartCoroutine(cleanup(delay));
	}
	
    private IEnumerator cleanup(float delay) {
        yield return new WaitForSeconds(delay);
        //TODO maybe add fadeout 
        Destroy(gameObject);
        
    }
}
