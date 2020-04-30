using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMusicSelectEvent : MonoBehaviour
{
    public GameObject arrowRight;
    public GameObject arrowLeft;

    private MusicSelect musicSelect;

    // Start is called before the first frame update
    void Start()
    {
        this.musicSelect = GameObject.Find("MusicSelectManager").GetComponent<MusicSelect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "laser") {
            arrowRight.GetComponent<ColliderEvent>().setNotActiveSprite();
            arrowLeft.GetComponent<ColliderEvent>().setNotActiveSprite();
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
                musicSelect.selectedMusic();
            }
        }
    }
}
