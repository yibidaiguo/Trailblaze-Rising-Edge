public class DialogTaskInfo : TaskInfoBase
{
    public string npcID;
    public string dialogConfigId;
    public override void ConverFromString(string valueString)
    {
        string[] args = valueString.Split(',');
        npcID = args[0];
        dialogConfigId = args[1];
    }
}
