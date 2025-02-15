public class CollectItemTaskInfo : TaskInfoBase
{
    public string targetItemId;
    public int count;
    public override void ConverFromString(string valueString)
    {
        string[] args = valueString.Split(',');
        targetItemId = args[0];
        count = int.Parse(args[1]);
    }
    public override int GetCount()
    {
        return count;
    }
}
