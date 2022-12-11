using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;


public class GameManager : MonoBehaviour
{
    //광고 함수
    private InterstitialAd interstitial;

    //언어 함수
    public string language;

    //타일 함수
    public static GameManager instance;

    //가까운 적을 탐색하는 함수
    public GameObject targetEnemy;
    public float shortDis;
    public int enemyCount;
    public Camera cam;
    public List<GameObject> poolObj;
    bool getPool = false;

    //플레이어 함수
    public PlayerMove playerMove;
    public Image life;
    public Transform playerPos;
    public GameObject player;
    public int playerLevel;
    float levelUpExp;
    public string character;
    public int maxLevelCount;


    //적군 소환
    public List<EnemySpawn> spawnList;
    public string[] enemyObjs;
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;
    public ObjectManager objectManager;
    public int spawnIndex;
    public bool spawnEnd;
    public int stage;

    //UI 함수
    public Image expBar;
    public Text levelText;
    public Text timeText;
    float _sec;
    float _min;
    public Text enemyClearCountText;
    public Image delayBar;
    public Text glodText;
    public GameObject playerUI;
    public GameObject pausePanel;
    public GameObject pauseButton;
    public GameObject bossHealthBar;
    public Image bossHealth;
    public GameObject playerDeadPanel;
    public GameObject playerDeadChancePanel;
    public GameObject eventPanel;
    public Text eventText;
    public Animator camAnim;
    public Text gameOverText;
    public Text[] retryButtonText;
    public Text[] HomeButtonText;
    public Text continueButtonText;
    public Text explaneText;
    public Canvas myCanvas;

    //레벨업 셀렉 UI
    public GameObject selectPanel;
    public Image[] viewImage;
    public Sprite[] wizardWeaponImage;
    public Sprite[] hunterWeaponImage;
    public Sprite[] hunter2WeaponImage;
    public Sprite[] hunter3WeaponImage;
    public Sprite[] hunter4WeaponImage;
    public int[] selectNumber;
    public Text[] viewText;
    string[] weaponText;
    string[,] weaponExText;
    public Text[] viewExText;
    public Text levelUpText;
    public Image[] selectPanelImage;
    public Image[] pauseWeaponImage;
    public Text[] pauseWeaponText;
    public Text[] pauseWeaponExText;

    int selectCount; //남은 선택지 개수 분별 함수
    string coinSelectText;
    public int weaponCount; //현재 무기 개수
    public int passiveCount; //현재 패시브 개수
    string[,] pauseExText; // 일시정지 상세 텍스트
    int totalWeaponCount; // 총 무기 개수

    float targetCountTime; //새로운 타겟을 선별 하는 타임 함수
    bool isLevelUp = false;

    //상자 함수
    bool isBoxOpen;
    int boxOpenCount;


    //저장 함수
    int clearStage;
    int gold;
    int grobalGold;

