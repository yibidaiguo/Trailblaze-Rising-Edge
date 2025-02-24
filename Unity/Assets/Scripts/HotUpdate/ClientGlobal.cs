using JKFrame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ClientGlobal : SingletonMono<ClientGlobal>
{
    [SerializeField] private ClientGlobalConfig config;
    public ClientGlobalConfig Config => config;
    public GameSetting gameSetting { get; private set; }
    public GameBasicSetting basicSetting { get; private set; }
    public static Vector2 canvasSize => new Vector2(Screen.width, Screen.height);
    private bool activeMouse;
    public bool ActiveMouse
    {
        get => activeMouse;
        set
        {
            activeMouse = value;
            Cursor.lockState = activeMouse ? CursorLockMode.None : CursorLockMode.Locked;
            EventSystem.TypeEventTrigger(new MouseActiveStateChangedEvent { activeState = activeMouse });
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        LoadGameSetting();
        NetworkVaribaleSerializationBinder.Init();
        InitWindowData();
        InitNetworkSideControllerTypeDic();
        LocalizationSystem.GlobalConfig = ResSystem.LoadAsset<LocalizationConfig>("GlobalLocalizationConfig");
        ResSystem.InstantiateGameObject<NetManager>("NetworkManager").FirstInit();
        EventSystem.AddTypeEventListener<GameSceneLanunchEvent>(OnGameSceneLanunchEvent);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_Disonnect, OnDisonnect);
        JKLog.Succeed("InitClient");
        EnterLoginScene();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActiveMouse = !ActiveMouse;
        }
    }

    private void InitWindowData()
    {
        UISystem.AddUIWindowData<UI_MainMenuWindow>(new UIWindowData(false, nameof(UI_MainMenuWindow), 0));
        UISystem.AddUIWindowData<UI_MessagePopupWindow>(new UIWindowData(true, nameof(UI_MessagePopupWindow), 4));
        UISystem.AddUIWindowData<UI_RegisterWindow>(new UIWindowData(false, nameof(UI_RegisterWindow), 1));
        UISystem.AddUIWindowData<UI_LoginWindow>(new UIWindowData(false, nameof(UI_LoginWindow), 1));
        UISystem.AddUIWindowData<UI_GamePopupWindow>(new UIWindowData(false, nameof(UI_GamePopupWindow), 3));
        UISystem.AddUIWindowData<UI_GameSettingsWindow>(new UIWindowData(false, nameof(UI_GameSettingsWindow), 3));
        UISystem.AddUIWindowData<UI_ChatWindow>(new UIWindowData(false, nameof(UI_ChatWindow), 2));
        UISystem.AddUIWindowData<UI_BagWindow>(new UIWindowData(true, nameof(UI_BagWindow), 2));
        UISystem.AddUIWindowData<UI_ItemInfoPopupWindow>(new UIWindowData(true, nameof(UI_ItemInfoPopupWindow), 2));
        UISystem.AddUIWindowData<UI_ShortcutBarWindow>(new UIWindowData(false, nameof(UI_ShortcutBarWindow), 2));
        UISystem.AddUIWindowData<UI_ShopWindow>(new UIWindowData(true, nameof(UI_ShopWindow), 2));
        UISystem.AddUIWindowData<UI_CraftWindow>(new UIWindowData(true, nameof(UI_CraftWindow), 2));
        UISystem.AddUIWindowData<UI_PlayerInfoWindow>(new UIWindowData(true, nameof(UI_PlayerInfoWindow), 1));
        UISystem.AddUIWindowData<UI_NPCInteractionMenuWindow>(new UIWindowData(true, nameof(UI_NPCInteractionMenuWindow), 2));
        UISystem.AddUIWindowData<UI_DialogWindow>(new UIWindowData(false, nameof(UI_DialogWindow), 3));
        UISystem.AddUIWindowData<UI_TaskWindow>(new UIWindowData(false, nameof(UI_TaskWindow), 2));
        UISystem.AddUIWindowData<UI_TaskGuiderWindow>(new UIWindowData(false, nameof(UI_TaskGuiderWindow), 2));
        UISystem.AddUIWindowData<UI_DeepSeekDialog>(new UIWindowData(false, nameof(UI_DeepSeekDialog), 3));
    }

    private void InitNetworkSideControllerTypeDic()
    {
        NetworkEntityBase.sideControllerTypeDic = new System.Collections.Generic.Dictionary<System.Type, System.Type>
        {
            { typeof(PlayerController),typeof(PlayerClientController)},
            { typeof(MonsterController),typeof(MonsterClientController)},
            { typeof(BullectController),typeof(BullectClientController)},
        };
    }

    private void OnGameSceneLanunchEvent(GameSceneLanunchEvent @event)
    {
        ResSystem.InstantiateGameObject("ClientOnGameScene");
    }

    private void LoadGameSetting()
    {
        gameSetting = SaveSystem.LoadSetting<GameSetting>();
        basicSetting = SaveSystem.LoadSetting<GameBasicSetting>();
        if (gameSetting == null)
        {
            gameSetting = new GameSetting();
            SaveGameSetting();
        }
        if (basicSetting == null)
        {
            basicSetting = new GameBasicSetting();
            basicSetting.languageType = Application.systemLanguage == SystemLanguage.ChineseSimplified ? LanguageType.SimplifiedChinese : LanguageType.English;
            SaveGameSetting();
        }
        LocalizationSystem.LanguageType = basicSetting.languageType;
        AudioSystem.BGVolume = gameSetting.musicVolume;
        AudioSystem.EffectVolume = gameSetting.soundEffectVolume;
    }

    public void SaveGameSetting()
    {
        SaveSystem.SaveSetting(gameSetting);
    }

    public void SaveGameBasicSetting()
    {
        SaveSystem.SaveSetting(basicSetting);
    }

    public void RemerberAccount(string name, string password)
    {
        gameSetting.remerberPlayerName = name;
        gameSetting.remerberpassword = password;
        SaveGameSetting();
    }

    public void EnterLoginScene()
    {
        UISystem.CloseAllWindow();
        Addressables.LoadSceneAsync("LoginScene").WaitForCompletion();
    }

    public void EnterGameScene()
    {
        UISystem.CloseAllWindow();
        SceneSystem.LoadScene("GameScene");
    }

    private void OnDisonnect(ulong clientID, INetworkSerializable serializable)
    {
        S_C_Disonnect message = (S_C_Disonnect)serializable;
        if (message.errorCode != ErrorCode.None)
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(message.errorCode.ToString(), Color.red);
            Invoke(nameof(EnterLoginScene), 1);
        }
        else EnterLoginScene();
    }
}
