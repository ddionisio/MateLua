namespace M8.Lua {
    [System.Flags]
    public enum MateCoreModules {
        Input = 0x1,
        Localizer = 0x2,
        SceneManager = 0x4,
        SceneState = 0x8,
    }

    internal static class MateCoreModules_Ext {
        public static bool Check(this MateCoreModules flags, MateCoreModules mask) {
            return (flags & mask) == mask;
        }
    }
}