using Management;

namespace Console.Commands
{
    public sealed class InputDebugCommand : AbstractCommand
    {
        public override string name { get; protected set; }
        public override string command { get; protected set; }
        public override string description { get; protected set; }
        public override string help { get; protected set; }
        
        public override bool OnCommand(string[] args)
        {
            if (args.Length == 1)
            {
                GlobalGameManager.instance.ToggleInput();
                GlobalConsole.Log("Input debug toggled");
                return true;
            }
            else if (args.Length == 2)
            {
                if (int.TryParse(args[1], out var id))
                {
                    if (0 <= id && id < 4) { return true; }
                    GlobalConsole.Error("Player with this id was not found");
                    return true;
                }
                GlobalConsole.Error("Invalid player id provided");
                return true;
            }
            return false;
        }

        public InputDebugCommand()
        {
            name = "InputDebugCommand";
            command = "input";
            description = "Toggles input debug information on screen.";
            help = "input (player_id)";
            
            // Integrate
            GlobalConsole.instance.AddCommand(command, this);
        }
    }
}