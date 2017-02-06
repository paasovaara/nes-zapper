using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    [SerializeField]
    protected int m_maxHealth = 3;

    protected int m_health = 0;

    void Start() {
        m_health = Mathf.RoundToInt(Random.Range(1.0f, (float)m_maxHealth));
        Debug.Log("My health is " + m_health);
    }

    protected int dec() {
        return --m_health;
    }
}
