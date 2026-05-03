using System;

namespace AdvancedDebugTool
{
    public struct MethodContext : IEquatable<MethodContext>
    {
        public const uint DEFAULT_ID = 0;
        public const uint DEBUG_TOOL_VIEW_ID = 1;
        public const uint FIRST_METHOD_ID = 2;
        
        public uint InstanceId { get; set; }
        public uint MethodId { get; set; }
        
        
        public static bool operator ==(MethodContext lhs, MethodContext rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(MethodContext lhs, MethodContext rhs)
        {
            return !lhs.Equals(rhs);
        }

        public bool Equals(MethodContext other)
        {
            return InstanceId == other.InstanceId && MethodId == other.MethodId;
        }

        public override bool Equals(object obj)
        {
            return obj is MethodContext other && Equals(other);
        }

        public int GetHashCode(MethodContext obj)
        {
            return HashCode.Combine(obj.InstanceId, obj.MethodId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InstanceId, MethodId);
        }
    }
}