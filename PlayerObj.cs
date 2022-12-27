using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    float playerObjDamagedTime;
    public RuntimeAnimatorController[] animCharacter;
    public Sprite[] sprites;
    public string type;
    public float power;
    public float life;
    public GameManager gameManager;
    SpriteRenderer spriteRenderer;
    public bool playerObjDead;
    public bool maxLevel;
    Rigidbody2D rigid;
    Animator anim;
    public PlayerMove playerMove;
    float AttackTime; // 스컬 공격 쿨타임
    public float speed; // 이동 속도
    public float maxLife; // 최대 체력
    float healingCount; // 회복 카운트
    public Queue<Vector3> parentPos;
    public Vector3 followPos;
    int followDelay = 12;
    public bool isMaxOn;
    public string elementalType;

    // 플레이어 오브젝트 상태 함수
    //화염
    public bool isFire; //불에 붙었는가?
    public float fireOffCount;
    public float fireDamage;
    float fireDamageTextTime;

    //얼음
    public bool isIce; // 얼음 상태인가? (빙결 x)
    public float iceOffCount = 3f;
    float iceSlow = 1f;
    public float iceDamage;
    public bool isFreezingOn;
    float freezingOffCount = 3f;
    int freezStop = 1;

    //전기
    public bool isLightning;
    public float lightningOffCount = 3f;
    float lightingDamageTextTime;
    public bool ElectricShockOn;
    float ElectricShockOffCount = 3f;
    float ElectricShockDamageTextTime;
    public float lightningDamage;
    int lightningStop = 1;
    //물
    public bool isWater;
    public float waterDamage;
    float waterDamageTextTime;
    public float waterOffCount;

    //바람
    float windDamageTextTime;

    //물리
    float noneDamageTextTime;

    //빛
    float lightDamageTextTime;

    //독
    float poisonCount;
    float poisonDamageTextTime;
    bool isPoison;
    float poisonDamage;


    // 오디오
    AudioSource audioSource;


    void Start()
    {
        if(type == "Skull"){
            anim.runtimeAnimatorController = animCharacter[0];
            spriteRenderer.sprite = sprites[0];
        } else if(type== "BowSkull"){
            anim.runtimeAnimatorController = animCharacter[1];
            spriteRenderer.sprite = sprites[1];
        }
        // playerObjDead = false;
        maxLife = life;
    }
    void OnEnable()
    {
        playerObjDead = false;
        if(type=="Lake"){
            Invoke("ActiveOff",15f);
        } else if(type == "Turret"){
            life = 100f;
            elementalType = "None";
        }
    }
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.instance;
        playerMove = GameManager.instance.playerMove;
        parentPos = new Queue<Vector3>();
    }

    void FixedUpdate()
    {
        if(playerObjDead)
            return;

        if(type=="Bear" && gameObject.activeSelf){ // 곰 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = new Vector2((direction1.normalized.x * Time.fixedDeltaTime)*1.5f,(+direction1.normalized.y * Time.fixedDeltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
                rigid.velocity = Vector2.zero;
                if(playerMove.transform.position.x > transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
            } else {
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=0.6f){// 골렘과 적군 사이가 가깝다면 공격모션
                    anim.SetFloat("Speed",0);
                } else {
                    if(playerMove.targetImage.transform.position.x > transform.position.x) { // 적군 위치에 따른 적군 좌 우 이미지 반전
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                    anim.SetFloat("Speed",1);
                    Vector2 direction = new Vector2(transform.position.x - playerMove.targetImage.transform.position.x,transform.position.y - playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = new Vector2(direction.normalized.x * Time.fixedDeltaTime * speed,+direction.normalized.y * Time.fixedDeltaTime * speed);
                    rigid.MovePosition(rigid.position - nextVec);
                    rigid.velocity = Vector2.zero;
                }
            }
        } else if(type=="Skull" && gameObject.activeSelf){ // 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                if(playerMove.transform.position.x < transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = new Vector2((direction1.normalized.x * Time.fixedDeltaTime)*1.5f,(+direction1.normalized.y * Time.fixedDeltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
                rigid.velocity = Vector2.zero;
            } else {
                //적군 위치에 따른 적군 좌 우 이미지 반전
                if(playerMove.targetImage.transform.position.x < transform.position.x) {
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=0.6f){// 스컬과 적군 사이가 가깝다면 공격모션
                    if(AttackTime<=0){
                        Fire(maxLevel);
                        if(maxLevel){
                            AttackTime = 0.5f;
                        } else {
                            AttackTime = 1f;
                        }
                    } else {
                        AttackTime -= Time.fixedDeltaTime;
                    }
                } else {
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = direction.normalized * Time.fixedDeltaTime * 1.5f * (iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
                    rigid.MovePosition(rigid.position - nextVec);
                    rigid.velocity = Vector2.zero;
                }
            }
        } else if(type=="BowSkull" && gameObject.activeSelf){ // 활 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                if(playerMove.transform.position.x < transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = direction1.normalized * Time.fixedDeltaTime * 1.5f * (iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
                rigid.MovePosition(rigid.position - nextVec1);
                rigid.velocity = Vector2.zero;
            } else {
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=2f){// 스컬과 적군 사이가 가깝다면 멀리 떨어져라
                    if(playerMove.targetImage.transform.position.x > transform.position.x) { // 도망가기에 반대로
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = direction.normalized * Time.fixedDeltaTime * 1.5f * (iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
                    rigid.MovePosition(rigid.position + nextVec);
                    rigid.velocity = Vector2.zero;
                } else {
                    //적군 위치에 따른 적군 좌 우 이미지 반전
                    if(playerMove.targetImage.transform.position.x < transform.position.x) {
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                    if(AttackTime<=0){
                        Fire(maxLevel);
                        if(maxLevel){
                            AttackTime = 0.5f;
                        } else {
                            AttackTime = 1f;
                        }
                    } else {
                        AttackTime -= Time.fixedDeltaTime;
                    }
                }
            }
        } else if(type=="IceGolem" || type=="FireGolem" || type=="StoneGolem" || type=="WaterGolem" ||type=="LightningGolem"){ // 골렘의 경우
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                anim.SetFloat("Speed",1);
                if(playerMove.transform.position.x > transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = direction1.normalized * Time.fixedDeltaTime * 1.5f * (iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
                rigid.MovePosition(rigid.position - nextVec1);
                rigid.velocity = Vector2.zero;
            } else {
                //적군 위치에 따른 적군 좌 우 이미지 반전
                if(playerMove.targetImage.transform.position.x > transform.position.x) {
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=0.6f){// 골렘과 적군 사이가 가깝다면 공격모션
                    anim.SetFloat("Speed",0);
                } else {
                    anim.SetFloat("Speed",1);
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = direction.normalized * Time.fixedDeltaTime * 1.5f * (iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
                    rigid.MovePosition(rigid.position - nextVec);
                    rigid.velocity = Vector2.zero;
                }
            }
        }

    }

    void Update()
    {
        if(type == "Lake"){
            if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=4f){// 호수과 적군 사이가 가깝다면 공격모션
                if(AttackTime<=0){
                    Fire(maxLevel);
                    if(maxLevel){
                        AttackTime = 0.5f;
                    } else {
                        AttackTime = 1f;
                    }
                } else {
                    AttackTime -= Time.deltaTime;
                }
            }
            return;
        } else if(type == "Bird"){
            // 딜레이, 따라가는 위치, 부모 위치 값
            if(!parentPos.Contains(playerMove.transform.position)){
                parentPos.Enqueue(playerMove.transform.position);
            }
            if(parentPos.Count>followDelay){
                followPos = parentPos.Dequeue();
            } else if(parentPos.Count<followDelay) {
                followPos = playerMove.transform.position;
            }
            Vector3 nextPos = new Vector3(followPos.x,followPos.y + 1f);
            transform.position = nextPos;
            if(playerMove.transform.position.x < transform.position.x) { // 플레이어에게 갈때
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
            return;
        }

        if(life<=0){
            playerObjDead = true;
            gameObject.SetActive(false);
            if(type == "Skull" || type == "BowSkull"){
                playerMove.skullCount--;
            }
            if(type == "Turret"){ // 터렛의 경우 폭발 데미지
                GameObject dieEffect = gameManager.objectManager.Get(54);
                Effect dieEffectLogic = dieEffect.GetComponent<Effect>();
                dieEffect.transform.position = transform.position;
                dieEffectLogic.power = power;
            }
        }
        PlayerObjState();
    }
    public void Fire(bool maxLevel)
    {
        if(playerObjDead)
            return;

        if(type == "Turret"){ // 터렛 총알 공격
             if(maxLevel){ // 터렛 궁극기 화염 방사기
                GameObject bullet = gameManager.objectManager.Get(56);
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.matchCount += 4;
                
                Vector3 dirVec = gameManager.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * gameManager.playerMove.bulletSpeed/2, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.Get(22);
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;

                //회전 함수
                float degree = Mathf.Atan2(transform.position.y-gameManager.playerMove.targetImage.transform.position.y,transform.position.x-gameManager.playerMove.targetImage.transform.position.x)*180f/Mathf.PI;
                bullet.transform.rotation = Quaternion.Euler(0,0,degree + 90f);
                        
                Vector3 dirVec = gameManager.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * gameManager.playerMove.bulletSpeed, ForceMode2D.Impulse);
                bulletLogic.turretBullet = true;
            }
        } else if(type == "Bear"){ // 곰 공격
            GameObject bullet = gameManager.objectManager.Get(38);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            
            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-playerMove.targetImage.transform.position.y,transform.position.x-playerMove.targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree+45f);

            Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized;
            if(maxLevel){
                PointEffector2D bulletPointer = bullet.GetComponent<PointEffector2D>();
                bulletPointer.forceMagnitude = 100;
            }
        } else if(type == "Skull"){ // 스컬 공격
            anim.SetTrigger("IsAttack");
            GameObject bullet = gameManager.objectManager.Get(41);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bulletLogic.elementalType = "None";

            Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized * 0.5f;
        } else if(type == "BowSkull"){ // 활 스컬 공격
            anim.SetTrigger("IsAttack");
            GameObject bullet = gameManager.objectManager.Get(40);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            
            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-playerMove.targetImage.transform.position.y,transform.position.x-playerMove.targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree+90f);

            Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position;
            rigid.AddForce(dirVec.normalized * playerMove.bulletSpeed, ForceMode2D.Impulse);
        } else if(type == "IceGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.Get(14);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.position = transform.position;
                bulletLogic.power = power;
                bulletLogic.maxLevel = maxLevel;

                //회전 함수
                float degree = Mathf.Atan2(transform.position.y-GameManager.instance.playerMove.targetImage.transform.position.y,transform.position.x-GameManager.instance.playerMove.targetImage.transform.position.x)*180f/Mathf.PI;
                bullet.transform.rotation = Quaternion.Euler(0,0,degree+90f);

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position;
                rigid.AddForce(dirVec.normalized * GameManager.instance.playerMove.bulletSpeed, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.Get(41);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Ice";

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "FireGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.Get(18);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.position = transform.position;
                bulletLogic.power = power;

                bulletLogic.matchCount = 5;
                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * GameManager.instance.playerMove.bulletSpeed/2, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.Get(41);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Fire";
            
                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "StoneGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.Get(13);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.position = transform.position;
                bulletLogic.power = power;
                bulletLogic.maxLevel = maxLevel;
                bulletLogic.playerObjPos = this.gameObject;
                
                rigid.bodyType = RigidbodyType2D.Dynamic;
                Vector3 dirVec = playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * playerMove.bulletSpeed, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.Get(41);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Stone";

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "WaterGolem"){
            if(maxLevel){
                if(!isMaxOn){
                    isMaxOn = true;
                    GameObject bullet = gameManager.objectManager.Get(20);
                    Bullet bulletLogic = bullet.GetComponent<Bullet>();
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                    bulletLogic.power = power;
                    bulletLogic.maxLevel = maxLevel;
                    bulletLogic.playerObjPos = this.gameObject;
                    bullet.transform.position = transform.position;
                    bullet.transform.localScale = new Vector3(0.4f,0.4f,0);
                }
            } else {
                GameObject bullet = gameManager.objectManager.Get(41);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Water";

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "LightningGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.Get(19);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bulletLogic.power = power;
                
                Vector3 dirVec = new Vector3(GameManager.instance.playerMove.targetImage.transform.position.x,GameManager.instance.playerMove.targetImage.transform.position.y+2.65f,0);
                bullet.transform.position = dirVec;
            } else {
                GameObject bullet = gameManager.objectManager.Get(41);
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Lightning";

                Vector3 dirVec = new Vector3(GameManager.instance.playerMove.targetImage.transform.position.x - transform.position.x,GameManager.instance.playerMove.targetImage.transform.position.y - transform.position.y,0);
                bullet.transform.position = transform.position + dirVec.normalized * 0.5f;
            }
        } else if(type == "Lake"){
            GameObject bullet = gameManager.objectManager.Get(41);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bulletLogic.elementalType = "Water";
            bullet.transform.position = GameManager.instance.playerMove.targetImage.transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(playerObjDead)
            return;

        if(type == "Bird")
            return;

        if(type == "Lake")
            return;

        if(other.gameObject.tag == "BorderBullet"){
            gameObject.SetActive(false);
        } else if(other.gameObject.tag == "EnemyBullet"){
            PlayerObjDamaged(other.gameObject, "Bullet");
        } else if(type=="Effect"){
            PlayerObjDamaged(other.gameObject,"Effect");
        }
    }
    void OnCollisionStay2D(Collision2D other) {
        if(playerObjDead)
            return;

        if(type == "Bird")
            return;

        if(type == "Lake")
            return;
        
        //적군과 맞닿아 있다면 데미지가 들어가라
        if(other.gameObject.tag == "Enemy"){
            EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
            if(playerObjDamagedTime>=1){
                PlayerObjDamaged(other.gameObject, "Enemy");
                playerObjDamagedTime = 0;
            } else {
                spriteRenderer.color = new Color(1,0,0);
                playerObjDamagedTime += Time.deltaTime;
            }
            if(type == "IceGolem"){
                enemy.isIce = true;
                enemy.iceDamage = power;
                enemy.iceOffCount = 3f;
            } else if(type == "FireGolem"){
                enemy.isFire = true;
                enemy.fireDamage = power;
                enemy.fireOffCount = 3f;
            } else if(type == "WaterGolem"){
                enemy.isWater = true;
                enemy.waterDamage = power;
                enemy.waterOffCount = 3f;
            } else if(type == "LightningGolem"){
                enemy.isLightning = true;
                enemy.lightningDamage = power;
                enemy.lightningOffCount = 3f;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(!gameObject.activeSelf)
            return;

        if(type == "Bird")
            return;

        if(type == "Lake")
            return;
        
        // 호수에 닿아 있다면 회복
        if(other.gameObject.tag == "PlayerObj"){
            PlayerObj lakeLogic = other.gameObject.GetComponent<PlayerObj>();
            if(lakeLogic.type == "Lake"){
                if(healingCount>=5){
                    Healing(lakeLogic.power*0.05f);
                    healingCount = 0;
                } else {
                    healingCount += Time.deltaTime;
                }
            }
        }
    }
    //플레이어 오브젝트 데미지
    void PlayerObjDamaged(GameObject target, string type)
    {
        if(playerObjDead)
            return;

        audioSource.clip = gameManager.audioManager.hit0Auido;
        if(!audioSource.isPlaying){
            gameManager.audioManager.PlayOneShotSound(audioSource, audioSource.clip, audioSource.volume);
        }
        if(type == "Enemy"){
            EnemyMove enemyLogic = target.GetComponent<EnemyMove>();
            float dmg = enemyLogic.CriticalHit(enemyLogic.power);
            gameManager.DamageText(transform.position, dmg, enemyLogic.elementalType, enemyLogic.isCritical);
            life -= dmg;
            
            // 적군 공격 타입 별 플레이어 오브젝트 데미지 입는 함수 넣어야 함!
            if(enemyLogic.elementalType =="Fire"){
                
            } else if(enemyLogic.elementalType =="Ice") {

            } else if(enemyLogic.elementalType =="Lightning"){

            } else if(enemyLogic.elementalType =="Water"){

            }
        } else if(type == "Bullet"){
            Bullet bullet = target.gameObject.GetComponent<Bullet>();
            gameManager.DamageText(transform.position, bullet.power, bullet.elementalType, bullet.enemyCritical);
            life -= bullet.power;
            PlayerObjRed();
            target.gameObject.SetActive(false);
            // 적군 공격 타입 별 플레이어 오브젝트 데미지 입는 함수
            if(bullet.elementalType =="Fire"){
                IsFire(bullet);
            } else if(bullet.elementalType =="Ice") {
                IsIce(bullet);
            } else if(bullet.elementalType =="Lightning"){
                IsLightning(bullet);
            } else if(bullet.elementalType =="Water"){
                IsWater(bullet);
            }
        } else if(type=="EnemyEffect"){
            Effect effect = target.gameObject.GetComponent<Effect>();
            gameManager.DamageText(transform.position, effect.power, effect.elementalType, false);
            life -= effect.power;
            PlayerObjRed();
        }
    }
    public void PlayerObjStateHit(float power, string type)
    {
        //맞은 데미지 출력
        gameManager.DamageText(transform.position, power, type, false);
        life -= power;
        PlayerObjRed();
    }
    void OnCollisionExit2D(Collision2D other) {
        if(playerObjDead)
            return;

        if(type == "Bird")
            return;

        if(type == "Lake")
            return;

        //적군과 떨어지면 색상을 복구해라
        if(other.gameObject.tag == "Enemy"){
            ReturnSprite();
        }
    }
    void OnDisable()
    {
        PlayerStateClear();
        if(type=="IceGolem" || type=="FireGolem" || type=="StoneGolem" || type=="WaterGolem" || type=="LightningGolem"){ // 골렘의 경우
            isMaxOn = false;
            playerMove.golemCount = 3;
            playerMove.isGolem = false;
        } else if(type == "Lake"){
            CancelInvoke();
            playerMove.lakeCount = 3;
            playerMove.isLake = false;
        } else if(type == "Bear"){
            playerMove.brearCount = 3;
        } else if(type == "Skull" || type == "BowSkull" ){
            anim.runtimeAnimatorController = null;
            spriteRenderer.sprite = null;
        }
    }
    void Healing(float healValue)
    {        
        if(playerObjDead)
            return;
        
        audioSource.clip = gameManager.audioManager.healingAudio;
        if(!audioSource.isPlaying){
            gameManager.audioManager.PlayOneShotSound(audioSource, audioSource.clip, audioSource.volume);
        }
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
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    void PlayerObjRed()
    {
        spriteRenderer.color = new Color(1,0,0);
        Invoke("ReturnSprite",0.2f);
    }
    void ReturnSprite()
    {
        switch(type){
            case "FireGolem":
                spriteRenderer.color = new Color(0.7f,0,0);
            break;
            case "StoneGolem":
                spriteRenderer.color = new Color(1,1,0);
            break;
            case "WaterGolem":
                spriteRenderer.color = new Color(0.2f,0.5f,1);
            break;
            case "LightningGolem":
                spriteRenderer.color = new Color(1,0,1);
            break;
            case "IceGolem":
                spriteRenderer.color = new Color(1,1,1);
            break;
            default:
                spriteRenderer.color = new Color(1,1,1);
            break;
        }
    }
    // 상태 이상
    // 초전도 함수
    public void isSuperconductivity(GameObject bullet, string type)
    {
        isLightning = false;
        lightningOffCount = 0;
        isIce = false;
        iceOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Superconductivity");
        PlayerObjDamaged(bullet, type);
    }
    //과부화 함수
    public void isOverload(string type)
    {
        GameObject overload = gameManager.objectManager.Get(55);
        Effect overloadLogic = overload.GetComponent<Effect>();
        overload.gameObject.transform.position = transform.position;
        overloadLogic.power = lightningDamage + fireDamage;
        isLightning = false;
        lightningOffCount = 0;
        isFire = false;
        fireOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Overload");
    }
    //융해 함수
    void isMelting(GameObject bullet, string type)
    {
        isIce = false;
        iceOffCount = 0;
        isFire = false;
        fireOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Melting");
        PlayerObjDamaged(bullet, type);
    }
    //빙결 함수
    void isFreezing()
    {
        isIce = false;
        iceOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Freezing");
        freezingOffCount = 3f;
        isFreezingOn = true;
    }
    void PlayerObjFreezing()
    {
        if (isFreezingOn) {
            if (freezingOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                freezStop = 1;
                isFreezingOn = false;
                return;
            }

            freezingOffCount -= Time.deltaTime;
            freezStop = 0;
            spriteRenderer.color = new Color(0.2f, 0.5f, 0.8f);
        }
    }
    //증발 함수
    void isEvaporation(GameObject bullet, string type)
    {
        isFire = false;
        fireOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Evaporation");
        PlayerObjDamaged(bullet, type);
    }

    //감전 함수
    public void isElectricShock()
    {
        isWater = false;
        waterOffCount = 0;
        isLightning = false;
        lightningOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "ElectricShock");
        ElectricShockOffCount = 3f;
        ElectricShockOn = true;
    }
    void PlayerObjElectricShock()
    {
        if (ElectricShockOn) {
            if (ElectricShockOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                lightningStop = 1;
                ElectricShockOn = false;
                return;
            }

            spriteRenderer.color = new Color(0.5f, 0, 1);
            ElectricShockOffCount -= Time.deltaTime;
            ElectricShockDamageTextTime += Time.deltaTime;
            lightningStop = 1;

            if (ElectricShockDamageTextTime >= 0.9f) {
                lightningStop = 0;
            }
            if (ElectricShockDamageTextTime >= 1) {
                PlayerObjStateHit(lightningDamage, "Lightning");
                ElectricShockDamageTextTime = 0;
            }
        }
    }

    //불에 붙은 경우 초당 불 데미지를 입는다.
    void PlayerObjFireDamaged()
    {
        if (isFire) {
            if (fireOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                isFire = false;
                return;
            }

            spriteRenderer.color = new Color(1, 0, 0);
            fireOffCount -= Time.deltaTime;
            fireDamageTextTime += Time.deltaTime;

            if (fireDamageTextTime >= 1) {
                PlayerObjStateHit(fireDamage, "Fire");
                fireDamageTextTime = 0;
            }
        }
    }
    // 독에 맞은경우 초당 독 데미지
    void PlayerObjPoisonDamaged()
    {
        if (isPoison) {
            if (poisonCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                isPoison = false;
                return;
            }

            spriteRenderer.color = new Color(0, 0.5f, 0);
            poisonCount -= Time.deltaTime;
            poisonDamageTextTime += Time.deltaTime;

            if (poisonDamageTextTime >= 1) {
                PlayerObjStateHit(poisonDamage, "Poison");
                poisonDamageTextTime = 0;
            }
        }
    }
    //얼음에 맞을 경우 이동속도가 느려진다.
    void PlayerObjIceSlow()
    {
        if (isIce) {
            if (iceOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                iceSlow = 1;
                isIce = false;
                return;
            }

            spriteRenderer.color = new Color(0, 0, 1);
            iceSlow = 0.5f;
            iceOffCount -= Time.deltaTime;
        }
    }
    //전기에 맞을 경우 초당 50% 데미지가 들어간다.
    void PlayerObjLightning()
    {
        if (isLightning) {
            if (lightningOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                isLightning = false;
                return;
            }

            spriteRenderer.color = new Color(0.5f, 1, 1);
            lightningOffCount -= Time.deltaTime;
            lightingDamageTextTime += Time.deltaTime;

            if (lightingDamageTextTime >= 1) {
                PlayerObjStateHit(lightningDamage, "Lightning");
                lightingDamageTextTime = 0;
            }
        }
    }
    //물에 젖을 경우(색상변화)
    void PlayerObjWater()
    {
        if (isWater) {
            if (waterOffCount <= 0) {
                spriteRenderer.color = new Color(1, 1, 1);
                isWater = false;
                return;
            }

            spriteRenderer.color = new Color(0.3f, 0.7f, 0.8f);
            waterOffCount -= Time.deltaTime;
        }
    }
    void PlayerObjState()
    {
        PlayerObjPoisonDamaged();
        PlayerObjFireDamaged();
        PlayerObjIceSlow();
        PlayerObjLightning();
        PlayerObjWater();
        PlayerObjFreezing();
        PlayerObjElectricShock();
    }
    public void IsFire(Bullet bullet)
    {
        if (isFire) {
            fireOffCount = 3f;

            if (isIce) {//융해
                isMelting(bullet.gameObject, bullet.elementalType);
            } else if (isLightning) {//과부화
                lightningDamage = bullet.power;
                isOverload(bullet.elementalType);
            } else if (isWater) {// 증발
                isEvaporation(bullet.gameObject, bullet.elementalType);
            }
            return;
        }

        //불 붙는 함수
        isFire = true;
        fireOffCount = 3f;
        fireDamage = bullet.power;
        if (isIce) {// 융해
            isMelting(bullet.gameObject, bullet.elementalType);
        } else if (isLightning) {//과부화
            fireDamage = bullet.power;
            isOverload(bullet.elementalType);
        } else if (isWater) {// 증발
            isEvaporation(bullet.gameObject, bullet.elementalType);
        }
    }
    public void IsIce(Bullet bullet)
    {
        PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
        if (isIce) {
            iceOffCount = 3f;
            if (isFire) {// 융해
                isMelting(bullet.gameObject, bullet.elementalType);
            } else if (isLightning) {// 초전도
                isSuperconductivity(bullet.gameObject, bullet.elementalType);
            } else if (isWater) {// 프리즈
                isFreezing();
            }
        } else {
            // 얼음 이동속도 감소
            isIce = true;
            iceDamage = bullet.power;
            iceOffCount = 3f;
            if (isFire) {// 융해
                isMelting(bullet.gameObject, bullet.elementalType);
            } else if (isLightning) {//초전도
                isSuperconductivity(bullet.gameObject, bullet.elementalType);
            } else if (isWater) {// 프리즈
                isFreezing();
            }
        }
    }
    public void IsLightning(Bullet bullet)
    {
        lightningOffCount = 3f;
        isLightning = true;
        lightningDamage = bullet.power;
        PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
        if (isFire) { // 불이 붙었다면 과부화
            isOverload("Overload");
        } else if (isIce) { // 얼음이 붙었다면 초전도
            isSuperconductivity(bullet.gameObject, "Superconductivity");
        } else if (isWater) { // 물이 묻었다면 감전
            isElectricShock();
        }
    }
    public void IsWater(Bullet bullet)
    {
        PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
        isWater = true;
        waterOffCount = 3f;
        if (isIce) {
            isFreezing();
        } else if (isLightning) {
            isElectricShock();
        } else if (isFire) {
            isEvaporation(bullet.gameObject, bullet.elementalType);
        }
    }
    public void IsWind(Bullet bullet)
    {
        if (isFire) {//불이 붙어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
            PlayerObjDamaged(bullet.gameObject, "Fire");
        } else if (isIce) {//얼음이 뭍어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
            PlayerObjDamaged(bullet.gameObject, "Ice");
        } else if (isLightning) {//전기가 뭍어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
            PlayerObjDamaged(bullet.gameObject, "Lightning");
        } else if (isWater) {//물이 붙어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
            PlayerObjDamaged(bullet.gameObject, "Water");
        } else {
            PlayerObjDamaged(bullet.gameObject, bullet.elementalType);
        }
    }
    // 플레이어 오브젝트 스탯 초기화 함수
    public void PlayerStateClear()
    {
        isPoison = false;
        poisonCount = 0;
        isFire = false;
        fireOffCount = 0;
        isIce = false;
        iceOffCount = 0;
        isLightning = false;
        lightningOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        ElectricShockOn = false;
        ElectricShockOffCount = 0;
        isFreezingOn = false;
        freezingOffCount = 0;
        lightningStop = 1;
        iceSlow = 1;
        freezStop = 1;
        spriteRenderer.color = new Color(1, 1, 1);
    }
}