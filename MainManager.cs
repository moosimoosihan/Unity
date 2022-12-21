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
    int gold;
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

    //인벤토리 함수
    public Image[] slutImage;
    public GameObject inventoryInfoPanel;
    public Image infoImage;
    public Text infoText;
    public Text infoExText;
    public Sprite[] infoImageSprite;
    string curItem; // 현재 보고있는 아이템

    // 스탯 레벨
    int stateLevel;
    public Text[] stateLevelButtonTexts;

    //광고
    //private BannerView bannerView;


    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });
        //RequestBanner();
        
        language = "English";
        character = "Wizard";
        characterNum = 0;
        clearStage = 1;
        if(PlayerPrefs.HasKey("GrobalGold")){ // 골드가 저장된게 있다면
            grobalGold = PlayerPrefs.GetInt("GrobalGold");
        }
        if(PlayerPrefs.HasKey("Character")){ // 저장된 케릭터가 있다면
            character = PlayerPrefs.GetString("Character");
        }
        if(PlayerPrefs.HasKey("CharacterNum")){ // 저장된 케릭터가 있다면
            characterNum = PlayerPrefs.GetInt("CharacterNum");
        }
        if(PlayerPrefs.HasKey("Language")){ // 저장된 언어가 있다면
            language = PlayerPrefs.GetString("Language");
        }
        if(PlayerPrefs.HasKey("ClearStage")){ // 저장된 클리어 스테이지 수 가 있다면
            clearStage = PlayerPrefs.GetInt("ClearStage");
        }
        if(PlayerPrefs.HasKey("StateLevel")){ // 스탯 저장된 내용이 있다면
            stateLevel = PlayerPrefs.GetInt("AttackLevel");
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
            inventoryButtonText.text = "Inventory";
            stateLevelButtonText.text = "Level";
            characterText.text = "Character";
            exText = new string[]{"Wizard\nCovers fire, wind, ice, water, and lightning elements that can\ndiffuse, superconduct, superconduct, melt, evaporate, or freeze.",
                                    "Hunter1\nDoubles all HP recovery.\nUse high-damage physical attacks.",
                                    "Hunter2\nStart with double your health.\nDirectly collide with the enemy and fight back.",
                                    "Hunter3\nMovement speed is 20% faster.\nThrows all throwable objects at enemies.",
                                    "Hunter4\nDamage is increased double.\nSummons allies to fight."};
            helpExText.text = "Wizards can use magic in various combinations.\nDamage varies depending on the combination of elements.\nElement Combination Table\nWind + Element = Spread\nYou will be hit once more by the damage of the currently attached element. .\nLightning + Fire = Overload\nAn explosion occurs, knocking back nearby enemies and dealing damage.\nLightning + Ice = Superconductivity\nDefense by 1.5x.\nIce + Water = Freezing\nEnemies are frozen. However, if you are hit with another element (fire, lightning, stone), the ice will break.\nWater + Electricity = Electrocution\nYou take damage per second and stop moving for a while.\nWater + Fire = Evaporation\n1.5 Takes double damage.\nFire + Ice = Melting\nInflicts 1.5 times the damage.\nWater = Enemies get wet.\nFire = Deals 100% damage per second to enemies. .\nIce = slows down the movement speed of enemy characters.\nElectricity = deals 50% damage per second to enemy characters.\nWind = pushes back enemies.\nStone = pushes back enemies and gives a certain chance of a shield item creates.\nLight = Recovers HP by a certain amount of damage.\n\nHunter\nCan deal physical damage.\nEach character has unique stats.";
        } else if(language == "Korean"){
            helpText[0].text = "도움";
            helpText[1].text = "도움말";
            gameStartButtonText.text = "게임 시작";
            settingButtonText.text = "세팅";
            languageText.text = "언어";
            engButtonText.text = "영어";
            korButtonText.text = "한국어";
            characterButtonText.text = "캐릭터";
            inventoryButtonText.text = "장비";
            stateLevelButtonText.text = "레벨";
            characterText.text = "캐릭터";
            exText = new string[]{"마법사\n마법을 사용합니다..",
                                    "헌터1\n모든 체력회복을 2배로 회복합니다.\n데미지가 높은 물리적인 공격을 사용합니다.",
                                    "헌터2\n체력을 2배로 시작합니다.\n직접 적군과 부딪히며 맞서 싸웁니다.",
                                    "헌터3\n이동속도가 20% 더 빠릅니다.\n적군에게 던질 수 있는 모든 물체를 던집니다.",
                                    "헌터4\n데미지가 2배로 증가합니다.\n아군을 소환하여 싸웁니다."};
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
        toastText.text = "Replay please.";
        toastTextObj.SetActive(true);
        PlayerPrefs.Save();
    }
    public void KORButton()
    {
        PlayerPrefs.SetString("Language", "Korean");
        language = PlayerPrefs.GetString("Language");
        toastText.text = "다시 시작해주세요.";
        toastTextObj.SetActive(true);
        PlayerPrefs.Save();
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
    }
    public void CharacterPanelButton()
    {
        characterPanel.SetActive(true);
        character = PlayerPrefs.GetString("Character");
        characterNum = PlayerPrefs.GetInt("CharacterNum");
        for(int i=0;i<characterSelectButtonImage.Length;i++){ // 선택된 버튼을 제외한 나머지는 하얀색
            if(i==characterNum){
                characterSelectButtonImage[i].sprite = buttonSprite[0];
                characterExText.text = exText[i];
            } else {
                characterSelectButtonImage[i].sprite = buttonSprite[1];
            }
        }
    }
    public void CharacterSelectButtonNum(int btnNum)
    {
        PlayerPrefs.SetInt("CharacterNum",btnNum);
        PlayerPrefs.Save();
        for(int i=0;i<characterSelectButtonImage.Length;i++){ // 선택된 버튼을 제외한 나머지는 하얀색
            if(i==btnNum){
                characterSelectButtonImage[i].sprite = buttonSprite[0];
                characterExText.text = exText[i];
            } else {
                characterSelectButtonImage[i].sprite = buttonSprite[1];
            }
        }
    }
    public void CharacterSelectButton(string character)
    {
        PlayerPrefs.SetString("Character",character);
        PlayerPrefs.Save();
    }
    public void StateLevelPanelButton()
    {
        toastTextObj.SetActive(true);
        if (language == "English")
        {
            toastText.text = "Not yet...";
        }
        else if (language == "Korean")
        {
            toastText.text = "업데이트 중입니다..";
        }

        // 해당 레벨을 불러와 그만큼 버튼을 활성화 시켜야 함 레벨마다 가격이 달라짐!

        //stateLevelPanel.SetActive(true);

    }
    public void HelpButton()
    {
        helpPanel.SetActive(true);
    }
    public void InventoryInfoOut()
    {
        inventoryInfoPanel.SetActive(false);
    }
    public void InventoryPanelButton()
    {
        toastTextObj.SetActive(true);
        if (language == "English")
        {
            toastText.text = "Not yet...";
        }
        else if (language == "Korean")
        {
            toastText.text = "업데이트 중입니다..";
        }

        //착용하고 있는것이 있다면 불러와서 장착
        //if (PlayerPrefs.HasKey("Slut1")){
            
        //}
        //inventoryPanel.SetActive(true);
    }
    public void InventoryInfo(string name)
    {
        //장비에 따른 이미지와 텍스트를 인포에 넣어준다. 현재 창에 띄워져 있는 아이템 이름을 함수로 받아와 장착 여부 확인 및 장착 해제 할 수 있게 해야 됨!
        // 해당 아이템을 끼우고 있다면 아이템 해제 버튼, 아니라면 장비 버튼으로 바꿔야 함
        switch(name){
            case "Mag": //획득 반경 +1%
                infoImage.sprite = infoImageSprite[0];
                if(!PlayerPrefs.HasKey(name)){
                    if(language=="English"){
                        infoText.text = name + " " + "0";
                        infoExText.text = "Haven't gotten the item yet.";
                    } else if (language=="Korean"){
                        infoText.text = "자석 " + "0";
                        infoExText.text = "아직 획득하지 못하였습니다.";
                    }
                    curItem = "None";
                } else {
                    if(language=="English"){
                        infoText.text = name + " " + PlayerPrefs.GetInt(name);
                        infoExText.text = "Item acquisition radius is increased by "+ PlayerPrefs.GetInt(name)*100 +"%.";
                    } else if (language=="Korean"){
                        infoText.text = "자석 " + PlayerPrefs.GetInt(name);
                        infoExText.text = "아이템 획득 반경이 " + PlayerPrefs.GetInt(name)*100 + "% 증가합니다.";
                    }
                    curItem = name;
                }
            break;
            case "Power": // 데미지 +1

            break;
            case "Speed": // 이동 속도 +1%
                
            break;
            case "Delay": // 딜레이 + 0.1%
                
            break;
            case "BulletSizeUp": // 총알 크기 증가 0.1%
                
            break;
            case "Penetrate": // 관통 +1
                
            break;
            case "PlayerSizeDown": // 플레이어 크기 감소 0.1%
                
            break;
            case "BossPlusDamage": // 정예, 보스 추가데미지 +1
                
            break;
            case "BulletSpeed": // 총알 속도 + 0.1%
                
            break;
        }
        inventoryInfoPanel.SetActive(true);
    }
    public void EquipAndClearButton()
    {
        // 해당 아이템을 끼우고 있다면 아이템 해제 버튼, 아니라면 장비 버튼으로 실행
        if(curItem=="None"){// 아이템이 없을 경우
            if(language=="English"){
                toastText.text = "No item.";
            } else if(language=="Korean"){
                toastText.text = "아이템이 없습니다.";
            }
            toastTextObj.SetActive(true);
        } else if (curItem=="Mag"){ // 자석 아이템이라면
            //빈 슬롯에 해당 아이템을 장착하고 저장
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
}
