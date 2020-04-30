using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vivration : MonoBehaviour
{
    public float VIVERATION_TIME = 0.1f;


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "MusicObject") {
            StartCoroutine(Vivration(VIVERATION_TIME));
        }
    }

    IEnumerator Vivration(float time) {
        var activeController = OVRInput.GetActiveController();
        OVRInput.SetControllerVibration(1, 1, activeController);
        yield return new WaitForSeconds(time);
        OVRInput.SetControllerVibration(0, 0, activeController);
    }
}
