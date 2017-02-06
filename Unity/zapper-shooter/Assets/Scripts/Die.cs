using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Die : Health {

    [SerializeField]
    GameObject m_diePrefab;

    public bool shotHit() {
        if (dec() > 0) {
            woundMe();
            return false;
        }
        else {
            killMe();
            return true;
        }
    }

    private void woundMe() {
        Transform highest = null;
        foreach (Transform partTransform in transform) {
            if (highest == null) {
                highest = partTransform;
            }
            else if (partTransform.position.y > highest.position.y) {
                highest = partTransform;
            }
        }
        if (highest) {
            Rigidbody rb = highest.gameObject.GetComponent<Rigidbody>();
            rb.AddExplosionForce(200.0f, highest.transform.position, 15.0f, 3.0f);
        }
    }

    private void killMe() {      
        GameObject zombie = Instantiate(m_diePrefab) as GameObject;
        zombie.transform.position = gameObject.transform.position;
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
