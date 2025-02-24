public class CrafterController : NPCControllerBase
{
    public override string nameKey => "工匠";

    protected override void MainInteraction()
    {
        PlayerManager.Instance.OpenCraft(configName);
    }
}
