/****************************************************
	文件：SheetEditor.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/12 11:53   	
	Features：转表编辑器
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System;
using System.Text;
using  EdgeFramework;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using System.CodeDom.Compiler;


public class SheetEditor
{
    private const string SHEETROOTPATH = "Excels/xlsx";
    private const string SHEETEXT = ".xlsx";
    private static string OUTSHEETCSPATH = "Assets/Scripts/Global/Sheet/SheetProtobuf.cs";
    private static string OUTSHEETMANAGERPATH = "Assets/Scripts/Global/Sheet/SheetManager.cs";
    private static string OUTSHEETBYTES = "Assets/ABResources/Data/Sheets/{0}.bytes";
    private static string OUTSHEETLUA = "Excels/lua/{0}.lua";



    /// <summary>
    /// 默认情况下，表格只需导出以id为key的字典。特殊情况下，要在下面自定义key和导出数据的类型
    /// </summary>
    private static readonly Dictionary<string, SheetExportBase> mSheetExportConst = new Dictionary<string, SheetExportBase>
    {
        { "Example", new SheetExportBase("Example").SetKey("exampleInt") },
        { "PreloadSheet", new SheetExportBase("PreloadSheet").SetExportDataType(EExportDataType.ONLY_ARRAY) },
         { "UIPanelSheet", new SheetExportBase("UIPanelSheet").SetKeyType("UIPanelTypeEnum").SetKey("PanelType").SetExportDataType (EExportDataType.ONLY_DICT)},
           { "SceneSheet", new SheetExportBase("SceneSheet").SetKeyType("string").SetKey("Name").SetExportDataType (EExportDataType.ONLY_DICT)},
         { "MusicSheet", new SheetExportBase("MusicSheet").SetKeyType("MusicEnum").SetKey("MusicType").SetExportDataType (EExportDataType.ONLY_DICT)},
        { "SoundSheet", new SheetExportBase("SoundSheet").SetKeyType("SoundEnum").SetKey("SoundType").SetExportDataType (EExportDataType.ONLY_DICT)}

    };

    /// <summary>
    /// 枚举类型存储地方
    /// </summary>
    private static Dictionary<string, HashSet<string>> mSheetEnumDict = new Dictionary<string, HashSet<string>>();

 
    public  static void ExportBytes()
    {
        mSheetEnumDict.Clear();
        var sheetDir = new DirectoryInfo(SHEETROOTPATH);
        var files = sheetDir.GetFiles();
        var sheetDict = new Dictionary<string, DataTable>();

        //------------------------------生成CS--------------------------------
        var sheetCSSB = new StringBuilder();
        sheetCSSB.Append(LineText("/**"));
        sheetCSSB.Append(LineText(" * Tool generation, do not modify!!!"));
        sheetCSSB.Append(LineText(" */\n"));
        sheetCSSB.Append(LineText("using ProtoBuf;\nusing System.IO;\nusing System.Collections.Generic;\n"));
        sheetCSSB.Append(LineText("namespace EdgeFramework.Sheet"));
        sheetCSSB.Append(LineText("{"));
        sheetCSSB.Append(LineText(GetBaseArrayCode()));
        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            EditorUtility.DisplayProgressBar("Generate Protobuf CS", file.FullName, 1.0f * (i + 1) / files.Length);
            if (file.Extension != SHEETEXT) continue;
            var name = file.Name.Replace(SHEETEXT, "");
            sheetCSSB.Append(LineText("[ProtoContract]", 1));
            sheetCSSB.Append(LineText("public class " + name, 1));
            sheetCSSB.Append(LineText("{", 1));
            var dataSet = GetDataSet(file.FullName);
            var table = dataSet.Tables[0];
            sheetDict.Add(name, table);
            var cols = table.Columns.Count;
            var count = 0;
            for (var j = 0; j < cols; j++)
            {
                var row0 = table.Rows[0][j].ToString();
                if (FilterKey(row0, "client"))
                {
                    count++;
                    var row1 = table.Rows[1][j].ToString();
                    var row2 = table.Rows[2][j].ToString();
                    sheetCSSB.Append(LineText(string.Format("[ProtoMember({0})]", count), 2));
                    if (row1.StartsWith("array"))
                    {
                        var reg = new Regex("<(.*)>");
                        var match = reg.Match(row1);
                        var value = match.Groups[1].Value;
                        sheetCSSB.Append(LineText(string.Format("public List<{0}> {1} = new List<{0}>();", value, row2), 2));
                    }
                    else
                    {
                        sheetCSSB.Append(LineText(string.Format("public {0} {1};", row1, row2), 2));
                    }
                    if (row1.EndsWith("Enum"))
                    {
                        if (!mSheetEnumDict.ContainsKey(row1)) mSheetEnumDict.Add(row1, new HashSet<string>());
                        for (int k = 4; k < table.Rows.Count; k++)
                        {
                            var enumStr = table.Rows[k][j].ToString();
                            if (!mSheetEnumDict[row1].Contains(enumStr))
                            {
                                mSheetEnumDict[row1].Add(enumStr);
                            }
                        }
                    }
                }
            }
            sheetCSSB.Append(LineText("}\n", 1));
            sheetCSSB.Append(LineText("[ProtoContract]", 1));
            sheetCSSB.Append(LineText("public class " + name + "List : BaseList", 1));
            sheetCSSB.Append(LineText("{", 1));
            sheetCSSB.Append(LineText("[ProtoMember(1)]", 2));
            sheetCSSB.Append(LineText(string.Format("public List<{0}> Items = new List<{0}>();", name), 2));
            sheetCSSB.Append(LineText("}\n", 1));
        }
        //添加枚举
        foreach (var sheetEnum in mSheetEnumDict)
        {
            sheetCSSB.Append(LineText("public enum " + sheetEnum.Key, 1));
            sheetCSSB.Append(LineText("{", 1));
            foreach (var sheetEnumKey in sheetEnum.Value)
            {
                sheetCSSB.Append(LineText(sheetEnumKey + ",", 2));
            }
            sheetCSSB.Append(LineText("Max", 2));
            sheetCSSB.Append(LineText("}\n", 1));
        }
        sheetCSSB.Append(LineText("}"));

        Utility.FileHelper.WriteAllText(OUTSHEETCSPATH, sheetCSSB.ToString());
        Debug.Log("Generate Protobuf CS Done!");

        //------------------------------生成SheetManager------------------------------
        var sheetManagerSB = new StringBuilder();
        sheetManagerSB.Append(LineText("/**"));
        sheetManagerSB.Append(LineText(" * Tool generation, do not modify!!!"));
        sheetManagerSB.Append(LineText(" */\n"));
        sheetManagerSB.Append(LineText("using EdgeFramework.Sheet;\nusing EdgeFramework;\nusing System.Collections.Generic;\n"));
        sheetManagerSB.Append(LineText("public partial class SheetManager : Singleton<SheetManager>"));
        sheetManagerSB.Append(LineText("{"));
        sheetManagerSB.Append(LineText("SheetManager(){}", 1));
        var sheetCount = 0;
        foreach (var table in sheetDict)
        {
            sheetCount++;
            var sheetName = table.Key;
            EditorUtility.DisplayProgressBar("Generate SheetManager", sheetName, 1.0f * sheetCount / sheetDict.Count);
            SheetExportBase sheetExportBase;
            if (!mSheetExportConst.TryGetValue(sheetName, out sheetExportBase))
            {
                sheetExportBase = new SheetExportBase(sheetName);
            }

            string exportText = sheetExportBase.ExportScript();
            sheetManagerSB.Append(exportText);
        }

        sheetManagerSB.Append("}");

        Utility.FileHelper.WriteAllText(OUTSHEETMANAGERPATH, sheetManagerSB.ToString());
        Debug.Log("Generate SheetManager Done!");

        //------------------------------生成bytes-------------------------------------
        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters();
        parameters.ReferencedAssemblies.Add(Application.dataPath + "/EdgeFramework/Plugins/Protobuf/protobuf-net.dll");
