using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTranslate : MonoBehaviour
{
    public Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 thisPos = transform.position;
        Vector3 pos = trans.position;
        thisPos.x = pos.x;
        thisPos.z = pos.z;
        transform.position = pos;
    }
}
