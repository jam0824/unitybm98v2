using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    private Vector3 oldPosition;
    private MusicPlayManager musicPlayManager;

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
}
