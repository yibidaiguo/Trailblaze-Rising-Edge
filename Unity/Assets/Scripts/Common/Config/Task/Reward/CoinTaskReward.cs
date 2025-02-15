public class CoinTaskReward : TaskRewardBase
{
    public int count;
    public override void ConverFromString(string valueString)
    {
        count = int.Parse(valueString);
    }
}
