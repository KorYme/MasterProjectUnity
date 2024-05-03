using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine;
#else
using System;
#endif


namespace MasterProject.Debugging
{
    public static class DebugLogger
    {
#if UNITY_EDITOR
        private static Dictionary<string, string> m_SenderColors = new Dictionary<string, string>();
        private static readonly string[] COLOR_PALETTE = new string[]
        {
            "#ff0000", "#ffff00", "#00ff00", "#0d3dff", "#00ffff", "#ff00ff",
            "#ffa500", "#808000", "#800080", "#8b0000", "#006400", "#ff8C00", "#ffd700",
        };
#else
        private static Dictionary<string, ConsoleColor> m_SenderColors = new Dictionary<string, ConsoleColor>();
        private static readonly ConsoleColor[] COLOR_PALETTE = new ConsoleColor[]
        {
            ConsoleColor.Red, ConsoleColor.Gray, ConsoleColor.Blue, ConsoleColor.Magenta,
            ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.White
        };
#endif

#if UNITY_EDITOR
        private static string GetOrAddColorValue(string sender)
        {
            if (!m_SenderColors.TryGetValue(sender, out string colorValue))
            {
                colorValue = COLOR_PALETTE[m_SenderColors.Count % COLOR_PALETTE.Length];
                m_SenderColors.Add(sender, colorValue);
            }
            return colorValue;
        }
#else
        private static ConsoleColor GetOrAddColorValue(string sender)
        {
            if (!m_SenderColors.TryGetValue(sender, out ConsoleColor colorValue))
            {
                colorValue = COLOR_PALETTE[m_SenderColors.Count % COLOR_PALETTE.Length];
                m_SenderColors.Add(sender, colorValue);
            }
            return colorValue;
        }
#endif
        public static void Info<T>(T sender, string message) where T : class
        {
            var colorValue = GetOrAddColorValue(typeof(T).Name);
#if UNITY_EDITOR
            Debug.Log($"[<color={colorValue}>{typeof(T).Name}</color>] : {message}");
#else
            Console.ForegroundColor = colorValue;
            Console.Write(sender);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
#endif
        }

        public static void Warning(string sender, string message)
        {
            var colorValue = GetOrAddColorValue(sender);
#if UNITY_EDITOR
            Debug.LogWarning($"[<color={colorValue}>{sender}</color>] : {message}");
#else
            Console.ForegroundColor = colorValue;
            Console.Write(sender);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
#endif
        }

        public static void Error(string sender, string message)
        {
            var colorValue = GetOrAddColorValue(sender);
#if UNITY_EDITOR
            Debug.LogError($"[<color={colorValue}>{sender}</color>] : {message}");
#else
            Console.ForegroundColor = colorValue;
            Console.Write(sender);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
#endif
        }
    }
}