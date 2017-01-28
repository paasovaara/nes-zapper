using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Die : MonoBehaviour {

    [SerializeField]
    GameObject m_diePrefab;

	public void killMe() {
        GameObject zombie = Instantiate(m_diePrefab) as GameObject;
        //add other sounds when parts collide to the floor
        AudioManager.Instance.playClip(AudioManager.AppAudioClip.DieZombie);

        foreach (Transform partTransform in zombie.transform) {
            GameObject zombiePart = partTransform.gameObject;
            Rigidbody rb = zombiePart.GetComponent<Rigidbody>();
            rb.AddExplosionForce(200.0f, zombie.transform.position, 15.0f, 3.0f);
        }
        
        Destroy(gameObject);
        
    }

}
