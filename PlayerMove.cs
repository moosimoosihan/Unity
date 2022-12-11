using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{    
    //총알 관련 함수
    public float maxShootDelay;
    public float realDelay;
    public float curShootDelay;
    public float bulletSpeed;
    public int[] weaponLevel;
    public GameObject targetEnemy;
    int weapon2Count; // 돌맹이 회전 쿨타임 수
    bool weapon2First = true; //돌맹이 처음에 회전
    float weapon4Count; // 따발총 쿨타임 수
    float weapon4Delay; // 따발총 쿨타임
    float weapon4MaxCount; // 띠발총 최대 쿨타임
    int weapon6Count; // 윈드 포스 쿨타임
    public GameObject[] weapon2LV;
    float weapon0Dmg; // 삽 데미지
    float weapon1Dmg; // 삼지창 데미지
    float weapon3Dmg; // 아이스 스피어 데미지
    float weapon4Dmg; // 따발총 데미지
    float weapon5Dmg; // 샷건 데미지
    float weapon6Dmg; // 윈드 포스 데미지
    float weapon7Dmg; // 파이어 볼 데미지
    public GameObject weapon9Obj; // 물 오로라 오브젝트
    public GameObject thronArmor; // 가시 갑옷 오브젝트
    public int criticalChance = 1; //치명타 확률 (0~100)
    public float criticalDamage = 1.5f; //치명타 데미지
    public bool isCritical;
    float weaponPickaxeDamage; // 곡괭이 데미지
    int weaponHunter2Count; // 헌터 2초 딜레이
    public GameObject weaponWindForceMax; // 태풍 소환
    int weaponCrazyFireCount; // 미친 불 카운트
    int tsunamiCount; // 쓰나미 카운트
    int weaponMeteoCount; // 메테오 카운트
    float shieldCount = 0; // 쉴드 카운트
    bool isShield; // 쉴드가 현재 생성되어 있는지 여부
    public GameObject shieldObj; // 쉴드 이미지
    float hammerCount; // 돌아가는 해머 카운트
    float horseCount; // 말 달리는 카운트
    bool isHorse; // 말에 타고있는가
    public GameObject horseAttackObj; // 말 타있는 오브젝트
    
    //패시브 관련 함수
    public bool[,] passiveLevel; // 딜레이, 스피드, 체력, 파워, 자석
    public float power = 1f;
    float magScale = 1f;
    float passiveShootDelay = 1f;
    float passiveHealingCount; //따로 체력 10초 세어서 회복
    float passiveHealingTimeCount; // 체력 1초 간격

    //이동 관련 함수
    public Vector2 inputVec;
    public float speed;
    public bool joyStickOn;
    public float exp;
    public float life;
    public float maxLife;
    public int gold;
    public int enemyClearNum;
    public JoyStick joystick;

    public GameManager gameManager;
    public ObjectManager objectManager;

    public Camera cam;
    
    //플레이어가 죽은 상태를 나타내는 값
    public bool playerDead = false;
    public int playerDeadCount = 0;

    Rigidbody2D rigid;
    public Animator anim;
    public RuntimeAnimatorController[] animCharacter;
    SpriteRenderer spriteRenderer;
    public GameObject borderBullet;

    //자석 함수
    public GameObject mag;

    //체력 회복량
    public float healthValue;

    //폭탄 이펙트
    public GameObject boomEffect;

    // 타겟 확인 이미지
    public GameObject targetImage;

    // 플레이어를 따라다니는 오브젝트 모음
    public GameObject playerFollowObj;

    void Start()
    {
        //모든 무기 레벨을 0으로 초기화
        for(int i=0;i<weaponLevel.Length;i++){
            weaponLevel[i] = 0;
        }
        passiveLevel = new bool[7,5];
        power = 1;
        gold = 0;
        switch(gameManager.character){
            case "Wizard":
                anim.runtimeAnimatorController = animCharacter[0];
            break;
            case "Hunter":
                // 모든 체력회복 2배
                anim.runtimeAnimatorController = animCharacter[1];
            break;
            case "Hunter2":
                anim.runtimeAnimatorController = animCharacter[2];
                life *= 2;
            break;
            case "Hunter3":
                anim.runtimeAnimatorController = animCharacter[3];
                speed += 0.2f;
            break;
            case "Hunter4":
                anim.runtimeAnimatorController = animCharacter[4];
                power += 1f;
            break;
        }
        maxLife=life;
    }
    void Awake()
    {
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDeadCount = 0;
    }

    void Update()
    {
        //체력이 0이면 죽는 에니메이션
        if(life <= 0 && playerDeadCount == 0 && !playerDead){
            life = 0;
            playerDead = true;
            ReturnSprite();
            //죽으면 몬스터랑 만나지 않는 레이어로 바꿔줘
            gameObject.layer = 7;
            //죽음 에니메이션
            anim.SetBool("Dead",playerDead);
            //멈춰라
            Invoke("TimeStopChance",0.5f);
        } else if(life <= 0 && playerDeadCount == 1 && !playerDead) {
            life = 0;
            playerDead = true;
            ReturnSprite();
            //죽으면 몬스터랑 만나지 않는 레이어로 바꿔줘
            gameObject.layer = 7;
            //죽음 에니메이션
            anim.SetTrigger("Dead");
            //멈춰라
            Invoke("TimeStopDead",0.5f);
            gameManager.playerDead();
        } else {
            if(targetEnemy!=null && targetEnemy.activeSelf && gameManager.IsTargetVisible(cam, targetEnemy.transform)){
                targetImage.SetActive(true);
                targetImage.transform.position = targetEnemy.transform.position;
            } else {
                targetImage.SetActive(false);
            }
            playerFollowObj.transform.position = transform.position;

            Fire();
            Passive();
            Weapon4Fire();
            Reload();
            PassiveHealing();
            Shield();
        }
    }
    void FixedUpdate()
    {
        if(playerDead)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void LateUpdate()
    {
        if(playerDead)
            return;
        
    //     if(joyStickOn)
    //         return;
            
         // 걷는 에니메이션(magnitude 순수 크기)
        anim.SetFloat("Speed", inputVec.magnitude);

        if(inputVec.x != 0 && !joyStickOn){
        // 캐릭터 좌, 우 방향전환
        spriteRenderer.flipX = inputVec.x < 0;
        }
    }

    // //인풋 시스템으로 이동 함수 단축
    // void OnMove(InputValue value)
    // {
    //     if(playerDead)
    //         return;

    //     inputVec = value.Get<Vector2>();
    // }

    void OnCollisionStay2D(Collision2D other) {
        //적군과 맞닿아 있다면 데미지가 들어가라
        if(playerDead) return;
        
        if(other.gameObject.tag == "Enemy"){
            if(isShield){
                if(shieldCount>=0.5f){
                    isShield = false;
                    shieldCount = 0;
                } else {
                    shieldCount += Time.deltaTime;
                }
            } else {
                EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
                PlayerDamaged(enemy.power);
            }
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        //적군과 떨어지면 색상을 복구해라
        if(other.gameObject.tag == "Enemy"){
            ReturnSprite();
        }
    }
    void PlayerRed()
    {
        spriteRenderer.color = new Color(1,0,0);
        Invoke("ReturnSprite",0.2f);
    }
    //플레이어 데미지
    public void PlayerDamaged(float dmg)
    {
        spriteRenderer.color = new Color(1,0,0);
        //해당 적군을 찾아서 해당 적군의 데미지가 초당 들어가라
        life -= dmg * Time.deltaTime;
    }
    //플레이어 화염 데미지
    //한번에 구현 할 필요 있음!

    void OnCollisionEnter2D(Collision2D other) {
         if (other.gameObject.tag == "Enemy") {
            //적군과 맞닿았다면 데미지가 들어가라
            if(playerDead)
                return;

            if(isShield){
                if(shieldCount>=0.5f){
                    isShield = false;
                    shieldCount = 0;
                } else {
                    shieldCount += 0.1f;
                }
            } else {
                EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
                life -= enemy.power;
            }
         }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "EnemyBullet"){
            if(isShield){ // 총알에 맞으면 쉴드가 바로 없어짐
                isShield = false;
                shieldCount = 0;
                other.gameObject.SetActive(false);
            } else {
                Bullet bullet = other.gameObject.GetComponent<Bullet>();
                life -= bullet.power;
                PlayerRed();
                other.gameObject.SetActive(false);
            }
            
        } else if(other.gameObject.tag == "Item"){
            Item item = other.gameObject.GetComponent<Item>();
            switch(item.type){
                //체력 회복
                case "Health":
                    Healing(healthValue);
                break;
                //자석
                case "Mag":
                    GameObject[] itemExp0 = objectManager.GetPool("ItemExp0");
                    for(int i=0;i<itemExp0.Length;i++){
                        Item itemExp0Logic = itemExp0[i].GetComponent<Item>();
                        itemExp0Logic.isMagnetOn = true;
                    }
                    GameObject[] itemExp1 = objectManager.GetPool("ItemExp1");
                    for(int i=0;i<itemExp1.Length;i++){
                        Item itemExp1Logic = itemExp1[i].GetComponent<Item>();
                        itemExp1Logic.isMagnetOn = true;
                    }
                    GameObject[] itemExp2 = objectManager.GetPool("ItemExp2");
                    for(int i=0;i<itemExp2.Length;i++){
                        Item itemExp2Logic = itemExp2[i].GetComponent<Item>();
                        itemExp2Logic.isMagnetOn = true;
                    }
                break;
                //경험치 10
                case "Exp0":
                    exp += 10;
                break;
                //경험치 50
                case "Exp1":
                    exp += 50;
                break;
                //경험치 100
                case "Exp2":
                    exp += 100;
                break;
                //동전0
                case "Coin0":
                    gold += 10;
                break;
                //동전1
                case "Coin1":
                    gold += 50;
                break;
                //동전2
                case "Coin2":
                    gold += 100;
                break;

                //폭탄 아이템
                case "Boom":
                    Boom(3000, true);
                break;
                case "Box":
                    // 박스를 먹을 경우 다른걸로 대체
                break;
                case "Shield":
                    if(isShield){ // 쉴드가 있다면 쉴드를 다시 0초로
                        shieldCount = 0f;
                    } else { // 없다면 쉴드 생성하고 0초로
                        shieldCount = 0f;
                        isShield = true;
                    }
                break;
            }
            other.gameObject.SetActive(false);
        }
    }
    void ReturnSprite()
    {
        spriteRenderer.color = new Color(1,1,1);
    }

    //미사일이 발사되는 함수
    void Fire()
    {
        if(curShootDelay < realDelay || !targetImage.activeSelf){
            return;
        }

        //무기
        if(gameManager.character=="Wizard"){
        // 아이스 스피어 (데미지 증가 스케일 업)
        switch(weaponLevel[0]){
            case 1:
                weapon3Dmg = 50 * power;
                WeaponIceSpear(weapon3Dmg, 1f,false);
            break;
            case 2:
                weapon3Dmg = 100 * power;
                WeaponIceSpear(weapon3Dmg, 1.2f,false);
            break;
            case 3:
                weapon3Dmg = 150 * power;
                WeaponIceSpear(weapon3Dmg, 1.3f,false);
            break;
            case 4:
                weapon3Dmg = 200 * power;
                WeaponIceSpear(weapon3Dmg, 1.4f,false);
            break;
            case 5:
                weapon3Dmg = 250* power;
                WeaponIceSpear(weapon3Dmg, 1.5f,false);
            break;
            case 6:
                weapon3Dmg = 500* power;
                WeaponIceSpear(weapon3Dmg, 1.8f,true);
            break;
            }
            //윈드포스
            switch(weaponLevel[1]){
            case 1:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 30 * power;
                    WeaponWindForce(weapon6Dmg,1f,1f,1f,0.1f);
                }
            break;
            case 2:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 60 * power;
                    WeaponWindForce(weapon6Dmg,1.1f,1.2f,1.2f,0.1f);
                }
            break;
            case 3:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 90 * power;
                    WeaponWindForce(weapon6Dmg,1.2f,1.6f,1.5f,0.1f);
                }
            break;
            case 4:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 120 * power;
                    WeaponWindForce(weapon6Dmg,1.3f,2f,1.7f,0.1f);
                }
            break;
            case 5:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 160 * power;
                    WeaponWindForce(weapon6Dmg,1.5f,2.5f,2f,0.2f);
                }
            break;
            case 6:
                if(weapon6Count<4){
                    weapon6Count++;
                } else {
                    weapon6Dmg = 200 * power;
                    WeaponWindForce(weapon6Dmg,1.5f,2.5f,2f,0.2f);
                }
                if(!weaponWindForceMax.activeSelf){
                    weaponWindForceMax.SetActive(true);
                }
                
            break;
            }
            switch(weaponLevel[2]){//파이어 볼
            case 1:
                weapon7Dmg = 50f * power;
                WeaponFireBall(weapon7Dmg, 1f, false);
            break;
            case 2:
                weapon7Dmg = 80f * power;
                WeaponFireBall(weapon7Dmg, 1.2f, false);
            break;
            case 3:
                weapon7Dmg = 120f * power;
                WeaponFireBall(weapon7Dmg, 1.4f, false);
            break;
            case 4:
                weapon7Dmg = 150f * power;
                WeaponFireBall(weapon7Dmg, 1.6f, false);
            break;
            case 5:
                weapon7Dmg = 190f * power;
                WeaponFireBall(weapon7Dmg, 1.8f, false);
            break;
            case 6:
                weapon7Dmg = 250f * power;
                WeaponFireBall(weapon7Dmg, 2f, true);
            break;
            }
            switch(weaponLevel[3]){// 체인 라이트닝
            case 1:
                WeaponChainLightning(40*power, 2,false);
            break;
            case 2:
                WeaponChainLightning(80*power, 3,false);
            break;
            case 3:
                WeaponChainLightning(120*power, 4,false);
            break;
            case 4:
                WeaponChainLightning(160*power, 5,false);
            break;
            case 5:
                WeaponChainLightning(200*power, 6,false);
            break;
            case 6:
                WeaponChainLightning(300*power, 6,true);
            break;
            }
            switch(weaponLevel[4]){//물 오로라
            case 1:
                weapon9Obj.SetActive(true);
            break;
            case 2:
                weapon9Obj.SetActive(true);
                weapon9Obj.transform.localScale = new Vector3(0.65f,0.65f,0);
            break;
            case 3:
                weapon9Obj.SetActive(true);
                weapon9Obj.transform.localScale = new Vector3(0.8f,0.8f,0);
            break;
            case 4:
                weapon9Obj.SetActive(true);
                weapon9Obj.transform.localScale = new Vector3(0.95f,0.95f,0);
            break;
            case 5:
                weapon9Obj.SetActive(true);
                weapon9Obj.transform.localScale = new Vector3(1.1f,1.1f,0);
            break;
            case 6:
                weapon9Obj.SetActive(true);
                weapon9Obj.transform.localScale = new Vector3(1.2f,1.2f,0);
                if(tsunamiCount>2){
                    WeaponTsunami(500 * power);
                } else {
                    tsunamiCount++;
                }
            break;
            }
            //돌아가는 돌맹이
            switch(weaponLevel[5]){
            case 1:
                if(weapon2First){
                    weapon2LV[0].SetActive(true);
                    weapon2LV[1].SetActive(true);
                    weapon2First = false;
                }
                if(weapon2Count>=10){
                    if(!weapon2LV[0].activeSelf){
                        weapon2LV[0].SetActive(true);
                        weapon2LV[1].SetActive(true);
                        weapon2Count = 0;
                    }
                } else {
                    weapon2Count++;
                }
                break;
                case 2:
                if(weapon2Count>=9){
                    if(weapon2LV[0].activeSelf){
                        weapon2LV[0].SetActive(false);
                        weapon2LV[1].SetActive(false);
                    }
                    if(!weapon2LV[2].activeSelf){
                        weapon2LV[2].SetActive(true);
                        weapon2LV[3].SetActive(true);
                        weapon2LV[4].SetActive(true);
                        weapon2Count = 0;
                    }
                } else {
                    weapon2Count++;
                }   
                break;
                case 3:
                if(weapon2Count>=8){
                    if(weapon2LV[2].activeSelf){
                        weapon2LV[2].SetActive(false);
                        weapon2LV[3].SetActive(false);
                        weapon2LV[4].SetActive(false);
                    }
                    if(!weapon2LV[5].activeSelf){
                        weapon2LV[5].SetActive(true);
                        weapon2LV[6].SetActive(true);
                        weapon2LV[7].SetActive(true);
                        weapon2LV[8].SetActive(true);
                        weapon2Count = 0;
                    }
                } else {
                    weapon2Count++;
                }   
                break;
                case 4:
                if(weapon2Count>=7){
                    if(weapon2LV[5].activeSelf){
                        weapon2LV[5].SetActive(false);
                        weapon2LV[6].SetActive(false);
                        weapon2LV[7].SetActive(false);
                        weapon2LV[8].SetActive(false);
                    }
                    if(!weapon2LV[9].activeSelf){
                        weapon2LV[9].SetActive(true);
                        weapon2LV[10].SetActive(true);
                        weapon2LV[11].SetActive(true);
                        weapon2LV[12].SetActive(true);
                        weapon2LV[13].SetActive(true);
                        weapon2Count = 0;
                    }
                } else {
                    weapon2Count++;
                }   
                break;
                case 5:
                if(weapon2Count>=6){
                    if(weapon2LV[9].activeSelf){
                        weapon2LV[9].SetActive(false);
                        weapon2LV[10].SetActive(false);
                        weapon2LV[11].SetActive(false);
                        weapon2LV[12].SetActive(false);
                        weapon2LV[13].SetActive(false);
                    }
                    if(!weapon2LV[14].activeSelf){
                        weapon2LV[14].SetActive(true);
                        weapon2LV[15].SetActive(true);
                        weapon2LV[16].SetActive(true);
                        weapon2LV[17].SetActive(true);
                        weapon2LV[18].SetActive(true);
                        weapon2LV[19].SetActive(true);
                        weapon2Count = 0;
                    }
                } else {
                    weapon2Count++;
                }
                break;
                case 6:
                    if(weapon2Count>=6){
                        if(weapon2LV[9].activeSelf){
                            weapon2LV[9].SetActive(false);
                            weapon2LV[10].SetActive(false);
                            weapon2LV[11].SetActive(false);
                            weapon2LV[12].SetActive(false);
                            weapon2LV[13].SetActive(false);
                        }
                        if(!weapon2LV[14].activeSelf){
                            weapon2LV[14].SetActive(true);
                            weapon2LV[15].SetActive(true);
                            weapon2LV[16].SetActive(true);
                            weapon2LV[17].SetActive(true);
                            weapon2LV[18].SetActive(true);
                            weapon2LV[19].SetActive(true);
                            weapon2Count = 0;
                        }
                    } else {
                        weapon2Count++;
                    }
                    if(weaponMeteoCount>=10){
                        WeaponMeteo(1000);
                        weaponMeteoCount=0;
                    } else {
                        weaponMeteoCount++;
                    }
                break;
            }
        } else if(gameManager.character == "Hunter"){
                if(weaponHunter2Count>1){
                    //곡괭이
                    switch(weaponLevel[0]){
                        case 1:
                            weaponPickaxeDamage = 120 * power;
                            WeaponPickaxe(weaponPickaxeDamage,1,false);
                        break;
                        case 2:
                            weaponPickaxeDamage = 200 * power;
                            WeaponPickaxe(weaponPickaxeDamage,2,false);
                        break;
                        case 3:
                            weaponPickaxeDamage = 300 * power;
                            WeaponPickaxe(weaponPickaxeDamage,3,false);
                        break;
                        case 4:
                            weaponPickaxeDamage = 400 * power;
                            WeaponPickaxe(weaponPickaxeDamage,4,false);
                        break;
                        case 5:
                            weaponPickaxeDamage = 600 * power;
                            WeaponPickaxe(weaponPickaxeDamage,5,false);
                        break;
                        case 6:
                            weaponPickaxeDamage = 800 * power;
                            WeaponPickaxe(weaponPickaxeDamage,5,true);
                        break;
                    }
                    //아래로 던지는 관통 삽
                    switch(weaponLevel[1]){
                        case 1:
                            weapon0Dmg = 100 * power;
                            WeaponShovel(weapon0Dmg,1);
                        break;
                        case 2:
                            weapon0Dmg = 200 * power;
                            WeaponShovel(weapon0Dmg,2);
                        break;
                        case 3:
                            weapon0Dmg = 300 * power;
                            WeaponShovel(weapon0Dmg,3);
                        break; 
                        case 4:
                            weapon0Dmg = 400 * power;
                            WeaponShovel(weapon0Dmg,4);
                        break;
                        case 5:
                            weapon0Dmg = 500 * power;
                            WeaponShovel(weapon0Dmg,5);
                        break;
                        case 6:
                            weapon0Dmg = 600 * power;
                            WeaponShovel(weapon0Dmg,10);
                        break;
                        }
                    //관통 창 무기
                    switch(weaponLevel[2]){
                        case 1:
                            weapon1Dmg = 80 * power;
                            WeaponTrident(weapon1Dmg,0.5f,false);
                        break;
                        case 2:                
                            weapon1Dmg = 160 * power;
                            WeaponTrident(weapon1Dmg,0.7f,false);
                        break;
                        case 3:                
                            weapon1Dmg = 240 * power;
                            WeaponTrident(weapon1Dmg,0.9f,false);
                        break;
                        case 4:                
                            weapon1Dmg = 320 * power;
                            WeaponTrident(weapon1Dmg,1.4f,false);
                        break;
                        case 5:                
                            weapon1Dmg = 400 * power;
                            WeaponTrident(weapon1Dmg,1.8f,false);
                        break;
                        case 6:                
                            weapon1Dmg = 500 * power;
                            WeaponTrident(weapon1Dmg,1.8f,true);
                        break;
                    }
                    weaponHunter2Count = 0;
                } else {
                        weaponHunter2Count++;
                    }
                //샷건 (탄 증가)
                switch(weaponLevel[4]){
                case 1:
                    weapon5Dmg = 100 * power;
                    WeaponShotgun(weapon5Dmg,2,false);
                break;
                case 2:
                    weapon5Dmg = 200 * power;
                    WeaponShotgun(weapon5Dmg,3,false);
                break;
                case 3:
                    weapon5Dmg = 300 * power;
                    WeaponShotgun(weapon5Dmg,4,false);
                break;
                case 4:
                    weapon5Dmg = 400 * power;
                    WeaponShotgun(weapon5Dmg,5,false);
                break;
                case 5:                
                    weapon5Dmg = 500 * power;
                    WeaponShotgun(weapon5Dmg,6,false);
                break;
                case 6:                
                    weapon5Dmg = 600 * power;
                    WeaponShotgun(weapon5Dmg,24,true);
                break;
                }
                //권총
                switch(weaponLevel[5]){
                    case 1:
                        WeaponPistol(100 * power,0.5f);
                    break;
                    case 2:
                        WeaponPistol(200 * power,0.5f);
                    break;
                    case 3:
                        WeaponPistol(300 * power,0.5f);
                    break;
                    case 4:
                        WeaponPistol(400 * power,0.5f);
                    break;
                    case 5:
                        WeaponPistol(500 * power,0.5f);
                    break;
                    case 6:
                        WeaponPistol(1000 * power,3f);
                    break;
                }
            } else if(gameManager.character == "Hunter2"){ //헌터 2 성기사 느낌
                switch(weaponLevel[0]){ // 가까이 있는 경우 데미지
                    case 1:
                        if(!thronArmor.activeSelf){
                            thronArmor.SetActive(true);
                        }
                    break;
                    case 6:
                        // 가시갑옷 궁극기!
                        Bullet bullet = thronArmor.GetComponent<Bullet>();
                        if(!bullet.armorKill){
                            bullet.armorKill=true;
                        }
                    break;
                }
                switch(weaponLevel[1]){ // 돌아가는 망치
                    case 1:
                        if(hammerCount>1){
                            ThrowHammer(50*power,false);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                    case 2:
                        if(hammerCount>1){
                            ThrowHammer(80*power,false);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                    case 3:
                        if(hammerCount>1){
                            ThrowHammer(120*power,false);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                    case 4:
                        if(hammerCount>1){
                            ThrowHammer(150*power,false);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                    case 5:
                        if(hammerCount>1){
                            ThrowHammer(180*power,false);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                    case 6:
                        if(hammerCount>1){
                            ThrowHammer(250*power,true);
                            hammerCount = 0;
                        } else {
                            hammerCount++;
                        }
                    break;
                }
                switch(weaponLevel[2]){ // 메이스 공격
                    case 1:
                        MaceAttack(80*power,50,false);
                    break;
                    case 2:
                        MaceAttack(160*power,100,false);
                    break;
                    case 3:
                        MaceAttack(240*power,150,false);
                    break;
                    case 4:
                        MaceAttack(320*power,200,false);
                    break;
                    case 5:
                        MaceAttack(400*power,250,false);
                    break;
                    case 6:
                        MaceAttack(500*power,250,true);
                    break;
                }
                switch(weaponLevel[3]){ // 5초마다 빠르게 전진하며 적군을 밀쳐내며 공격
                    case 1:
                        //2초 빠르게 달리고 5초 쉬고
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(50*power,false);
                        } else {
                            horseCount--;
                        }
                    break;
                    case 2:
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(100*power,false);
                        } else {
                            horseCount--;
                        }
                    break;
                    case 3:
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(140*power,false);
                        } else {
                            horseCount--;
                        }
                    break;
                    case 4:
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(200*power,false);
                        } else {
                            horseCount--;
                        }
                    break;
                    case 5:
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(240*power,false);
                        } else {
                            horseCount--;
                        }
                    break;
                    case 6:
                        if(horseCount<=0){
                            horseCount = 7;
                            HorseAttack(300*power,true);
                        } else {
                            horseCount--;
                        }
                    break;
                }
                switch(weaponLevel[4]){ // 십자가 모양으로 공격
                    case 1:
                    break;
                }
                switch(weaponLevel[5]){ // 빛 망치
                    case 1:
                    break;
                }
            } else if(gameManager.character == "Hunter3"){ //헌터 3 돌아다니는 물체
                switch(weaponLevel[0]){ // 얇은 통과 투사체
                    case 1:
                    break;
                }
                switch(weaponLevel[1]){ // 여러번 튕기는 투사체
                    case 1:
                    break;
                }
                switch(weaponLevel[2]){ // 랜덤 물체 (의자, 책상, 돌, 화염병, 꽝 등등)
                    case 1:
                    break;
                }
                switch(weaponLevel[3]){ // 지뢰
                    case 1:
                    break;
                }
                switch(weaponLevel[4]){ // 포탑 설치
                    case 1:
                    break;
                }
                switch(weaponLevel[5]){ // 활
                    case 1:
                    break;
                }
            } else if(gameManager.character == "Hunter4"){ // 헌터 4 소환수
                switch(weaponLevel[0]){ // 느린 공격 데미지 높은 곰
                    case 1:
                    break;
                }
                switch(weaponLevel[1]){ // 스켈레톤
                    case 1:
                    break;
                }
                switch(weaponLevel[2]){ // 적군을 쫒아 공격하는 새
                    case 1:
                    break;
                }
                switch(weaponLevel[3]){ // 독을 뿜는 뱀
                    case 1:
                    break;
                }
                switch(weaponLevel[4]){ // 얼음 골램
                    case 1:
                    break;
                }
                switch(weaponLevel[5]){ // 물 호수
                    case 1:
                    break;
                }
            }
        //재장전
        curShootDelay = 0;
    }
    void Weapon4Fire()
    {
        if(gameManager.character != "Hunter" || weapon4Count < weapon4MaxCount || !targetImage.activeSelf)
            return;

        //따발총(총알 딜레이 감소)
        switch(weaponLevel[3]){
            case 1:
                weapon4Delay = 2f;
                weapon4Dmg = 50 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
            case 2:
                weapon4Delay = 3f;
                weapon4Dmg = 100 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
            case 3:
                weapon4Delay = 4f;
                weapon4Dmg = 150 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
            case 4:
                weapon4Delay = 5f;
                weapon4Dmg = 200 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
            case 5:
                weapon4Delay = 6f;
                weapon4Dmg = 250 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
            case 6:
                weapon4Delay = 10f;
                weapon4Dmg = 350 * power;
                WeaponMachineGun(weapon4Dmg);
            break;
        }
    }
    void PassiveHealing()
    {
        if(passiveHealingTimeCount>=1){
            switch(weaponLevel[8]){ // 5초에 한번 체력 회복
            case 1:
                if(passiveHealingCount > 5){
                    Healing(maxLife*0.01f);
                    passiveHealingCount = 0;
                } else {
                    passiveHealingCount++;
                }
            break;
            case 2:
                if(passiveHealingCount > 5){
                    Healing(maxLife*0.02f);
                    passiveHealingCount = 0;
                } else {
                    passiveHealingCount++;
                }
            break;
            case 3:
                if(passiveHealingCount > 5){
                    Healing(maxLife*0.03f);
                    passiveHealingCount = 0;
                } else {
                    passiveHealingCount++;
                }
            break;
            case 4:
                if(passiveHealingCount > 5){
                    Healing(maxLife*0.04f);
                    passiveHealingCount = 0;
                } else {
                    passiveHealingCount++;
                }
            break;
            case 5:
                if(passiveHealingCount > 5){
                    Healing(maxLife*0.05f);
                    passiveHealingCount = 0;
                } else {
                    passiveHealingCount++;
                }
            break;
            }
            passiveHealingTimeCount = 0;
        }
    }
    void Passive()
    {
        //패시브
        //쿨타임 감소
        switch(weaponLevel[6]){
            case 1:
            passiveLevel[0,0] = true;
            passiveShootDelay = 0.9f;
            break;
            case 2:
            passiveLevel[0,0] = false;
            passiveLevel[0,1] = true;
            passiveShootDelay = 0.8f;
            break;
            case 3:
            passiveLevel[0,1] = false;
            passiveLevel[0,2] = true;
            passiveShootDelay = 0.7f;
            break;
            case 4:
            passiveLevel[0,2] = false;
            passiveLevel[0,3] = true;
            passiveShootDelay = 0.6f;
            break;
            case 5:
            passiveLevel[0,3] = false;
            passiveLevel[0,4] = true;
            passiveShootDelay = 0.5f;
            break;

        }
        //스피드 업
        switch(weaponLevel[7]){
            case 1:
                if(!passiveLevel[1,0]){
                    passiveLevel[1,0] = true;
                    speed += 0.2f;
                }
            break;
            case 2:
                if(!passiveLevel[1,1]){
                    passiveLevel[1,1] = true;
                    speed += 0.2f;
                }
            break;
            case 3:
                if(!passiveLevel[1,2]){
                    passiveLevel[1,2] = true;
                    speed += 0.2f;
                }
            break;
            case 4:
                if(!passiveLevel[1,3]){
                    passiveLevel[1,3] = true;
                    speed += 0.2f;
                }
            break;
            case 5:
                if(!passiveLevel[1,4]){
                    passiveLevel[1,4] = true;
                    speed += 0.2f;
                }
            break;
        }
        //체력 업
        switch(weaponLevel[8]){
            case 1:
            if(!passiveLevel[2,0]){
                passiveLevel[2,0] = true;
                maxLife += 0.1f;
            }
            break;
            case 2:
            if(!passiveLevel[2,1]){
                passiveLevel[2,1] = true;
                maxLife += 0.1f;
            }
            break;
            case 3:
            if(!passiveLevel[2,2]){
                passiveLevel[2,2] = true;
                maxLife += 0.1f;
            }
            break;
            case 4:
            if(!passiveLevel[2,3]){
                passiveLevel[2,3] = true;
                maxLife += 0.1f;
            }
            break;
            case 5:
            if(!passiveLevel[2,4]){
                passiveLevel[2,4] = true;
                maxLife += 0.1f;
            }
            break;
        }
        //공격력 업
        switch(weaponLevel[9]){
            case 1:
                if(!passiveLevel[3,0]){
                    passiveLevel[3,0] = true;
                    power += 0.1f;
                }
            break;
            case 2:
                if(!passiveLevel[3,1]){
                    passiveLevel[3,1] = true;
                    power += 0.1f;
                }
            break;
            case 3:
                if(!passiveLevel[3,2]){
                    passiveLevel[3,2] = true;
                    power += 0.1f;
                }
            break;
            case 4:
                if(!passiveLevel[3,3]){
                    passiveLevel[3,3] = true;
                    power += 0.1f;
                }
            break;
            case 5:
                if(!passiveLevel[3,4]){
                    passiveLevel[3,4] = true;
                    power += 0.1f;
                }
            break;
        }
        //자석
        switch(weaponLevel[10]){
            case 1:
            if(!passiveLevel[4,0]){
                passiveLevel[4,0] = true;
                magScale += 0.9f;
                mag.transform.localScale = new Vector3(magScale,magScale,0);
            }
            break;
            case 2:
            if(!passiveLevel[4,1]){
                passiveLevel[4,1] = true;
                magScale += 0.9f;
                mag.transform.localScale = new Vector3(magScale,magScale,0);
            }
            break;
            case 3:
            if(!passiveLevel[4,2]){
                passiveLevel[4,2] = true;
                magScale += 0.9f;
                mag.transform.localScale = new Vector3(magScale,magScale,0);
            }
            break;
            case 4:
            if(!passiveLevel[4,3]){
                passiveLevel[4,3] = true;
                magScale += 0.9f;
                mag.transform.localScale = new Vector3(magScale,magScale,0);
            }
            break;
            case 5:
            if(!passiveLevel[4,4]){
                passiveLevel[4,4] = true;
                magScale += 0.9f;
                mag.transform.localScale = new Vector3(magScale,magScale,0);
            }
            break;
        }
        //크리티컬 확률 및 데미지
        switch(weaponLevel[11]){
            case 1:
                if(!passiveLevel[5,0]){
                    passiveLevel[5,0] = true;
                    criticalDamage += 0.05f;
                    criticalChance += 4;
                }
            break;
            case 2:
                if(!passiveLevel[5,1]){
                    passiveLevel[5,1] = true;
                    criticalDamage += 0.05f;
                    criticalChance += 5;
                }
            break;
            case 3:
                if(!passiveLevel[5,2]){
                    passiveLevel[5,2] = true;
                    criticalDamage += 0.05f;
                    criticalChance += 5;
                }
            break;
            case 4:
                if(!passiveLevel[5,3]){
                    passiveLevel[5,3] = true;
                    criticalDamage += 0.05f;
                    criticalChance += 5;
                }
            break;
            case 5:
                if(!passiveLevel[5,4]){
                    passiveLevel[5,4] = true;
                    criticalDamage += 0.05f;
                    criticalChance += 5;
                }
            break;
        }
    }
    void Reload()
    {
        passiveHealingTimeCount += Time.deltaTime;
        realDelay = maxShootDelay * passiveShootDelay;
        curShootDelay += Time.deltaTime;
        weapon4Count += Time.deltaTime;
        weapon4MaxCount = maxShootDelay/weapon4Delay;
    }
    void WeaponShovel(float dmg,int num)
    {
        for (int i=0;i<num;i++){
            float ranX = Random.Range(-2f,2f);
            float ranZ = Random.Range(-2f,2f);
            GameObject bullet = objectManager.MakeObj("BulletPlayer0");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;

            //회전 함수
            Vector3 dirVec = new Vector3(ranX, -7.5f, ranZ);
            rigid.AddForce(dirVec, ForceMode2D.Impulse);
        }
    }
    void WeaponTrident(float dmg, float scale, bool maxLevel)
    {
        if(!maxLevel){
            GameObject bullet = objectManager.MakeObj("BulletPlayer1");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            bulletLogic.power = dmg;

            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
            bullet.transform.localScale = new Vector3(scale,scale,0);
            Vector3 dirVec = targetImage.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        } else {
            //앞
            GameObject bullet = objectManager.MakeObj("BulletPlayer1");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            bulletLogic.power = dmg;

            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
            bullet.transform.localScale = new Vector3(scale,scale,0);
            Vector3 dirVec = targetImage.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            //뒤
            GameObject bullet2 = objectManager.MakeObj("BulletPlayer1");
            Bullet bulletLogic2 = bullet2.GetComponent<Bullet>();
            bullet2.transform.position = transform.position;
            Rigidbody2D rigid2 = bullet2.GetComponent<Rigidbody2D>();
            bulletLogic2.power = dmg;

            //회전 함수
            float degree2 = Mathf.Atan2(transform.position.y+targetImage.transform.position.y,transform.position.x+targetImage.transform.position.x)*180f/Mathf.PI;
            bullet2.transform.rotation = Quaternion.Euler(0,0,degree2 + 90f);
            bullet2.transform.localScale = new Vector3(scale,scale,0);
            Vector3 dirVec2 = targetImage.transform.position - transform.position;
            rigid2.AddForce(-dirVec2.normalized * bulletSpeed, ForceMode2D.Impulse);
        }
    }
    void WeaponIceSpear(float dmg, float scale, bool maxLevel)
    {
        GameObject bullet = objectManager.MakeObj("BulletPlayer3");
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        bulletLogic.power = dmg;
        bulletLogic.iceBoom = maxLevel;
        
        //회전 함수
        float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
        bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
                
        bullet.transform.localScale = new Vector3(scale,scale,0);
        Vector3 dirVec = targetImage.transform.position - transform.position;
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
    }
    void WeaponMachineGun(float dmg)
    {
        GameObject bullet = objectManager.MakeObj("BulletPlayer4");
        Vector3 ranVec = new Vector3(Random.Range(-0.2f,0.3f),Random.Range(-0.2f,0.3f),0);
        bullet.transform.position = transform.position + ranVec;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.power = dmg;

        //회전 함수
        float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
        bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
                
        Vector3 dirVec = (targetImage.transform.position - transform.position)*Random.Range(0.00f,0.10f);
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        weapon4Count = 0;
    }
    void WeaponShotgun(float dmg, int count, bool maxLevel)
    {
        if(!maxLevel){
            for(int index = 0;index < count;index++){
                GameObject bullet = objectManager.MakeObj("BulletPlayer5");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = dmg;
                Vector3 ranVec = new Vector3(Random.Range(-0.2f,0.3f)*count,Random.Range(-0.2f,0.3f)*count,0);

                //회전 함수
                float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
                bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);

                Vector3 dirVec = targetImage.transform.position - transform.position + ranVec;
                rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
            }
        } else {
            for(int index = 0;index < count;index++){
                GameObject bullet = objectManager.MakeObj("BulletPlayer5");
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = dmg;
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / count), Mathf. Sin(Mathf.PI * 2 * index / count));
                rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

                //총알 회전 로직
                Vector3 rotVec = Vector3.forward * 360 * index / count + Vector3.forward * 90;
                bullet.transform.Rotate(rotVec);
            }
        }
    }
    void WeaponWindForce(float dmg, float scale, float timer, float nuckBack, float nuckBackTime)
    {
        GameObject bullet = objectManager.MakeObj("BulletPlayer6");
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.waepon6Timer = timer;

        bulletLogic.nuckBack = nuckBack;
        bulletLogic.nuckBackTime = nuckBackTime;
        bulletLogic.power = dmg;
        bullet.transform.localScale = new Vector3(scale,scale,0);
        bullet.transform.position = transform.position;
        weapon6Count = 0;
    }
    void WeaponFireBall(float dmg, float scale, bool maxLevel)
    {
        if(!maxLevel){
            GameObject bullet = objectManager.MakeObj("BulletPlayer7");
            bullet.transform.position = transform.position;
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            bulletLogic.power = dmg;
            bullet.transform.localScale = new Vector3(scale,scale,0);
            Vector3 dirVec = targetImage.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed/3, ForceMode2D.Impulse);
        } else {
            if(weaponCrazyFireCount>=3){
                weaponCrazyFireCount=0;
            }else if(weaponCrazyFireCount==2){
                for (int i = 0;i<10;i++ ){
                    GameObject bullet = objectManager.MakeObj("BulletPlayer7");
                    bullet.transform.position = transform.position;
                    Bullet bulletLogic = bullet.GetComponent<Bullet>();
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

                    bulletLogic.power = dmg;
                    bullet.transform.localScale = new Vector3(scale,scale,0);
                
                    //회전 함수
                    Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / 10), Mathf. Sin(Mathf.PI * 2 * i / 10));
                    rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

                    //총알 회전 로직
                    Vector3 rotVec = Vector3.forward * 360 * i / 10 + Vector3.forward * 90;
                    bullet.transform.Rotate(rotVec);
                }
                weaponCrazyFireCount++;
            } else if(weaponCrazyFireCount==1) {
                for (int i = 0;i<6;i++ ){
                    GameObject bullet = objectManager.MakeObj("BulletPlayer7");
                    bullet.transform.position = transform.position;
                    Bullet bulletLogic = bullet.GetComponent<Bullet>();
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

                    bulletLogic.power = dmg;
                    bullet.transform.localScale = new Vector3(scale,scale,0);

                    //회전 함수
                    Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / 6), Mathf. Sin(Mathf.PI * 2 * i / 6));
                    rigid.AddForce(-dirVec.normalized * bulletSpeed/2, ForceMode2D.Impulse);

                    //총알 회전 로직
                    Vector3 rotVec = Vector3.forward * 360 * i / 6 + Vector3.forward * 90;
                    bullet.transform.Rotate(rotVec);
                }
                weaponCrazyFireCount++;
            } else {
                for (int i = 0;i<3;i++ ){
                    GameObject bullet = objectManager.MakeObj("BulletPlayer7");
                    bullet.transform.position = transform.position;
                    Bullet bulletLogic = bullet.GetComponent<Bullet>();
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

                    bulletLogic.power = dmg;
                    bullet.transform.localScale = new Vector3(scale,scale,0);

                    //회전 함수
                    Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i /3), Mathf. Sin(Mathf.PI * 2 * i /3));
                    rigid.AddForce(-dirVec.normalized * bulletSpeed/3, ForceMode2D.Impulse);

                    //총알 회전 로직
                    Vector3 rotVec = Vector3.forward * 360 * i /3 + Vector3.forward * 90;
                    bullet.transform.Rotate(rotVec);
                }
                weaponCrazyFireCount++;
            }
        }
    }
    void WeaponChainLightning(float dmg, int num, bool maxLevel)
    {
        if(!maxLevel){
            for(int index = 0;index < num;index++){
                float ranX = Random.Range(-2.5f,2.5f);
                float ranY = Random.Range(-2.5f,8.5f);
                GameObject bullet8 = objectManager.MakeObj("BulletPlayer8");
                bullet8.transform.position = new Vector3(transform.position.x+ranX, transform.position.y+ranY);
                Rigidbody2D rigid8 = bullet8.GetComponent<Rigidbody2D>();
                Bullet bulletLogic8 = bullet8.GetComponent<Bullet>();
                bulletLogic8.power = dmg;
            }
        } else {
            for(int index = 0;index < num;index++){
                float ranX = Random.Range(-2f,2f);
                float ranY = Random.Range(-2f,8f);
                float ranScaleX = Random.Range(1f,2f);
                float ranScaleY = Random.Range(1.5f,2.5f);
                GameObject bullet8 = objectManager.MakeObj("BulletPlayer8");
                bullet8.transform.localScale = new Vector3(ranScaleX,ranScaleY,0);
                bullet8.transform.position = new Vector3(transform.position.x+ranX, transform.position.y+ranY);
                Rigidbody2D rigid8 = bullet8.GetComponent<Rigidbody2D>();
                Bullet bulletLogic8 = bullet8.GetComponent<Bullet>();
                bulletLogic8.power = dmg;
            }
        }
    }
    void WeaponPickaxe(float dmg, int num, bool maxLevel)
    {
        for(int index = 0;index < num;index++){
            float ranX = Random.Range(-2f,2f);
            float ranZ = Random.Range(-2f,2f);
            GameObject bullet = objectManager.MakeObj("BulletPlayer10");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            if(maxLevel){
                bullet.transform.localScale = new Vector3(2,2,0);
            }
            //회전 함수
            Vector3 dirVec = new Vector3(ranX, 7.5f, ranZ);
            rigid.AddForce(dirVec, ForceMode2D.Impulse);
        }
    }
    void WeaponPistol(float dmg, float scale)
    {
        GameObject bullet = objectManager.MakeObj("BulletPlayer11");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.power = dmg;
        bullet.transform.localScale = new Vector3(scale,scale,0);

        //회전 함수
        float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
        bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
                
        Vector3 dirVec = targetImage.transform.position - transform.position;
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
    }
    void WeaponTsunami(float dmg)
    {
        for(int i =0;i<9;i++){
            GameObject bullet = objectManager.MakeObj("BulletPlayer13");
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            Vector3 vec =  new Vector3(transform.position.x - 3.5f,transform.position.y+(4f-i),0);
            bullet.transform.position = vec;
            rigid.AddForce(Vector2.right * bulletSpeed, ForceMode2D.Impulse);
        }
        tsunamiCount=0;
    }
    void WeaponMeteo(float dmg)
    {
        GameObject bullet = objectManager.MakeObj("BulletPlayer14");
        bullet.transform.position = new Vector3(transform.position.x -4,transform.position.y+7,0);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.power = dmg;
        rigid.AddForce(Vector2.right*2, ForceMode2D.Impulse);
        rigid.AddForce(Vector2.down*0.5f, ForceMode2D.Impulse);
    }
    // 돌아가는 망치
    void ThrowHammer(float dmg, bool maxlevel)
    {
        if(!maxlevel){
            GameObject bullet = objectManager.MakeObj("BulletPlayer16");
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            bulletLogic.hammerBack = maxlevel;
            bulletLogic.playerCurVec = transform.position;
        } else {
            GameObject bullet = objectManager.MakeObj("BulletPlayer16");
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            bulletLogic.playerCurVec = transform.position;

            GameObject bullet1 = objectManager.MakeObj("BulletPlayer16");
            Rigidbody2D rigid1 = bullet1.GetComponent<Rigidbody2D>();
            Bullet bulletLogic1 = bullet1.GetComponent<Bullet>();
            bulletLogic1.power = dmg;
            bulletLogic1.hammerBack = maxlevel;
            bulletLogic1.playerCurVec = transform.position;
        }
    }
    //철퇴 공격
    void MaceAttack(float dmg, float maceSpeed, bool maxlevel)
    {
        if(maxlevel){
            GameObject bullet = objectManager.MakeObj("BulletPlayer17");
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            bullet.transform.localScale = new Vector3(0.5f,0.5f,0);
            
            Vector3 dirVec = targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized;
            bulletLogic.maceSpeed = maceSpeed;
            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
        } else {
            GameObject bullet = objectManager.MakeObj("BulletPlayer17");
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = dmg;
            
            Vector3 dirVec = targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized;
            bulletLogic.maceSpeed = maceSpeed;
            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-targetImage.transform.position.y,transform.position.x-targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
        }
        
    }
    //말타고 달리기
    void HorseAttack(float dmg, bool maxLevel)
    {
        horseAttackObj.SetActive(true);
        Bullet bulletLogic = horseAttackObj.GetComponent<Bullet>();

        bulletLogic.power = dmg;
        if(maxLevel){
            bulletLogic.nuckBack = 3f;
        }
    }
    //치명타
    public float CriticalHit(float dmg)
    {
        int ran = Random.Range(1,101);
        if(criticalChance >= ran){
            dmg = criticalDamage * dmg;
            isCritical = true;
        } else {
            isCritical = false;
        }
        return dmg;
    }
    void TimeStopChance()
    {
        gameManager.playerDeadChance();
        Time.timeScale = 0;
        CancelInvoke("TimeStopChance");
    }
    void TimeStopDead()
    {
        gameManager.playerDead();
        Time.timeScale = 0;
        CancelInvoke("TimeStopDead");
    }
    void Healing(float healValue)
    {
        if(gameManager.character=="Hunter"){
            healValue *= 2f;
            if(life+healValue>maxLife){
                gameManager.DamageText(transform.position, maxLife - life, "Healing", false);
                life = maxLife;
            } else if (life>=maxLife){
                gameManager.DamageText(transform.position, 0, "Healing", false);
                life = maxLife;
            } else {
                gameManager.DamageText(transform.position, healValue, "Healing", false);
                life += healValue;
            }
        } else {
            if(life+healValue>maxLife){
                gameManager.DamageText(transform.position, maxLife - life, "Healing", false);
                life = maxLife;
            } else if (life>=maxLife){
                gameManager.DamageText(transform.position, 0, "Healing", false);
                life = maxLife;
            } else {
                gameManager.DamageText(transform.position, healValue, "Healing", false);
                life += healValue;
            }
        }
        
    }
    public void Boom(float dmg, bool isBoom)
    {
        //화면 에있는 모든 적들에게 에게 데미지를 준다
        boomEffect.SetActive(true);
        GameObject[] enemyA = objectManager.GetPool("EnemyA");
        GameObject[] enemyB = objectManager.GetPool("EnemyB");
        GameObject[] enemyC = objectManager.GetPool("EnemyC");
        GameObject[] enemyD = objectManager.GetPool("EnemyD");
        GameObject[] enemyE = objectManager.GetPool("EnemyE");
        for(int index=0; index<enemyA.Length;index++){
            if(objectManager.GetPool("EnemyA")[index].activeSelf){
                Vector3 screenPoint = cam.WorldToViewportPoint(objectManager.GetPool("EnemyA")[index].transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if(onScreen){
                    EnemyMove enemyLogicA = enemyA[index].GetComponent<EnemyMove>();
                    enemyLogicA.OnHit(dmg);
                }
            }
        }
        for(int index=0; index<enemyB.Length;index++){
            if(objectManager.GetPool("EnemyB")[index].activeSelf){
                Vector3 screenPoint = cam.WorldToViewportPoint(objectManager.GetPool("EnemyB")[index].transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if(onScreen){
                    EnemyMove enemyLogicB = enemyB[index].GetComponent<EnemyMove>();
                    enemyLogicB.OnHit(dmg);
                }
            }
        }
        for(int index=0; index<enemyC.Length;index++){
            if(objectManager.GetPool("EnemyC")[index].activeSelf){
                Vector3 screenPoint = cam.WorldToViewportPoint(objectManager.GetPool("EnemyC")[index].transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if(onScreen){
                    EnemyMove enemyLogicC = enemyC[index].GetComponent<EnemyMove>();
                    enemyLogicC.OnHit(dmg);
                }
            }
        }
        for(int index=0; index<enemyD.Length;index++){
            if(objectManager.GetPool("EnemyD")[index].activeSelf){
                Vector3 screenPoint = cam.WorldToViewportPoint(objectManager.GetPool("EnemyD")[index].transform.position);
                bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if(onScreen){
                    EnemyMove enemyLogicD = enemyD[index].GetComponent<EnemyMove>();
                    enemyLogicD.OnHit(dmg);
                }
            }
        }
        // 보스는 사라지기에 그냥 놔둔다;
        if(objectManager.GetPool("EnemyE")[0].activeSelf){
            Vector3 screenPoint = cam.WorldToViewportPoint(objectManager.GetPool("EnemyE")[0].transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if(onScreen){
                EnemyMove enemyLogicE = enemyE[0].GetComponent<EnemyMove>();
                enemyLogicE.OnHit(dmg);
            }
        }
        if(!isBoom)
            return;

        // 모든 적군의 총알을 없앤다
        GameObject[] bulletA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletB = objectManager.GetPool("BulletEnemyB");
        for(int index=0; index<bulletA.Length;index++){
            bulletA[index].SetActive(false);
            bulletB[index].SetActive(false);
        }
    }
    void Shield()
    {
        if(isShield){
            shieldObj.SetActive(true);
        } else {
            shieldObj.SetActive(false);
        }
    }
}
