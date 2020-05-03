using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private Vector3 oldPosition;
    private MusicPlayManager musicPlayManager;

    public bool isLeftHand = false;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        oldPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        musicPlayManager.addMovingDistance(calcDistance());
    }

    float calcDistance() {
        float dist = (this.transform.position - oldPosition).magnitude;
        oldPosition = this.transform.position;
        return Mathf.Abs(dist);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "MusicObject") {
            StartCoroutine(Vivration(0.1f, 0.3f));
        }
    }

    IEnumerator Vivration(float time, float strong) {
        if (isLeftHand) {
            OVRInput.SetControllerVibration(strong, strong, OVRInput.Controller.LTouch);
        }
        else {
            OVRInput.SetControllerVibration(strong, strong, OVRInput.Controller.RTouch);
        }
        yield return new WaitForSeconds(time);
        if (isLeftHand) {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        }
        else {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
