using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //적군
    public GameObject enemyAPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyCPrefab;
    public GameObject enemyDPrefab;
    public GameObject enemyEPrefab;
    GameObject[] enemyA;
    GameObject[] enemyB;
    GameObject[] enemyC;
    GameObject[] enemyD;
    GameObject[] enemyE;


    //아이템
    public GameObject itemExp0Prefab;
    public GameObject itemExp1Prefab;
    public GameObject itemExp2Prefab;
    public GameObject itemHealthPrefab;
    public GameObject itemMagPrefab;
    public GameObject itemBoomPrefab;
    public GameObject itemCoin0Prefab;
    public GameObject itemCoin1Prefab;
    public GameObject itemCoin2Prefab;
    public GameObject itemShieldPrefab;
    GameObject[] itemExp0;
    GameObject[] itemExp1;
    GameObject[] itemExp2;
    GameObject[] itemCoin0;
    GameObject[] itemCoin1;
    GameObject[] itemCoin2;
    GameObject[] itemHealth;
    GameObject[] itemMag;
    GameObject[] itemBoom;
    GameObject[] itemShield;

    //미사일(플레이어 2번, 9번, 15번은 하드 오브젝트)
    public GameObject bulletPlayer0Prefab;
    public GameObject bulletPlayer1Prefab;

    public GameObject bulletPlayer3Prefab;
    public GameObject bulletPlayer4Prefab;
    public GameObject bulletPlayer5Prefab;
    public GameObject bulletPlayer6Prefab;
    public GameObject bulletPlayer7Prefab;
    public GameObject bulletPlayer8Prefab;

    public GameObject bulletPlayer10Prefab;
    public GameObject bulletPlayer11Prefab;

    public GameObject bulletPlayer13Prefab;
    public GameObject bulletPlayer14Prefab;

    public GameObject bulletPlayer16Prefab;
    public GameObject bulletPlayer17Prefab;

    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    GameObject[] bulletPlayer0; // 삽
    GameObject[] bulletPlayer1; // 삼지창
                                // 돌
    GameObject[] bulletPlayer3; // 아이스 스피어
    GameObject[] bulletPlayer4; // 따발총
    GameObject[] bulletPlayer5; // 샷건
    GameObject[] bulletPlayer6; // 윈드포스
    GameObject[] bulletPlayer7; // 파이어 볼
    GameObject[] bulletPlayer8; // 체인 라이트닝
                                // 물 오로라
    GameObject[] bulletPlayer10; // 곡괭이
    GameObject[] bulletPlayer11; // 권총
                                 // 태풍
    GameObject[] bulletPlayer13; // 쓰나미
    GameObject[] bulletPlayer14; // 메테오
                                 // 가시 갑옷
    GameObject[] bulletPlayer16; // 해머
    GameObject[] bulletPlayer17; // 철퇴

    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;

    //이펙트
    public GameObject overloadPrefab;
    GameObject[] overload;

    //텍스트
    public GameObject damageTextPrefab;
    GameObject[] damageText;

    //박스
    public GameObject box1Prefab;
    public GameObject box2Prefab;
    GameObject[] box1;
    GameObject[] box2;


    GameObject[] targetPool;

    void Awake()
    {
        //적군
        enemyA = new GameObject[1000];
        enemyB = new GameObject[1000];
        enemyC = new GameObject[500];
        enemyD = new GameObject[500];
        enemyE = new GameObject[1];

        //아이템
        itemExp0 = new GameObject[1000];
        itemExp1 = new GameObject[1000];
        itemExp2 = new GameObject[1000];
        itemCoin0 = new GameObject[100];
        itemCoin1 = new GameObject[100];
        itemCoin2 = new GameObject[100];
        itemHealth = new GameObject[10];
        itemMag = new GameObject[100];
        itemBoom = new GameObject[100];
        itemShield = new GameObject[100];

        //미사일
        bulletPlayer0 = new GameObject[100];
        bulletPlayer1 = new GameObject[100];
        
        bulletPlayer3 = new GameObject[300];
        bulletPlayer4 = new GameObject[300];
        bulletPlayer5 = new GameObject[300];
        bulletPlayer6 = new GameObject[10];
        bulletPlayer7 = new GameObject[200];
        bulletPlayer8 = new GameObject[30];

        bulletPlayer10 = new GameObject[30];
        bulletPlayer11 = new GameObject[100];

        bulletPlayer13 = new GameObject[100];
        bulletPlayer14 = new GameObject[1];

        bulletPlayer16 = new GameObject[50];
        bulletPlayer17 = new GameObject[2];

        bulletEnemyA = new GameObject[500];
        bulletEnemyB = new GameObject[500];

        //이펙트
        overload = new GameObject[100];

        //텍스트
        damageText = new GameObject[500];
        
        //박스
        box1 = new GameObject[200];
        box2 = new GameObject[10];

        Generate();

    }
    void Generate()
    {
        //적군
        for(int index=0;index<enemyA.Length;index++){
            enemyA[index] = Instantiate(enemyAPrefab);
            enemyA[index].SetActive(false);
        }
        for(int index=0;index<enemyB.Length;index++){
            enemyB[index] = Instantiate(enemyBPrefab);
            enemyB[index].SetActive(false);
        }
        for(int index=0;index<enemyC.Length;index++){
            enemyC[index] = Instantiate(enemyCPrefab);
            enemyC[index].SetActive(false);
        }
        for(int index=0;index<enemyD.Length;index++){
            enemyD[index] = Instantiate(enemyDPrefab);
            enemyD[index].SetActive(false);
        }
        for(int index=0;index<enemyE.Length;index++){
            enemyE[index] = Instantiate(enemyEPrefab);
            enemyE[index].SetActive(false);
        }
        //아이템
        for(int index=0;index<itemExp0.Length;index++){
            itemExp0[index] = Instantiate(itemExp0Prefab);
            itemExp0[index].SetActive(false);
        }
        for(int index=0;index<itemExp1.Length;index++){
            itemExp1[index] = Instantiate(itemExp1Prefab);
            itemExp1[index].SetActive(false);
        }
        for(int index=0;index<itemExp2.Length;index++){
            itemExp2[index] = Instantiate(itemExp2Prefab);
            itemExp2[index].SetActive(false);
        }
        for(int index=0;index<itemCoin0.Length;index++){
            itemCoin0[index] = Instantiate(itemCoin0Prefab);
            itemCoin0[index].SetActive(false);
        }
        for(int index=0;index<itemCoin1.Length;index++){
            itemCoin1[index] = Instantiate(itemCoin1Prefab);
            itemCoin1[index].SetActive(false);
        }
        for(int index=0;index<itemCoin2.Length;index++){
            itemCoin2[index] = Instantiate(itemCoin2Prefab);
            itemCoin2[index].SetActive(false);
        }
        for(int index=0;index<itemHealth.Length;index++){
            itemHealth[index] = Instantiate(itemHealthPrefab);
            itemHealth[index].SetActive(false);
        }
        for(int index=0;index<itemMag.Length;index++){
            itemMag[index] = Instantiate(itemMagPrefab);
            itemMag[index].SetActive(false);
        }
        for(int index=0;index<itemBoom.Length;index++){
            itemBoom[index] = Instantiate(itemBoomPrefab);
            itemBoom[index].SetActive(false);
        }
        for(int index=0;index<itemShield.Length;index++){
            itemShield[index] = Instantiate(itemShieldPrefab);
            itemShield[index].SetActive(false);
        }
        //총알
        for(int index=0;index<bulletPlayer0.Length;index++){
            bulletPlayer0[index] = Instantiate(bulletPlayer0Prefab);
            bulletPlayer0[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer1.Length;index++){
            bulletPlayer1[index] = Instantiate(bulletPlayer1Prefab);
            bulletPlayer1[index].SetActive(false);
        }

        for(int index=0;index<bulletPlayer3.Length;index++){
            bulletPlayer3[index] = Instantiate(bulletPlayer3Prefab);
            bulletPlayer3[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer4.Length;index++){
            bulletPlayer4[index] = Instantiate(bulletPlayer4Prefab);
            bulletPlayer4[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer5.Length;index++){
            bulletPlayer5[index] = Instantiate(bulletPlayer5Prefab);
            bulletPlayer5[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer6.Length;index++){
            bulletPlayer6[index] = Instantiate(bulletPlayer6Prefab);
            bulletPlayer6[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer7.Length;index++){
            bulletPlayer7[index] = Instantiate(bulletPlayer7Prefab);
            bulletPlayer7[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer8.Length;index++){
            bulletPlayer8[index] = Instantiate(bulletPlayer8Prefab);
            bulletPlayer8[index].SetActive(false);
        }

        for(int index=0;index<bulletPlayer10.Length;index++){
            bulletPlayer10[index] = Instantiate(bulletPlayer10Prefab);
            bulletPlayer10[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer11.Length;index++){
            bulletPlayer11[index] = Instantiate(bulletPlayer11Prefab);
            bulletPlayer11[index].SetActive(false);
        }

        for(int index=0;index<bulletPlayer13.Length;index++){
            bulletPlayer13[index] = Instantiate(bulletPlayer13Prefab);
            bulletPlayer13[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer14.Length;index++){
            bulletPlayer14[index] = Instantiate(bulletPlayer14Prefab);
            bulletPlayer14[index].SetActive(false);
        }

        for(int index=0;index<bulletPlayer16.Length;index++){
            bulletPlayer16[index] = Instantiate(bulletPlayer16Prefab);
            bulletPlayer16[index].SetActive(false);
        }
        for(int index=0;index<bulletPlayer17.Length;index++){
            bulletPlayer17[index] = Instantiate(bulletPlayer17Prefab);
            bulletPlayer17[index].SetActive(false);
        }


        for(int index=0;index<bulletEnemyA.Length;index++){
            bulletEnemyA[index] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[index].SetActive(false);
        }
        for(int index=0;index<bulletEnemyB.Length;index++){
            bulletEnemyB[index] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[index].SetActive(false);
        }

        //이펙트
        for(int index=0;index<overload.Length;index++){
            overload[index] = Instantiate(overloadPrefab);
            overload[index].SetActive(false);
        }

        //텍스트
        for(int index=0;index<damageText.Length;index++){
            damageText[index] = Instantiate(damageTextPrefab);
            damageText[index].SetActive(false);
        }
        //박스
        for(int index=0;index<box1.Length;index++){
            box1[index] = Instantiate(box1Prefab);
            box1[index].SetActive(false);
        }
        for(int index=0;index<box2.Length;index++){
            box2[index] = Instantiate(box2Prefab);
            box2[index].SetActive(false);
        }
    }
        public GameObject MakeObj(string type)
    {
        switch(type)
        {
            //적군
            case "EnemyA":
                targetPool = enemyA;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyC":
                targetPool = enemyC;
                break;
            case "EnemyD":
                targetPool = enemyD;
                break;
            case "EnemyE":
                targetPool = enemyE;
                break;

            //아이템
            case "ItemExp0":
                targetPool = itemExp0;
                break;
            case "ItemExp1":
                targetPool = itemExp1;
                break;
            case "ItemExp2":
                targetPool = itemExp2;
                break;
                case "ItemCoin0":
                targetPool = itemCoin0;
                break;
                case "ItemCoin1":
                targetPool = itemCoin1;
                break;
                case "ItemCoin2":
                targetPool = itemCoin2;
                break;
            case "ItemHealth":
                targetPool = itemHealth;
                break;
            case "ItemMag":
                targetPool = itemMag;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "ItemShield":
                targetPool = itemShield;
                break;

            //총알
            case "BulletPlayer0":
                targetPool = bulletPlayer0;
                break;
            case "BulletPlayer1":
                targetPool = bulletPlayer1;
                break;
            case "BulletPlayer3":
                targetPool = bulletPlayer3;
                break;
            case "BulletPlayer4":
                targetPool = bulletPlayer4;
                break;
            case "BulletPlayer5":
                targetPool = bulletPlayer5;
                break;
            case "BulletPlayer6":
                targetPool = bulletPlayer6;
                break;
            case "BulletPlayer7":
                targetPool = bulletPlayer7;
                break;
            case "BulletPlayer8":
                targetPool = bulletPlayer8;
                break;
            case "BulletPlayer10":
                targetPool = bulletPlayer10;
                break;
            case "BulletPlayer11":
                targetPool = bulletPlayer11;
                break;
            case "BulletPlayer13":
                targetPool = bulletPlayer13;
            break;
            case "BulletPlayer14":
                targetPool = bulletPlayer14;
            break;
            case "BulletPlayer16":
                targetPool = bulletPlayer16;
            break;
            case "BulletPlayer17":
                targetPool = bulletPlayer17;
            break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;

            //이펙트
            case "Overload":
                targetPool = overload;
            break;

            //텍스트
            case "DamageText":
                targetPool = damageText;
                break;

            //박스
            case "Box1":
                targetPool = box1;
                break;
            case "Box2":
                targetPool = box2;
                break;
        }
        for (int index=0; index < targetPool.Length;index++){
            if(!targetPool[index].activeSelf){
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }
        return null;
    }
    public GameObject[] GetPool(string type)
    {
        switch(type)
        {
            //적군
            case "EnemyA":
                targetPool = enemyA;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "EnemyC":
                targetPool = enemyC;
                break;
            case "EnemyD":
                targetPool = enemyD;
                break;
            case "EnemyE":
                targetPool = enemyE;
                break;

            //아이템
            case "ItemExp0":
                targetPool = itemExp0;
                break;
            case "ItemExp1":
                targetPool = itemExp1;
                break;
            case "ItemExp2":
                targetPool = itemExp2;
                break;
            case "ItemCoin0":
                targetPool = itemCoin0;
                break;
            case "ItemCoin1":
                targetPool = itemCoin1;
                break;
            case "ItemCoin2":
                targetPool = itemCoin2;
                break;
            case "ItemHealth":
                targetPool = itemHealth;
                break;
            case "ItemMag":
                targetPool = itemMag;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "ItemShield":
                targetPool = itemShield;
                break;

            //총알
            case "BulletPlayer0":
                targetPool = bulletPlayer0;
                break;
            case "BulletPlayer1":
                targetPool = bulletPlayer1;
                break;
            case "BulletPlayer3":
                targetPool = bulletPlayer3;
                break;
            case "BulletPlayer4":
                targetPool = bulletPlayer4;
                break;
            case "BulletPlayer5":
                targetPool = bulletPlayer5;
                break;
            case "BulletPlayer6":
                targetPool = bulletPlayer6;
                break;
            case "BulletPlayer7":
                targetPool = bulletPlayer7;
                break;
            case "BulletPlayer8":
                targetPool = bulletPlayer8;
                break;
            case "BulletPlayer10":
                targetPool = bulletPlayer10;
                break;
            case "BulletPlayer11":
                targetPool = bulletPlayer11;
                break;
            case "BulletPlayer13":
                targetPool = bulletPlayer13;
            break;
            case "BulletPlayer14":
                targetPool = bulletPlayer14;
            break;
            case "BulletPlayer16":
                targetPool = bulletPlayer16;
            break;
            case "BulletPlayer17":
                targetPool = bulletPlayer17;
            break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;

            //이펙트
            case "Overload":
                targetPool = overload;
            break;
            
            //텍스트
            case "DamageText":
                targetPool = damageText;
                break;

            //박스
            case "Box1":
                targetPool = box1;
                break;
            case "Box2":
                targetPool = box2;
                break;
        }
        return targetPool;
    }

}
