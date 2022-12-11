using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxItem : MonoBehaviour
{
    public ObjectManager objectManager;
    bool boxOpen;
    void OnEnable()
    {
        boxOpen = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Bullet" || other.gameObject.tag == "Player"){
            if(boxOpen)
                return;

            BoxItemDrop();
            gameObject.SetActive(false);

        }
    }
    void BoxItemDrop()
    {
        boxOpen = true;
        int ran = Random.Range(0,10);
        if(ran < 4){//40% 확률로 코인0
            GameObject coin0 = objectManager.MakeObj("ItemCoin0");
            coin0.transform.position = transform.position;
        } else if (ran < 6){ // 20% 확률로 코인1
            GameObject coin1 = objectManager.MakeObj("ItemCoin1");
            coin1.transform.position = transform.position;
        } else if (ran < 7){ // 10%확률로 코인2
            GameObject coin2 = objectManager.MakeObj("ItemCoin2");
            coin2.transform.position = transform.position;
        } else if (ran < 8){ //10% 확률로 폭탄
            GameObject boom = objectManager.MakeObj("ItemBoom");
            boom.transform.position = transform.position;
        } else if (ran < 9){ //10% 확률로 체력
            GameObject health = objectManager.MakeObj("ItemHealth");
            health.transform.position = transform.position;
        } else if (ran < 10){ // 10% 확률로 자석
            GameObject mag = objectManager.MakeObj("ItemMag");
            mag.transform.position = transform.position;
        }
    }
}