    void Start()
    {
        if(PlayerPrefs.HasKey("Language")){
            language = PlayerPrefs.GetString("Language");
        } else {
            language = "English";
        }
        character = PlayerPrefs.GetString("Character");
        //언어 초기화
        if(language == "English"){
            levelUpText.text = "LevelUp!";
            retryButtonText[0].text = "Retry";
            retryButtonText[1].text = "Retry";
            retryButtonText[2].text = "(AD)\nResurrect";
            HomeButtonText[0].text = "Home";
            HomeButtonText[1].text = "Home";
            HomeButtonText[2].text = "Home";
            explaneText.text = "Report the ad and resurrect with 50% HP";
            continueButtonText.text = "Continue";
            coinSelectText = "Gold+100";
            if(character == "Wizard"){
                weaponText = new string[]{"IceSpear","WindForce","FireBall","ThunderV","Water","Stone","Delay","Speed","Life","Power","Magnet","Critical"};
                weaponExText = new string[,] {{"Throws a powerful Icespear.\nType Ice","DamageUp\nScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","Throws an ice bomb"},
                                            {"Summons a wind that pushes enemies away every 5 seconds.\nType Wind","DamageUp\nScaleUp\nTimeUp","DamageUp\nScaleUp\nTimeUp","DamageUp\nScaleUp\nTimeUp","DamageUp\nScaleUp\nTimeUp","Summons a hurricane that circles around you."},
                                            {"Fire fires.\nType Fire","DamageUp\nScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","Fire spreads in all directions."},
                                            {"Thunder crashes from the sky.\n2 Thunder\nType Lightning","3 Thunder","4 Thunder","5 Thunder","6 Thunder","A strong lightning strikes."},
                                            {"Water wraps around the wizard.\nType Water","ScaleUp","ScaleUp","ScaleUp","ScaleUp","A tsunami sweeps through the area.\n3 Seconds"},
                                            {"2 Rocks circle around the wizard.\nType Stone","DelayDown\n3 Rocks","DelayDown\n4 Rocks","DelayDown\n5 Rocks","DelayDown\n6 Rocks","Meteorites fall around.\n10 Seconds"},

                                            {"Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp",""},
                                            {"Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                pauseExText = new string[,] {{"Haven't learned yet.","Damage 50\n100% size","Damage 100\n120% size","Damage 150\n130% size","Damage 200\n140% size","Damage 250\n150% size","Damage 500\n180% size\nIce is scattered in all directions."},
                                            {"Haven't learned yet.","Damage 30\n100% size\n1 seconds","Damage 60\n110% size\n1.2 seconds","Damage 90\n120% size\n1.6 seconds","Damage 120\n130% size\n1.7 seconds","Damage 160\n150% size\n2.5 seconds","Damage 200\n150% size\n2.5 seconds\nA typhoon whirls around."},
                                            {"Haven't learned yet.","Damage 50\n100% size","Damage 80\n120% size","Damage 120\n140% size","Damage 150\n160% size","Damage 190\n180% size","Damage 250\n200% size\nFire spreads in all directions."},
                                            {"Haven't learned yet.","Damage 40\n2 Thunder","Damage 80\n3 Thunder","Damage 120\n4 Thunder","Damage 160\n5 Thunder","Damage 200\n6 Thunder","Damage 300\n6 Thunder\nBig size"},
                                            {"Haven't learned yet.","Damage 50\n50% size","Damage 100\n100% size","Damage 150\n150% size","Damage 200\n200% size","Damage 250\n250% size","Damage 300\n250% size\nA tsunami comes every 3 seconds."},
                                            {"Haven't learned yet.","Damage 50\n2 rocks","Damage 100\n3 rocks","Damage 150\n4 rocks","Damage 200\n5 rocks","Damage 250\n6 rocks","Damage 300\n6 rocks\nMeteors drop every 10 seconds."},
                                            
                                            {"Haven't learned yet.","Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Haven't learned yet.","Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Haven't learned yet.","Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Haven't learned yet.","Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Haven't learned yet.","Mag Scale 200%","Mag Scale 275%","Mag Scale 325%","Mag Scale 375%","Mag Scale 450%",""},
                                            {"Haven't learned yet.","Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
            } else if (character == "Hunter"){
                weaponText = new string[]{"Pickaxe","Shovel","Trident","Machinegun","Shotgun","Pistor","Delay","Speed","Life","Power","Magnet","Critical"};
                weaponExText = new string[,] {{"Throw the pickaxe up 2 Seconds.","2 Pickaxe\nDamageUp","3 Pickaxe\nDamageUp","4 Pickaxe\nDamageUp","5 Pickaxe\nDamageUp","Throws a huge pickaxe."},
                                            {"Throws a shovel under once 2 seconds.\nType None","2 shovels\nDamageUp","3 shovels\nDamageUp","4 shovels\nDamageUp","5 shovels\nDamageUp","The number of shovels doubles."},
                                            {"Throws a piercing spear at an enemy every 2 seconds.\nType None","DamageUpScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","DamageUp\nScaleUp","The trident goes back and forth."},
                                            {"Use a Machinegun with less delay.\nType None","0.33 Seconds\nDamageUp","0.25 Seconds\nDamageUp","0.12 Seconds\nDamageUp","0.10 Seconds\nDamageUp","Bullets fire very quickly."},
                                            {"Bullets are fired spread.\n2 Bullet\nType None","3 Bullet\nDamageUp","4 Bullet\nDamageUp","5 Bullet\nDamageUp","6 Bullet\nDamageUp","Bullets spread evenly in all directions."},
                                            {"Use a high-damage pistol.","DamageUp","DamageUp","DamageUp","DamageUp","Bullet size increases."},

                                            {"Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp",""},
                                            {"Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                pauseExText = new string[,] {{"Haven't learned yet.","Damage 120\n1 pickaxe","Damage 200\n2 pickaxe","Damage 300\n3 pickaxe","Damage 400\n4 pickaxe","Damage 600\n5 pickaxe","Damage 800\n5 pickaxe\nBig size"},
                                            {"Haven't learned yet.","Damage 100\n1 shovel","Damage 200\n2 shovel","Damage 300\n3 shovel","Damage 400\n4 shovel","Damage 500\n5 shovel","Damage 600\n10 shovel"},
                                            {"Haven't learned yet.","Damage 80\n50% size","Damage 160\n70% size","Damage 240\n90% size","Damage 320\n140% size","Damage 400\n180% size","Damage 500\n180% size\nfires backwards."},
                                            {"Haven't learned yet.","Damage 50\n0.5 Seconds","Damage 100\n0.33 Seconds","Damage 150\n0.25 Seconds","Damage 200\n 0.2 Seconds","Damage 250\n0.16 Seconds","Damage 350\n0.1 Seconds"},
                                            {"Haven't learned yet.","Damage 100\n2 bullet","Damage 200\n3 bullet","Damage 300\n4 bullet","Damage 400\n5 bullet","Damage 500\n6 bullet","Damage 600\n12 bullet\nIt fires in a circle."},
                                            {"Haven't learned yet.","Damage 200","Damage 400","Damage 600","Damage 800","Damage 1000","Damage 1500\nBig Size"},
                                            
                                            {"Haven't learned yet.","Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Haven't learned yet.","Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Haven't learned yet.","Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Haven't learned yet.","Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Haven't learned yet.","Mag Scale 200%","Mag Scale 275%","Mag Scale 325%","Mag Scale 375%","Mag Scale 450%",""},
                                            {"Haven't learned yet.","Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                                                        } else if( character == "Hunter2"){
                weaponText = new string[]{"Thorn armor","Hammer","Mace","Knight","The Cross","Strike","Delay","Speed","Life","Power","Magnet","Critical"};
                weaponExText = new string[,] {{"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp",""},
                                            {"Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"Haven't learned yet.","Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Haven't learned yet.","Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Haven't learned yet.","Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Haven't learned yet.","Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Haven't learned yet.","Mag Scale 200%","Mag Scale 275%","Mag Scale 325%","Mag Scale 375%","Mag Scale 450%",""},
                                            {"Haven't learned yet.","Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
            } else if( character == "Hunter3"){
                                weaponText = new string[]{"Drill","Bouncy ball","toss at random","Land mine","Turret","Bow","Delay","Speed","Life","Power","Magnet","Critical"};
                weaponExText = new string[,] {{"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp",""},
                                            {"Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"Haven't learned yet.","Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Haven't learned yet.","Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Haven't learned yet.","Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Haven't learned yet.","Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Haven't learned yet.","Mag Scale 200%","Mag Scale 275%","Mag Scale 325%","Mag Scale 375%","Mag Scale 450%",""},
                                            {"Haven't learned yet.","Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
            } else if( character == "Hunter4"){
                weaponText = new string[]{"Bear","Skull","Bird","Snake","Ice golem","Lake","Delay","Speed","Life","Power","Magnet","Critical"};
                weaponExText = new string[,] {{"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp","Mag ScaleUp",""},
                                            {"Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"Haven't learned yet.","Delay 10%","Delay 20%","Delay 30%","Delay 40%","Delay 50%",""},
                                            {"Haven't learned yet.","Speed+20%","Speed+40%","Speed+60%","Speed+80%","Speed+100%",""},
                                            {"Haven't learned yet.","Maxhealth+10%\nLife Recovery 1% 5 Seconds","Maxhealth+20%\nLife Recovery 2% 5 Seconds","Maxhealth+30%\nLife Recovery 3% 5 Seconds","Maxhealth+40%\nLife Recovery 4% 5 Seconds","Maxhealth+50%\nLife Recovery 5% 5 Seconds",""},
                                            {"Haven't learned yet.","Power+10%","Power+20%","Power+30%","Power+40%","Power+50%",""},
                                            {"Haven't learned yet.","Mag Scale 200%","Mag Scale 275%","Mag Scale 325%","Mag Scale 375%","Mag Scale 450%",""},
                                            {"Haven't learned yet.","Critical Damage155%\nCritical Chance5%","Critical Damage160%\nCritical Chance10%","Critical Damage165%\nCritical Chance15%","Critical Damage170%\nCritical Chance20%","Critical Damage175%\nCritical Chance25%",""}};
        }
        } else if (language == "Korean"){
            levelUpText.text = "레벨 업!";
            retryButtonText[0].text = "다시하기";
            retryButtonText[1].text = "다시하기";
            retryButtonText[2].text = "광고보고\n부활하기";
            HomeButtonText[0].text = "홈";
            HomeButtonText[1].text = "홈";
            HomeButtonText[2].text = "홈";
            explaneText.text = "광고를 보고 50%체력으로 부활하세요.";
            continueButtonText.text = "이어하기";
            coinSelectText = "골드+100";
            if(character == "Wizard"){
                weaponText = new string[]{"아이스 스피어","윈드포스","파이어볼","천둥 번개","물 오로라","돌","딜레이","스피드","체력","파워","획득 범위","크리티컬"};
                weaponExText = new string[,] {{"데미지가 강한 고드름을 발사합니다.\n타입 얼음","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","얼음 폭탄을 던집니다.\n데미지 증가\n크기 증가"},
                                            {"5초마다 마법사 주변 적군을 밀어내는 바람을 소환합니다.\n타입 바람","데미지 증가\n범위 증가\n지속시간 증가","데미지 증가\n범위 증가\n지속시간 증가","데미지 증가\n범위 증가\n지속시간 증가","데미지 증가\n범위 증가\n지속시간 증가","항상 태풍이 주변을 맴돕니다.\n데미지 증가"},
                                            {"불이 발사됩니다.\n타입 불","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","불이 사방으로 퍼집니다.\n데미지 증가\n크기 증가"},
                                            {"하늘에서 2초마다 천둥번개가 내리칩니다.\n2개의 천둥\n타입 전기","3개의 천둥","4개의 천둥","5개의 천둥","6개의 천둥","강력한 천둥번개가 칩니다."},
                                            {"마법사 주변에 물 오로라가 생깁니다.\n타입 물","범위 증가","범위 증가","범위 증가","범위 증가","3초마다 쓰나미가 몰려옵니다."},
                                            {"2개의 바위가 마법사 주변을 돕니다.\n타입 바위","딜레이 감소\n바위 3개","딜레이 감소\n바위 4개","딜레이 감소\n바위 5개","딜레이 감소\n바위 6개","10초마다 운석이 떨어집니다."},

                                            {"10% 딜레이 감소","20% 딜레이 감소","30% 딜레이 감소","40% 딜레이 감소","50% 딜레이 감소",""},
                                            {"20% 속도 증가","40% 속도 증가","60% 속도 증가","80% 속도 증가","100% 속도 증가",""},
                                            {"10% 최대 체력 증가\n5초 마다 최대 체력의 1% 회복","20% 최대 체력 증가\n5초 마다 최대 체력의 2% 회복","30% 최대 체력 증가\n5초 마다 최대 체력의 3% 회복","40% 최대 체력 증가\n5초 마다 최대 체력의 4% 회복","50% 최대 체력 증가\n5초 마다 최대 체력의 5% 회복",""},
                                            {"10% 파워 증가","20%  파워 증가","30%  파워 증가","40%  파워 증가","50%  파워 증가",""},
                                            {"아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가",""},
                                            {"155% 크리티컬 데미지\n5% 크리티컬 확률","160% 크리티컬 데미지\n10% 크리티컬 확률","165% 크리티컬 데미지\n15% 크리티컬 확률","170% 크리티컬 데미지\n20% 크리티컬 확률","175% 크리티컬 데미지\n25% 크리티컬 확률",""}};
                pauseExText = new string[,] {{"아직 배우지 못했습니다.","데미지 50\n100% 사이즈","데미지 100\n120% 크기","데미지 150\n130% 크기","데미지 200\n140% 크기","데미지 250\n150% 크기","데미지 500\n180% 크기\n명중시 아이스 스피어가 사방으로 퍼집니다."},
                                            {"아직 배우지 못했습니다.","데미지 30\n100% 크기\n1 초의 지속시간","데미지 60\n110% 크기\n1.2 초의 지속시간","데미지 90\n120% 크기\n1.6 초의 지속시간","데미지 120\n130% 크기\n1.7 초의 지속시간","데미지 160\n150% 크기\n2.5 초의 지속시간","데미지 200\n150% 크기\n2.5 초의 지속시간\n태풍이 주변을 맴돕니다."},
                                            {"아직 배우지 못했습니다.","데미지 50\n100% 크기","데미지 80\n120% 크기","데미지 120\n140% 크기","데미지 150\n160% 크기","데미지 190\n180% 크기","데미지 250\n200% 크기\n불이 사방으로 퍼집니다."},
                                            {"아직 배우지 못했습니다.","데미지 40\n2 개의 천둥","데미지 80\n3 개의 천둥","데미지 120\n4 개의 천둥","데미지 160\n5 개의 천둥","데미지 200\n6 개의 천둥","데미지 300\n6 개의 천둥\n거대한 크기"},
                                            {"아직 배우지 못했습니다.","데미지 50\n50% 크기","데미지 100\n100% 크기","데미지 150\n150% 크기","데미지 200\n200% 크기","데미지 250\n250% 크기","데미지 300\n250% 크기\n3초마다 쓰나미가 몰려옵니다."},
                                            {"아직 배우지 못했습니다.","데미지 50\n2 개의 바위","데미지 100\n3 개의 바위","데미지 150\n4 개의 바위","데미지 200\n5 개의 바위","데미지 250\n6 개의 바위","데미지 300\n6 개의 바위\n10초마다 운석이 떨어집니다."},
                                            
                                            {"아직 배우지 못했습니다.","딜레이 10%","딜레이 20%","딜레이 30%","딜레이 40%","딜레이 50%",""},
                                            {"아직 배우지 못했습니다.","이동속도+20%","이동속도+40%","이동속도+60%","이동속도+80%","이동속도+100%",""},
                                            {"아직 배우지 못했습니다.","최대 체력+10%\n체력회복 1% 5 초 마다","최대 체력+20%\n체력회복 2% 5 초 마다","최대 체력+30%\n체력회복 3% 5 초 마다","최대 체력+40%\n체력회복 4% 5 초 마다","최대 체력+50%\n체력회복 5% 5 초 마다",""},
                                            {"아직 배우지 못했습니다.","파워+10%","파워+20%","파워+30%","파워+40%","파워+50%",""},
                                            {"아직 배우지 못했습니다.","아이템 획득 범위 200%","아이템 획득 범위 275%","아이템 획득 범위 325%","아이템 획득 범위 375%","아이템 획득 범위 450%",""},
                                            {"아직 배우지 못했습니다.","크리티컬 데미지155%\n크리티컬 확률5%","크리티컬 데미지160%\n크리티컬 확률10%","크리티컬 데미지165%\n크리티컬 확률15%","크리티컬 데미지170%\n크리티컬 확률20%","크리티컬 데미지175%\n크리티컬 확률25%",""}};
            } else if (character == "Hunter"){
                weaponText = new string[]{"곡괭이","삽","삼지창","기관총","샷건","권총","딜레이","스피드","체력","파워","획득 범위","크리티컬"};
                weaponExText = new string[,] {{"2 초마다 곡괭이를 위로 랜덤하게 던집니다.","곡괭이 2개\n데미지 증가","곡괭이 3개\n데미지 증가","곡괭이 4개\n데미지 증가","곡괭이 5개\n데미지 증가","거대한 곡괭이를 던집니다."},
                                            {"2 초마다 삽을 아래로 던집니다.\n타입 물리","2개의 삽을 던집니다.\n데미지 증가","3개의 삽을 던집니다.\n데미지 증가","4개의 삽을 던집니다.\n데미지 증가","5개의 삽을 던집니다.\n데미지 증가","삽을 2배로 던집니다."},
                                            {"2 초마다 적을 관통하는 삼지창을 던집니다.\n타입 물리","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","데미지 증가\n크기 증가","삼지창을 앞 뒤로 던집니다."},
                                            {"딜레이가 적은 기관총을 발사합니다.\n타입 물리","0.33 초마다 발사\n데미지 증가","0.25 초마다 발사\n데미지 증가","0.2 초마다 발사\n데미지 증가","0.16 초마다 발사\n데미지 증가","총알을 아주 빠르게 발사합니다."},
                                            {"탄이 퍼짐이 있는 샷건을 사용합니다.\n총알 2개\n타입 물리","총알 3개\n데미지 증가","총알 4개\n데미지 증가","총알 5개\n데미지 증가","총알 6개\n데미지 증가","동그랗게 탄을 발사합니다."},
                                            {"데미지가 강한 권총을 사용합니다.","데미지 증가","데미지 증가","데미지 증가","데미지 증가","총알의 사이즈가 커집니다."},

                                            {"10% 딜레이 감소","20% 딜레이 감소","30% 딜레이 감소","40% 딜레이 감소","50% 딜레이 감소",""},
                                            {"20% 속도 증가","40% 속도 증가","60% 속도 증가","80% 속도 증가","100% 속도 증가",""},
                                            {"10% 최대 체력 증가\n5초 마다 최대 체력의 1% 회복","20% 최대 체력 증가\n5초 마다 최대 체력의 2% 회복","30% 최대 체력 증가\n5초 마다 최대 체력의 3% 회복","40% 최대 체력 증가\n5초 마다 최대 체력의 4% 회복","50% 최대 체력 증가\n5초 마다 최대 체력의 5% 회복",""},
                                            {"10% 파워 증가","20%  파워 증가","30%  파워 증가","40%  파워 증가","50%  파워 증가",""},
                                            {"아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가",""},
                                            {"155% 크리티컬 데미지\n5% 크리티컬 확률","160% 크리티컬 데미지\n10% 크리티컬 확률","165% 크리티컬 데미지\n15% 크리티컬 확률","170% 크리티컬 데미지\n20% 크리티컬 확률","175% 크리티컬 데미지\n25% 크리티컬 확률",""}};
                pauseExText = new string[,] {{"아직 배우지 못했습니다.","데미지 120\n1 개의 곡괭이","데미지 200\n2 개의 곡괭이","데미지 300\n3 개의 곡괭이","데미지 400\n4 개의 곡괭이","데미지 600\n5 곡괭이","데미지 800\n5 개의 곡괭이\n크기가 커집니다."},
                                            {"아직 배우지 못했습니다.","데미지 100\n1 개의 삽","데미지 200\n2 개의 삽","데미지 300\n3 개의 삽","데미지 400\n4 개의 삽","데미지 500\n5 개의 삽","데미지 600\n10 개의 삽"},
                                            {"아직 배우지 못했습니다.","데미지 80\n50% 크기","데미지 160\n70% 크기","데미지 240\n90% 크기","데미지 320\n140% 크기","데미지 400\n180% 크기","데미지 500\n180% 크기\nfires backwards."},
                                            {"아직 배우지 못했습니다.","데미지 50\n0.5 초마다 발사","데미지 100\n0.33 초마다 발사","데미지 150\n0.25 초마다 발사","데미지 200\n 0.2 초마다 발사","데미지 250\n0.16 초마다 발사","데미지 350\n0.1 초마다 발사"},
                                            {"아직 배우지 못했습니다.","데미지 100\n2 개의 총알","데미지 200\n3 개의 총알","데미지 300\n4 개의 총알","데미지 400\n5 개의 총알","데미지 500\n6 개의 총알","데미지 600\n12 개의 총알\n원 형태로 발사됩니다."},
                                            {"아직 배우지 못했습니다.","데미지 200","데미지 400","데미지 600","데미지 800","데미지 1000","데미지 1500\n총알이 거대해집니다."},
                                            
                                            {"아직 배우지 못했습니다.","딜레이 10%","딜레이 20%","딜레이 30%","딜레이 40%","딜레이 50%",""},
                                            {"아직 배우지 못했습니다.","이동속도+20%","이동속도+40%","이동속도+60%","이동속도+80%","이동속도+100%",""},
                                            {"아직 배우지 못했습니다.","최대 체력+10%\n체력회복 1% 5 초 마다","최대 체력+20%\n체력회복 2% 5 초 마다","최대 체력+30%\n체력회복 3% 5 초 마다","최대 체력+40%\n체력회복 4% 5 초 마다","최대 체력+50%\n체력회복 5% 5 초 마다",""},
                                            {"아직 배우지 못했습니다.","파워+10%","파워+20%","파워+30%","파워+40%","파워+50%",""},
                                            {"아직 배우지 못했습니다.","아이템 획득 범위 200%","아이템 획득 범위 275%","아이템 획득 범위 325%","아이템 획득 범위 375%","아이템 획득 범위 450%",""},
                                            {"아직 배우지 못했습니다.","크리티컬 데미지155%\n크리티컬 확률5%","크리티컬 데미지160%\n크리티컬 확률10%","크리티컬 데미지165%\n크리티컬 확률15%","크리티컬 데미지170%\n크리티컬 확률20%","크리티컬 데미지175%\n크리티컬 확률25%",""}};
            } else if( character == "Hunter2"){
                weaponText = new string[]{"가시갑옷","망치","메이스","적토마","십자가","망치","딜레이","스피드","체력","파워","획득 범위","크리티컬"};
                weaponExText = new string[,] {{"주변에 적군에게 데미지를 줍니다.\n적군 처치 수에 따른 데미지 증가","데미지 증가","데미지 증가","데미지 증가","데미지 증가","5% 확률로 즉사\n보스, 정예 몬스터 제외"},
                                            {"동그랗게 회전하는 망치를 던집니다.","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"10% 딜레이 감소","20% 딜레이 감소","30% 딜레이 감소","40% 딜레이 감소","50% 딜레이 감소",""},
                                            {"20% 속도 증가","40% 속도 증가","60% 속도 증가","80% 속도 증가","100% 속도 증가",""},
                                            {"10% 최대 체력 증가\n5초 마다 최대 체력의 1% 회복","20% 최대 체력 증가\n5초 마다 최대 체력의 2% 회복","30% 최대 체력 증가\n5초 마다 최대 체력의 3% 회복","40% 최대 체력 증가\n5초 마다 최대 체력의 4% 회복","50% 최대 체력 증가\n5초 마다 최대 체력의 5% 회복",""},
                                            {"10% 파워 증가","20%  파워 증가","30%  파워 증가","40%  파워 증가","50%  파워 증가",""},
                                            {"아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가",""},
                                            {"155% 크리티컬 데미지\n5% 크리티컬 확률","160% 크리티컬 데미지\n10% 크리티컬 확률","165% 크리티컬 데미지\n15% 크리티컬 확률","170% 크리티컬 데미지\n20% 크리티컬 확률","175% 크리티컬 데미지\n25% 크리티컬 확률",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"아직 배우지 못했습니다.","딜레이 10%","딜레이 20%","딜레이 30%","딜레이 40%","딜레이 50%",""},
                                            {"아직 배우지 못했습니다.","이동속도+20%","이동속도+40%","이동속도+60%","이동속도+80%","이동속도+100%",""},
                                            {"아직 배우지 못했습니다.","최대 체력+10%\n체력회복 1% 5 초 마다","최대 체력+20%\n체력회복 2% 5 초 마다","최대 체력+30%\n체력회복 3% 5 초 마다","최대 체력+40%\n체력회복 4% 5 초 마다","최대 체력+50%\n체력회복 5% 5 초 마다",""},
                                            {"아직 배우지 못했습니다.","파워+10%","파워+20%","파워+30%","파워+40%","파워+50%",""},
                                            {"아직 배우지 못했습니다.","아이템 획득 범위 200%","아이템 획득 범위 275%","아이템 획득 범위 325%","아이템 획득 범위 375%","아이템 획득 범위 450%",""},
                                            {"아직 배우지 못했습니다.","크리티컬 데미지155%\n크리티컬 확률5%","크리티컬 데미지160%\n크리티컬 확률10%","크리티컬 데미지165%\n크리티컬 확률15%","크리티컬 데미지170%\n크리티컬 확률20%","크리티컬 데미지175%\n크리티컬 확률25%",""}};
            } else if( character == "Hunter3"){
                                weaponText = new string[]{"드릴","탱탱볼","막 던지기","지뢰","포탑","활","딜레이","스피드","체력","파워","획득 범위","크리티컬"};
                weaponExText = new string[,] {{"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"10% 딜레이 감소","20% 딜레이 감소","30% 딜레이 감소","40% 딜레이 감소","50% 딜레이 감소",""},
                                            {"20% 속도 증가","40% 속도 증가","60% 속도 증가","80% 속도 증가","100% 속도 증가",""},
                                            {"10% 최대 체력 증가\n5초 마다 최대 체력의 1% 회복","20% 최대 체력 증가\n5초 마다 최대 체력의 2% 회복","30% 최대 체력 증가\n5초 마다 최대 체력의 3% 회복","40% 최대 체력 증가\n5초 마다 최대 체력의 4% 회복","50% 최대 체력 증가\n5초 마다 최대 체력의 5% 회복",""},
                                            {"10% 파워 증가","20%  파워 증가","30%  파워 증가","40%  파워 증가","50%  파워 증가",""},
                                            {"아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가",""},
                                            {"155% 크리티컬 데미지\n5% 크리티컬 확률","160% 크리티컬 데미지\n10% 크리티컬 확률","165% 크리티컬 데미지\n15% 크리티컬 확률","170% 크리티컬 데미지\n20% 크리티컬 확률","175% 크리티컬 데미지\n25% 크리티컬 확률",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"아직 배우지 못했습니다.","딜레이 10%","딜레이 20%","딜레이 30%","딜레이 40%","딜레이 50%",""},
                                            {"아직 배우지 못했습니다.","이동속도+20%","이동속도+40%","이동속도+60%","이동속도+80%","이동속도+100%",""},
                                            {"아직 배우지 못했습니다.","최대 체력+10%\n체력회복 1% 5 초 마다","최대 체력+20%\n체력회복 2% 5 초 마다","최대 체력+30%\n체력회복 3% 5 초 마다","최대 체력+40%\n체력회복 4% 5 초 마다","최대 체력+50%\n체력회복 5% 5 초 마다",""},
                                            {"아직 배우지 못했습니다.","파워+10%","파워+20%","파워+30%","파워+40%","파워+50%",""},
                                            {"아직 배우지 못했습니다.","아이템 획득 범위 200%","아이템 획득 범위 275%","아이템 획득 범위 325%","아이템 획득 범위 375%","아이템 획득 범위 450%",""},
                                            {"아직 배우지 못했습니다.","크리티컬 데미지155%\n크리티컬 확률5%","크리티컬 데미지160%\n크리티컬 확률10%","크리티컬 데미지165%\n크리티컬 확률15%","크리티컬 데미지170%\n크리티컬 확률20%","크리티컬 데미지175%\n크리티컬 확률25%",""}};
            } else if( character == "Hunter4"){
                weaponText = new string[]{"곰","해골","새","뱀","얼음 골램","호수","딜레이","스피드","체력","파워","획득 범위","크리티컬"};
                weaponExText = new string[,] {{"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},
                                            {"","","","","",""},

                                            {"10% 딜레이 감소","20% 딜레이 감소","30% 딜레이 감소","40% 딜레이 감소","50% 딜레이 감소",""},
                                            {"20% 속도 증가","40% 속도 증가","60% 속도 증가","80% 속도 증가","100% 속도 증가",""},
                                            {"10% 최대 체력 증가\n5초 마다 최대 체력의 1% 회복","20% 최대 체력 증가\n5초 마다 최대 체력의 2% 회복","30% 최대 체력 증가\n5초 마다 최대 체력의 3% 회복","40% 최대 체력 증가\n5초 마다 최대 체력의 4% 회복","50% 최대 체력 증가\n5초 마다 최대 체력의 5% 회복",""},
                                            {"10% 파워 증가","20%  파워 증가","30%  파워 증가","40%  파워 증가","50%  파워 증가",""},
                                            {"아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가","아이템 획득 범위 증가",""},
                                            {"155% 크리티컬 데미지\n5% 크리티컬 확률","160% 크리티컬 데미지\n10% 크리티컬 확률","165% 크리티컬 데미지\n15% 크리티컬 확률","170% 크리티컬 데미지\n20% 크리티컬 확률","175% 크리티컬 데미지\n25% 크리티컬 확률",""}};
                pauseExText = new string[,] {{"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            {"","","","","","",""},
                                            
                                            {"아직 배우지 못했습니다.","딜레이 10%","딜레이 20%","딜레이 30%","딜레이 40%","딜레이 50%",""},
                                            {"아직 배우지 못했습니다.","이동속도+20%","이동속도+40%","이동속도+60%","이동속도+80%","이동속도+100%",""},
                                            {"아직 배우지 못했습니다.","최대 체력+10%\n체력회복 1% 5 초 마다","최대 체력+20%\n체력회복 2% 5 초 마다","최대 체력+30%\n체력회복 3% 5 초 마다","최대 체력+40%\n체력회복 4% 5 초 마다","최대 체력+50%\n체력회복 5% 5 초 마다",""},
                                            {"아직 배우지 못했습니다.","파워+10%","파워+20%","파워+30%","파워+40%","파워+50%",""},
                                            {"아직 배우지 못했습니다.","아이템 획득 범위 200%","아이템 획득 범위 275%","아이템 획득 범위 325%","아이템 획득 범위 375%","아이템 획득 범위 450%",""},
                                            {"아직 배우지 못했습니다.","크리티컬 데미지155%\n크리티컬 확률5%","크리티컬 데미지160%\n크리티컬 확률10%","크리티컬 데미지165%\n크리티컬 확률15%","크리티컬 데미지170%\n크리티컬 확률20%","크리티컬 데미지175%\n크리티컬 확률25%",""}};
            }
        }
        playerLevel = 1;
        _min = 0;
        _sec = 0;
        levelUpExp = 100;
        totalWeaponCount = weaponCount + passiveCount;

        for (int i = 0;i<5;i++){ // 처음 시작시 랜덤하게 5개 주고 시작
            Vector3 ranVec = new Vector3(Random.Range(-3.0f,3.0f),Random.Range(0.5f,3.0f),0);
            GameObject itemExp0 = objectManager.MakeObj("ItemExp0");
            itemExp0.transform.position = playerPos.position + ranVec;
        }
        
        StartLevelUp();
    }
    public void StageStart()
    {
        //적군 스폰 파일 읽기
        ReadSpawnFile();
    }
    public void StageEnd()
    {
        Invoke("StageClear",5f);
    }
    //적군소환을 위한 리스폰 데이터 읽기
    void ReadSpawnFile()
    {
        //변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //리스폰 파일 읽기(텍스트 파일이 아니면 null 처리) / System.IO 필요
        TextAsset textFile = Resources.Load("Stage" + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);
            //만약 비었다면 끝내라
            if(line == null)
                break;
                
            //리스폰 데이터 생성
            EnemySpawn spawnData = new EnemySpawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //파일을 꼭 닫아주자
        stringReader.Close();

        //첫번재 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void spawnEnemy()
    {
        //랜덤으로 부를 경우
        //int ranEnemy = Random.Range(0, enemyObjs.Length);
        //int ranPoint = Random.Range(0, spawnPoints.Length);
        int enemyIndex = 0;
        switch(spawnList[spawnIndex].type){
            case "A":
                enemyIndex = 5;
                break;
            case "B":
                enemyIndex = 4;
                break;
            case "C":
                enemyIndex = 3;
                break;
            case "D":
                enemyIndex = 2;
                break;
            case "E":
                bossHealthBar.SetActive(true);
                enemyIndex = 1;
                break;
            case "I":
                enemyIndex = 0;
                break;
            case "Event1":
                eventPanel.SetActive(true);
                camAnim.SetBool("IsOn",true);
                if(language == "English"){
                    eventText.text = "warning!\nMonsters are coming.";
                } else if(language == "Korean"){
                    eventText.text = "경고!\n몬스터 무리가 다가옵니다.";
                }
                
                break;
            case "EventOut":
                //적군이 더 이상 없을 경우 아웃(혹시 있다면 시간을 딜레이)
                camAnim.SetBool("IsOn",false);
            break;
            case "Event2":
                eventPanel.SetActive(true);
                camAnim.SetBool("IsOn",true);
                if(language == "English"){
                    eventText.text = "warning!\nBoss is coming.";
                } else if(language == "Korean"){
                    eventText.text = "경고!\n보스가 등장합니다.";
                }
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        EnemyMove enemyLogic = enemy.GetComponent<EnemyMove>();
        enemyLogic.player = player;
        enemyLogic.playerLogic = playerMove;
        enemyLogic.objectManager = objectManager;
        enemyLogic.gameManager = this;
        enemyCount++;
        //리스폰 인덱스 증가
        spawnIndex++;
        //스폰이 끝난 경우
        if(spawnIndex == spawnList.Count){
            spawnEnd = true;
            return;
        }
        //다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    void Awake()
    {
        stage = PlayerPrefs.GetInt("CurStage");
        instance = this;
        spawnList = new List<EnemySpawn>();
        StageStart();
    }
    void Update()
    {
        Timer();

        if(language == "English"){
            levelText.text = "Level " + playerLevel.ToString();
        } else if(language == "Korean"){
            levelText.text = "레벨 " + playerLevel.ToString();
        }
        
        expBar.fillAmount = playerMove.exp / levelUpExp;

        //체력에 게이지
        life.fillAmount = playerMove.life / playerMove.maxLife;

        //적군 잡은 수
        enemyClearCountText.text = playerMove.enemyClearNum.ToString();

        //딜레이
        delayBar.fillAmount = playerMove.curShootDelay/playerMove.realDelay;

        //골드
        glodText.text = playerMove.gold.ToString();

        //플레이어를 따라가는 UI
        playerUI.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + new Vector3(0, -0.4f, 0));

        //소환 함수
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd){
            spawnEnemy();
            curSpawnDelay = 0;
        }
        if(!getPool){
            GetPoolEnemy();
        }
            
        TargetFind();

        
        //레벨 업 함수
        if(playerMove.exp >= levelUpExp && !isLevelUp){
            isLevelUp = true;
            playerMove.exp -= levelUpExp;
            LevelUp();
            playerLevel++;
            levelUpExp += 50;
        }
    }
    void Timer()
    {
        _sec += Time.deltaTime;
        timeText.text = string.Format("{0:D2}:{1:D2}", (int)_min, (int)_sec);
        if((int)_sec>59){
            _sec = 0;
            _min++;
        }
    }

    void LevelUp()
    {
        //레벨업시 무기 혹은 패시브 스킬 고르는 함수
        selectPanel.SetActive(true);
        SelectPanelView(totalWeaponCount);
        Time.timeScale = 0;
    }
    public void ClickSelectButton(int num)
    {
        if(selectNumber[num]==-1){
            playerMove.gold += 100;
        } else {
            if(selectNumber[num] < 6 && playerMove.weaponLevel[selectNumber[num]]==5){ // 무기라면 카운트 증가
                maxLevelCount++;
            }
            playerMove.weaponLevel[selectNumber[num]]++;
        }
        for(int i = 0;i<3;i++){
            selectPanelImage[i].color = new Color(0.13f,0.46f,0.43f);
        }
        selectPanel.SetActive(false);
        pauseButton.SetActive(true);
        TimeReturn();
        isLevelUp = false;
    }
    void SelectPanelView(int max)
    {
        int curSelectCount = 3;
        selectCount = max;

        List<int> ran = new List<int>();
        int currentNumber = Random.Range(0, max);
        for(int y = 0;y<playerMove.weaponLevel.Length;y++){ // 남은 선택지가 2개 이하일 경우
            if (y>5){ // 패시브 레벨이 5일 경우
                if(playerMove.weaponLevel[y]==5){
                    selectCount--;
                }
            } else if (y<6){ // 무기 최대치 레벨이 6, 5(필살기가 2개)일 경우
                if(playerMove.weaponLevel[y]==6){
                    selectCount--;
                } else if(playerMove.weaponLevel[y]==5){
                    if(maxLevelCount>=2){
                        selectCount--;
                    }
                }
            }
        }
        //빈칸은 -1로 출력하여 코인을 얻게 한다.
        if(selectCount>2){
            curSelectCount = 3;
        }else if (selectCount == 2) { 
            curSelectCount = selectCount;
            ran.Add(-1);
        } else if (selectCount == 1) {
            curSelectCount = selectCount;
        ran.Add(-1);
            ran.Add(-1);
        } else if (selectCount == 0){
            curSelectCount = selectCount;
            ran.Add(-1);
            ran.Add(-1);
            ran.Add(-1);
        }
        // 랜덤 생성 (중복 배제)
        for (int i = 0; i < curSelectCount;){
            if (ran.Contains(currentNumber)){ //같은것이 있는 경우
                currentNumber = Random.Range(0, max);
            } else if(playerMove.weaponLevel[currentNumber] == 6){// 해당 무기가 필살기 일 경우
                    currentNumber = Random.Range(0, max);
            } else if(currentNumber<6){
                if(playerMove.weaponLevel[currentNumber] == 5){// 무기 레벨이 5일경우 (필살기)
                    if(maxLevelCount>=2){
                        currentNumber = Random.Range(0, max);
                    } else {
                        selectPanelImage[i].color = new Color(0.5f,0.5f,0.8f);
                        ran.Add(currentNumber);
                        i++;
                    }
                } else {
                    ran.Add(currentNumber);
                    i++;
                }
            } else if(currentNumber>5){
                if(playerMove.weaponLevel[currentNumber] == 5){// 패시브 레벨이 5일경우
                    currentNumber = Random.Range(0, max);
                } else {
                    ran.Add(currentNumber);
                    i++;
                }
            }
        }
        if(ran[0] == -1 && ran [1] == -1 && ran [2] == -1){
            // 3개 다 비었을 경우 다른걸 표시
            for(int i = 0 ;i<viewImage.Length; i++){
                viewImage[i].sprite = wizardWeaponImage[totalWeaponCount];
                viewText[i].text = coinSelectText;
                viewExText[i].text = coinSelectText;
                selectNumber[i] = -1;
            }
        } else {
            if(ran[0] == -1){
                // 1번 표시 x
                viewImage[0].sprite = wizardWeaponImage[totalWeaponCount];
                viewText[0].text = coinSelectText;
                viewExText[0].text = coinSelectText;
                selectNumber[0] = -1;
            } else {
                if(character=="Wizard"){
                    viewImage[0].sprite = wizardWeaponImage[ran[0]];
                } else if(character=="Hunter"){
                    viewImage[0].sprite = hunterWeaponImage[ran[0]];
                } else if(character=="Hunter2"){
                    viewImage[0].sprite = hunter2WeaponImage[ran[0]];
                } else if(character=="Hunter3"){
                    viewImage[0].sprite = hunter3WeaponImage[ran[0]];
                } else if(character=="Hunter4"){
                    viewImage[0].sprite = hunter4WeaponImage[ran[0]];
                }
                if(language=="English"){
                    viewText[0].text = weaponText[ran[0]] + " Lv. " + (playerMove.weaponLevel[ran[0]]+1);
                } else if(language=="Korean"){
                    viewText[0].text = weaponText[ran[0]] + " 레벨. " + (playerMove.weaponLevel[ran[0]]+1);
                }
                selectNumber[0] = ran[0];
                viewExText[0].text = weaponExText[ran[0],playerMove.weaponLevel[ran[0]]];
            }
            
            if(ran[1] == -1){
                // 2번 표시 x
                viewImage[1].sprite = wizardWeaponImage[totalWeaponCount];
                viewText[1].text = coinSelectText;
                viewExText[1].text = coinSelectText;
                selectNumber[1] = -1;
            } else {
                if(character=="Wizard"){
                    viewImage[1].sprite = wizardWeaponImage[ran[1]];
                } else if(character=="Hunter"){
                    viewImage[1].sprite = hunterWeaponImage[ran[1]];
                } else if(character=="Hunter2"){
                    viewImage[1].sprite = hunter2WeaponImage[ran[1]];
                } else if(character=="Hunter3"){
                    viewImage[1].sprite = hunter3WeaponImage[ran[1]];
                } else if(character=="Hunter4"){
                    viewImage[1].sprite = hunter4WeaponImage[ran[1]];
                }
                if(language=="English"){
                    viewText[1].text = weaponText[ran[1]] + " Lv. " + (playerMove.weaponLevel[ran[1]]+1);
                } else if(language=="Korean"){
                    viewText[1].text = weaponText[ran[1]] + " 레벨. " + (playerMove.weaponLevel[ran[1]]+1);
                }
                selectNumber[1] = ran[1];
                viewExText[1].text = weaponExText[ran[1],playerMove.weaponLevel[ran[1]]];
            }
            
            if(ran[2] == -1){
                // 3번 표시 x
                viewImage[2].sprite = wizardWeaponImage[totalWeaponCount];
                viewText[2].text = coinSelectText;
                viewExText[2].text = coinSelectText;
                selectNumber[2] = -1;
            } else {
                if(character=="Wizard"){
                    viewImage[2].sprite = wizardWeaponImage[ran[2]];
                } else if(character=="Hunter"){
                    viewImage[2].sprite = hunterWeaponImage[ran[2]];
                } else if(character=="Hunter2"){
                    viewImage[2].sprite = hunter2WeaponImage[ran[2]];
                } else if(character=="Hunter3"){
                    viewImage[2].sprite = hunter3WeaponImage[ran[2]];
                } else if(character=="Hunter4"){
                    viewImage[2].sprite = hunter4WeaponImage[ran[2]];
                }
                if(language=="English"){
                    viewText[2].text = weaponText[ran[2]] + " Lv. " + (playerMove.weaponLevel[ran[2]]+1);
                } else if(language=="Korean"){
                    viewText[2].text = weaponText[ran[2]] + " 레벨. " + (playerMove.weaponLevel[ran[2]]+1);
                }
                selectNumber[2] = ran[2];
                viewExText[2].text = weaponExText[ran[2],playerMove.weaponLevel[ran[2]]];
            }
        }
    }
    void StartLevelUp()
    {
        //시작시 무기 스킬 고르는 함수
        selectPanel.SetActive(true);
        SelectPanelView(weaponCount);
        Time.timeScale = 0;
    }
    public void DamageText(Vector3 pos, float dmg, string type, bool criticalCheck)
    {
        GameObject damageText = objectManager.MakeObj("DamageText");
        DamageText damageTextLogic = damageText.GetComponent<DamageText>();

        damageText.transform.position = pos;
        damageTextLogic.StartDamageText(pos, dmg, type, criticalCheck);
    }
    public void StateText(Vector3 pos, string type)
    {
        GameObject damageText = objectManager.MakeObj("DamageText");
        DamageText damageTextLogic = damageText.GetComponent<DamageText>();
        damageTextLogic.language = language;

        damageText.transform.position = pos;
        damageTextLogic.StartStateText(pos, type);
    }
    public void PauseButton()
    {
        if(pausePanel.activeSelf){
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        } else{
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            for(int i =0;i<playerMove.weaponLevel.Length;i++){
                if(character=="Wizard"){
                    pauseWeaponImage[i].sprite = wizardWeaponImage[i];
                } else if(character=="Hunter"||character=="Hunter2"||character=="Hunter3"||character=="Hunter4"){
                    pauseWeaponImage[i].sprite = hunterWeaponImage[i];
                }
                if(language=="English"){
                    pauseWeaponText[i].text = weaponText[i] + " Lv. " + playerMove.weaponLevel[i];
                } else if(language=="Korean"){
                    pauseWeaponText[i].text = weaponText[i] + " 레벨. " + playerMove.weaponLevel[i];
                }
                pauseWeaponExText[i].text = pauseExText[i,playerMove.weaponLevel[i]];
            }
        }
    }
    public void ContinueButton()
    {
        TimeReturn();
        pausePanel.SetActive(false);
    }
    public void HomeButton()
    {
        gold = playerMove.gold;
        grobalGold = PlayerPrefs.GetInt("GrobalGold");
        grobalGold += gold;
        PlayerPrefs.SetInt("GrobalGold",grobalGold);
        PlayerPrefs.SetString("Language",language);
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }
    public void playerDead()
    {
        pauseButton.SetActive(false);
        if(language == "English"){
            gameOverText.text = stage + "Game Over!";
        } else if(language == "Korean"){
            gameOverText.text = stage + " 게임 오버!";
        }
        playerDeadPanel.SetActive(true);
    }
    public void RertyButton()
    {
        gold = playerMove.gold;
        grobalGold = PlayerPrefs.GetInt("GrobalGold");
        grobalGold += gold;
        PlayerPrefs.SetInt("GrobalGold",grobalGold);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
    public void playerDeadChance()
    {
        pauseButton.SetActive(false);
        playerDeadChancePanel.SetActive(true);
    }
    public void ResurrectionButton()
    {
        // 광고 시청 버튼을 누를 경우
        RequestInterstitial();
        
        // 광고 로딩 함수
        StartCoroutine(showInterstitial());
        IEnumerator showInterstitial(){
            while(!this.interstitial.IsLoaded()){
                yield return new WaitForSeconds(0.2f);
            }
            this.interstitial.Show();
            myCanvas.sortingOrder = -1;
        }

        // if (this.interstitial.IsLoaded()) {
        //     this.interstitial.Show();
        // }
    }
    void StageClear()
    {
        if(language == "English"){
            gameOverText.text = stage + " Stage Clear!";
        } else if(language == "Korean"){
            gameOverText.text = stage + " 스테이지 클리어!";
        }
        
        playerDeadPanel.SetActive(true);
        stage++;
        gold = playerMove.gold;
        grobalGold = PlayerPrefs.GetInt("GrobalGold");
        grobalGold += gold;
        PlayerPrefs.SetInt("GrobalGold",grobalGold);
        if(clearStage<stage){
            PlayerPrefs.SetInt("ClearStage",stage);
        }
        PlayerPrefs.Save();
        Time.timeScale = 0;
    }
    public bool IsTargetVisible(Camera _camera, Transform _transform)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        var point = _transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
    private IEnumerator InvokeRealTimeHelper (string fuctionName, float delay){
        float timeElapsed = 0f;
        while (timeElapsed < delay){
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        SendMessage(fuctionName);
    }
    void TimeReturn()
    {
        Time.timeScale = 1;
    }
    void TimeStop()
    {
        Time.timeScale = 0;
    }

    // 찾기 전 먼저 적군과 상자 위치를 받아오기
    void GetPoolEnemy()
    {
        getPool = true;
        for(int x=0;x<enemyObjs.Length;x++){
            for(int i = 0;i<objectManager.GetPool(enemyObjs[x]).Length;i++){
                poolObj.Add(objectManager.GetPool(enemyObjs[x])[i]);
            }
        }
    }
    void TargetFind()
    {
        if(targetCountTime>=1){
            for(int i=0;i<poolObj.Count;i++){
                if(poolObj[i].activeSelf){ // 해당 적군이 활성화 되었을 경우
                    if(IsTargetVisible(cam, poolObj[i].transform)){ // 해당 적군이 화면 안에 있다면
                        shortDis = Vector3.Distance(playerPos.position, poolObj[i].transform.position); // 첫번째를 기준으로 잡아주기
                        targetEnemy = poolObj[i]; // 첫번째를 먼저
                        for(int y=0;y<poolObj.Count;y++){
                            if(poolObj[y].activeSelf){
                                float Distance = Vector3.Distance(playerPos.position, poolObj[y].transform.position);
                                if (Distance < shortDis){ // 위에서 잡은 기준으로 거리 재기
                                    targetEnemy = poolObj[y];
                                    shortDis = Distance;                                    }
                            }
                        }
                    }
                }
                playerMove.targetEnemy = targetEnemy;
            }
            targetCountTime = 0;
        } else {
            targetCountTime += Time.deltaTime;
        }
    }
    void BoxAlarm()
    {
        //박스 위치를 알려주는 함수 만들어야 함
    }
    void BoxOpenItem()
    {
        //박스를 열면 캐릭터 조각을 준다!

    }

    //광고 함수
    private void RequestInterstitial()
    {
        // 주의! 실행 전 꼭 테스트로 실행 할 것!
        // 테스트 ca-app-pub-3940256099942544/1033173712
        // 광고 ca-app-pub-4730748511418289/9524493073
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    public void HandleOnAdClosed(object sender, System.EventArgs args)
    {
        //광고가 닫혔을경우 부활
        playerMove.playerDeadCount++;
        playerDeadChancePanel.SetActive(false);
        Time.timeScale = 1;
        playerMove.playerDead = false;
        playerMove.anim.SetBool("Dead",playerMove.playerDead);
        playerMove.life += playerMove.maxLife/2;
        GameObject boom = objectManager.MakeObj("ItemBoom");
        boom.transform.position = player.transform.position;
        playerMove.gameObject.layer = 6;
        pauseButton.SetActive(true);
    }
}