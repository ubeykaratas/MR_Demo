using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Unity.PerformanceTesting;

public class T_RobotPlayTests
{
    private RobotAnim ra;
    private StatusChange rs;
    private float delta = 0.01f;
    
    [SetUp]
    public void SetUp()
    {
        var robot = new GameObject();
        rs = robot.AddComponent<StatusChange>();
        ra = robot.AddComponent<RobotAnim>();
        
        rs.SetScanTimes(1, 10);
        rs.SetPauseTimes(1, 1);
        rs._startButton = new GameObject().AddComponent<Button>();
        rs._stopButton = new GameObject().AddComponent<Button>();
        
        GameObject pauseButton = new GameObject();
        rs._pauseButton = pauseButton.AddComponent<Button>();
        CreatePauseButtonChildren(ref pauseButton);
        
        rs._slider = new GameObject().AddComponent<Slider>();
        rs._statusText = new GameObject().AddComponent<TextMeshProUGUI>();
        rs._taskText = new GameObject().AddComponent<TextMeshProUGUI>();
        
        ra.SetStep(0.2f);
        ra.SetMovementArea(1, 1, 5, 5);
        ra._rs = rs;
    }
    
    [UnityTest]
    public IEnumerator StartButtonTest()
    {
        rs._startButton.onClick.Invoke();
        Assert.AreEqual(rs.CurrentStatus, StatusChange.RobotStatus.Working);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator PauseButtonTest()
    {
        rs.SetStatus(StatusChange.RobotStatus.Working);  // Start working
        rs._pauseButton.onClick.Invoke();  // Simulate pause
        Assert.AreEqual(rs._pauseButton.GetComponentInChildren<TextMeshProUGUI>().text, "Continue");
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator StopButtonTest()
    {
        rs.SetStatus(StatusChange.RobotStatus.Working);
        rs._stopButton.onClick.Invoke();
        Assert.AreEqual(rs.CurrentStatus, StatusChange.RobotStatus.Returning);
        yield return null;
    }

    private void CreatePauseButtonChildren(ref GameObject gameObject)
    {
        gameObject.AddComponent<Image>();
        
        var pauseChild = new GameObject();
        pauseChild.AddComponent<TextMeshProUGUI>();
        pauseChild.transform.SetParent(gameObject.transform);
    }
    
    [UnityTest, Performance]
    public IEnumerator Robot_Working_Performance()
    {
        ra.StartWorking(5);
        
        yield return Measure.Frames()
            .WarmupCount(5)
            .MeasurementCount(30)
            .Run();
    }
    
    [UnityTest, Performance]
    public IEnumerator DefectDetected_Performance()
    {
        ra.DefectDetected();
        
        yield return Measure.Frames()
            .WarmupCount(5)
            .MeasurementCount(30)
            .Run();
    }
}
