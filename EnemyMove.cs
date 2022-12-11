using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    //적군 총알 함수
    public string enemyName;
    public float maxShootDelay;
    public float curShootDelay;
    public float bulletSpeed;
    public float bulletPower;
    public GameObject player;

    public GameManager gameManager;
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    Vector3 dir;
    SpriteRenderer spriteRenderer;
    public PlayerMove playerLogic;
    bool isNuckBack;
    //화염
    public bool isFire; //불에 붙었는가?
    float fireOffCount;
    float fireDamage;
    float fireDamageTextTime;
    //얼음
    public bool isIce; // 얼음 상태인가? (빙결 x)
    float iceOffCount = 3f;
    float iceSlow = 1f;
    float iceDamage;
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
    float waterDamage;
    float waterDamageTextTime;
    float waterOffCount;

    //바람
    float windDamageTextTime;

    //물리
    float noneDamageTextTime;

    Rigidbody2D rigid;
    Animator anim;
    public float enemyLife;
    public float speed;
    public float power;
    public ObjectManager objectManager;
    public float enemyMaxLife;
    public int stage;

    //보스 패턴 관련 함수
    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    bool isDrop; //아이템 한번만 떨어지는 함수

    void Awake()
    {
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stage = GameManager.instance.stage;
    }
    void OnEnable()
    {
        gameObject.layer = 8;
        isDrop = false;
        enemyStateClear();
        switch(enemyName){
            case "EnemyA":
                enemyLife = 80*stage;
                speed = 1.1f + (stage*0.1f);
                power = 20*stage;
                break;
            case "EnemyB":
                enemyLife = 500*stage;
                speed = 1.4f + (stage*0.1f);
                power = 30*stage;
                break;
            case "EnemyC":
                enemyLife = 4000*stage;
                speed = 1.4f + (stage*0.1f);
                power = 50*stage;
                break;
            case "EnemyD":
                enemyLife = 10000*stage;
                speed = 1.7f + (stage*0.1f);
                power = 80*stage;
                break;
            case "Box":
                enemyLife = 1;
                break;
            case "EnemyE":
                enemyLife = 50000*stage;
                speed = 1.9f + (stage*0.1f);
                power = 100*stage;
                Invoke("Think", 5);
                break;
        }
        enemyMaxLife = enemyLife;
    }
    void Update()
    {
        //보스라면 체력을 표기해라
        if(enemyName == "EnemyE" && gameObject.activeSelf){
            gameManager.bossHealth.fillAmount =  enemyLife / enemyMaxLife;
            Move();
            EnemyState();
            return;
        }

        if(enemyName=="Box")
            return;

        if(enemyLife<=0)
            return;

        if(isNuckBack)
            return;

        Move();
        Fire();
        Reload();

        //상태이상
        EnemyState();
    }

    void Move()
    {
        Vector2 direction = new Vector2(
            transform.position.x - player.transform.position.x,
            transform.position.y - player.transform.position.y
        );
        //플레이어를 따라가는 좌표
        dir = direction;
        
        //플레이어 위치에 따른 적군 좌 우 이미지 반전
        if(player.transform.position.x > transform.position.x) {
            spriteRenderer.flipX = false;
        } else {
            spriteRenderer.flipX = true;
        }
        //이동 함수(이 함수를 넣음으로서 플레이어에게 밀리지 않음)
        Vector2 nextVec = -dir.normalized * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec * iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
    }
    
    //적군이 사라지는 함수
    void DestroyEnemy()
    {
        gameObject.SetActive(false);;
    }
    
    //총알에 맞으면 총알의 파워만큼 데미지를 입는다.
    void OnTriggerEnter2D(Collider2D other) {
        if(enemyLife <= 0)
            return;

        if(other.gameObject.tag == "Bullet"){
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            //받은 속성 체크
            if(bullet.elementalType == "Wind"){ // 바람               
                IsWind(bullet);

            } else if(bullet.elementalType == "Fire"){ // 불 데미지
                if(enemyName=="Box"){
                    OnHit(transform.position, 1, 0, 0, bullet.elementalType);
                }
                OnHit(transform.position, bullet.power, 0, 0, bullet.elementalType);
                IsFrie(bullet);

            } else if(bullet.elementalType == "Stone") {// 바위 데미지
                OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);

                //보호막
                StoneShield();

            } else if(bullet.elementalType == "Ice"){ // 얼음 데미지
                IsIce(bullet);

            } else if(bullet.elementalType == "Water") {//물
                IsWater(bullet);

            } else if(bullet.elementalType == "Lightning"){
                IsLightning(bullet);

            } else if(bullet.weaponType == 15){ // 가시 갑옷
                if(bullet.armorKill && (enemyName != "EnemyD" || enemyName != "EnemyE")){ // 보스가 아닐경우 5% 확률로 즉사
                    int ran = Random.Range(0,100);
                    if(ran>94){
                        OnHit(transform.position, enemyMaxLife, bullet.nuckBack, bullet.nuckBackTime, "None");
                        gameManager.StateText(transform.position, "InDeath");
                        noneDamageTextTime = 0;
                    } else {
                        OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, "None");
                        noneDamageTextTime = 0;
                    }
                } else {
                    OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, "None");
                    noneDamageTextTime = 0;
                }
            } else { // 노멀데미지
                OnHit(other.transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, "None");
            }
            //닿으면 사라지는 총알은 사라져라
            if(bullet.weaponType > 2 && bullet.weaponType < 6 || bullet.weaponType == 11 || bullet.weaponType == 7){
                other.gameObject.SetActive(false);
            }
        } else if(other.gameObject.tag == "Effect"){ //과부화 폭발 데미지
            Effect effect = other.GetComponent<Effect>();
            if(effect.effectName == "Overload"){
                OnHit(transform.position, effect.power, 2, 0.2f, "Fire");
            }
        } else if (other.gameObject.tag == "BorderBullet"){
            if(enemyName != "EnemyE"){
                int ran = Random.Range(0,10);
                gameObject.transform.position = gameManager.spawnPoints[ran].position;
            }
        }
    }
    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Bullet"){
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if(bullet.elementalType == "Water") {// 물 데미지
                waterDamageTextTime += Time.deltaTime;
                if(waterDamageTextTime >= 1){
                    IsWater(bullet);
                    waterDamageTextTime = 0;
                }
            } else if (bullet.elementalType == "Wind") {// 바람 데미지
                windDamageTextTime += Time.deltaTime;
                if(windDamageTextTime >= 1){
                    IsWind(bullet);
                    windDamageTextTime = 0;
                }
            } else if (bullet.elementalType == "None"){
                noneDamageTextTime += Time.deltaTime;
                if(noneDamageTextTime >= 1){
                    if(bullet.weaponType == 15){ // 가시 갑옷
                        if(bullet.armorKill && (enemyName != "EnemyD" || enemyName != "EnemyE")){ // 보스가 아닐경우 5% 확률로 즉사
                            int ran = Random.Range(0,100);
                            if(ran>94){
                                OnHit(transform.position, enemyMaxLife, bullet.nuckBack, bullet.nuckBackTime, "None");
                                gameManager.StateText(transform.position, "InDeath");
                                noneDamageTextTime = 0;
                            } else {
                                OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, "None");
                                noneDamageTextTime = 0;
                            }
                        }
                    } else {
                        OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, "None");
                        noneDamageTextTime = 0;
                    }
                    
                }
            }
        }
    }
    //초전도 함수
    public void isSuperconductivity(float power, float nuckBack, float nuckBackTime, string type)
    {
        isLightning = false;
        lightningOffCount = 0;
        isIce = false;
        iceOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "Superconductivity");
        OnHit(transform.position, power * 1.5f, nuckBack, nuckBackTime, type);
    }
    //과부화 함수
    public void isOverload(string type)
    {
        GameObject overload = objectManager.MakeObj("Overload");
        Effect overloadLogic = overload.GetComponent<Effect>();
        overload.gameObject.transform.position = transform.position;
        overloadLogic.power = lightningDamage + fireDamage;
        isLightning = false;
        lightningOffCount = 0;
        isFire = false;
        fireOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "Overload");
    }
    //융해 함수
    void isMelting(float power, float nuckBack, float nuckBackTime, string type)
    {
        isIce = false;
        iceOffCount = 0;
        isFire = false;
        fireOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "Melting");
        OnHit(transform.position, power * 1.5f, nuckBack, nuckBackTime, type);
    }
    //빙결 함수
    void isFreezing()
    {
        isIce = false;
        iceOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "Freezing");
        freezingOffCount = 3f;
        isFreezingOn=true;
    }
    void EnemyFreezing()
    {
        if(isFreezingOn){
            if(freezingOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                freezStop = 1;
                isFreezingOn = false;
                return;
            }

            freezingOffCount -= Time.deltaTime;
            freezStop = 0;
            spriteRenderer.color = new Color(0.2f,0.5f,0.8f);
        }
    }
    //증발 함수
    void isEvaporation(float power, float nuckBack, float nuckBackTime, string type)
    {
        isFire = false;
        fireOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "Evaporation");
        OnHit(transform.position, power * 1.5f, nuckBack, nuckBackTime, type);
    }

    //감전 함수
    public void isElectricShock()
    {
        isWater = false;
        waterOffCount = 0;
        isLightning = false;
        lightningOffCount = 0;
        Vector3 plusPos = new Vector3(0,0.1f,0);
        gameManager.StateText(transform.position, "ElectricShock");
        ElectricShockOffCount = 3f;
        ElectricShockOn = true;
    }
    void EnemyElectricShock()
    {
        if(ElectricShockOn){
            if(ElectricShockOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                lightningStop = 1;
                ElectricShockOn=false;
                return;
            }
            
            spriteRenderer.color = new Color(0.5f,0,1);
            ElectricShockOffCount -= Time.deltaTime;
            ElectricShockDamageTextTime += Time.deltaTime;
            lightningStop = 1;

            if(ElectricShockDamageTextTime >=0.9f){
                lightningStop = 0;
            }
            if(ElectricShockDamageTextTime >= 1){
                OnHit(transform.position, lightningDamage, 0, 0, "Lightning");
                ElectricShockDamageTextTime = 0;
            }
        }
    }

    //불에 붙은 경우 초당 불 데미지를 입는다.
    void EnemyFireDamaged()
    {
        if(isFire){
            if(fireOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                isFire = false;
                return;
            }

            spriteRenderer.color = new Color(1,0,0);
            fireOffCount -= Time.deltaTime;
            fireDamageTextTime += Time.deltaTime;

            if(fireDamageTextTime >= 1){
                OnHit(transform.position, fireDamage, 0, 0, "Fire");
                fireDamageTextTime = 0;
            }
        }
        
    }
    //얼음에 맞을 경우 이동속도가 느려진다.
    void EnemyIceSlow()
    {
        if(isIce){
            if(iceOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                iceSlow = 1;
                isIce = false;
                return;
            }

            spriteRenderer.color = new Color(0,0,1);
            iceSlow = 0.5f;
            iceOffCount -= Time.deltaTime;
        }
    }
    //전기에 맞을 경우 초당 50% 데미지가 들어간다.
    void EnemeyLightning()
    {
        if(isLightning){
            if(lightningOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                isLightning = false;
                return;
            }
            
            spriteRenderer.color = new Color(0.5f,1,1);
            lightningOffCount -= Time.deltaTime;
            lightingDamageTextTime += Time.deltaTime;

            if(lightingDamageTextTime >= 1){
                OnHit(transform.position, lightningDamage * 0.5f, 0, 0, "Lightning");
                lightingDamageTextTime = 0;
            }
        }
    }
    //물에 젖을 경우(색상변화)
    void EnemyWater()
    {
        if(isWater){
            if(waterOffCount<=0){
                spriteRenderer.color = new Color(1,1,1);
                isWater = false;
                return;
            }
        
        spriteRenderer.color = new Color(0.3f,0.7f,0.8f);
        waterOffCount -= Time.deltaTime;
        }
    }
    //총알의 맞은 경우
    public void OnHit(Vector3 pos,float dmg, float nuckBack, float nuckBackTime, string type)
    {
        if(type == "Fire" || type == "None" || type == "Lightning" || type == "Stone"){
            freezingOffCount = 0;
        }

        dmg = playerLogic.CriticalHit(dmg);
        enemyLife -= dmg;

        //맞은 데미지 출력
        gameManager.DamageText(pos, dmg, type, playerLogic.isCritical);

        if(enemyName!="Box"){
            anim.SetTrigger("Hit");
        }

        if(enemyLife <= 0){
            gameObject.layer = 10;
            if(enemyName!="Box"){
                rigid.velocity = Vector2.zero;
                playerLogic.enemyClearNum++;
                gameManager.enemyCount--;
                anim.SetBool("Dead", true);
                Invoke("DestroyEnemy", 0.5f);
            } else if(enemyName=="Box"){
                DestroyEnemy();
            }
            ItemDrop(enemyName);
            //보스를 잡았을 경우
            if(enemyName == "EnemyE"){
                CancelInvoke("FireShot");
                CancelInvoke("FireAround");
                CancelInvoke("FireArc");
                gameManager.camAnim.SetBool("IsOn",false);
                gameManager.bossHealthBar.SetActive(false);
                gameManager.StageEnd();
            }
        } else {
            if(enemyName == "EnemyE")
                return;

            //넉백
            isNuckBack = true;
            if(player.transform.position.x > transform.position.x && player.transform.position.y < transform.position.y){// 플레이어보다 왼쪽 위에 있을 경우
                rigid.AddForce(Vector2.up * nuckBack, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.left * nuckBack, ForceMode2D.Impulse);
            } else if (player.transform.position.x == transform.position.x && player.transform.position.y < transform.position.y){// 플레이 바로 위에 있을 경우
                rigid.AddForce(Vector2.up * nuckBack, ForceMode2D.Impulse);
            } else if(player.transform.position.x < transform.position.x && player.transform.position.y < transform.position.y){// 플레이어보다 오른쪽 위에 있을 경우
                rigid.AddForce(Vector2.up * nuckBack, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.right * nuckBack, ForceMode2D.Impulse);
            } else if(player.transform.position.x > transform.position.x && player.transform.position.y == transform.position.y){// 플레이어 바로 왼쪽에 있을 경우
                rigid.AddForce(Vector2.left * nuckBack, ForceMode2D.Impulse);
            } else if (player.transform.position.x < transform.position.x && player.transform.position.y == transform.position.y){// 플레이 바로 오른쪽에 있을 경우
                rigid.AddForce(Vector2.right * nuckBack, ForceMode2D.Impulse);
            } else if(player.transform.position.x > transform.position.x && player.transform.position.y > transform.position.y){// 플레이어보다 왼쪽 아래에 있을 경우
                rigid.AddForce(Vector2.down * nuckBack, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.left * nuckBack, ForceMode2D.Impulse);
            } else if(player.transform.position.x == transform.position.x && player.transform.position.y > transform.position.y){// 플레이어바로 아래에 있을 경우
                rigid.AddForce(Vector2.down * nuckBack, ForceMode2D.Impulse);
            } else if(player.transform.position.x < transform.position.x && player.transform.position.y > transform.position.y){// 플레이어보다 오른쪽 아래에 있을 경우
                rigid.AddForce(Vector2.down * nuckBack, ForceMode2D.Impulse);
                rigid.AddForce(Vector2.right * nuckBack, ForceMode2D.Impulse);
            }
            Invoke("NuckBackOff",nuckBackTime);
        }
    }

    void NuckBackOff()
    {
        isNuckBack = false;
    }

    //아이템 드롭
    void ItemDrop(string type)
    {
        if(isDrop)
            return;

        isDrop=true;
        switch(type){
            case "EnemyA":
                GameObject itemExp0 = objectManager.MakeObj("ItemExp0");
                itemExp0.transform.position = transform.position;
                break;
            case "EnemyB":
                GameObject itemExp1 = objectManager.MakeObj("ItemExp1");
                itemExp1.transform.position = transform.position;
                break;
            case "EnemyC":
                GameObject itemExp2 = objectManager.MakeObj("ItemExp2");
                itemExp2.transform.position = transform.position;
                break;
            case "EnemyD":
                //체력 회복
                GameObject itemHealth = objectManager.MakeObj("ItemHealth");
                itemHealth.transform.position = transform.position;
                break;
            case "EnemyE":
                //다른 아이템을 드랍해야 함 (일단 코인 10개)
                for(int i=0;i<5;i++){
                    Vector3 ranVec = new Vector3(Random.Range(-2.0f,2.0f)+transform.position.x,Random.Range(-2.0f,2.0f)+transform.position.y,0);
                    GameObject coin1B = objectManager.MakeObj("ItemCoin1");
                    coin1B.transform.position = ranVec;
                    GameObject coin2B = objectManager.MakeObj("ItemCoin2");
                    coin2B.transform.position = ranVec;
                }
                break;
            case "Box":
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
            break;
        }
    }

    //적군 스탯 초기화 함수
    void enemyStateClear()
    {
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
        spriteRenderer.color = new Color(1,1,1);
    }
    //폭탄의 경우
    public void OnHit(float dmg)
    {
        enemyLife -= dmg;
        anim.SetTrigger("Hit");

        if(enemyLife <= 0){
            gameObject.layer = 10;
            rigid.velocity = Vector2.zero;
            playerLogic.enemyClearNum++;
            gameManager.enemyCount--;
            anim.SetBool("Dead", true);
            ItemDrop(enemyName);
            Invoke("DestroyEnemy", 0.5f);
            
            //보스를 잡았을 경우
            if(enemyName == "EnemyE"){
                CancelInvoke("FireShot");
                CancelInvoke("FireAround");
                CancelInvoke("FireArc");
                gameManager.camAnim.SetBool("IsOn",false);
                gameManager.bossHealthBar.SetActive(false);
                gameManager.StageEnd();
            }
        }
    }

    //적군미사일이 발사되는 함수
    void Fire()
    {
        if(curShootDelay < maxShootDelay){
            return;
        }
        
        if(enemyName == "EnemyB"){
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        } else if(enemyName == "EnemyC"){
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        }

        //재장전
        curShootDelay = 0;
    }

    void Reload()
    {
        curShootDelay += Time.deltaTime;
    }
    void EnemyState()
    {
        EnemyFireDamaged();
        EnemyIceSlow();
        EnemeyLightning();
        EnemyWater();
        EnemyFreezing();
        EnemyElectricShock();
    }

    //보스 패턴 함수
    void Think()
    {
        if(!gameObject.activeSelf)
            return;

        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch(patternIndex){
            case 0:
                FireShot();
                break;
            case 1:
                FireArc();
                break;
            case 2:
                FireAround();
                break;
            case 3:
                BreakTime();
                break;
        }
    }
    void FireShot()
    {
        if (enemyLife <= 0) return;

        //플레이어 방향으로 샷건
        for(int index = 0;index < 5;index++){
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-2f,2f), Random.Range(-2f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        }

        //패턴 진행 수
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex]){
            Invoke("FireShot", 1.2f);
        } else {
            Invoke("Think", 3f);
        }
    }
    void FireArc()
    {
        if (enemyLife <= 0) return;

        //부채모양으로 발사
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]),Mathf.Sin(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]));
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        
        //패턴 진행 수
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex]){
            Invoke("FireArc", 0.1f);
        } else {
            Invoke("Think", 3f);
        }
    }
    void FireAround()
    {
        if (enemyLife <= 0) return;

        //원 형태로 전체 공격
        int roundNumA = 30;
        int roundNumB = 20;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
        for(int index=0;index<roundNumA;index++){
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf. Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            //총알 회전 로직
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        //패턴 진행 수
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex]){
            Invoke("FireAround", 2f);
        } else {
            Invoke("Think", 3f);
        }
    }
    // 네번째 패턴
    void BreakTime()
    {
        if (enemyLife <= 0) return;

        //패턴 진행 수
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex]){
            Invoke("BreakTime", 1f);
        } else {
            Invoke("Think", 3f);
        }
    }
    public void IsFrie(Bullet bullet)
    {
        if(isFire){
            fireOffCount = 3f;
                    
            if(isIce){//융해
                isMelting(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            } else if(isLightning){//과부화
                lightningDamage = bullet.power;
                isOverload(bullet.elementalType);
            } else if(isWater){// 증발
                isEvaporation(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            }
            return;
        }

        //불 붙는 함수
        isFire = true;
        fireOffCount = 3f;
        fireDamage = bullet.power;
        if(isIce){// 융해
            isMelting(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        } else if(isLightning){//과부화
            fireDamage = bullet.power;
            isOverload(bullet.elementalType);
        } else if(isWater){// 증발
            isEvaporation(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        }
    }
    public void IsIce(Bullet bullet)
    {
        OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        if(isIce){
            iceOffCount = 3f;
            if(isFire){// 융해
                isMelting(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            } else if(isLightning){// 초전도
                isSuperconductivity(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            } else if(isWater) {// 프리즈
                isFreezing();
            }
        } else {
            // 얼음 이동속도 감소
            isIce = true;
            iceDamage = bullet.power;
            iceOffCount = 3f;
            if(isFire){// 융해
                isMelting(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            } else if(isLightning){//초전도
                isSuperconductivity(bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            } else if(isWater) {// 프리즈
                isFreezing();
            }
        }
    }
    public void IsLightning(Bullet bullet)
    {
        lightningOffCount = 3f;
        isLightning = true;
        lightningDamage = bullet.power;
        OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        if(isFire){ // 불이 붙었다면 과부화
            isOverload("Overload");
        } else if (isIce){ // 얼음이 붙었다면 초전도
            isSuperconductivity(bullet.power, bullet.nuckBack, bullet.nuckBackTime, "Superconductivity");
        } else if (isWater){ // 물이 묻었다면 감전
            isElectricShock();
        }
    }
    public void IsWind(Bullet bullet)
    {
        if(isFire){//불이 붙어 있다면
            Vector3 plusPos = new Vector3(0,0.1f,0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            OnHit(transform.position + plusPos, fireDamage, 0, 0, "Fire");
        } else if(isIce) {//얼음이 뭍어 있다면
            Vector3 plusPos = new Vector3(0,0.1f,0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            OnHit(transform.position + plusPos, iceDamage, 0, 0, "Ice");
        } else if(isLightning) {//전기가 뭍어 있다면
            Vector3 plusPos = new Vector3(0,0.1f,0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            OnHit(transform.position + plusPos, lightningDamage, 0, 0, "Lightning");
        } else if(isWater) {//물이 붙어 있다면
            Vector3 plusPos = new Vector3(0,0.1f,0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
            OnHit(transform.position + plusPos, waterDamage, 0, 0, "Water");
        } else {
            OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        }
    }
    public void IsWater(Bullet bullet)
    {
        OnHit(transform.position, bullet.power, bullet.nuckBack, bullet.nuckBackTime, bullet.elementalType);
        isWater = true;
        waterOffCount = 3f;
        if(isIce){
            isFreezing();
        } else if(isLightning){
            isElectricShock();
        } else if(isFire){
            isEvaporation(bullet.power,bullet.nuckBack,bullet.nuckBackTime,bullet.elementalType);
        }
    }
    void StoneShield()
    {
        int ran = Random.Range(0,100);
        if(ran<5){//5퍼센트 확률로 보호막 생성
            GameObject itemShield = objectManager.MakeObj("ItemShield");
            Vector3 ranVec = new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),0);
            itemShield.transform.position = transform.position + ranVec;
        }
    }
}
