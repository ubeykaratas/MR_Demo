using UnityEngine;

public class HandleConf : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button _confirmButton;
    [SerializeField] private UnityEngine.UI.Button _recheckButton;
    [SerializeField] private StatusChange _rs;
    [SerializeField] private RobotAnim _ra;

    private void Start()
    {
        _confirmButton.onClick.AddListener(OnConfirmClicked);
        _recheckButton.onClick.AddListener(OnRecheckClicked);
    }

    private void OnConfirmClicked()
    {
        _ra.Confirm();
    }

    private void OnRecheckClicked()
    {
        _ra.NavigateToCoordinate();
    }
}
