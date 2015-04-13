using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;

namespace M8.Lua {
    public static class MateCoreModuleRegister {
        public static Table RegisterMateCoreModules(this Table table, MateCoreModules modules) {
            //store all Unity related stuff to Unity table
            DynValue mateVal = DynValue.NewTable(table.OwnerScript);
            Table mateTable = mateVal.Table;

            table["Mate"] = mateVal;

            //if(modules.Check(MateCoreModules.Input)) mateTable.RegisterMateInput();

            return table;
        }
    }
}