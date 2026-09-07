using System.Collections.Generic;

namespace AriesContador.Core.Models.Permissions
{
    public class ModulePermission
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public int UserType { get; set; }
        public bool HasAccess { get; set; }
        public List<WindowPermission> Windows { get; set; } = new List<WindowPermission>();
    }

    public class WindowPermission
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Comments { get; set; }
        public bool Active { get; set; }
        public bool HasAccess { get; set; }
        public bool CanInsert { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanRemove { get; set; }
        public bool CanList { get; set; }
    }
}
