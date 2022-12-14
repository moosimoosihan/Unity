using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    public RuntimeAnimatorController[] animCharacter;
    public Sprite[] sprites;
    public string type;
    public float power;
    public float life;
    public GameManager gameManager;
    SpriteRenderer spriteRenderer;
    bool playerObjDead;
    public bool maxLevel;
    Rigidbody2D rigid;
    Animator anim;
    public PlayerMove playerMove;
    float AttackTime; // 스컬 공격 쿨타임
    public float speed; // 이동 속도
    float maxLife; // 최대 체력
    float healingCount; // 회복 카운트
    public Queue<Vector3> parentPos;
    public Vector3 followPos;
    int followDelay = 12;
    public bool isMaxOn;
    void Start()
    {
        switch(type){
            case "Turret":
                life = 100f;
            break;
            case "Skull":
                anim.runtimeAnimatorController = animCharacter[0];
                spriteRenderer.sprite = sprites[0];
            break;
            case "BowSkull":
                anim.runtimeAnimatorController = animCharacter[1];
                spriteRenderer.sprite = sprites[1];
            break;
            case "IceGolem":
                spriteRenderer.color = new Color(1,1,1);
            break;
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
        }
        maxLife = life;
    }
    void OnEnable()
    {
        playerObjDead = false;
        if(type == "Lake"){
            Invoke("ActiveOff",15f);
        }
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.instance;
        playerMove = GameManager.instance.playerMove;
        parentPos = new Queue<Vector3>();
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
                GameObject dieEffect = gameManager.objectManager.MakeObj("Overload");
                Effect dieEffectLogic = dieEffect.GetComponent<Effect>();
                dieEffect.transform.position = transform.position;
                dieEffectLogic.power = power;
            }
        }
        if(type=="Bear" && gameObject.activeSelf){ // 곰 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 moveVec1;
                //이동 함수
                if(direction1.x >= 1){
                    anim.SetInteger("X", -1);
                    anim.SetInteger("Y", 0);
                    moveVec1 = new Vector2(1,0);
                } else if(direction1.x <= -1){
                    anim.SetInteger("X", 1);
                    anim.SetInteger("Y", 0);
                    moveVec1 = new Vector2(-1,0);
                } else if(direction1.y >= 1){
                    anim.SetInteger("Y", -1);
                    anim.SetInteger("X", 0);
                    moveVec1 = new Vector2(0,1);
                } else if(direction1.y <= -1){
                    anim.SetInteger("Y", 1);
                    anim.SetInteger("X", 0);
                    moveVec1 = new Vector2(0,-1);
                } else {
                    moveVec1 = new Vector2(0,0);
                }
                Vector2 nextVec1 = new Vector2((moveVec1.x * Time.deltaTime)*1.5f,(+moveVec1.y * Time.deltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
            } else {
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=1f){// 곰과 적군 사이가 가깝다면 공격모션
                    anim.SetInteger("X",0);
                    anim.SetInteger("Y",0);
                } else {
                    Vector2 direction = new Vector2(transform.position.x - playerMove.targetImage.transform.position.x,transform.position.y - playerMove.targetImage.transform.position.y);
                    Vector2 moveVec;
                    //이동 함수
                    if(direction.x >= 1){
                        anim.SetInteger("X", -1);
                        anim.SetInteger("Y", 0);
                        moveVec = new Vector2(1,0);
                    } else if(direction.x <= -1){
                        anim.SetInteger("X", 1);
                        anim.SetInteger("Y", 0);
                        moveVec = new Vector2(-1,0);
                    } else if(direction.y >= 1){
                        anim.SetInteger("Y", -1);
                        anim.SetInteger("X", 0);
                        moveVec = new Vector2(0,1);
                    } else if(direction.y <= -1){
                        anim.SetInteger("Y", 1);
                        anim.SetInteger("X", 0);
                        moveVec = new Vector2(0,-1);
                    } else {
                        moveVec = new Vector2(0,0);
                    }
                    Vector2 nextVec = new Vector2(moveVec.x * Time.deltaTime * speed,+moveVec.y * Time.deltaTime * speed);
                    rigid.MovePosition(rigid.position - nextVec);
                }
            }
        } else if(type=="Skull" && gameObject.activeSelf){ // 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                anim.SetBool("IsAttack",false);
                if(playerMove.transform.position.x < transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = new Vector2((direction1.x * Time.deltaTime)*1.5f,(+direction1.y * Time.deltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
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
                        AttackTime -= Time.deltaTime;
                    }
                } else {
                    anim.SetBool("IsAttack",false);
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = new Vector2((direction.x * Time.deltaTime) * 1.5f,(+direction.y * Time.deltaTime) * 1.5f);
                    rigid.MovePosition(rigid.position - nextVec);
                }
            }
        } else if(type=="BowSkull" && gameObject.activeSelf){ // 활 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                anim.SetBool("IsAttack",false);
                if(playerMove.transform.position.x < transform.position.x) { // 플레이어에게 갈때
                    spriteRenderer.flipX = false;
                } else {
                    spriteRenderer.flipX = true;
                }
                Vector2 direction1 = new Vector2(transform.position.x - GameManager.instance.playerMove.transform.position.x,transform.position.y - GameManager.instance.playerMove.transform.position.y);
                Vector2 nextVec1 = new Vector2((direction1.x * Time.deltaTime)*1.5f,(+direction1.y * Time.deltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
            } else {
                if(Vector3.Distance(transform.position, playerMove.targetImage.transform.position)<=2f){// 스컬과 적군 사이가 가깝다면 멀리 떨어져라
                    anim.SetBool("IsAttack",false);
                    if(playerMove.targetImage.transform.position.x > transform.position.x) { // 도망가기에 반대로
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = new Vector2((direction.x * Time.deltaTime)*1.5f,(+direction.y * Time.deltaTime)*1.5f);
                    rigid.MovePosition(rigid.position + nextVec);
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
                        AttackTime -= Time.deltaTime;
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
                Vector2 nextVec1 = new Vector2((direction1.x * Time.deltaTime)*1.5f,(+direction1.y * Time.deltaTime)*1.5f);
                rigid.MovePosition(rigid.position - nextVec1);
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
                    Vector2 nextVec = new Vector2((direction.x * Time.deltaTime) * 1.5f,(+direction.y * Time.deltaTime) * 1.5f);
                    rigid.MovePosition(rigid.position - nextVec);
                }
            }
        }
    }
    public void Fire(bool maxLevel)
    {
        if(type == "Turret"){ // 터렛 총알 공격
             if(maxLevel){ // 터렛 궁극기 화염 방사기
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer7");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.matchCount += 4;
                
                Vector3 dirVec = gameManager.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * gameManager.playerMove.bulletSpeed/2, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer11");
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
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer27");
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
            anim.SetBool("IsAttack",true);
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bulletLogic.elementalType = "None";

            Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized * 0.5f;
        } else if(type == "BowSkull"){ // 활 스컬 공격
            anim.SetBool("IsAttack",true);
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer29");
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
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer3");
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
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Ice";

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "FireGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer7");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bullet.transform.position = transform.position;
                bulletLogic.power = power;

                bulletLogic.matchCount = 5;
                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * GameManager.instance.playerMove.bulletSpeed/2, ForceMode2D.Impulse);
            } else {
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Fire";
            
                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "StoneGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer2");
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
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
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
                    GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer9");
                    Bullet bulletLogic = bullet.GetComponent<Bullet>();
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                    bulletLogic.power = power;
                    bulletLogic.maxLevel = maxLevel;
                    bulletLogic.playerObjPos = this.gameObject;
                    bullet.transform.position = transform.position;
                    bullet.transform.localScale = new Vector3(0.4f,0.4f,0);
                }
            } else {
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Water";

                Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
                bullet.transform.position = transform.position + dirVec.normalized * 0.6f;
            }
        } else if(type == "LightningGolem"){
            if(maxLevel){
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer8");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                bulletLogic.power = power;
                
                Vector3 dirVec = new Vector3(GameManager.instance.playerMove.targetImage.transform.position.x,GameManager.instance.playerMove.targetImage.transform.position.y+2.65f,0);
                bullet.transform.position = dirVec;
            } else {
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.elementalType = "Lightning";

                Vector3 dirVec = new Vector3(GameManager.instance.playerMove.targetImage.transform.position.x - transform.position.x,GameManager.instance.playerMove.targetImage.transform.position.y - transform.position.y,0);
                bullet.transform.position = transform.position + dirVec.normalized * 0.5f;
            }
        } else if(type == "Lake"){
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bulletLogic.elementalType = "Water";
            bullet.transform.position = GameManager.instance.playerMove.targetImage.transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "BorderBullet"){
            gameObject.SetActive(false);
        }
    }
    void OnCollisionStay2D(Collision2D other) {
        if(playerObjDead) return;
        if(type == "Bird") return;
        if(type == "Lake") return;
        
        //적군과 맞닿아 있다면 데미지가 들어가라
        if(other.gameObject.tag == "Enemy"){
            EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
            PlayerObjDamaged(enemy.power);
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
        if(playerObjDead) return;
        if(type == "Bird") return;
        if(type == "Lake") return;
        
        // 호수에 닿아 있다면 회복
        if(other.gameObject.tag == "PlayerObj"){
            PlayerObj lakeLogic = other.gameObject.GetComponent<PlayerObj>();
            if(lakeLogic.type == "Lake"){
                healingCount += Time.deltaTime;
                if(healingCount>=5){
                    Healing(power*0.05f);
                    healingCount = 0;
                }
            }
        }
    }
    //플레이어 오브젝트 데미지
    void PlayerObjDamaged(float dmg)
    {
        spriteRenderer.color = new Color(1,0,0);
        //해당 적군을 찾아서 해당 적군의 데미지가 초당 들어가라
        life -= dmg * Time.deltaTime;
    }
    void OnCollisionExit2D(Collision2D other) {
        //적군과 떨어지면 색상을 복구해라
        if(other.gameObject.tag == "Enemy"){
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
                default:
                    spriteRenderer.color = new Color(1,1,1);
                break;
            }
        }
    }
    void OnDisable()
    {
        if(type=="IceGolem" || type=="FireGolem" || type=="StoneGolem" || type=="WaterGolem" ||type=="LightningGolem"){ // 골렘의 경우
            isMaxOn = false;
            playerMove.golemCount = 3;
            playerMove.isGolem = false;
        } else if(type == "Lake"){
            CancelInvoke();
            playerMove.lakeCount = 3;
            playerMove.isLake = false;
        } else if(type == "Bear"){
            playerMove.brearCount = 3;
        }
    }
    void Healing(float healValue)
    {
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
}