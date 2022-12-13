using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    public RuntimeAnimatorController[] animCharacter;
    public string type;
    public float power;
    public float life;
    public GameManager gameManager;
    SpriteRenderer spriteRenderer;
    bool playerObjDead;
    public bool maxLevel;
    float count;
    Rigidbody2D rigid;
    Animator anim;
    public PlayerMove playerMove;
    float skullAttackTime; // 스컬 공격 쿨타임
    public float speed; // 이동 속도
    void Start()
    {
        switch(type){
            case "Turret":
                life = 100f;
            break;
            case "Skull":
                anim.runtimeAnimatorController = animCharacter[0];
            break;
            case "BowSkull":
                anim.runtimeAnimatorController = animCharacter[1];
            break;
        }
    }
    void OnEnable()
    {
        playerObjDead = false;
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.instance;
        playerMove = GameManager.instance.playerMove;
    }

    void Update()
    {
        if(life<=0){
            if(type == "Bird")
                return;

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
        if(type=="Turret" && gameObject.activeSelf && maxLevel){ // 터렛 궁극기 화염 방사기
            count-= Time.deltaTime;
            if(count<=0){
                GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer7");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Bullet bulletLogic = bullet.GetComponent<Bullet>();
                bulletLogic.power = power;
                bulletLogic.fireTime = 3f;
                        
                Vector3 dirVec = gameManager.playerMove.targetImage.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * gameManager.playerMove.bulletSpeed/2, ForceMode2D.Impulse);
                count = 0.3f;
            }
        } else if(type=="Bear" && gameObject.activeSelf){ // 곰 움직임 구현
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
        } else if(type=="Skull" && gameObject.activeSelf){ // 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                anim.SetFloat("Speed",1);
                anim.SetBool("IsAttack",false);
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
                    anim.SetFloat("Speed",0);
                    skullAttackTime -= Time.deltaTime;
                    if(skullAttackTime<=0){
                        Fire(maxLevel);
                        skullAttackTime = 1f;
                    }
                } else {
                    anim.SetBool("IsAttack",false);
                    anim.SetFloat("Speed",1);
                    Vector2 direction = new Vector2(transform.position.x - GameManager.instance.playerMove.targetImage.transform.position.x,transform.position.y - GameManager.instance.playerMove.targetImage.transform.position.y);
                    Vector2 nextVec = new Vector2((direction.x * Time.deltaTime) * 1.5f,(+direction.y * Time.deltaTime) * 1.5f);
                    rigid.MovePosition(rigid.position - nextVec);
                }
            }
        } else if(type=="BowSkull" && gameObject.activeSelf){ // 활 스컬 움직임 구현
            if(Vector3.Distance(transform.position, playerMove.transform.position)>3f){ // 플레이어와 거리가 멀다면 플레이어에게 다가가라
                anim.SetFloat("Speed",1);
                anim.SetBool("IsAttack",false);
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
                    anim.SetFloat("Speed",1);
                } else {
                    //적군 위치에 따른 적군 좌 우 이미지 반전
                    anim.SetFloat("Speed",0);
                    if(playerMove.targetImage.transform.position.x < transform.position.x) {
                        spriteRenderer.flipX = false;
                    } else {
                        spriteRenderer.flipX = true;
                    }
                    skullAttackTime -= Time.deltaTime;
                    if(skullAttackTime<=0){
                        Fire(maxLevel);
                        skullAttackTime = 1f;
                    }
                }
            }
        }
    }
    public void Fire(bool maxLevel)
    {
        if(type == "Turret"){ // 터렛 총알 공격
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
        } else if(type == "Bear"){ // 곰 공격
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer27");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            
            //회전 함수
            float degree = Mathf.Atan2(transform.position.y-playerMove.targetImage.transform.position.y,transform.position.x-playerMove.targetImage.transform.position.x)*180f/Mathf.PI;
            bullet.transform.rotation = Quaternion.Euler(0,0,degree+45f);

            Vector3 dirVec = GameManager.instance.playerMove.targetImage.transform.position - transform.position;
            bullet.transform.position = transform.position + dirVec.normalized;
        } else if(type == "Skull"){ // 스컬 공격
            anim.SetBool("IsAttack",true);
            GameObject bullet = gameManager.objectManager.MakeObj("BulletPlayer30");
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;

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
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "BorderBullet"){
            gameObject.SetActive(false);
        }
    }
    void OnCollisionStay2D(Collision2D other) {
        //적군과 맞닿아 있다면 데미지가 들어가라
        if(playerObjDead) return;
        if(type == "Bird") return;
        
        if(other.gameObject.tag == "Enemy"){
            EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
            PlayerObjDamaged(enemy.power);
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
            spriteRenderer.color = new Color(1,1,1);
        }
    }
}
