using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    //적군 소환
    public Transform[] spawnPoints;
    public float curSpawnDelay;
    public ObjectManager objectManager;
    public bool spawnEnd;
    public bool bossClear;
    bool wave1;
    bool wave2;
    bool wave3;
    bool waveOn;
    float spawnTime;
    public bool isBoss; // 보스랑 싸우는 중인가?
    public SpawnData[] spawnData;
    
    public GameObject eventPanel;
    public Text eventText;
    
    //보스 클리어 체크
    public bool boss1Clear;
    public bool boss2Clear;

    public GameObject bossSquare; // 보스 전장
    public GameObject bossHealthBar;
    public Image bossHealth;
    public GameManager gameManager;
    
    //상자 함수
    float boxCreatTime;

    void Start()
    {
        
    }

    void Update()
    {
        BoxCreat();
        BossCheck();
        if(spawnEnd && !bossClear){
            spawnEnd = false;
        }
        if(isBoss || gameManager.stageEnd){
            spawnEnd = true;
        }

        spawnEnemy();
    }
    void spawnEnemy()
    {
        //소환 함수
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > spawnTime && !spawnEnd){
            int enemyLevel;
            if(gameManager._min<5){
                enemyLevel = 0;
            } else if(gameManager._min<10){
                enemyLevel = 1;
            } else {
                enemyLevel = 2;
            }
            int ranPoint = Random.Range(0, spawnPoints.Length-1);

            GameObject enemy = objectManager.Get(0);
            enemy.GetComponent<EnemyMove>().Init(spawnData[enemyLevel]);
            if(!gameManager.poolObj.Contains(enemy)){
                gameManager.poolObj.Add(enemy);
            }
            enemy.transform.position = spawnPoints[ranPoint].position;
            Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
            EnemyMove enemyLogic = enemy.GetComponent<EnemyMove>();
            Rigidbody2D playerRigid = gameManager.playerMove.GetComponent<Rigidbody2D>();
            enemyLogic.playerRigid = playerRigid;
            enemyLogic.player = gameManager.player;
            enemyLogic.playerLogic = gameManager.playerMove;
            enemyLogic.objectManager = objectManager;
            enemyLogic.gameManager = gameManager;
            gameManager.enemyCount++;
            curSpawnDelay = 0;

            if(!waveOn){
                spawnTime = 1.8f/(gameManager._min+1);
            }
            // 몬스터 웨이브
            if(gameManager._min >= 4 && !wave1){
                wave1 = true;
                waveOn = true;
                Event("EventIn");
                spawnTime = 0.1f;
            } else if(gameManager._min >= 5 && wave1){
                waveOn = false;
            }
            if(gameManager._min >= 9 && !wave2){
                wave2 = true;
                waveOn = true;
                Event("EventIn");
                spawnTime = 0.05f;
            } else if(gameManager._min >= 10 && wave2){
                waveOn = false;
            }
            if(gameManager._min >= 14 && !wave3){
                wave3 = true;
                waveOn = true;
                Event("EventIn");
                spawnTime = 0.01f;
            } else if(gameManager._min >= 15 && wave3){
                waveOn = false;
            }
            
        }
    }
    void BossCheck(){
        if(isBoss)
            return;

        if(gameManager._min>=5){
            if(!boss1Clear){
                Boss(1);
            }
        }
        if(gameManager._min>=10){
            if(!boss2Clear){
                Boss(1);
            }
        }
        if(gameManager._min>=15 && boss2Clear && boss1Clear){
            Boss(2);
        }
    }
    void Event(string type)
    {
        switch(type){
            case "EventIn":
                eventPanel.SetActive(true);
                // camAnim.SetBool("IsOn",true);
                if(gameManager.language == "English"){
                    eventText.text = "warning!\nMonsters are coming.";
                } else if(gameManager.language == "Korean"){
                    eventText.text = "경고!\n몬스터 무리가 다가옵니다.";
                }
                break;
            case "EventOut":
                // 적군이 더 이상 없을 경우 아웃(혹시 있다면 시간을 딜레이)
                // camAnim.SetBool("IsOn",false);
            break;
        }
    }
    void Boss(int bossNum)
    {
        isBoss = true;
        eventPanel.SetActive(true);
        // camAnim.SetBool("IsOn",true);
        if(gameManager.language == "English"){
            eventText.text = "warning!\nBoss is coming.";
        } else if(gameManager.language == "Korean"){
            eventText.text = "경고!\n보스가 등장합니다.";
        }
        if(bossNum == 1){ // 중간 보스
            Invoke("EnemyClearAndBoss1",3f);
        } else if(bossNum == 2){ // 마지막 보스
            Invoke("EnemyClearAndBoss2",3f);
        }
        
    }
    void EnemyClearAndBoss1()
    {
        spawnEnd = true;
        gameManager.playerMove.EnemyOff();
        bossHealthBar.SetActive(true);
        GameObject enemy = objectManager.Get(0);
        enemy.GetComponent<EnemyMove>().Init(spawnData[3]);
        if(!gameManager.poolObj.Contains(enemy)){
            gameManager.poolObj.Add(enemy);
        }
        enemy.transform.localScale = new Vector3(2,2,0);
        enemy.transform.position = spawnPoints[20].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        EnemyMove enemyLogic = enemy.GetComponent<EnemyMove>();
        Rigidbody2D playerRigid = gameManager.playerMove.GetComponent<Rigidbody2D>();
        enemyLogic.playerRigid = playerRigid;
        enemyLogic.player = gameManager.player;
        enemyLogic.playerLogic = gameManager.playerMove;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = gameManager;
        gameManager.cineCam.Follow = bossSquare.transform;
        bossSquare.SetActive(true);
        bossSquare.transform.position = gameManager.playerMove.transform.position;
        gameManager.enemyCount++;
    }
    void EnemyClearAndBoss2()
    {
        spawnEnd = true;
        gameManager.playerMove.EnemyOff();
        bossHealthBar.SetActive(true);
        GameObject enemy = objectManager.Get(0);
        enemy.GetComponent<EnemyMove>().Init(spawnData[4]);
        if(!gameManager.poolObj.Contains(enemy)){
            gameManager.poolObj.Add(enemy);
        }
        enemy.transform.localScale = new Vector3(2,2,0);
        enemy.transform.position = spawnPoints[20].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        EnemyMove enemyLogic = enemy.GetComponent<EnemyMove>();
        Rigidbody2D playerRigid = gameManager.playerMove.GetComponent<Rigidbody2D>();
        enemyLogic.playerRigid = playerRigid;
        enemyLogic.player = gameManager.player;
        enemyLogic.playerLogic = gameManager.playerMove;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = gameManager;
        gameManager.cineCam.Follow = bossSquare.transform;
        bossSquare.SetActive(true);
        bossSquare.transform.position = gameManager.playerMove.transform.position;
        gameManager.enemyCount++;
    }
    void BoxCreat()
    {
        if(boxCreatTime<=0){
            int ranTime = Random.Range(30,60);
            boxCreatTime = ranTime;
            int ran = Random.Range(0,spawnPoints.Length-1);
            GameObject enemy = objectManager.Get(57);
            enemy.transform.position = spawnPoints[ran].position;
            Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
            EnemyMove enemyLogic = enemy.GetComponent<EnemyMove>();
            Rigidbody2D playerRigid = gameManager.playerMove.GetComponent<Rigidbody2D>();
            if(!gameManager.poolObj.Contains(enemy)){
                gameManager.poolObj.Add(enemy);
            }
            enemyLogic.playerRigid = playerRigid;
            enemyLogic.player = gameManager.player;
            enemyLogic.playerLogic = gameManager.playerMove;
            enemyLogic.objectManager = objectManager;
            enemyLogic.gameManager = gameManager;
        } else {
            boxCreatTime -= Time.deltaTime;
        }
    }
}
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public int enemyLife;
    public float speed;
    public float power;
    public string elementalType; // 적군 데미지 타입
    public int criticalChance; // 크리티컬 확률
    public float criticalDamage = 1.5f; // 크리티컬 데미지

    //적군 총알 함수
    public string enemyName;
    public float maxShootDelay;
    public float bulletSpeed;
}