using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    bool isStart;
    public string elementalType;
    bool windChangeOn;
    string windChangeType;
    public bool enemyWeapon;
    float weapon2Time = 4.9f;
    public int weaponType;
    public float power;
    public Transform playerTransform;
    public float weapon2Speed;
    public float nuckBack;
    public float nuckBackTime;
    float fireTime;
    public bool iceBoom;
    public float waepon6Timer; //윈드포스 타이머
    ParticleSystem particle;
    float weaponAngle;
    public ObjectManager objectManager;
    public PlayerMove playerMove;
    float weaponTyphoonSpeed = 150f;
    public int matchCount; // 관통 할 수 있는 횟수
    public int enemyArmorClearCount; // 가시갑옷 진화
    public bool armorKill; // 가시갑옷 즉사

    float deg; //각도
    public Vector3 playerCurVec;
    public bool hammerBack = false;
    public float maceSpeed;
    public bool horseMax = false;
    
    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        playerTransform = GameManager.instance.playerPos;
        objectManager = GameManager.instance.objectManager;
        playerMove = GameManager.instance.playerMove;
    }
    void OnEnable()
    {
        deg = 0;
        if(weaponType == 6){ // 윈드포스의 경우
            ParticleSystem.MainModule psMain = particle.main;
            psMain.startColor = Color.green;
            windChangeOn = false;
            windChangeType = "Wind";
            Invoke("ActiveOff", waepon6Timer);
        } else if (weaponType == 7){ // 파이어 워커의 경우
            fireTime = 3f;
        } else if (weaponType == 8){ // 체인 라이트닝의 경우
            Invoke("ActiveOff",0.2f);
        } else if (enemyWeapon){ //적 총알의 경우 3초 후 삭제
            Invoke("ActiveOff", 3f);
        } else if(weaponType == 13){ // 웨이브의 경우
            Invoke("ActiveOff", 1.5f);
        } else if(weaponType == 14){ // 메테오의 경우
            Invoke("MeteoAttack", 1f);
        } else if (weaponType == 17){ // 철퇴의 경우
            Invoke("ActiveOff", 0.5f);
        } else if (weaponType == 18){ // 말타기의 경우
            Invoke("HorseOff", 2f);
            playerMove.speed += 3;
        }
    }
    void Update()
    {
        if(weaponType == 2 && gameObject.activeSelf){ //주위를 도는 돌
            transform.RotateAround(playerTransform.position, Vector3.forward, Time.deltaTime* weapon2Speed);
            float weapon2CurPower = 50;
            float weapon2Power = weapon2CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[5];
            power = weapon2Power;
            weapon2Time -= Time.deltaTime;
            if(weapon2Time <= 0){
                weapon2Time = 4.9f;
                ActiveOff();
            }
        } else if(weaponType== 7 && gameObject.activeSelf){ // 파이어 워커
            fireTime-=Time.deltaTime;
            if(fireTime<=0){
                ActiveOff();
            }
        } else if(weaponType == 9 && gameObject.activeSelf){ // 물 오로라
            float weapon10CurPower = 50;
            float weapon10Power = weapon10CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[4];
            power = weapon10Power;
            float weapon10Speed = 100f;
            Vector3 weapon10Rotate = new Vector3(0,0,Time.deltaTime*weapon10Speed);
            transform.Rotate(weapon10Rotate);
        } else if(weaponType== 10 && gameObject.activeSelf){ // 곡괭이
            weaponAngle += Time.deltaTime * 100;
            transform.rotation = Quaternion.Euler(0,0,weaponAngle);
        } else if (weaponType==0 && gameObject.activeSelf){ // 삽
            weaponAngle += Time.deltaTime * 50;
            transform.rotation = Quaternion.Euler(0,0,weaponAngle);        
        } else if (weaponType==12 && gameObject.activeSelf){ // 태풍
            transform.RotateAround(playerTransform.position, Vector3.forward, Time.deltaTime * weaponTyphoonSpeed);
            float weaponTyphoonCurPower = 200;
            float weaponTyphoonPower = weaponTyphoonCurPower * GameManager.instance.playerMove.power;
            power = weaponTyphoonPower;
        } else if(weaponType == 15 && gameObject.activeSelf){ // 가시갑옷
            float weapon15CurPower = 50;
            float weapon15PlusPower = GameManager.instance.playerMove.enemyClearNum/10;
            power = (weapon15PlusPower + weapon15CurPower) * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[0];
        } else if (weaponType == 16 && gameObject.activeSelf){ // 돌아가는 망치
            if(hammerBack){
                deg += Time.deltaTime;
                transform.position = playerCurVec - new Vector3(deg*Mathf.Cos(deg)+0.5f,deg*Mathf.Sin(deg));
            } else {
                deg += Time.deltaTime;
                transform.position = playerCurVec - new Vector3(deg*Mathf.Sin(deg)-0.5f,deg*Mathf.Cos(deg));
            }
        } else if (weaponType == 17 && gameObject.activeSelf){ // 철퇴
            transform.RotateAround(playerMove.transform.position, Vector3.forward, Time.deltaTime * maceSpeed);
        } else if (weaponType == 18 && gameObject.activeSelf){ // 말타기

        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //총알이 끝에 닿으면 총알 삭제
        if(other.gameObject.tag == "BorderBullet") {
            gameObject.SetActive(false);
        } else if(weaponType == -1 && other.gameObject.tag =="Bullet"){ // 돌 무기에 닿으면 적군 총알 삭제
            Bullet otherLogic = other.GetComponent<Bullet>();
            if(otherLogic.weaponType == 2){
                gameObject.SetActive(false);
            }
        } else if (elementalType=="Wind" && !windChangeOn && weaponType == 6 && other.gameObject.tag == "Bullet"){ // 파티클때문에 윈드포스만 변경
            Bullet otherLogic = other.GetComponent<Bullet>();
            ParticleSystem.MainModule psMain = particle.main;
            switch(otherLogic.elementalType){
                case "Fire":
                    windChangeOn = true;
                    psMain.startColor = Color.red;
                    windChangeType = otherLogic.elementalType;
                break;
                case "Ice":
                    windChangeOn = true;
                    psMain.startColor = Color.cyan;
                    windChangeType = otherLogic.elementalType;
                break;
                case "Lightning":
                    windChangeOn = true;
                    psMain.startColor = Color.magenta;
                    windChangeType = otherLogic.elementalType;
                break;
                case "Water":
                    windChangeOn = true;
                    psMain.startColor = Color.blue;
                    windChangeType = otherLogic.elementalType;
                break;
            }
        } else if (elementalType=="Wind" && !windChangeOn && weaponType == 6 && other.gameObject.tag == "Enemy"){ // 바람의 경우 다른 원소와 닿으면 해당 원소를 흡수하여 해당 원소로 변경
            EnemyMove otherLogic = other.GetComponent<EnemyMove>();
            ParticleSystem.MainModule psMain = particle.main;
            if(otherLogic.isFire){
                    windChangeOn = true;
                    psMain.startColor = Color.red;
                    windChangeType = "Fire";
            } else if(otherLogic.isIce){
                    windChangeOn = true;
                    psMain.startColor = Color.cyan;
                    windChangeType = "Ice";
            } else if(otherLogic.isLightning){
                    windChangeOn = true;
                    psMain.startColor = Color.magenta;
                    windChangeType = "Lightning";
            } else if(otherLogic.isWater){
                    windChangeOn = true;
                    psMain.startColor = Color.blue;
                    windChangeType = "Water";
            }
        } else if(elementalType=="Wind" && windChangeOn && other.gameObject.tag == "Enemy"){ // 바람이 변경되었을 경우 적군에게 입히는 데미지
            EnemyMove otherLogic = other.GetComponent<EnemyMove>();
            switch(windChangeType){
                case "Fire":
                    otherLogic.IsFrie(this);
                break;
                case "Ice":
                    otherLogic.IsIce(this);
                break;
                case "Lightning":
                    otherLogic.IsLightning(this);
                break;
                case "Water":
                    otherLogic.IsWater(this);
                break;
            }
        } else if (iceBoom && other.gameObject.tag == "Enemy"){ // 아이스 붐이 적군이랑 만났을 경우
            for (int i =0;i<10;i++){
                GameObject bullet = objectManager.MakeObj("BulletPlayer3");
                bullet.transform.position = transform.position;
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bulletLogic.power = power;
                bulletLogic.iceBoom = false;
                bullet.transform.localScale = new Vector3(1.8f,1.8f,0);

                //회전 함수
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / 10), Mathf. Sin(Mathf.PI * 2 * i / 10));
                rigid.AddForce(dirVec.normalized * playerMove.bulletSpeed, ForceMode2D.Impulse);

                //총알 회전 로직
                Vector3 rotVec = Vector3.forward * 360 * i / 10 + Vector3.forward * 90;
                bullet.transform.Rotate(rotVec);
            }
        }
        //관통 함수 만들어야 함!
        
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    void MeteoAttack()
    {
        if(!isStart){
            isStart=true;
            return;
        }
        for(int i = 0;i<5;i++){
            GameObject bullet = objectManager.MakeObj("BulletPlayer7");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power/2;
            bulletLogic.fireTime = 3f;
            Vector3 ranVec = new Vector3(Random.Range(0.5f,3.5f),Random.Range(-1.5f,-2.5f),0);
            bullet.transform.position = transform.position + ranVec;
        }
        playerMove.Boom(power, false);
        playerMove.boomEffect.SetActive(true);
        ActiveOff();
    }
    void HorseOff()
    {
        playerMove.speed -= 3;
        gameObject.SetActive(false);
    }
}