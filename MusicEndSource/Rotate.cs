using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateV;
    private Transform t;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ang = t.transform.eulerAngles;
        ang.z += rotateV;
        if (ang.z > 360.0f) ang.z = 0f;
        t.transform.eulerAngles = ang;
        
    }
}
