public class MerchantController : NPCControllerBase
{
    public override string nameKey => "商人";

    protected override void MainInteraction()
    {
        PlayerManager.Instance.OpenShop(configName);
    }
}
