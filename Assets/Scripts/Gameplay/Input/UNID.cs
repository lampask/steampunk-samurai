using System;
using SharpDX.XInput;

namespace Gameplay.Input
{
    // Unified identification
    public class Unid
    {
        public string idBase { get; private set;}
        
        public Unid(string idBase) { this.idBase = idBase; }
        public Unid(UserIndex idBase) { this.idBase = ((int) idBase).ToString(); }
        public Unid(Guid idBase) { this.idBase = idBase.ToString(); }
        
        public bool isXInput => !Guid.TryParse(idBase, out _);
        public Guid ExtractGuid() { return Guid.Parse(idBase); }
        public UserIndex ExtractUserIndex() { return (UserIndex) int.Parse(idBase); }
        
        public override int GetHashCode()             
        {  
            return idBase.GetHashCode(); 
        }
        public override bool Equals(object obj) 
        { 
            return Equals(obj as Unid); 
        }

        public bool Equals(Unid obj)
        { 
            return obj != null && obj.idBase == this.idBase; 
        }
    }
}