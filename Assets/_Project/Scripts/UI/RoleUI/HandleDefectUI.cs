using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class HandleDefectUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Transform _panel;
    [SerializeField] private GameObject _textPrefab;
    [SerializeField, Tooltip("UI elements outside of panel")] private GameObject _panelExtras;
    [SerializeField] private RectTransform _panelOutline;
    [SerializeField] private float _outlineCorrector;
    
    [Header("References")]
    [SerializeField, Tooltip("Assign the button that resets the all defect information")] 
    private UnityEngine.UI.Button _resetButton;
    [SerializeField] private StatusChange _rs;
    
    private readonly List<GameObject> _defects = new List<GameObject>();
    private Vector2 _originalOutline;
    
    
    
    #region monos

    private void Start()
    {  
        ChangeUIVisibility(false);
        _originalOutline = _panelOutline.sizeDelta;
        _resetButton.onClick.AddListener(RemoveUI);
    }

    #endregion
    
    public void CreateCoordinate(Vector3 position)
    {
        if (!_panel.gameObject.activeSelf) ChangeUIVisibility(true);
        
        GameObject newText = Instantiate(_textPrefab, _panel);
        TextMeshProUGUI temp = newText.GetComponent<TextMeshProUGUI>();
        
        temp.text = $"X: {position.x:0.0}, Y: {position.y:0.0}, Z: {position.z:0.0}";
        
        Vector2 newOutlineSize = _panelOutline.sizeDelta;
        newOutlineSize.y += _outlineCorrector;
        _panelOutline.sizeDelta = newOutlineSize;
        
        newText.SetActive(true);
        _defects.Add(newText);
    }

    public void RemoveUI()
    {
        if(_rs.CurrentStatus != StatusChange.RobotStatus.Idle) return;
        foreach (GameObject defect in _defects)
        {
            Destroy(defect);
        }
        _defects.Clear();
        _panelOutline.sizeDelta = _originalOutline;
        ChangeUIVisibility(false);
    }

    private void ChangeUIVisibility(bool visibility)
    {
        _panelExtras.SetActive(visibility);
        _panel.gameObject.SetActive(visibility);
    }
}
