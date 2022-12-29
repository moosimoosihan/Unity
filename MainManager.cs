using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using GoogleMobileAds.Api;

public class MainManager : MonoBehaviour
{
    public string language;
    public int stage = 1;
    public int grobalGold;
    public int clearStage = 1;
    public int maxstage;
    public string character;

    // UI 함수
    public Text stageText;
    public GameObject stageBackButton;
    public GameObject stageNextButton;
    public Text toastText;
    public GameObject toastTextObj;
    public GameObject settingPanel;
    public Text settingButtonText;
    public Text goldText;
    public Text languageText;
    public Text engButtonText;
    public Text korButtonText;
    public Text characterButtonText;
    public Text inventoryButtonText;
    public Text stateLevelButtonText;
    public Text gameStartButtonText;
    public GameObject characterPanel;
    public Image[] characterSelectButtonImage;
    public Sprite[] buttonSprite;
    int characterNum;
    public Text characterText;
    public Text characterExText;
    string[] exText;
    public GameObject stateLevelPanel;
    public Text[] helpText;
    public Text helpExText;
    public GameObject helpPanel;
    public GameObject inventoryPanel;
    public GameObject creatorInfoPannel;
    public Text[] creatorText;
    public Text characterBuyButtonText;
    public GameObject characterBuyButton;

    //스탯 정보 함수
    public Image stateInfoImage;
    public GameObject stateInfoPanel;
    public Text stateInfoText;
    public Text stateInfoExText;
    public Sprite[] stateInfoCharacterImage;

    // 스탯 레벨
    int stateLevelAtttack;
    int stateLevelLife;
    int stateLevelSpeed;
    public Text[] stateLevelButtonTexts;
    int stateLevelCriticalChance;
    int stateLevelCriticalDamage;
    int stateLevelReLife;
    int stateLevelLifeValue;

    // 캐릭터 버튼
    public Image[] characterButtonImage;
    string[] exBuyText;
    int characterBuyNum;

