using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderEvent : MonoBehaviour
{
    public Sprite NOT_ACTIVE_SPRITE;
    public Sprite ACTIVE_SPRITE;

    private Sprite arrowSprite;
    private MusicSelect musicSelect;

    private bool isStay = false;

    public bool isRight;

    // Start is called before the first frame update
    void Start()
    {
        this.arrowSprite = this.GetComponent<Image>().sprite;
        this.musicSelect = GameObject.Find("MusicSelectManager").GetComponent<MusicSelect>();
    }

    public void setNotActiveSprite() {
        isStay = false;
        this.GetComponent<Image>().sprite = NOT_ACTIVE_SPRITE;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyUp(KeyCode.Z))&&(this.isRight)) {
            selectNextMusic();
        }
        if((isStay) && (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))) {
            selectNextMusic();
        }
    }

    void OnTriggerStay(Collider other) {
        if(other.gameObject.tag == "laser") {
            this.GetComponent<Image>().sprite = ACTIVE_SPRITE;
            isStay = true;
            
        }
    }

    void selectNextMusic() {
        musicSelect.nextMusic(this.isRight);
        musicSelect.isRightAnim = this.isRight;
    }

   
}
