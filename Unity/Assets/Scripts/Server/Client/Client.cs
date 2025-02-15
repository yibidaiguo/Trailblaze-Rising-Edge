using JKFrame;

public class Client
{
    public ulong clientID;
    public ClientState clientState;
    public PlayerData playerData;
    public PlayerServerController playerController;

    public void OnDestroy()
    {
        playerData = null;
        this.ObjectPushPool();
    }
}
