using UnityEngine;
using UnityEditor;

namespace Console.Commands
{
    public sealed class QuitCommand : AbstractCommand
    {
        public override string name { get; protected set; }
        public override string command { get; protected set; }
        public override string description { get; protected set; }
        public override string help { get; protected set; }
        
        public override bool OnCommand(string[] args)
        {
            if (args.Length == 1)
            {
                GlobalConsole.Log("Shutting down");
                if (Application.isEditor)
                {
                    #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                    #endif
                }
                else
                {
                    Application.Quit();
                }
                return true;
            }
            return false;
        }

        public QuitCommand()
        {
            name = "QuitCommand";
            command = "quit";
            description = "Causes game to close itself.";
            help = "quit";
            
            // Integrate
            GlobalConsole.instance.AddCommand(command, this);
        }
    }
}