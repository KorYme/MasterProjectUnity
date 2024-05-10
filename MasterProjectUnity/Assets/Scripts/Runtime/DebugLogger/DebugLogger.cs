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
        private enum LogLevel
        {
            Info,
            Warning, 
            Error
        }

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
            string senderName = typeof(T).Name;
            Info(senderName, message);
        }

        public static void Info(string senderName, string message)
        {
            WriteMessage(LogLevel.Info, senderName, message);
        }

        public static void Warning<T>(T sender, string message) where T : class
        {
            string senderName = typeof(T).Name;
            Warning(senderName, message);
        }

        public static void Warning(string senderName, string message)
        {
            WriteMessage(LogLevel.Warning, senderName, message);
        }

        public static void Error<T>(T sender, string message) where T : class
        {
            string senderName = typeof(T).Name;
            Error(senderName, message);
        }

        public static void Error(string senderName, string message)
        {
            WriteMessage(LogLevel.Error, senderName, message);
        }

        private static void WriteMessage(LogLevel logLevel, string senderName, string message)
        {
            var colorValue = GetOrAddColorValue(senderName);
#if UNITY_EDITOR
            switch (logLevel)
            {
                case LogLevel.Info:
                    Debug.Log($"[<color={colorValue}>{senderName}</color>] : {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[<color={colorValue}>{senderName}</color>] : {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[<color={colorValue}>{senderName}</color>] : {message}");
                    break;
                default:
                    break;
            }
#else
            Console.ForegroundColor = colorValue;
            Console.Write(senderName);
            switch (logLevel)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }
            Console.WriteLine(message);
            Console.ResetColor();
#endif
        }
    }
}