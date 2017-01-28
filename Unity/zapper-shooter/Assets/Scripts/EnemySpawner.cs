using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private GameObject m_targetPrefab;

    [SerializeField]
    private List<Transform> m_spawnPoints = new List<Transform>();

    private List<GameObject> m_zombies = new List<GameObject>();

    private bool m_createAgain = true;
    
    public List<GameObject> getZombies() {
        return m_zombies;
    }

    void Start() {
        StartCoroutine(createAndWait(3f, 5f));
    }
    
    // Update is called once per frame
    void Update() {
        if (m_createAgain) {
            float untilNext = Random.Range(5.0f, 15.0f);
            StartCoroutine(createAndWait(0f, untilNext));
        }
    }

    bool canSpawnHere (Transform point) {
        // Make sure we don't spawn two monsters to same spawn position
        //TODO make sure we don't spawn enemies when we can see them.
        Vector3 pos = point.position;
        foreach (GameObject zombie in m_zombies) {
            Vector3 zombiePos = zombie.transform.position;
            float distance = (zombiePos - pos).magnitude;
            if ( distance < 0.1) {
                Debug.Log("Cannot spawn zombie to " + pos + " since nearest zombie distance is " + distance);
                return false;
            }            
        }
        return true;
    }
    
    void createRandomZombie() {
        Debug.Log("Time to create a Zombie!");
        //create to random unoccupied place
        List<Transform> unoccupied = new List<Transform>();
        foreach (Transform point in m_spawnPoints) {
            if (canSpawnHere(point)) {
                unoccupied.Add(point);
            }
        }

        if (unoccupied.Count == 0) {
            Debug.Log("no vacancy for zombies. maybe next time?");
        }
        else {
            unoccupied.Shuffle();

            int index = Mathf.RoundToInt(Random.Range(0f, unoccupied.Count - 1));
            if (index < unoccupied.Count) {
                Transform t = unoccupied[index];
                Debug.Log("Found a perfect spot, spawning new zombie to " + t.position);
                //TODO add slight randomness to the spot?

                //TODO get the parent object
                GameObject zombie = Instantiate(m_targetPrefab) as GameObject;
                zombie.transform.position = t.position;
                m_zombies.Add(zombie);

                AudioManager.Instance.playClip(AudioManager.AppAudioClip.ZombieSpawned);
            }

        }
    }

    public void KillZombie(GameObject zombie) {
        if (!m_zombies.Remove(zombie)) {
            Debug.LogError("Failed to find Zombie from EnemySpawner list! Bug?!");
            Debug.Assert(false);
        }
        zombie.GetComponent<Die>().killMe(zombie.transform);
    }

    private IEnumerator createAndWait(float preDelay, float waitDelay) {
        m_createAgain = false;
        yield return new WaitForSeconds(preDelay);
        createRandomZombie();
        yield return new WaitForSeconds(waitDelay);
        m_createAgain = true;
    }
}
