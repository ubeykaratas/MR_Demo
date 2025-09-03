using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LogManager
{
    private static string logFilePath;

    static LogManager()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(folderPath);

        logFilePath = Path.Combine(folderPath, $"MRLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv");

        if (!File.Exists(logFilePath))
        {
            File.AppendAllText(logFilePath, "Timestamp, Kaynak, Olay, Detay\n");
        }
    }

    public static void Log(LogSource source, LogEvent eventName, string detail)
    {
        string sourceText = LogSourceMap[source];
        string eventText = LogEventMap[eventName];
        
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp},{sourceText},{eventText},{detail}\n";
        File.AppendAllText(logFilePath, logEntry);
    }

    private static readonly Dictionary<LogSource, string> LogSourceMap = new()
    {
        { LogSource.User, "Kullanıcı"},
        { LogSource.System, "Sistem"},
    };
    
    private static readonly Dictionary<LogEvent, string> LogEventMap = new()
    {
        { LogEvent.ButtonInteraction, "ButonEtkileşimi"},
        { LogEvent.RobotStatus, "RobotDurumu"},
        { LogEvent.TaskStatus, "GörevDurumu"},
        { LogEvent.HandGesture, "ElHareketi"},
        { LogEvent.DefectDetected, "KusurTespiti"}
    };
}


public enum LogSource
{
    User,
    System,
}

public enum LogEvent
{
    ButtonInteraction,
    TaskStatus,
    RobotStatus,
    HandGesture,
    DefectDetected,
}