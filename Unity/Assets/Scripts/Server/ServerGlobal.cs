using JKFrame;
using UnityEngine;

public class ServerGlobal : SingletonMono<ServerGlobal>
{
    [SerializeField] private ServerConfig serverConfig;
    public ServerConfig ServerConfig { get => serverConfig; }
    public LayerMask PlayerLayerMask { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitNetworkSideControllerTypeDic();
        PlayerLayerMask = LayerMask.GetMask("Player");
        DontDestroyOnLoad(gameObject);
        EventSystem.AddTypeEventListener<GameSceneLanunchEvent>(OnGameSceneLanunchEvent);
    }

    private void OnGameSceneLanunchEvent(GameSceneLanunchEvent @event)
    {
        ServerResSystem.InstantiateServerOnGameScene();
    }
    private void InitNetworkSideControllerTypeDic()
    {
        NetworkEntityBase.sideControllerTypeDic = new System.Collections.Generic.Dictionary<System.Type, System.Type>
        {
            { typeof(PlayerController),typeof(PlayerServerController)},
            { typeof(MonsterController),typeof(MonsterServerController)},
            { typeof(BullectController),typeof(BullectServerController)},
        };
    }
}
