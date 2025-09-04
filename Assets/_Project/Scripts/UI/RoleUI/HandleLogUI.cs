using TMPro;
using UnityEngine;

public class HandleLogUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _spawnPoint;
    [SerializeField] private GameObject _logElementsPrefab;
    [SerializeField, Tooltip("UI elements outside of panel")] private GameObject _panelExtras;
    [SerializeField] private RectTransform _panelOutline;
    [SerializeField] private float _outlineCorrector;
    [SerializeField] private float _maxOutlineHeight;
    
    private readonly int LOG_ELEMENTS_COUNT;
    
    #region monos

    private void Start()
    {  
        ChangeUIVisibility(false);
        LogManager.OnLogCreated += LogToUI;
    }

    private void OnDestroy()
    {
        LogManager.OnLogCreated -= LogToUI;
    }

    #endregion

    private void LogToUI(string timestamp, string source, string eventName, string detail)
    {
        if (!_panel.gameObject.activeSelf) ChangeUIVisibility(true);
        
        GameObject newElement = Instantiate(_logElementsPrefab, _spawnPoint);
        TextMeshProUGUI[] texts = newElement.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= LOG_ELEMENTS_COUNT)
        {
            texts[0].text = timestamp;
            texts[1].text = source;
            texts[2].text = eventName;
            texts[3].text = detail;
        }
        else Debug.LogWarning($"Log elements do not match the expected number of {LOG_ELEMENTS_COUNT}");
        
        newElement.SetActive(true);

        Vector2 newOutlineSize = _panelOutline.sizeDelta;
        newOutlineSize.y += _outlineCorrector;
        if(newOutlineSize.y >= _maxOutlineHeight) newOutlineSize.y = _maxOutlineHeight; 
        _panelOutline.sizeDelta = newOutlineSize;
        Debug.LogError($"O: {_panelOutline.sizeDelta}");
    }

    private void ChangeUIVisibility(bool visibility)
    {
        _panelExtras.SetActive(visibility);
        _panel.gameObject.SetActive(visibility);
    }
}
