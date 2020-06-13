using System;
using SharpDX.XInput;

namespace Gameplay.Input
{
    // Unified identification
    public class Unid
    {
        public string idBase { get; private set;}
        
        public Unid(string idBase) { this.idBase = idBase; }
        public Unid(UserIndex idBase) { this.idBase = idBase.ToString(); }
        public Unid(Guid idBase) { this.idBase = idBase.ToString(); }
        
        public bool isXInput => !Guid.TryParse(idBase, out _);
        public Guid ExtractGuid() { return Guid.Parse(idBase); }
        public UserIndex ExtractUserIndex() { return (UserIndex) int.Parse(idBase); }
    }
}