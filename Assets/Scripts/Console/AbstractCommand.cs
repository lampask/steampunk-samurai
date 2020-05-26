using UnityEngine.Events;
namespace Console
{
    [System.Serializable]
    public class CommandEvent : UnityEvent<string[]> {  }

    public abstract class AbstractCommand : ICommand {

        public abstract string name {get; protected set;}
        public abstract string command {get; protected set;}
        public abstract string description {get; protected set;}
        public abstract string help {get; protected set;}
        public CommandEvent process { get; }

        public abstract bool OnCommand(string[] args);

        public AbstractCommand() {
            if (process == null)
                process = new CommandEvent();
            process.AddListener((args) => {
                if (!OnCommand(args)) {
                    GlobalConsole.Error($"Usage: {help}");
                }
            });
        }
    }
}