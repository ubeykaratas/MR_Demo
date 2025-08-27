using TMPro;
using UnityEngine;

public class HandleAdministratorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _totalTimeText;
    [SerializeField] private TextMeshProUGUI _totalDefectText;
    [SerializeField] private TextMeshProUGUI _completedTasksText;
    
    [Header("References")]
    [SerializeField] private RobotAnim _ra;

    #region monos

    private void OnEnable()
    {
        RobotAnim.OnTotalTimeChanged += SetTotalTimeText;
        RobotAnim.OnTotalDefectChanged += SetTotalDefectText;
        RobotAnim.OnCompletedTasksChanged += SetCompletedTasksText;
        
        SetTotalTimeText();
        SetTotalDefectText();
        SetCompletedTasksText();
    }

    private void OnDisable()
    {
        RobotAnim.OnTotalTimeChanged -= SetTotalTimeText;
        RobotAnim.OnTotalDefectChanged -= SetTotalDefectText;
        RobotAnim.OnCompletedTasksChanged -= SetCompletedTasksText;
    }
    
    private void Start()
    {
        SetTotalTimeText();
        SetTotalDefectText();
        SetCompletedTasksText();
    }

    #endregion
    
    #region Setter

    private void SetTotalTimeText()
    {
        _totalTimeText.text = _ra.TotalTime.ToString("F1");
    }

    private void SetTotalDefectText()
    {
        _totalDefectText.text = _ra.TotalDefect.ToString();
    }

    private void SetCompletedTasksText()
    {
        _completedTasksText.text = _ra.CompletedTasks.ToString();
    }

    #endregion
    
}
