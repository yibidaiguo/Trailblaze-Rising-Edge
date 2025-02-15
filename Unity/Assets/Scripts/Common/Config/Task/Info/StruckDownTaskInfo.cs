public class StruckDownTaskInfo : TaskInfoBase
{
    public string targetMonsterKeyword; // 怪物的名称包含这个关键字就可以
    public int count;

    public override void ConverFromString(string valueString)
    {
        string[] args = valueString.Split(',');
        targetMonsterKeyword = args[0];
        count = int.Parse(args[1]);
    }
    public override int GetCount()
    {
        return count;
    }
}
