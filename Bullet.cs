using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool enemyCritical;
    public string elementalType;
    bool windChangeOn;
    string windChangeType;
    public bool enemyWeapon;
    float weapon2Time = 4.9f;
    public int weaponType;
    public float power;
    public Transform playerTransform;
    public float waepon6Timer; //윈드포스 타이머
    ParticleSystem particle;
    float weaponAngle;
    public ObjectManager objectManager;
    public PlayerMove playerMove;
    public int matchCount; // 관통 할 수 있는 횟수
    public bool throwBullet; // 관통 여부
    public bool armorKill; // 가시갑옷 즉사
    Rigidbody2D rigid;
    PointEffector2D pointEffector2D;
    

    float deg; //각도
    public Vector3 playerCurVec;
    public bool maxLevel;
    public float bulletSpeed;
    float energyForceTime;
    bool enemyForeceMax;
    Animator anim;
    public bool turretBullet;
    public GameObject playerObjPos;

    public Vector2 attack2Vec;
    
    void Awake()
    {
        pointEffector2D = GetComponent<PointEffector2D>();
        anim = GetComponent<Animator>();
        particle = GetComponent<ParticleSystem>();
        playerTransform = GameManager.instance.playerPos;
        objectManager = GameManager.instance.objectManager;
        playerMove = GameManager.instance.playerMove;
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        enemyForeceMax = false;
        deg = 0;
        if(weaponType == 6){ // 윈드포스의 경우
            ParticleSystem.MainModule psMain = particle.main;
            psMain.startColor = Color.green;
            windChangeOn = false;
            windChangeType = "Wind";
            Invoke("ActiveOff", waepon6Timer);
        } else if (weaponType == 7){ // 파이어 워커의 경우
            Invoke("ActiveOff", 3f);
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
            Invoke("HorseOff", 3f);
            playerMove.speed += 3;
        } else if (weaponType == 19){ // 십자가의 경우
            Invoke("ActiveOff", 1.5f);
        } else if (weaponType == 20){ // 빔의 경우
            transform.position = new Vector2(playerMove.transform.position.x, playerMove.transform.position.y + 1.2f);
        } else if (weaponType == 21){ // 에너지 파동의 경우
            Invoke("EnergeForece", 1.5f);
        } else if (weaponType == 23){ // 지뢰가 10초간 터지지 않았을 경우
            Invoke("TrapOn",10f);
        } else if (weaponType == 27){ // 곰 공격의 경우
            Invoke("ActiveOff", 0.7f);
        } else if (weaponType == 30){ // 스켈레톤 공격의 경우
            Invoke("ActiveOff", 0.2f);
        } else if (weaponType == 32){ // 새 공격의 경우
            Invoke("ActiveOff", 3f);
        } else if (weaponType == 33){ // 뱀의 경우
            Invoke("ActiveOff", 10f);
        } else if(weaponType == 22){ // 물의 파동의 경우
            elementalType = "Water";
        } else if(weaponType == -2){ // 휩쓸기 공격의 경우
            Invoke("ActiveOff", 0.2f);
        } else if(weaponType == -4) // 보스 필살기의 경우
        {
            Invoke("ActiveOff", 4f);
        }
    }
    void Update()
    {
        if(weaponType == 2 && gameObject.activeSelf){ //주위를 도는 돌
            if(!maxLevel){ // 던지는 돌이 아닐 경우
                if(GameManager.instance.character == "Wizard" && playerMove.weaponLevel[5] == 6){  // 위자드 돌 6렙일 경우
                    bulletSpeed = 200f;
                    transform.RotateAround(playerTransform.position, Vector3.forward, Time.deltaTime * bulletSpeed);
                    float weapon2CurPower = 50;
                    float weapon2Power = weapon2CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[5];
                    power = weapon2Power;
                } else {
                    bulletSpeed = 100f;
                    transform.RotateAround(playerTransform.position, Vector3.forward, Time.deltaTime * bulletSpeed);
                    float weapon2CurPower = 50;
                    float weapon2Power = weapon2CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[5];
                    power = weapon2Power;
                    weapon2Time -= Time.deltaTime;
                    if(weapon2Time <= 0){
                        weapon2Time = 4.9f;
                        ActiveOff();
                    }
                }
            }
        } else if(weaponType == 9 && gameObject.activeSelf){ // 물 오로라
            if(maxLevel){
                PlayerObj playerObjLogic = playerObjPos.GetComponent<PlayerObj>();
                transform.position = playerObjPos.transform.position;
                float weapon10Speed = 100f;
                Vector3 weapon10Rotate = new Vector3(0,0,Time.deltaTime*weapon10Speed);
                transform.Rotate(weapon10Rotate);
                gameObject.SetActive(playerObjLogic.isMaxOn);
            } else {
                float weapon10CurPower = 30;
                float weapon10Power = weapon10CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[4];
                power = weapon10Power;
                float weapon10Speed = 100f;
                Vector3 weapon10Rotate = new Vector3(0,0,Time.deltaTime*weapon10Speed);
                transform.Rotate(weapon10Rotate);
            }
        } else if(weaponType== 10 && gameObject.activeSelf){ // 곡괭이
            weaponAngle += Time.deltaTime * 100;
            transform.rotation = Quaternion.Euler(0,0,weaponAngle);
        } else if (weaponType==0 && gameObject.activeSelf){ // 삽
            weaponAngle += Time.deltaTime * 50;
            transform.rotation = Quaternion.Euler(0,0,weaponAngle);        
        } else if (weaponType==12 && gameObject.activeSelf){ // 태풍
            bulletSpeed = 150f;
            transform.RotateAround(playerTransform.position, Vector3.forward, Time.deltaTime * bulletSpeed);
            float weaponTyphoonCurPower = 200;
            float weaponTyphoonPower = weaponTyphoonCurPower * GameManager.instance.playerMove.power;
            power = weaponTyphoonPower;
        } else if(weaponType == 15 && gameObject.activeSelf){ // 가시갑옷
            float weapon15CurPower = 30;
            float weapon15PlusPower = GameManager.instance.playerMove.enemyClearNum/10;
            power = (weapon15CurPower * GameManager.instance.playerMove.power * GameManager.instance.playerMove.weaponLevel[0]) + weapon15PlusPower;
        } else if (weaponType == 16 && gameObject.activeSelf){ // 돌아가는 망치
            if(maxLevel){
                deg += Time.deltaTime;
                transform.position = playerCurVec - new Vector3(deg*Mathf.Cos(deg)+0.5f,deg*Mathf.Sin(deg));
            } else {
                deg += Time.deltaTime;
                transform.position = playerCurVec - new Vector3(deg*Mathf.Sin(deg)-0.5f,deg*Mathf.Cos(deg));
            }
        } else if (weaponType == 17 && gameObject.activeSelf){ // 철퇴
            transform.RotateAround(playerCurVec, Vector3.forward, Time.deltaTime * bulletSpeed);
        } else if (weaponType == 19 && gameObject.activeSelf && maxLevel){ // 십자가 궁극기
            transform.RotateAround(playerCurVec, Vector3.forward, Time.deltaTime * 300);
        } else if (weaponType == 20 && gameObject.activeSelf){ // 빔
            Vector2 direction = new Vector2(transform.position.x - playerMove.targetImage.transform.position.x,transform.position.y - playerMove.targetImage.transform.position.y-1.2f);
            //이동 함수
            Vector2 nextVec = new Vector2(direction.normalized.x * bulletSpeed * Time.deltaTime,+direction.normalized.y * bulletSpeed * Time.deltaTime);
            rigid.MovePosition(rigid.position - nextVec);
            rigid.velocity = Vector2.zero;
        } else if (weaponType == 21 && gameObject.activeSelf && maxLevel){ // 에너지 파동 궁극기
            if(!enemyForeceMax)
                return;

            energyForceTime -= Time.deltaTime;
            if(energyForceTime<=0){
                GameObject bullet = objectManager.MakeObj("BulletPlayer7");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power/2;
                Vector3 ranVec = new Vector3(Random.Range(0.5f,3.5f),Random.Range(-1.5f,-2.5f),0);
                bullet.transform.position = transform.position;
                energyForceTime = 0.1f;
            }
        } else if(weaponType == -3 && gameObject.activeSelf){ // 보스 내려 찍기    
            if(gameObject.transform.position.y<=attack2Vec.y){
                gameObject.GetComponent<CircleCollider2D>().enabled = true;
                rigid.velocity = Vector2.zero;
                Invoke("ActiveOff",0.3f);
            } else {
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                transform.Translate(Vector3.down * 5f *Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //총알이 끝에 닿으면 총알 삭제
        if(other.gameObject.tag == "BorderBullet") {
            ActiveOff();
        } else if(weaponType == -1 && other.gameObject.tag =="Bullet"){ // 돌 무기에 닿으면 적군 총알 삭제
            Bullet otherLogic = other.GetComponent<Bullet>();
            if(otherLogic.weaponType == 2){
                ActiveOff();
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
        } else if(other.gameObject.tag == "Enemy"){
            if(elementalType=="Wind"){ // 바람의 경우 다른 원소와 닿으면 해당 원소를 흡수하여 해당 원소로 변경
                if(weaponType == 6){ 
                    if(!windChangeOn){
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
                    } else { // 바람이 변경되었을 경우 적군에게 입히는 데미지
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
                    } 
                }
            } else if(elementalType == "Ice"){
                if(weaponType == 3){ // 아이스 붐이 적군이랑 만났을 경우
                    if(maxLevel){
                        for (int i =0;i<10;i++){
                            GameObject bullet = objectManager.MakeObj("BulletPlayer3");
                            bullet.transform.position = transform.position;
                            Bullet bulletLogic = bullet.GetComponent<Bullet>();
                            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                            bulletLogic.power = power;
                            bulletLogic.maxLevel = false;
                            bullet.transform.localScale = new Vector3(1.8f,1.8f,0);

                            //회전 함수
                            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / 10), Mathf. Sin(Mathf.PI * 2 * i / 10));
                            rigid.AddForce(dirVec.normalized * playerMove.bulletSpeed, ForceMode2D.Impulse);

                            //총알 회전 로직
                            Vector3 rotVec = Vector3.forward * 360 * i / 10 + Vector3.forward * 90;
                            bullet.transform.Rotate(rotVec);
                        }
                    }
                }
            } else if(elementalType=="Lightning"){
                if(weaponType==23){ // 적군이 지뢰를 밟은 경우
                    TrapOn();
                }
            } else if(weaponType == 11 && turretBullet){ // 터렛이 쏜 총알 일 경우
                GameObject dieEffect = GameManager.instance.objectManager.MakeObj("Overload");
                Effect dieEffectLogic = dieEffect.GetComponent<Effect>();
                dieEffect.transform.position = transform.position;
                dieEffectLogic.power = power;
            }
            if(!throwBullet && !enemyWeapon){ // 관통 여부
                if(matchCount<=0){
                    ActiveOff();
                } else {
                    if(weaponType == 25){ // 활 일 경우
                        Vector2 ranVec = new Vector2(Random.Range(-1,2),3f);
                        rigid.AddForce(ranVec, ForceMode2D.Impulse);
                        matchCount--;
                    } else {
                        matchCount--;
                    }
                }
            }
        }
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    void MeteoAttack()
    {
        for(int i = 0;i<5;i++){
            GameObject bullet = objectManager.MakeObj("BulletPlayer7");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power/2;
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
    void EnergeForece()
    {
        enemyForeceMax = true;
        rigid.AddForce(playerCurVec * 10, ForceMode2D.Impulse);
    }
    void TrapOn()
    {
        anim.SetTrigger("IsTrapOn");
        Invoke("ActiveOff",0.9f);
    }
    void OnDisable()
    {
        matchCount = 0;
        CancelInvoke();
        // maxLevel = false;
        if(weaponType == 33 ){ // 뱀 비활성화시
            playerMove.snake = false;
            playerMove.snakeCount = 3;
        } else if (weaponType == -3)
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}