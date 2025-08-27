using TMPro;
using UnityEngine;

public class HandleRoleChange:MonoBehaviour
{
    enum Role
    {
        Operator,
        Administrator,
    }
    
    [SerializeField] private TextMeshProUGUI _roleText;
    [SerializeField] private UnityEngine.UI.Button _roleChangeButton;
    [SerializeField] private GameObject[] _adminUI;
    [SerializeField] private GameObject[] _operatorUI;

    private Role _currentRole;

    #region monos

    private void Start()
    {
        SetRole(Role.Operator, true);
    }

    #endregion
    

    private void OnEnable()
    {
        _roleChangeButton.onClick.AddListener(ChangeRole);
    }

    private void OnDisable()
    {
        _roleChangeButton.onClick.RemoveListener(ChangeRole);
    }

    private void ChangeRole()
    {
        
        switch (_currentRole)
        {
            case Role.Operator:
                SetRole(Role.Administrator, false);
                break;
            case Role.Administrator:
                SetRole(Role.Operator, true);
                break;
        }        
    }

    private void SetRole(Role role, bool changeToOperator)
    {
        _currentRole = role;
        _roleText.text = $"    {_currentRole.ToString()} View";

        foreach (var ui in _operatorUI)
        {
            ui.SetActive(changeToOperator);
        }

        foreach (var ui in _adminUI)
        {
            ui.SetActive(!changeToOperator);
        }
    }
}
