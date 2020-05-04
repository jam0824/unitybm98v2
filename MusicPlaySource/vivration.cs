using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vivration : MonoBehaviour
{
    public float VIVERATION_TIME;
    public float VIVERATION_POWER;
    public string TRIGGER_TAG;
    public bool isLeftHand = false; //左手右手判別用

    //Triggerのエンター時に起動させる
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == TRIGGER_TAG) {
            StartCoroutine(Vivration(VIVERATION_TIME, VIVERATION_POWER));
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
