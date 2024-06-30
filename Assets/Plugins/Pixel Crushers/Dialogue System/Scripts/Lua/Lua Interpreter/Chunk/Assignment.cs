using System;
using System.Collections.Generic;

namespace Language.Lua
{
    public partial class Assignment : Statement
    {

        // [PixelCrushers] Supports monitoring of variable changes:
        // Local variable assignments take the form: x = 3
        public static HashSet<string> MonitoredLocalVariables = new HashSet<string>();
        public static System.Action<string, object> LocalVariableChanged = null;
        // Variable assignments are in the Variable[] table: Variable["x"] = 3
        public static HashSet<string> MonitoredVariables = new HashSet<string>();
        public static System.Action<string, object> VariableChanged = null;
        private static LuaValue VariableTableToMonitor = null;

        public static void InitializeVariableMonitoring()
        {
            MonitoredLocalVariables = new HashSet<string>();
            LocalVariableChanged = null;
            MonitoredVariables = new HashSet<string>();
            VariableChanged = null;
            VariableTableToMonitor = null;
        }

        public static void InvokeVariableChanged(string variable, object value)
        {
            VariableChanged?.Invoke(variable, value);
        }

        public override LuaValue Execute(LuaTable enviroment, out bool isBreak)
        {
            //[PixelCrushers] LuaValue[] values = this.ExprList.ConvertAll(expr => expr.Evaluate(enviroment)).ToArray();
			LuaValue[] values = LuaInterpreterExtensions.EvaluateAll(this.ExprList, enviroment).ToArray();

            LuaValue[] neatValues = LuaMultiValue.UnWrapLuaValues(values);

            for (int i = 0; i < Math.Min(this.VarList.Count, neatValues.Length); i++)
            {
                Var var = this.VarList[i];

                if (var.Accesses.Count == 0)
                {
                    VarName varName = var.Base as VarName;

                    if (varName != null)
                    {
                        SetKeyValue(enviroment, new LuaString(varName.Name), values[i]);
                        if (varName.Name == "Variable")
                        {
                            VariableTableToMonitor = values[0];
                        }
                        if (MonitoredLocalVariables.Contains(varName.Name) && values.Length >= 1) //[PixelCrushers]
                        {
                            try
                            {
                                LocalVariableChanged?.Invoke(varName.Name, values[0].Value);
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.LogException(e);
                            }
                        }
                        continue;
                    }
                }
                else
                {
                    LuaValue baseValue = var.Base.Evaluate(enviroment);

                    for (int j = 0; j < var.Accesses.Count - 1; j++)
                    {
                        Access access = var.Accesses[j];

                        baseValue = access.Evaluate(baseValue, enviroment);
                    }

                    Access lastAccess = var.Accesses[var.Accesses.Count - 1];

                    NameAccess nameAccess = lastAccess as NameAccess;
                    if (nameAccess != null)
                    {
						if (baseValue == null || (baseValue is LuaNil)) {
							throw new System.NullReferenceException("Cannot assign to a null value. Are you trying to assign to a nonexistent table element?.");
						}
                        SetKeyValue(baseValue, new LuaString(nameAccess.Name), values[i]);
                        continue;
                    }

                    KeyAccess keyAccess = lastAccess as KeyAccess;
                    if (lastAccess != null)
                    {
                        SetKeyValue(baseValue, keyAccess.Key.Evaluate(enviroment), values[i]);
                    }
                }
            }

            isBreak = false;
            return null;
        }

        private static void SetKeyValue(LuaValue baseValue, LuaValue key, LuaValue value)
        {
            LuaValue newIndex = LuaNil.Nil;
            LuaTable table = baseValue as LuaTable;
            if (table != null)
            {
                try
                {
                    if (table.ContainsKey(key))
                    {
                        table.SetKeyValue(key, value);
                        return;
                    }
                    else
                    {
                        if (table.MetaTable != null)
                        {
                            newIndex = table.MetaTable.GetValue("__newindex");
                        }

                        if (newIndex == LuaNil.Nil)
                        {
                            table.SetKeyValue(key, value);
                            return;
                        }
                    }
                }
                finally
                {
                    if (baseValue == VariableTableToMonitor && key != null && value != null) //[PixelCrushers]
                    {
                        if (MonitoredVariables.Contains(key.ToString()))
                        {
                            try
                            {
                                VariableChanged?.Invoke(key.ToString(), value.Value);
                            }
                            catch (Exception e)
                            {
                                UnityEngine.Debug.LogException(e);
                            }
                        }
                    }
                }
            }
            else
            {
                LuaUserdata userdata = baseValue as LuaUserdata;
                if (userdata != null)
                {
                    if (userdata.MetaTable != null)
                    {
                        newIndex = userdata.MetaTable.GetValue("__newindex");
                    }

                    if (newIndex == LuaNil.Nil)
                    {
                        throw new Exception("Assign field of userdata without __newindex defined.");
                    }
                }
            }

            LuaFunction func = newIndex as LuaFunction;
            if (func != null)
            {
                func.Invoke(new LuaValue[] { baseValue, key, value });
            }
            else
            {
                SetKeyValue(newIndex, key, value);
            }
        }
    }
}
