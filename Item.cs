using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameManager gameManager;
    public string type;
    public Rigidbody2D rigid;
    public bool isMagnetOn;
    GameObject player;
    AudioSource audioSource;
    void OnEnable()
    {
        isMagnetOn = false;
        if(type == "Shield"){// 보호막의 경우 5초 뒤에 사라짐
            Invoke("ActiveOff",5f);
        }
    }
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.instance;
        player = GameManager.instance.player;
        rigid = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        MagOn();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(isMagnetOn)
            return;

        if(other.gameObject.tag=="Mag" && !isMagnetOn){
            if(audioSource.clip != null && !audioSource.isPlaying && gameObject.activeSelf){
                gameManager.audioManager.PlayOneShotSound(audioSource, audioSource.clip, audioSource.volume);
            }
            Vector2 nextPos = player.transform.position - transform.position;
            rigid.velocity = -nextPos * 5;
            Invoke("isMagOn",0.3f);
        } else if (other.gameObject.tag=="BorderBullet"){ // 박스가 멀리 떨어지면
            if(type == "Box"){
                int ran = Random.Range(0,gameManager.spawner.spawnPoints.Length - 1);
                gameObject.transform.position = gameManager.spawner.spawnPoints[ran].position;
            }
        }
    }
    void MagOn()
    {
        if(!isMagnetOn)
            return;

        Vector3 nextPos = player.transform.position - transform.position;
        rigid.velocity = nextPos * 10;
    }
    public void isMagOn()
    {
        isMagnetOn = true;
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
}
