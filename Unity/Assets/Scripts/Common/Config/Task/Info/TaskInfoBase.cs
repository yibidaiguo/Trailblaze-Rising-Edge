public abstract class TaskInfoBase
{
    public abstract void ConverFromString(string valueString);
    public virtual int GetCount() { return -1; }
}
