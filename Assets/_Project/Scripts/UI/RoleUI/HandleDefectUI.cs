using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class HandleDefectUI : MonoBehaviour
{
    [SerializeField] private Transform _panel;
    [SerializeField] private GameObject _textPrefab;
    [SerializeField, Tooltip("UI elements outside of panel")] private GameObject _panelExtras;
    [SerializeField] private RectTransform _panelOutline;
    [SerializeField] private float _outlineCorrector;
    
    private readonly List<GameObject> _defects = new List<GameObject>();
    private Vector2 _originalOutline;
    
    #region monos

    private void Start()
    {  
        ChangeUIVisibility(false);
        _originalOutline = _panelOutline.sizeDelta;
    }

    #endregion
    
    public void CreateCoordinate()
    {
        if (!_panel.gameObject.activeSelf) ChangeUIVisibility(true);
        
        GameObject newText = Instantiate(_textPrefab, _panel);
        TextMeshProUGUI temp = newText.GetComponent<TextMeshProUGUI>();
        temp.text = "test";
        Vector2 newOutlineSize = _panelOutline.sizeDelta;
        newOutlineSize.y += _outlineCorrector;
        _panelOutline.sizeDelta = newOutlineSize;
        
        newText.SetActive(true);
        _defects.Add(newText);
    }

    public void RemoveUI()
    {
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