    //광고
    //private BannerView bannerView;


    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });
        //RequestBanner();
        
        language = "English";
        character = "Hunter";
        characterNum = 0;
        clearStage = 1;
        grobalGold = 0;
        stateLevelAtttack = 0;
        stateLevelLife = 0;
        stateLevelSpeed = 0;
        stateLevelCriticalChance = 0;
        stateLevelCriticalDamage = 0;
        stateLevelReLife = 0;
        stateLevelLifeValue = 0;
        if (PlayerPrefs.HasKey("GrobalGold")){ // 골드가 저장된게 있다면
            grobalGold = PlayerPrefs.GetInt("GrobalGold");
        } else {
            PlayerPrefs.SetInt("GrobalGold", grobalGold);
        }
        if(PlayerPrefs.HasKey("Character")){ // 저장된 케릭터가 있다면
            character = PlayerPrefs.GetString("Character");
        } else {
            PlayerPrefs.SetString("Character", character);
        }
        if (PlayerPrefs.HasKey("CharacterNum")){ // 저장된 케릭터가 있다면
            characterNum = PlayerPrefs.GetInt("CharacterNum");
        } else {
            PlayerPrefs.SetInt("CharacterNum", characterNum);
        }
        if (PlayerPrefs.HasKey("Language")){ // 저장된 언어가 있다면
            language = PlayerPrefs.GetString("Language");
        } else {
            PlayerPrefs.SetString("Language", language);
        }
        if (PlayerPrefs.HasKey("ClearStage")){ // 저장된 클리어 스테이지 수 가 있다면
            clearStage = PlayerPrefs.GetInt("ClearStage");
        } else {
            PlayerPrefs.SetInt("ClearStage", clearStage);
        }
        if (PlayerPrefs.HasKey("StateLevelAtttack")){ // 공격 스탯 저장된 내용이 있다면
            stateLevelAtttack = PlayerPrefs.GetInt("StateLevelAtttack");
        } else {
            PlayerPrefs.SetInt("StateLevelAtttack", stateLevelAtttack);
        }
        if (PlayerPrefs.HasKey("StateLevelLife")){ // 체력 스탯 저장된 내용이 있다면
            stateLevelLife = PlayerPrefs.GetInt("StateLevelLife");
        } else {
            PlayerPrefs.SetInt("StateLevelLife", stateLevelLife);
        }
        if (PlayerPrefs.HasKey("StateLevelSpeed")){ // 이동 속도 스탯 저장된 내용이 있다면
            stateLevelSpeed = PlayerPrefs.GetInt("StateLevelSpeed");
        } else {
            PlayerPrefs.SetInt("StateLevelSpeed", stateLevelSpeed);
        }
        if(PlayerPrefs.HasKey("StateLevelCriticalChance")){ // 크리티컬 확률 스탯 저장된 내용이 있다면
            stateLevelCriticalChance = PlayerPrefs.GetInt("StateLevelCriticalChance");
        } else {
            PlayerPrefs.SetInt("StateLevelCriticalChance", stateLevelCriticalChance);
        }
        if(PlayerPrefs.HasKey("StateLevelCriticalDamage")){ // 크리티컬 데미지 스탯 저장된 내용이 있다면
            stateLevelCriticalDamage = PlayerPrefs.GetInt("StateLevelCriticalDamage");
        } else {
            PlayerPrefs.SetInt("StateLevelSpeed", stateLevelCriticalDamage);
        }
        if(PlayerPrefs.HasKey("StateLevelReLife")){ // 부활 스탯 저장된 내용이 있다면
            stateLevelReLife= PlayerPrefs.GetInt("StateLevelReLife");
        } else {
            PlayerPrefs.SetInt("StateLevelReLife", stateLevelReLife);
        }
        if(PlayerPrefs.HasKey("StateLevelLifeValue")){ // 힐량 스탯 저장된 내용이 있다면
            stateLevelLifeValue= PlayerPrefs.GetInt("StateLevelLifeValue");
        } else {
            PlayerPrefs.SetInt("StateLevelReLife", stateLevelLifeValue);
        }
        stage = clearStage;
        PlayerPrefs.Save();
        if(language == "English" || language == null){
            helpText[0].text = "Help";
            helpText[1].text = "Help";
            gameStartButtonText.text = "GameStart";
            settingButtonText.text = "Setting";
            languageText.text = "Language";
            engButtonText.text = "English";
            korButtonText.text = "Korean";
            characterButtonText.text = "Character";
            inventoryButtonText.text = "State\nInfo";
            stateLevelButtonText.text = "State\nLevel";
            characterBuyButtonText.text = "Buy";
            stateInfoText.text = "State Infomation";
            characterText.text = "Character";
            creatorText[0].text = "Creator";
            creatorText[1].text = "Creator";
            creatorText[2].text = "Developer : CreapySmoothie\nDesign : pandayu\n\nSource provided\nGold Metal Studio undead survival\nDustyroom Casual Game Sound - One Shot SFX Pack\nBasic RPG Icons\nWarped Shooting Fx";
            exBuyText = new string[]{"Paladin\n1,000 Gold\nUse light weapons.",
                                    "Vagabond\n5,000 Gold\nDifficulty level is high.",
                                    "Zookeeper\n10,000 Gold\nThe summons fight for you.",
                                    "Wizard\n50,000 Gold\nIt uses great magic."};
            exText = new string[]{"Hunter\nDoubles all HP recovery.\nUse high-damage physical attacks.",
                                    "Paladin\nStart with double your health.\nDirectly collide with the enemy and fight back.",
                                    "Vagabond\nMovement speed is 20% faster.\nThrows all throwable objects at enemies.",
                                    "Zookeeper\nDamage is increased double.\nSummons allies to fight.",
                                    "Wizard\nUse magic",};
            helpExText.text = "Damage varies according to the combination of elements.\nCombination Table\nWind + Element = Spread\nYou will be hit once more with the damage of the currently attached element.\nLightning + Fire = Overload\nAn explosion occurs and nearby enemies Pushes back and deals damage.\nLightning + Ice = Super Conductive\nDamages 1.5 times.\nIce + Water = Freezing\nEnemies are frozen, but are frozen when hit with other elements (fire, lightning, stone). \nWater + electricity = electric shock\nYou take damage per second and stop moving for a while.\nWater + fire = evaporation\nInflicts 1.5 times the damage.\nFire + Ice = Melting\nInflicts 1.5 times the damage.\nWater = enemy characters get wet.\nFire = Deals 100% damage per second to enemies.\nIce = Slows enemy characters' movement speed.\nElectricity = Inflicts 50% damage per second to enemy characters.\nWind = Pushes back enemies.\nStone = Pushes enemies back and creates a barrier item with a certain chance.\nLight = Recovers HP by a certain amount of damage. .\n\nEach character has a unique stat.";
        } else if(language == "Korean"){
            helpText[0].text = "도움";
            helpText[1].text = "도움말";
            gameStartButtonText.text = "게임 시작";
            settingButtonText.text = "세팅";
            languageText.text = "언어";
            engButtonText.text = "영어";
            korButtonText.text = "한국어";
            characterButtonText.text = "캐릭터";
            inventoryButtonText.text = "능력치\n정보";
            stateLevelButtonText.text = "능력치\n레벨";
            characterBuyButtonText.text = "구입";
            stateInfoText.text = "정보창";
            characterText.text = "캐릭터";
            creatorText[0].text = "만든이";
            creatorText[1].text = "만든이";
            creatorText[2].text = "개발자 : 무시무시한스무디\n디자인 : pandayu\n\n사용 소스\n골드메탈 스튜디오 언데드 서바이벌\nDustyroom Casual Game Sound - One Shot SFX Pack\nBasic RPG Icons\nWarped Shooting Fx";
            exBuyText = new string[]{"성기사\n1,000 골드\n빛의 무기를 사용합니다.",
                                    "방랑자\n5,000 골드\n난이도가 높습니다.",
                                    "사육사\n10,000 골드\n소환물이 대신 싸워줍니다.",
                                    "마법사\n50,000 골드\n엄청난 마법을 사용합니다."};
            exText = new string[]{"헌터\n모든 체력회복을 2배로 회복합니다.\n데미지가 높은 물리적인 공격을 사용합니다.",
                                    "성기사\n체력을 2배로 시작합니다.\n직접 적군과 부딪히며 맞서 싸웁니다.",
                                    "방랑자\n이동속도가 20% 더 빠릅니다.\n적군에게 던질 수 있는 모든 물체를 던집니다.",
                                    "사육사\n데미지가 2배로 증가합니다.\n아군을 소환하여 싸웁니다.",
                                    "마법사\n마법을 사용합니다.."};
            helpExText.text = "원소 조합 따라 변화 데미지가 일어납니다.\n원소조합표\n바람 + 원소 = 확산\n현재 붙어있는 원소의 데미지로 한번 더 피격됩니다.\n번개 + 불 = 과부화\n폭발이 일어나 주변 적군을 밀치고 데미지를 줍니다.\n번개 + 얼음 = 초전도\n데미지를 1.5배 줍니다.\n얼음 + 물 = 빙결\n적군이 얼어붙습니다. 하지만 다른 원소(불, 번개, 돌)로 피격시 빙결이 풀려버립니다.\n물 + 전기 = 감전\n초당 데미지를 입고 잠시 이동을 멈춥니다.\n물 + 불 = 증발\n1.5배의 데미지가 들어갑니다.\n불 + 얼음 = 융해\n1.5배의 데미지가 들어갑니다.\n물 = 적군 캐릭터가 축축하게 젖습니다.\n불 = 적군에게 초당 100% 데미지를 입힙니다.\n얼음 = 적군 캐릭터의 이동속도가 느려집니다.\n전기 = 적군 캐릭터에게 초당 50% 데미지를 입힙니다.\n바람 = 적군을 밀어냅니다.\n돌 = 적군을 밀치고 특정 확률로 방어막 아이템을 생성합니다.\n빛 = 일정 데미지 만큼 체력을 회복합니다.\n\n캐릭터마다 고유 능력치가 있습니다.";
        }
    }
    public void GameStartButton()
    {
        PlayerPrefs.SetInt("CurStage", stage);
        PlayerPrefs.SetInt("GrobalGold", grobalGold);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
    void Update()
    {
        if(stage==1){
            stageBackButton.SetActive(false);
        } else {
            stageBackButton.SetActive(true);
        }
        if(stage == maxstage){
            stageNextButton.SetActive(false);
        } else {
            stageNextButton.SetActive(true);
        }

        //UI
        goldText.text = grobalGold.ToString();

        if(language == "English"){
            stageText.text = "Stage " + stage;
        } else if(language == "Korean"){
            stageText.text = "스테이지 " + stage;
        }
    }
    public void NextStageButton()
    {
        if(stage < clearStage){
            stage++;
        } else if (stage == clearStage) {
            if(language=="English") {
                toastText.text = "You must Clear Stage";
            } else if (language=="Korean") {
                toastText.text = "이전 스테이지를 깨야 합니다.";
            }
            toastTextObj.SetActive(true);
        } else if (stage == maxstage) {
            if(language=="English") {
                toastText.text = "All Clear!";
            } else if (language=="Korean") {
                toastText.text = "모두 클리어했습니다!";
            }
            toastTextObj.SetActive(true);
        }
    }
    public void BackStageButton()
    {
        if(stage != 1){
            stage--;
        }
    }
    public void ENGButton()
    {
        PlayerPrefs.SetString("Language", "English");
        language = PlayerPrefs.GetString("Language");
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }
    public void KORButton()
    {
        PlayerPrefs.SetString("Language", "Korean");
        language = PlayerPrefs.GetString("Language");
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }
    public void SettingPanelOn()
    {
        settingPanel.SetActive(true);
    }
    public void PanelOff()
    {
        settingPanel.SetActive(false);
        characterPanel.SetActive(false);
        stateLevelPanel.SetActive(false);
        helpPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        creatorInfoPannel.SetActive(false);
    }
    public void CharacterPanelButton()
    {
        characterPanel.SetActive(true);
        character = PlayerPrefs.GetString("Character");
        characterNum = PlayerPrefs.GetInt("CharacterNum");
        characterBuyButton.SetActive(false);
        for(int i=0;i<characterSelectButtonImage.Length;i++){ // 선택된 버튼을 제외한 나머지는 하얀색
            if(i==characterNum){
                characterSelectButtonImage[i].sprite = buttonSprite[0];
                characterExText.text = exText[i];
            } else {
                characterSelectButtonImage[i].sprite = buttonSprite[1];
            }
        }
        if(PlayerPrefs.HasKey("Hunter2")){
            characterButtonImage[1].color = new Color(1,1,1);
        } else {
            characterButtonImage[1].color = new Color(0,0,0);
        }
        if(PlayerPrefs.HasKey("Hunter3")){
            characterButtonImage[2].color = new Color(1,1,1);
        } else {
            characterButtonImage[2].color = new Color(0,0,0);
        }
        if(PlayerPrefs.HasKey("Hunter4")){
            characterButtonImage[3].color = new Color(1,1,1);
        } else {
            characterButtonImage[3].color = new Color(0,0,0);
        }
        if(PlayerPrefs.HasKey("Wizard")){
            characterButtonImage[4].color = new Color(1,1,1);
        } else {
            characterButtonImage[4].color = new Color(0,0,0);
        }
    }
    public void CharacterSelectButtonNum(int btnNum)
    {
        characterBuyNum = btnNum;
        if(btnNum==0){ // 헌터
            character = "Hunter";
            PlayerPrefs.SetInt("CharacterNum",btnNum);
            PlayerPrefs.SetString("Character",character);
            characterNum = btnNum;
            PlayerPrefs.Save();
            CharacterButtonImageSetUp(btnNum, true);
        } else if(btnNum==1){ // 헌터2
            CharacterSelect(btnNum, "Hunter2");
        } else if(btnNum==2){ // 헌터3
            CharacterSelect(btnNum, "Hunter3");
        } else if(btnNum==3){ // 헌터4
            CharacterSelect(btnNum, "Hunter4");
        } else if(btnNum==4){ // 위자드
            CharacterSelect(btnNum, "Wizard");
        }
    }
    void CharacterSelect(int btnNum, string name)
    {
        if(PlayerPrefs.HasKey(name)){
            character = name;
            PlayerPrefs.SetInt("CharacterNum",btnNum);
            PlayerPrefs.SetString("Character",character);
            characterNum = btnNum;
            PlayerPrefs.Save();
            CharacterButtonImageSetUp(btnNum, true);
        } else {
            characterBuyButton.SetActive(true);
            CharacterButtonImageSetUp(btnNum, false);
        }
    }
    void CharacterButtonImageSetUp(int btnNum, bool isHere)
    {
        for(int i=0;i<characterSelectButtonImage.Length;i++){ // 선택된 버튼을 제외한 나머지는 하얀색
            if(i == btnNum){
                characterSelectButtonImage[i].sprite = buttonSprite[0];
                if(i==0 || isHere){
                    characterExText.text = exText[i];
                    characterBuyButton.SetActive(false);
                } else {
                    characterExText.text = exBuyText[i-1];
                }
            } else {
                characterSelectButtonImage[i].sprite = buttonSprite[1];
            }
        }
    }
    public void StateLevelPanelButton()
    {
        // 해당 레벨을 불러와 그만큼 버튼을 활성화 시켜야 함 레벨마다 가격이 달라짐!

        stateLevelPanel.SetActive(true);
        // 공격력 무한으로 증가 가능 레벨당 10 증가 레벨 * 10골드
        // 체력 무한으로 증가 가능 레벨당 50 증가 레벨 * 10골드
        // 스피드 10레벨 만렙 레벨당 0.1포인트 증가 레벨 * 1,000골드
        // 크리티컬 확률 5레벨 만렙 레벨당 1% 증가 레벨 * 5,000골드
        // 크리티컬 데미지 5레벨 만렙 레벨당 5% 증가 레벨 * 10,000골드
        // 부활 1회 50,000 골드
        // 체력 회복량 증가 레벨당 50 증가 레벨 * 100골드
        if(language == "English"){
            stateLevelButtonTexts[0].text = "Attack Lv " + (stateLevelAtttack + 1) +"\n" + "Attack + " + (stateLevelAtttack + 1)*10 + "\n" + (stateLevelAtttack + 1)*10 + "Glod";
            stateLevelButtonTexts[1].text = "Health Lv " + (stateLevelLife + 1) +"\n" + "Health + " + (stateLevelLife + 1)*50 + "\n" + (stateLevelLife + 1)*10 + "Glod";
            if(stateLevelSpeed == 10){
                stateLevelButtonTexts[2].text = "Speed Lv Max";
            } else {
                stateLevelButtonTexts[2].text = "Speed Lv " + (stateLevelSpeed + 1) +"\n" + "Speed + " + (stateLevelSpeed + 1)*5 + "%" + "\n" + (stateLevelSpeed + 1)*1000 + "Glod";
            }
            if(stateLevelCriticalChance == 5){
                stateLevelButtonTexts[3].text = "Critical Chance Lv Max";
            } else {
                stateLevelButtonTexts[3].text = "Critical Chance Lv " + (stateLevelCriticalChance + 1) + "\n" + "Critical Chance + " + (stateLevelCriticalChance + 1) + "%" + "\n" + (stateLevelCriticalChance + 1)*5000 + "Glod";
            }
            if(stateLevelCriticalDamage == 5){
                stateLevelButtonTexts[4].text = "Critical Damage Lv Max";
            } else {
                stateLevelButtonTexts[4].text = "Critical Damage Lv " + (stateLevelCriticalDamage + 1) + "\n" + "Critical Damage + " + ((stateLevelCriticalDamage + 1) * 5) + "%" + "\n" + (stateLevelCriticalDamage + 1)*10000 + "Gold";
            }
            if(stateLevelReLife == 1){
                stateLevelButtonTexts[5].text = "Life Lv Max";
            } else {
                stateLevelButtonTexts[5].text = "Life Lv " + (stateLevelReLife + 1) + "\n" + "Life + " + (stateLevelReLife + 1) + "\n" + (stateLevelReLife + 1) * 50000 + "Gold";
            }
            stateLevelButtonTexts[6].text = "Amount of healing Lv " + (stateLevelLifeValue + 1) + "\n" + "Amount of healing + " + (stateLevelLifeValue + 1)*50 + "\n" + (stateLevelLifeValue + 1) * 100 + "Gold";

        } else if(language == "Korean") {
            stateLevelButtonTexts[0].text = "공격력 레벨 " + (stateLevelAtttack + 1) +"\n" + "공격력 + " + (stateLevelAtttack + 1)*10 + "\n" + (stateLevelAtttack + 1)*10 + "골드";
            stateLevelButtonTexts[1].text = "체력 레벨 " + (stateLevelLife + 1) + "\n" + "체력 + " + (stateLevelLife + 1)*50 + "\n" + (stateLevelLife + 1)*10 + "골드";
            if(stateLevelSpeed == 10){
                stateLevelButtonTexts[2].text = "이동속도 최대 레벨";
            } else {
                stateLevelButtonTexts[2].text = "이동속도 레벨 " + (stateLevelSpeed + 1) +"\n" + "이동속도 + " + (stateLevelSpeed + 1)*5 + "%" + "\n" + ((stateLevelSpeed + 1)*1000) + "골드";
            }
            if(stateLevelCriticalChance == 5){
                stateLevelButtonTexts[3].text = "크리티컬 확률 최대 레벨";
            } else {
                stateLevelButtonTexts[3].text = "크리티컬 확률 레벨 " + (stateLevelCriticalChance + 1) + "\n" + "크리티컬 확률 + " + (stateLevelCriticalChance + 1) + "%" + "\n" + (stateLevelCriticalChance + 1)*5000 + "골드";
            }
            if(stateLevelCriticalDamage == 5){
                stateLevelButtonTexts[4].text = "크리티컬 데미지 최대 레벨";
            } else {
                stateLevelButtonTexts[4].text = "크리티컬 데미지 레벨 " + (stateLevelCriticalDamage + 1) + "\n" + "크리티컬 데미지 + " + ((stateLevelCriticalDamage + 1) * 5) + "%" + "\n" + (stateLevelCriticalDamage + 1)*10000 + "골드";
            }
            if(stateLevelReLife == 1){
                stateLevelButtonTexts[5].text = "목숨 최대 레벨";
            } else {
                stateLevelButtonTexts[5].text = "목숨 레벨 " + (stateLevelReLife + 1) + "\n" + "부활 횟수 + " + (stateLevelReLife + 1) + "\n" + (stateLevelReLife + 1) * 50000 + "골드";
            }
            stateLevelButtonTexts[6].text = "힐량 레벨 " + (stateLevelLifeValue + 1) + "\n" + "힐량 + " + (stateLevelLifeValue + 1)*50 + "\n" + (stateLevelLifeValue + 1) * 100 + "골드";
        }
    }
    public void StateButton(int num)
    {
        if(num == 1){ // 공격력
            if(grobalGold >= (stateLevelAtttack + 1)*10){
                grobalGold -= (stateLevelAtttack + 1)*10;
                stateLevelAtttack++;
                PlayerPrefs.SetInt("StateLevelAtttack", stateLevelAtttack);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 2){ // 체력
            if(grobalGold >= (stateLevelLife + 1)*10){
                grobalGold -= (stateLevelLife + 1)*10;
                stateLevelLife++;
                PlayerPrefs.SetInt("StateLevelLife", stateLevelLife);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 3){ // 이동 속도
            if(stateLevelSpeed==10){
                toastTextObj.SetActive(true);
                if (language == "English") {
                    toastText.text = "This is the maximum level.";
                } else if (language == "Korean") {
                    toastText.text = "최대 레벨 입니다.";
                }
                return;
            }
            if(grobalGold >= (stateLevelSpeed + 1)*1000){
                grobalGold -= (stateLevelSpeed + 1)*1000;
                stateLevelSpeed++;
                PlayerPrefs.SetInt("StateLevelSpeed", stateLevelSpeed);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 4){ // 크리티컬 확률
            if(stateLevelCriticalChance==5){
                toastTextObj.SetActive(true);
                if (language == "English") {
                    toastText.text = "This is the maximum level.";
                } else if (language == "Korean") {
                    toastText.text = "최대 레벨 입니다.";
                }
                return;
            }
            if(grobalGold >= (stateLevelCriticalChance + 1)*5000){
                grobalGold -= (stateLevelCriticalChance + 1)*5000;
                stateLevelCriticalChance++;
                PlayerPrefs.SetInt("StateLevelCriticalChance", stateLevelCriticalChance);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 5){ // 크리티컬 데미지
            if(stateLevelCriticalDamage==5){
                toastTextObj.SetActive(true);
                if (language == "English") {
                    toastText.text = "This is the maximum level.";
                } else if (language == "Korean") {
                    toastText.text = "최대 레벨 입니다.";
                }
                return;
            }
            if(grobalGold >= (stateLevelCriticalDamage + 1)*10000){
                grobalGold -= (stateLevelCriticalDamage + 1)*10000;
                stateLevelCriticalDamage++;
                PlayerPrefs.SetInt("StateLevelCriticalDamage", stateLevelCriticalDamage);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 6){ // 목숨
            if(stateLevelReLife==1){
                toastTextObj.SetActive(true);
                if (language == "English") {
                    toastText.text = "This is the maximum level.";
                } else if (language == "Korean") {
                    toastText.text = "최대 레벨 입니다.";
                }
                return;
            }
            if(grobalGold >= (stateLevelReLife + 1)*50000){
                grobalGold -= (stateLevelReLife + 1)*50000;
                stateLevelReLife++;
                PlayerPrefs.SetInt("StateLevelReLife", stateLevelReLife);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        } else if(num == 7){ // 힐량
            if(grobalGold >= (stateLevelLifeValue + 1)*50){
                grobalGold -= (stateLevelLifeValue + 1)*50;
                stateLevelLifeValue++;
                PlayerPrefs.SetInt("StateLevelLifeValue", stateLevelLifeValue);
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.Save();
                StateLevelPanelButton();
            } else {
                GoldLess();
            }
        }
    }
    void GoldLess()
    {
        toastTextObj.SetActive(true);
        if (language == "English") {
            toastText.text = "Gold not enough.";
        } else if (language == "Korean") {
            toastText.text = "골드가 부족합니다.";
        }
    }
    public void HelpButton()
    {
        helpPanel.SetActive(true);
    }
    public void InventoryPanelButton()
    {            
        inventoryPanel.SetActive(true);
        stateInfoImage.sprite = stateInfoCharacterImage[characterNum];
        if(language == "English"){
            stateInfoExText.text = "Bonus attack power : " + ((character=="Hunter4"? 2 : 1) * (stateLevelAtttack*10))
                                    + "\nHealth : " + ((character=="Hunter2"? 2 : 1) * (1000 + stateLevelLife*50))
                                    + "\nCritical Chance : " + (0 + stateLevelCriticalChance)+ "%"
                                    + "\nCritical Damage : " + (150 + (stateLevelCriticalDamage*5))+ "%"
                                    + "\nSpeed : " + ((character=="Hunter3"? 20 : 0) + (100 + (stateLevelSpeed*5))) +"%"
                                    + "\nAttack speed : " + 1f + "Second"
                                    + "\nAmount of Healing : " + ((character=="Hunter"? 2 : 1) * (200 + stateLevelLifeValue*50))
                                    + "\nLife : " + (stateLevelReLife + 1);
        } else if(language == "Korean"){
            stateInfoExText.text = "추가 공격력 : " + ((character=="Hunter4"? 2 : 1) * (stateLevelAtttack*10))
                                    + "\n체력 : " + ((character=="Hunter2"? 2 : 1) * (1000 + stateLevelLife*50))
                                    + "\n크리티컬 확률 : " + (0 + stateLevelCriticalChance)+ "%"
                                    + "\n크리티컬 데미지 : " + (150 + (stateLevelCriticalDamage*5))+ "%"
                                    + "\n이동 속도 : " + ((character=="Hunter3"? 20 : 0) + (100 + (stateLevelSpeed*5))) +"%"
                                    + "\n공격 속도 : " + 1f + "초"
                                    + "\n체력 회복 : " + ((character=="Hunter"? 2 : 1) * (200 + stateLevelLifeValue*50))
                                    + "\n목숨 : " + (stateLevelReLife + 1);
        }
    }

    //광고 함수
    /*private void RequestBanner()
    {
        // 주의! 실행 전 꼭 테스트로 실행 할 것!
        // 테스트 ca-app-pub-3940256099942544/6300978111
        // 광고 ca-app-pub-4730748511418289/9025837610
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Clean up banner ad before creating a new one.
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }*/
    public void CreatorButton()
    {
        creatorInfoPannel.SetActive(true);
    }
    public void BuyButton()
    {
        if(characterBuyNum==1){ // 성기사 1,000골드  
            if(grobalGold >= 1000){
                grobalGold -= 1000;
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.SetString("Hunter2", "Hunter2");
                CharacterPanelButton();
                PlayerPrefs.Save();
            } else {
                GoldLess();
            }
        } else if(characterBuyNum==2){ // 방랑자 5,000 골드
            if(grobalGold >= 5000){
                grobalGold -= 5000;
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.SetString("Hunter3", "Hunter3");
                CharacterPanelButton();
                PlayerPrefs.Save();
            } else {
                GoldLess();
            }
        } else if(characterBuyNum==3){ // 사육사 10,000 골드
            if(grobalGold >= 10000){
                grobalGold -= 10000;
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.SetString("Hunter4", "Hunter4");
                CharacterPanelButton();
                PlayerPrefs.Save();
            } else {
                GoldLess();
            }
        } else if(characterBuyNum==4){ // 마법사 50,000 골드
            if(grobalGold >= 50000){
                grobalGold -= 50000;
                PlayerPrefs.SetInt("GrobalGold", grobalGold);
                PlayerPrefs.SetString("Wizard", "Wizard");
                CharacterPanelButton();
                PlayerPrefs.Save();
            } else {
                GoldLess();
            }
        }
    }
}