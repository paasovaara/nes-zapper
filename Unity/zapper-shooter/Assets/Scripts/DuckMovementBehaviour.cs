using UnityEngine;
using System.Collections;

public class Bounds {
    public float width;
    public float height;
}

public class DuckMovementBehaviour : MonoBehaviour {
    private Bounds m_bounds;

    float m_timer = 0.0f;
    float m_nextDirectionChange = 0.0f;
    const float MAX_DIRECTION_TIMER = 3.0f;
    const float VELOCITY_PER_FRAME = 0.02f;

    Vector2 m_direction = new Vector2();

	// Use this for initialization
	void Start () {
        m_bounds = new Bounds();
        m_bounds.width = 10.0f;
        m_bounds.height = 7.0f;
        Vector2 vec2 = Random.insideUnitCircle;
        this.transform.position = new Vector3(vec2.x, vec2.y, this.transform.position.z);

        initDirectionTimer();
        setNewDirection();
	}

    private void initDirectionTimer() {
        m_nextDirectionChange = Mathf.Clamp(Random.value * MAX_DIRECTION_TIMER, 0.5f, MAX_DIRECTION_TIMER);
        m_timer = 0.0f;
    }

    private void setNewDirection() {
        m_direction = Random.insideUnitCircle;
    }
	
    private void checkBounds() {
        //TODO bounce if goes out of bounds

    }

	// Update is called once per frame
	void Update () {
        m_timer += Time.deltaTime;
        if (m_timer > m_nextDirectionChange) {
            setNewDirection();
            initDirectionTimer();
        }

        //Move the duck
        //TODO would it be smarter to move rigidbody with some velocity?
        Transform t = this.gameObject.transform;
        Vector3 posDelta = VELOCITY_PER_FRAME * new Vector3(m_direction.x, m_direction.y, 0.0f);
        this.gameObject.transform.position = this.gameObject.transform.position + posDelta;

        checkBounds();
	}
}
