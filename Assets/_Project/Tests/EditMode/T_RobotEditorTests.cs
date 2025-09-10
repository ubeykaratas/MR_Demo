using NUnit.Framework;
using UnityEngine;
using Unity.PerformanceTesting;

public class T_RobotEditorTests
{
    private RobotAnim ra;
    private float delta = 0.01f;
    
    [SetUp]
    public void Setup()
    {
        var robot = new GameObject();
        ra = robot.AddComponent<RobotAnim>();
        ra.SetStep(0.2f);
    }
    
    [Test]
    public void IsCalculatedCorrectly()
    {
        //Parameters must be greater than 0
        ra.SetMovementArea(1, 1, 2, 2);
        var speed = ra.CalculateSpeed(10);
        
        Assert.Greater(speed, 0f);
    }
    
    [Test]
    public void TestZeroDuration()
    {
        var zeroDuration = 0;
        var expectedSpeed = -1f;
        
        var speed = ra.CalculateSpeed(zeroDuration);
        
        Assert.AreEqual(expectedSpeed, speed, delta);
    }

    [Test]
    public void TestNegativeDuration()
    {
        var negativeDuration = -5;
        var speed = ra.CalculateSpeed(negativeDuration);

        // Should be -1
        Assert.AreEqual(-1f, speed);
    }

    [Test]
    public void TestZeroDistance()
    {
        ra.SetMovementArea(1,1,1,1);
        var speed = ra.CalculateSpeed(1);
        
        Assert.AreEqual(-1f, speed);
    }

    [Test]
    public void TestZeroXDistance()
    {
        var minZ = 1f;
        var maxZ = 5f;
        var duration = 2;
        ra.SetMovementArea(1,minZ,1,maxZ);
        var speed = ra.CalculateSpeed(duration);
        float expectedSpeed = (maxZ - minZ) / duration;
        Assert.AreEqual(expectedSpeed, speed, delta);
    }

    [Test]
    public void TestZeroZDistance()
    {
        var minX = 1f;
        var maxX = 5f;
        var duration = 2;
        ra.SetMovementArea(minX,1,maxX,1);
        var speed = ra.CalculateSpeed(duration);
        float expectedSpeed = (maxX - minX) / duration;
        Assert.AreEqual(expectedSpeed, speed, delta);
    }
    
    [Test, Performance]
    public void CalculateSpeed_Performance()
    {
        Measure.Method(() =>
            {
                ra.CalculateSpeed(10);
            })
            .WarmupCount(5)
            .MeasurementCount(50)
            .IterationsPerMeasurement(1000)
            .Run();
    }
    
}