#if UNITY_EDITOR_OSX
        var pathName = "PATH";
        var envPath = Environment.GetEnvironmentVariable(pathName);
        var monoPath = Path.Combine(EditorApplication.applicationContentsPath, "Mono/bin");
        Environment.SetEnvironmentVariable(pathName, envPath + ":" + monoPath, EnvironmentVariableTarget.Process);
#endif
        var result = provider.CompileAssemblyFromSource(parameters, sheetCSSB.ToString());
        if (result.Errors.Count == 0)
        {
            Debug.Log("SheetProtobuf Build Success!");
            var ass = result.CompiledAssembly;
            sheetCount = 0;
            foreach (var table in sheetDict)
            {
                sheetCount++;
                var name = table.Key;
                var data = table.Value;
                EditorUtility.DisplayProgressBar("Generate Bytes", name, 1.0f * sheetCount / sheetDict.Count);
                var listObj = ass.CreateInstance("EdgeFramework.Sheet." + name + "List");
                var list = listObj.GetType().GetField("Items").GetValue(listObj);
                var addMethod = list.GetType().GetMethod("Add");
                var rows = data.Rows.Count;
                var cols = data.Columns.Count;
                for (var i = 4; i < rows; i++)
                {
                    var obj = ass.CreateInstance("EdgeFramework.Sheet." + name);
                    var type = obj.GetType();
                    for (var j = 0; j < cols; j++)
                    {
                        var row0 = data.Rows[0][j].ToString();
                        if (!FilterKey(row0, "client")) continue;
                        var row1 = data.Rows[1][j].ToString();
                        var row2 = data.Rows[2][j].ToString();
                        var value = data.Rows[i][j];
                        if (row1.StartsWith("array"))
                        {
                            var reg = new Regex("<(.*)>");
                            var match = reg.Match(row1);
                            var valueType = match.Groups[1].Value;
                            var arrayValue = obj.GetType().GetField(row2).GetValue(obj);
                            var valueStr = value.ToString();
                            valueStr = valueStr.Substring(1, valueStr.Length - 2);
                            var valueList = valueStr.Split(',');
                            for (var k = 0; k < valueList.Length; k++)
                            {
                                switch (valueType)
                                {
                                    case "int":
                                        ((List<int>)arrayValue).Add(Convert.ToInt32(valueList[k]));
                                        break;
                                    case "float":
                                        ((List<float>)arrayValue).Add(Convert.ToSingle(valueList[k]));
                                        break;
                                    case "string":
                                        ((List<string>)arrayValue).Add(valueList[k]);
                                        break;
                                    case "bool":
                                        ((List<bool>)arrayValue).Add(Convert.ToBoolean(valueList[k]));
                                        break;
                                }
                            }
                        }
                        else if (row1.EndsWith("Enum"))
                        {
                            var enumInst = ass.CreateInstance("EdgeFramework.Sheet." + row1);
                            type.GetField(row2).SetValue(obj, (int)Enum.Parse(enumInst.GetType(), Convert.ToString(value)));
                        }
                        else
                        {
                            SetValue(type, row1, row2, obj, value, i, j, name);
                        }
                    }

                    addMethod.Invoke(list, new object[] { obj });
                }
                var exportMethod = listObj.GetType().GetMethod("Export");
                var outFile = string.Format(OUTSHEETBYTES, name);
                if (outFile.CheckFileAndCreateDirWhenNeeded())
                {
                    exportMethod.Invoke(listObj, new object[] { outFile });
                }


            }

            Debug.Log("Generate Bytes Done!");
        }
        else
        {
            Debug.LogError("SheetProtobuf Build Failed!");
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 过滤列
    /// </summary>
    /// <param name="key">列标志</param>
    /// <param name="target">导出目标</param>
    private static bool FilterKey(string key, string target)
    {
        return (!string.IsNullOrEmpty(key)) && (key.Equals("-") || key.Equals(target, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    private static void SetValue(Type type, string key1, string key2, object obj, object value, int row, int col, string name)
    {
        try
        {
            switch (key1)
            {
                case "int":
                    type.GetField(key2).SetValue(obj, Convert.ToInt32(value));
                    break;
                case "float":
                    type.GetField(key2).SetValue(obj, Convert.ToSingle(value));
                    break;
                case "string":
                    type.GetField(key2).SetValue(obj, Convert.ToString(value));
                    break;
                case "bool":
                    if (value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        type.GetField(key2).SetValue(obj, true);
                    }
                    else
                    {
                        type.GetField(key2).SetValue(obj, false);
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(("[{0}] [{1}] [ROW]:{2} [COL]:{3}", name, key1, row + 1, Convert.ToChar(col + 'A')));
        }
    }

    /// <summary>
    /// 获得每一行的字符串
    /// </summary>
    public static string LineText(string text, int tabCount = 0)
    {
        string ret = "";
        for (int i = 1; i <= tabCount; i++)
        {
            ret = Utility.TextHelper.Concat("\t", ret);
        }
        ret = Utility.TextHelper.Concat(ret, text);
        ret = Utility.TextHelper.Concat(ret, "\n");
        return ret;
    }

    /// <summary>
    /// Gets the base array code.
    /// </summary>
    private static string GetBaseArrayCode()
    {
        return @"    public class BaseList
    {
        public void Export(string outFile)
        {
            using (MemoryStream m = new MemoryStream())
            {
                Serializer.Serialize(m, this);
                m.Position = 0;
                int length = (int)m.Length;
                var buffer = new byte[length];
                m.Read(buffer, 0, length);
                File.WriteAllBytes(outFile, buffer);
            }
        }
    }
    ";
    }

    /// <summary>
    /// excelReader.AsDataSet() 会因为表内有空值而报错
    /// </summary>
    public static DataSet GetDataSet(string path)
    {
        var stream = File.Open(path, FileMode.Open, FileAccess.Read);
        var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        var ds = new DataSet();
        do
        {
            DataTable dt = GetTable(excelReader);
            ds.Merge(dt);
        } while (excelReader.NextResult());
        excelReader.Close();
        excelReader.Dispose();
        stream.Close();
        stream.Dispose();
        return ds;
    }

    /// <summary>
    /// 获得表格数据
    /// </summary>
    private static DataTable GetTable(IExcelDataReader excelReader)
    {
        DataTable dt = new DataTable();
        dt.TableName = excelReader.Name;

        bool isInit = false;
        string[] ItemArray = null;
        while (excelReader.Read())
        {
            if (!isInit)
            {
                isInit = true;
                for (int i = 0; i < excelReader.FieldCount; i++)
                {
                    dt.Columns.Add("", typeof(string));
                }
                ItemArray = new string[excelReader.FieldCount];
            }

            if (excelReader.IsDBNull(0))
            {
                continue;
            }
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
                ItemArray[i] = value;
            }
            dt.Rows.Add(ItemArray);
        }
        return dt;
    }


}
