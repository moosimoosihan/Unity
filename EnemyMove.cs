using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    AudioSource audioSource;
    public RuntimeAnimatorController[] animCon;
    public string elementalType; // 적군 데미지 타입
    public bool isCritical; // 적군 크리티컬 여부
    public int criticalChance; // 크리티컬 확률
    public float criticalDamage = 1.5f; // 크리티컬 데미지

    bool enemyDead;

    //적군 총알 함수
    public string enemyName;
    public float maxShootDelay;
    public float curShootDelay;
    public float bulletSpeed;
    public GameObject player;
    public Rigidbody2D playerRigid;

    public GameManager gameManager;

    SpriteRenderer spriteRenderer;
    public PlayerMove playerLogic;

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
    bool isAttack;
    Vector3 playerVec;
    public Vector3 attackVec;
    int bossAttackCount;
    Vector3 attackPos;
    bool isAttack4;

    //소환몹 데미지
    float playerObjDamageCount;

    bool isDrop; //아이템 한번만 떨어지는 함수

    void Awake()
    {
        //초기화
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stage = GameManager.instance.stage;
    }
    void OnEnable()
    {
        if(enemyName == "Box"){
            enemyLife = 1;
        }
        enemyStateClear();
    }
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        power = data.power;
        elementalType = data.elementalType;
        criticalChance = data.criticalChance;
        criticalDamage = data.criticalDamage;
        enemyName = data.enemyName;
        enemyLife = data.enemyLife;
        maxShootDelay = data.maxShootDelay;
        bulletSpeed = data.bulletSpeed;
        switch (enemyName) {
            case "EnemyA":
                transform.localScale = new Vector3(0.4f,0.4f,0);
                enemyLife += ((stage - 1) * 100f);
                speed = 1f;
                power += ((stage - 1) * 10f);
                break;
            case "EnemyB":
                transform.localScale = new Vector3(0.4f,0.4f,0);
                enemyLife += ((stage - 1) * 100f);
                speed = 1f;
                power += ((stage - 1) * 10f);
                break;
            case "EnemyC":
                transform.localScale = new Vector3(0.4f,0.4f,0);
                enemyLife += ((stage - 1) * 100f);
                speed = 1.5f;
                power += ((stage - 1) * 10f);
                break;
            case "EnemyD":
                enemyLife += ((stage - 1) * 5000f);
                speed = 1.5f;
                power += ((stage - 1) * 10f);
                curPatternCount = -1;
                maxPatternCount = new int[4];
                maxPatternCount[0] = 5;
                maxPatternCount[1] = 49;
                maxPatternCount[2] = 5;
                maxPatternCount[3] = 5;
                Invoke("Think", 5f);
                break;
            case "EnemyE":
                enemyLife += ((stage - 1) * 10000f);
                speed = 1.6f;
                power += ((stage - 1) * 10f);
                Invoke("Check", 5f);

                break;
        }
        enemyMaxLife = enemyLife;
    }
    void Update()
    {
        //보스라면 체력을 표기해라
        if ((enemyName == "EnemyE") && gameObject.activeSelf) {
            gameManager.spawner.bossHealth.fillAmount = enemyLife / enemyMaxLife;
        } else if (enemyName == "EnemyD" && gameObject.activeSelf) {
            gameManager.spawner.bossHealth.fillAmount = enemyLife / enemyMaxLife;
        }

        if (enemyName == "Box")
            return;

        if (enemyLife <= 0)
            return;

        Fire();
        Reload();

        //상태이상
        EnemyState();
    }
    void FixedUpdate()
    {
        if (enemyName == "Box")
            return;

        if (enemyLife <= 0)
            return;

        if(enemyName == "EnemyE" && isAttack4)
        {
            BossAttack4Move();
        }

        Move();
    }
    void LateUpdate()
    {
        if (enemyName == "Box")
            return;

        if (enemyLife <= 0)
            return;

        //플레이어 위치에 따른 적군 좌 우 이미지 반전
        spriteRenderer.flipX = playerRigid.position.x < rigid.position.x;
    }

    void Move()
    {
        if (enemyName == "EnemyE") { // 보스 공격중이면 멈추기
            if (isAttack)
            {
                rigid.velocity = Vector2.zero;
                return;
            }
        }
        //플레이어를 따라가는 좌표
        Vector2 dirVec = playerRigid.position - rigid.position;

        //이동 함수(이 함수를 넣음으로서 플레이어에게 밀리지 않음)
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec * iceSlow * freezStop * lightningStop); // 얼음 공격시 슬로우, 빙결, 쇼크시 멈춤
        rigid.velocity = Vector2.zero;
    }
    void BossAttack4Move()
    {
        if(enemyName == "EnemyE")
        {
            if (isAttack4)
            {                
                rigid.MovePosition(rigid.position + (Vector2.down * 5 *Time.fixedDeltaTime));
                if (transform.position.y <= attackPos.y)
                {
                    gameObject.layer = 8;
                    isAttack4 = false;
                    isAttack = false;
                    Invoke("Check", 1f);
                    //원 형태로 전체 공격
                    int roundNumA = 6;
                    int roundNum = roundNumA;
                    for (int index = 0; index < roundNumA; index++) {
                        GameObject bullet = objectManager.Get(52);
                        bullet.transform.position = transform.position;
                        bullet.transform.rotation = Quaternion.identity;
                        Bullet bulletLogic = bullet.GetComponent<Bullet>();
                        bulletLogic.power = power;

                        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
                        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

                        //총알 회전 로직
                        Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
                        bullet.transform.Rotate(rotVec);
                    }
                    rigid.velocity = Vector2.zero;
                    bossAttackCount++;
                }
            }
        }
    }

    //적군이 사라지는 함수
    void DestroyEnemy()
    {
        if (enemyName != "Box") {
            gameManager.enemyCount--;
        }
        gameObject.SetActive(false); ;
    }

    //총알에 맞으면 총알의 파워만큼 데미지를 입는다.
    void OnTriggerEnter2D(Collider2D other) {
        if (enemyLife <= 0)
            return;

        if (other.gameObject.tag == "Bullet") {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            //받은 속성 체크
            if (bullet.elementalType == "Wind") { // 바람               
                IsWind(bullet);

            } else if (bullet.elementalType == "Fire") { // 불 데미지
                if (enemyName == "Box") {
                    OnHit(transform.position, 1, bullet.elementalType);
                }
                OnHit(transform.position, bullet.power, bullet.elementalType);
                IsFire(bullet);

            } else if (bullet.elementalType == "Stone") {// 바위 데미지
                OnHit(transform.position, bullet.power, bullet.elementalType);

                //보호막
                StoneShield();

            } else if (bullet.elementalType == "Ice") { // 얼음 데미지
                IsIce(bullet);
            } else if (bullet.elementalType == "Water") {//물
                IsWater(bullet);
                if (bullet.weaponType == 22) { // 2파동의 경우 2번의 데미지
                    Vector2 pos = new Vector2(transform.position.x + 0.1f, transform.position.y + 0.1f);
                    if (bullet.maxLevel) {
                        bullet.elementalType = "Ice";
                        OnHit(pos, bullet.power, bullet.elementalType);
                        if (isIce) {
                            iceOffCount = 3f;
                            if (isFire) {// 융해
                                isMelting(bullet.power, bullet.elementalType);
                            } else if (isLightning) {// 초전도
                                isSuperconductivity(bullet.power, bullet.elementalType);
                            } else if (isWater) {// 프리즈
                                isFreezing();
                            }
                        } else {
                            // 얼음 이동속도 감소
                            isIce = true;
                            iceDamage = bullet.power;
                            iceOffCount = 3f;
                            if (isFire) {// 융해
                                isMelting(bullet.power, bullet.elementalType);
                            } else if (isLightning) {//초전도
                                isSuperconductivity(bullet.power, bullet.elementalType);
                            } else if (isWater) {// 프리즈
                                isFreezing();
                            }
                        }
                    } else {
                        OnHit(pos, bullet.power, "None");
                    }
                }
            } else if (bullet.elementalType == "Lightning") {
                IsLightning(bullet);

            } else if (bullet.weaponType == 15) { // 가시 갑옷
                if (bullet.armorKill) { // 보스가 아닐경우 5% 확률로 즉사
                    if (enemyName != "EnemyD" && enemyName != "EnemyE") {
                        int ran = Random.Range(0, 100);
                        if (ran < 5) {
                            OnHit(transform.position, enemyMaxLife, "None");
                            gameManager.StateText(transform.position, "InDeath");
                        } else {
                            OnHit(transform.position, bullet.power, "None");
                        }
                    } else {
                        OnHit(transform.position, bullet.power, "None");
                    }
                } else {
                    OnHit(transform.position, bullet.power, "None");
                }
            } else if (bullet.elementalType == "Light") { // 빛 데미지
                OnHit(transform.position, bullet.power, "Light");
                playerLogic.Healing(bullet.power / 100);
            } else if (bullet.elementalType == "Poison") { // 독 데미지
                poisonCount = 3f;
                poisonDamage = bullet.power;
                OnHit(transform.position, bullet.power, "Poison");
                isPoison = true;
            } else { // 노멀데미지                    
                OnHit(other.transform.position, bullet.power, "None");
            }
        } else if (other.gameObject.tag == "Effect") { //과부화 폭발 데미지
            Effect effect = other.GetComponent<Effect>();
            if (effect.effectName == "Overload") {
                OnHit(transform.position, effect.power, effect.elementalType);
            }
        } else if (other.gameObject.tag == "BorderBullet") {
            if (enemyName != "Box") {
                Vector3 playerPos = GameManager.instance.playerMove.transform.position;
                Vector3 myPos = transform.position;
                Vector3 playerDir = GameManager.instance.playerMove.inputVec;
                transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0));
            } else {
                int ran = Random.Range(0, 20);
                gameObject.transform.position = gameManager.spawner.spawnPoints[ran].position;
            }
        }
    }
    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Bullet") {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.elementalType == "Water") {// 물 데미지
                waterDamageTextTime += Time.deltaTime;
                if (waterDamageTextTime >= 1) {
                    IsWater(bullet);
                    waterDamageTextTime = 0;
                }
            } else if (bullet.elementalType == "Wind") {// 바람 데미지
                windDamageTextTime += Time.deltaTime;
                if (windDamageTextTime >= 1) {
                    IsWind(bullet);
                    windDamageTextTime = 0;
                }
            } else if (bullet.elementalType == "None") {
                if (bullet.weaponType == 15) { // 가시 갑옷
                    noneDamageTextTime += Time.deltaTime;
                    if (noneDamageTextTime >= 1) {
                        if (bullet.armorKill) { // 보스가 아닐경우 5% 확률로 즉사
                            if (enemyName != "EnemyD" && enemyName != "EnemyE") {
                                int ran = Random.Range(0, 100);
                                if (ran < 5) {
                                    OnHit(transform.position, enemyMaxLife, "None");
                                    gameManager.StateText(transform.position, "InDeath");
                                    noneDamageTextTime = 0;
                                } else {
                                    OnHit(transform.position, bullet.power, "None");
                                    noneDamageTextTime = 0;
                                }
                            } else {
                                OnHit(transform.position, bullet.power, "None");
                                noneDamageTextTime = 0;
                            }
                        } else {
                            OnHit(transform.position, bullet.power, "None");
                            noneDamageTextTime = 0;
                        }
                    }
                }
            } else if (bullet.elementalType == "Light") { // 빛 데미지
                lightDamageTextTime += Time.deltaTime;
                if (lightDamageTextTime >= 1) {
                    OnHit(transform.position, bullet.power, "Light");
                    lightDamageTextTime = 0;
                    playerLogic.Healing(bullet.power / 100);
                }
            } else if (bullet.elementalType == "Poison") { // 독 데미지
                poisonCount = 3f;
                poisonDamage = bullet.power;
                isPoison = true;
            }
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        // 소환물에 닿아있다면 데미지가 들어가라
        if (other.gameObject.tag == "PlayerObj") {
            PlayerObj playerObj = other.gameObject.GetComponent<PlayerObj>();
            if (playerObjDamageCount >= 1) {
                OnHit(transform.position, playerObj.power, playerObj.elementalType);
                playerObjDamageCount = 0;
            } else {
                playerObjDamageCount += Time.deltaTime;
            }
        }
    }
    //초전도 함수
    public void isSuperconductivity(float power, string type)
    {
        isLightning = false;
        lightningOffCount = 0;
        isIce = false;
        iceOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Superconductivity");
        OnHit(transform.position, power * 1.5f, type);
    }
    //과부화 함수
    public void isOverload(string type)
    {
        GameObject overload = objectManager.Get(54);
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
    void isMelting(float power, string type)
    {
        isIce = false;
        iceOffCount = 0;
        isFire = false;
        fireOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Melting");
        OnHit(transform.position, power * 1.5f, type);
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
    void EnemyFreezing()
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
    void isEvaporation(float power, string type)
    {
        isFire = false;
        fireOffCount = 0;
        isWater = false;
        waterOffCount = 0;
        Vector3 plusPos = new Vector3(0, 0.1f, 0);
        gameManager.StateText(transform.position, "Evaporation");
        OnHit(transform.position, power * 1.5f, type);
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
    void EnemyElectricShock()
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
                OnHit(transform.position, lightningDamage, "Lightning");
                ElectricShockDamageTextTime = 0;
            }
        }
    }

    //불에 붙은 경우 초당 불 데미지를 입는다.
    void EnemyFireDamaged()
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
                OnHit(transform.position, fireDamage, "Fire");
                fireDamageTextTime = 0;
            }
        }
    }
    // 독에 맞은경우 초당 독 데미지
    void EnemyPoisonDamaged()
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
                OnHit(transform.position, poisonDamage, "Poison");
                poisonDamageTextTime = 0;
            }
        }
    }
    //얼음에 맞을 경우 이동속도가 느려진다.
    void EnemyIceSlow()
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
    void EnemeyLightning()
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
                OnHit(transform.position, lightningDamage * 0.5f, "Lightning");
                lightingDamageTextTime = 0;
            }
        }
    }
    //물에 젖을 경우(색상변화)
    void EnemyWater()
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
    //총알의 맞은 경우
    public void OnHit(Vector3 pos, float dmg, string type)
    {
        if (enemyDead)
            return;

        //오디오 출력 ( 바람, 불, 전기, 물, 얼음, 독, 빛 마땅히 넣을 소스가 없어 물리와 같음 )
        if(type == "None"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Stone"){
            audioSource.clip = gameManager.audioManager.hit1Auido;
        } else if(type == "Fire"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Lightning"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Water"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Ice"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Poison"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Light"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        } else if(type == "Wind"){
            audioSource.clip = gameManager.audioManager.hit0Auido;
        }
        if(!audioSource.isPlaying){
            gameManager.audioManager.PlayOneShotSound(audioSource, audioSource.clip, audioSource.volume);
        }
        if (type == "Fire" || type == "None" || type == "Lightning" || type == "Stone") {
            freezingOffCount = 0;
        }

        dmg = playerLogic.CriticalHit(dmg);
        enemyLife -= dmg;

        //맞은 데미지 출력
        gameManager.DamageText(pos, dmg, type, playerLogic.isCritical);

        if (enemyName != "Box") {
            anim.SetTrigger("Hit");
        }

        if (enemyLife <= 0) {
            enemyDead = true;
            audioSource.clip = gameManager.audioManager.deadAuido;
            gameManager.audioManager.PlayOneShotSound(audioSource, audioSource.clip, audioSource.volume);
            if (enemyName == "Box") {
                gameObject.layer = 10;
                DestroyEnemy();
            } else {
                playerLogic.enemyClearNum++;
                EnemyDead();
            }
            ItemDrop(enemyName);
            //보스를 잡았을 경우
            if (enemyName == "EnemyD") {
                if (!gameManager.spawner.boss1Clear) {
                    gameManager.spawner.boss1Clear = true;
                } else {
                    gameManager.spawner.boss2Clear = true;
                }
                CancelInvoke("FireShot");
                CancelInvoke("FireAround");
                CancelInvoke("FireArc");
                // gameManager.camAnim.SetBool("IsOn", false);
                gameManager.spawner.bossSquare.SetActive(false);
                gameManager.spawner.bossHealthBar.SetActive(false);
                gameManager.cineCam.Follow = player.transform;
                gameManager.spawner.isBoss = false;
            }
            if (enemyName == "EnemyE") {
                CancelInvoke("Check");
                // gameManager.camAnim.SetBool("IsOn", false);
                gameManager.spawner.bossSquare.SetActive(false);
                gameManager.spawner.bossHealthBar.SetActive(false);
                gameManager.cineCam.Follow = player.transform;
                gameManager.spawner.bossClear = true;
                gameManager.StageEnd();
            }
        }
    }
    //아이템 드롭
    void ItemDrop(string type)
    {
        if (isDrop)
            return;

        isDrop = true;
        switch (type) {
            case "EnemyA":
                GameObject itemExp0 = objectManager.Get(1);
                itemExp0.transform.position = transform.position;
                break;
            case "EnemyB":
                GameObject itemExp1 = objectManager.Get(2);
                itemExp1.transform.position = transform.position;
                break;
            case "EnemyC":
                GameObject itemExp2 = objectManager.Get(3);
                itemExp2.transform.position = transform.position;
                break;
            case "EnemyD":
                //체력 회복
                GameObject itemHealth = objectManager.Get(4);
                itemHealth.transform.position = transform.position;
                GameObject itemMag = objectManager.Get(5);
                itemMag.transform.position = new Vector2(transform.position.x+0.5f,transform.position.y);
                break;
            case "EnemyE":
                //다른 아이템을 드랍해야 함 (일단 코인 10개)
                for (int i = 0; i < 5; i++) {
                    Vector3 ranVec = new Vector3(Random.Range(-2.0f, 2.0f) + transform.position.x, Random.Range(-2.0f, 2.0f) + transform.position.y, 0);
                    GameObject coin1B = objectManager.Get(8);
                    coin1B.transform.position = ranVec;
                    GameObject coin2B = objectManager.Get(9);
                    coin2B.transform.position = ranVec;
                }
                break;
            case "Box":
                int ran = Random.Range(0, 10);
                if (ran < 4) {//40% 확률로 코인0
                    GameObject coin0 = objectManager.Get(7);
                    coin0.transform.position = transform.position;
                } else if (ran < 6) { // 20% 확률로 코인1
                    GameObject coin1 = objectManager.Get(8);
                    coin1.transform.position = transform.position;
                } else if (ran < 7) { // 10%확률로 코인2
                    GameObject coin2 = objectManager.Get(9);
                    coin2.transform.position = transform.position;
                } else if (ran < 8) { //10% 확률로 폭탄
                    GameObject boom = objectManager.Get(6);
                    boom.transform.position = transform.position;
                } else if (ran < 9) { //10% 확률로 체력
                    GameObject health = objectManager.Get(4);
                    health.transform.position = transform.position;
                } else if (ran < 10) { // 10% 확률로 자석
                    GameObject mag = objectManager.Get(5);
                    mag.transform.position = transform.position;
                }
                break;
        }
    }

    //적군 스탯 초기화 함수
    void enemyStateClear()
    {
        gameObject.layer = 8;
        isDrop = false;
        enemyDead = false;
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
    //폭탄의 경우
    public void OnHit(float dmg)
    {
        if (enemyDead)
            return;

        enemyLife -= dmg;
        anim.SetTrigger("Hit");

        if (enemyLife <= 0) {
            enemyDead = true;
            playerLogic.enemyClearNum++;
            EnemyDead();
            ItemDrop(enemyName);
            //보스를 잡았을 경우
            if (enemyName == "EnemyD")
            {
                if (!gameManager.spawner.boss1Clear)
                {
                    gameManager.spawner.boss1Clear = true;
                }
                else
                {
                    gameManager.spawner.boss2Clear = true;
                }
                CancelInvoke("FireShot");
                CancelInvoke("FireAround");
                CancelInvoke("FireArc");
                // gameManager.camAnim.SetBool("IsOn", false);
                gameManager.spawner.bossSquare.SetActive(false);
                gameManager.spawner.bossHealthBar.SetActive(false);
                gameManager.cineCam.Follow = player.transform;
                gameManager.spawner.isBoss = false;
                transform.localScale = new Vector3(1,1,0);
            }
                //보스를 잡았을 경우
                if (enemyName == "EnemyE") {
                CancelInvoke("Check");
                // gameManager.camAnim.SetBool("IsOn", false);
                gameManager.spawner.bossSquare.SetActive(false);
                gameManager.spawner.bossHealthBar.SetActive(false);
                gameManager.cineCam.Follow = player.transform;
                gameManager.StageEnd();
            }
        }
    }
    public void EnemyDead()
    {
        gameObject.layer = 10;
        rigid.velocity = Vector2.zero;
        anim.SetBool("Dead", true);
        Invoke("DestroyEnemy", 0.5f);
    }

    //적군미사일이 발사되는 함수
    void Fire()
    {
        if (curShootDelay < maxShootDelay) {
            return;
        }

        if (enemyName == "EnemyB") {
            GameObject bullet = objectManager.Get(47);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            float dmg = CriticalHit(power);
            bulletLogic.power = dmg;
            bulletLogic.enemyCritical = isCritical;
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        } else if (enemyName == "EnemyC") {
            GameObject bullet = objectManager.Get(48);
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
        EnemyPoisonDamaged();
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
        if (!gameObject.activeSelf)
            return;

        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex) {
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
        for (int index = 0; index < 5; index++) {
            GameObject bullet = objectManager.Get(47);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        }
        //패턴 진행 수
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex]) {
            Invoke("FireShot", 1.2f);
        } else {
            Invoke("Think", 3f);
        }
    }
    void FireArc()
    {
        if (enemyLife <= 0) return;

        //부채모양으로 발사
        GameObject bullet = objectManager.Get(47);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletLogic = bullet.GetComponent<Bullet>();
        bulletLogic.power = power;
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), Mathf.Sin(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]));
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

        //패턴 진행 수
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex]) {
            Invoke("FireArc", 0.1f);
        } else {
            Invoke("Think", 3f);
        }
    }
    void FireAround()
    {
        if (enemyLife <= 0) return;

        //원 형태로 전체 공격
        int roundNumA = 10;
        int roundNumB = 8;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
        for (int index = 0; index < roundNumA; index++) {
            GameObject bullet = objectManager.Get(48);
            Bullet bulletLogic = bullet.GetComponent<Bullet>();
            bulletLogic.power = power;
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            //총알 회전 로직
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        //패턴 진행 수
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex]) {
            Invoke("FireAround", 2f);
        } else {
            Invoke("Think", 3f);
        }
    }
    // 네번째 패턴
    void BreakTime()
    {
        if (enemyLife <= 0) return;

        speed = 0;

        //패턴 진행 수
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex]) {
            Invoke("BreakTime", 1f);
        } else {
            speed = 1.5f;
            Invoke("Think", 3f);
        }
    }
    public void IsFire(Bullet bullet)
    {
        if (isFire) {
            fireOffCount = 3f;

            if (isIce) {//융해
                isMelting(bullet.power, bullet.elementalType);
            } else if (isLightning) {//과부화
                lightningDamage = bullet.power;
                isOverload(bullet.elementalType);
            } else if (isWater) {// 증발
                isEvaporation(bullet.power, bullet.elementalType);
            }
            return;
        }

        //불 붙는 함수
        isFire = true;
        fireOffCount = 3f;
        fireDamage = bullet.power;
        if (isIce) {// 융해
            isMelting(bullet.power, bullet.elementalType);
        } else if (isLightning) {//과부화
            fireDamage = bullet.power;
            isOverload(bullet.elementalType);
        } else if (isWater) {// 증발
            isEvaporation(bullet.power, bullet.elementalType);
        }
    }
    public void IsIce(Bullet bullet)
    {
        OnHit(transform.position, bullet.power, bullet.elementalType);
        if (isIce) {
            iceOffCount = 3f;
            if (isFire) {// 융해
                isMelting(bullet.power, bullet.elementalType);
            } else if (isLightning) {// 초전도
                isSuperconductivity(bullet.power, bullet.elementalType);
            } else if (isWater) {// 프리즈
                isFreezing();
            }
        } else {
            // 얼음 이동속도 감소
            isIce = true;
            iceDamage = bullet.power;
            iceOffCount = 3f;
            if (isFire) {// 융해
                isMelting(bullet.power, bullet.elementalType);
            } else if (isLightning) {//초전도
                isSuperconductivity(bullet.power, bullet.elementalType);
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
        OnHit(transform.position, bullet.power, bullet.elementalType);
        if (isFire) { // 불이 붙었다면 과부화
            isOverload("Overload");
        } else if (isIce) { // 얼음이 붙었다면 초전도
            isSuperconductivity(bullet.power, "Superconductivity");
        } else if (isWater) { // 물이 묻었다면 감전
            isElectricShock();
        }
    }
    public void IsWater(Bullet bullet)
    {
        OnHit(transform.position, bullet.power, bullet.elementalType);
        isWater = true;
        waterOffCount = 3f;
        if (isIce) {
            isFreezing();
        } else if (isLightning) {
            isElectricShock();
        } else if (isFire) {
            isEvaporation(bullet.power, bullet.elementalType);
        }
    }
    public void IsWind(Bullet bullet)
    {
        if (isFire) {//불이 붙어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.elementalType);
            OnHit(transform.position + plusPos, fireDamage, "Fire");
        } else if (isIce) {//얼음이 뭍어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.elementalType);
            OnHit(transform.position + plusPos, iceDamage, "Ice");
        } else if (isLightning) {//전기가 뭍어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.elementalType);
            OnHit(transform.position + plusPos, lightningDamage, "Lightning");
        } else if (isWater) {//물이 붙어 있다면
            Vector3 plusPos = new Vector3(0, 0.1f, 0);
            gameManager.StateText(transform.position, "Diffusion");
            OnHit(transform.position, bullet.power, bullet.elementalType);
            OnHit(transform.position + plusPos, waterDamage, "Water");
        } else {
            OnHit(transform.position, bullet.power, bullet.elementalType);
        }
    }
    void StoneShield()
    {
        int ran = Random.Range(0, 100);
        if (ran < 5) {//5퍼센트 확률로 보호막 생성
            GameObject itemShield = objectManager.Get(10);
            Vector3 ranVec = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            itemShield.transform.position = transform.position + ranVec;
        }
    }
    public float CriticalHit(float dmg)
    {
        int ran = Random.Range(1, 101);
        if (criticalChance >= ran) {
            dmg = criticalDamage * dmg;
            isCritical = true;
        } else {
            isCritical = false;
        }
        return dmg;
    }
    void Check()
    {
        if (!gameObject.activeSelf)
            return;

        float distance = Vector3.Distance(player.transform.position,transform.position);
        if (!isAttack)
        {
            if (bossAttackCount >= 3)
            { // 필살 공격
                bossAttackCount = 0;
                isAttack = true;
                Debug.Log("필살 공격");
                GameObject bossEffect = gameManager.objectManager.Get(59);
                BossEffect bossEffectLogic = bossEffect.GetComponent<BossEffect>();
                SpriteRenderer bossEffectSpriteRenderer = bossEffect.GetComponent<SpriteRenderer>();
                bossEffectSpriteRenderer.sprite = bossEffectLogic.sprites[2]; // 원형으로 
                bossEffect.transform.localScale = new Vector3(8, 8, 0);
                bossEffect.transform.position = transform.position;
                attackVec = transform.position;
                StartCoroutine(BeforeAttack5(attackVec));
            }
            else
            {
                rigid.velocity = Vector2.zero;
                if (distance <= 1.5f)
                { // 가까울 경우 근접 공격
                    isAttack = true;
                    int ran = Random.Range(0, 2);
                    if (ran == 0)
                    { // 휩쓸기 공격
                        Debug.Log("휩쓸기 공격");
                        playerVec = player.transform.position;
                        GameObject bossEffect = gameManager.objectManager.Get(59);
                        BossEffect bossEffectLogic = bossEffect.GetComponent<BossEffect>();
                        SpriteRenderer bossEffectSpriteRenderer = bossEffect.GetComponent<SpriteRenderer>();
                        bossEffectSpriteRenderer.sprite = bossEffectLogic.sprites[1]; // 삼각형으로 
                        attackPos = transform.position - playerVec;
                        bossEffect.transform.localScale = new Vector3(4f, 4f, 0);
                        attackVec = attackPos;
                        //회전 함수
                        float degree = Mathf.Atan2(transform.position.y - playerVec.y, transform.position.x - playerVec.x) * 180f / Mathf.PI;
                        bossEffect.transform.rotation = Quaternion.Euler(0, 0, degree + 270f);
                        bossEffect.transform.position = transform.position - attackPos.normalized;
                        StartCoroutine(BeforeAttack1(attackVec));
                    }
                    else if (ran == 1)
                    { // 내려찍기 공격
                        Debug.Log("내려찍기 공격");
                        playerVec = player.transform.position;
                        GameObject bossEffect = gameManager.objectManager.Get(59);
                        BossEffect bossEffectLogic = bossEffect.GetComponent<BossEffect>();
                        SpriteRenderer bossEffectSpriteRenderer = bossEffect.GetComponent<SpriteRenderer>();
                        bossEffectSpriteRenderer.sprite = bossEffectLogic.sprites[2]; // 원형으로 
                        attackPos = transform.position - playerVec;
                        bossEffect.transform.localScale = new Vector3(4f, 4f, 0);
                        attackVec = attackPos;
                        bossEffect.transform.position = transform.position - attackPos.normalized;
                        StartCoroutine(BeforeAttack2(attackVec));
                    }
                }
                else if (distance > 1.5f)
                { // 멀 경우 점프하여 붙기, 멀리서 돌진하기, 총 쏘기 등 원거리 공격
                    isAttack = true;
                    int ran = Random.Range(0, 2);
                    if (ran == 0)
                    { // 랜덤 범위 공격
                        Debug.Log("랜덤 범위 공격");
                            playerVec = gameManager.spawner.bossSquare.transform.position;
                        for(int i =0;i<5;i++){
                            Vector3 ranVec = new Vector3(Random.Range(-3f,3f)+playerVec.x,(-6f+(i*3.2f))+playerVec.y,0);
                            GameObject bossEffect = gameManager.objectManager.Get(59);
                            BossEffect bossEffectLogic = bossEffect.GetComponent<BossEffect>();
                            SpriteRenderer bossEffectSpriteRenderer = bossEffect.GetComponent<SpriteRenderer>();
                            bossEffectSpriteRenderer.sprite = bossEffectLogic.sprites[2]; // 원형으로
                            bossEffect.transform.position = ranVec;
                            bossEffect.transform.localScale = new Vector3(3.5f, 3.5f, 0);
                            attackVec = ranVec;

                            StartCoroutine(BeforeAttack3(attackVec));
                        }
                        bossAttackCount++;
                    }
                    else if (ran == 1)
                    {
                        Debug.Log("점프하여 붙기 공격");
                        playerVec = player.transform.position;
                        GameObject bossEffect = gameManager.objectManager.Get(59);
                        BossEffect bossEffectLogic = bossEffect.GetComponent<BossEffect>();
                        SpriteRenderer bossEffectSpriteRenderer = bossEffect.GetComponent<SpriteRenderer>();
                        bossEffectSpriteRenderer.sprite = bossEffectLogic.sprites[2]; // 원형형으로 
                        attackPos = playerVec;
                        bossEffect.transform.localScale = new Vector3(2, 2, 0);
                        bossEffect.transform.position = attackPos;
                        gameObject.layer = 10;
                        StartCoroutine(BeforeAttack4());
                    }
                }
            }
        }
    }

    //보스 공격 함수
    IEnumerator BeforeAttack1(Vector3 attackPos){
        yield return new WaitForSeconds(1.5f);
        Debug.Log("휩쓸기 공격 준비");
        Attack1(attackPos);
    }
    void Attack1(Vector3 attackPos){
        Debug.Log("휩쓸기 공격 시작");
        GameObject bossAttack = gameManager.objectManager.Get(49);
        Bullet bossAttackLogic = bossAttack.GetComponent<Bullet>();
        bossAttackLogic.power = power;
        bossAttack.transform.localScale = new Vector3(1f, 1f, 0);
        bossAttack.transform.position = transform.position - attackPos.normalized;
        //회전 함수
        float degree = Mathf.Atan2(transform.position.y-playerVec.y,transform.position.x-playerVec.x)*180f/Mathf.PI;
        bossAttack.transform.rotation = Quaternion.Euler(0,0,degree + 180f);
        bossAttackCount++;
        isAttack = false;
        Invoke("Check", 1f);
    }
    IEnumerator BeforeAttack2(Vector3 attackPos)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("내려찍기 공격 준비");
        Attack2(attackPos);
    }
    void Attack2(Vector3 attackPos)
    {
        Debug.Log("내려찍기 시작");
        GameObject bossAttack = gameManager.objectManager.Get(50);
        Bullet bossAttackLogic = bossAttack.GetComponent<Bullet>();
        bossAttack.transform.localScale = new Vector3(1.5f, 1.5f, 0);
        bossAttackLogic.power = power;
        bossAttackLogic.attack2Vec = transform.position - attackPos.normalized;
        bossAttack.transform.position = new Vector3(transform.position.x - attackPos.normalized.x, transform.position.y - attackPos.normalized.y+3f);
        bossAttackCount++;
        isAttack = false;
        Invoke("Check", 0.5f);
    }
    IEnumerator BeforeAttack3(Vector3 attackVec)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("랜덤 범위 공격 준비");
        Attack3(attackVec);
    }
    void Attack3(Vector3 attackVec)
    {
        Debug.Log("랜덤 범위공격 시작");
        GameObject bossAttack = gameManager.objectManager.Get(53);
        Bullet bossAttackLogic = bossAttack.GetComponent<Bullet>();
        bossAttack.transform.localScale = new Vector3(3.5f, 3.5f, 0);
        bossAttackLogic.power = power;
        bossAttack.transform.position = attackVec;
        isAttack = false;
        Invoke("Check", 3f);

    }
    IEnumerator BeforeAttack4()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("점프 공격 준비");

        Attack4();
    }
    void Attack4()
    {
        transform.position = new Vector2(attackPos.x, attackPos.y + 3f);
        isAttack4 = true;
    }
    IEnumerator BeforeAttack5(Vector3 attackVec)
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("필살 공격 준비");

        Attack5(attackVec);
    }
    void Attack5(Vector3 attackVec)
    {
        GameObject bossAttack = gameManager.objectManager.Get(51);
        Bullet bossAttackLogic = bossAttack.GetComponent<Bullet>();
        bossAttackLogic.power = power/4;
        bossAttack.transform.localScale = new Vector3(0.8f, 0.8f, 0);
        bossAttack.transform.position = attackVec;
        Invoke("Attack5End", 7f);
    }
    void Attack5End()
    {
        isAttack = false;
        Check();
    }
}