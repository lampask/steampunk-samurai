namespace Console
{
    public interface ICommand
    {
        string command { get; }
        bool OnCommand(string[] args);
    }
}