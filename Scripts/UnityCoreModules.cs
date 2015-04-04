namespace M8.Lua {
    [System.Flags]
    public enum UnityCoreModules {
        Time = 0x1,
        Math = 0x2,
        Coroutine = 0x4,
    }

    internal static class UnityCoreModules_Ext {
        public static bool Check(this UnityCoreModules flags, UnityCoreModules mask) {
            return (flags & mask) == mask;
        }
    }
}