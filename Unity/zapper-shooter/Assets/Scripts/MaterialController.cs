using UnityEngine;
using System.Collections;

public class MaterialController : MonoBehaviour {

    [SerializeField]
    private Material _Idle;

    [SerializeField]
    private Material _OnPress;

    public enum MaterialState {
        NORMAL,
        BLANK,
        TARGET
    }


    // Use this for initialization
    void Start () {
        setMaterial(_Idle);
	}

    private void setMaterial(Material m) {
        //perhaps not the fastest operation? TODO optimize
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers) {
            r.material = m;
        }
    }

    public void setState(MaterialState state) {
        if (state == MaterialState.NORMAL) {
            setMaterial(_Idle);
        }
        else {
            setMaterial(_OnPress);
            Color c = (state == MaterialState.BLANK) ? Color.black : Color.white;
            _OnPress.SetColor("_MyColor", c);
        }
    }

	// Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            setMaterial(_OnPress);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) {
            setMaterial(_Idle);
        }
    }
}
