using System;
using System.Collections.Generic;
using System.IO;

namespace Py2Cs
{
    public static class Util
    {
        public static readonly string ProjectRoot = GetProjectRoot();

        public static List<string> Init(string iniName)
        {
            // 最简 INI 读取：只支持 key=value，且只读 [ConfigOrParameters] 下的 cop 键
            string full = Path.Combine(ProjectRoot, "input", iniName);
            if (!File.Exists(full))
                throw new FileNotFoundException("INI not found", full);

            string section = null;
            string raw = null;
            foreach (var line in File.ReadAllLines(full))
            {
                string t = line.Trim();
                if (t.StartsWith("[") && t.EndsWith("]"))
                    section = t[1..^1];
                else if (section == "ConfigOrParameters" && t.StartsWith("cop"))
                    raw = t.Split('=')[1].Trim();
            }

            if (raw == null)
                throw new Exception("cop key not found in [ConfigOrParameters]");

            // 去掉空格并按逗号拆分
            raw = raw.Replace(" ", "");
            return new List<string>(raw.Split(','));
        }

        private static string GetProjectRoot()
        {
            // 与 Python 代码等价：取父目录
            var dir = Directory.GetCurrentDirectory();
            return Directory.GetParent(dir).FullName;
        }
    }
}