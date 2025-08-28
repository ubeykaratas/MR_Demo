using UnityEngine;

public class DefectTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject _tooltipPrefab;
    [SerializeField] private float _verticalOffset;
    
    private GameObject _tooltipInstance;

    private void Start()
    {
        Vector3 pos = transform.position;
        _tooltipInstance = Instantiate(_tooltipPrefab, pos + Vector3.up * _verticalOffset, Quaternion.identity);
        _tooltipInstance.SetActive(true);
        HandleTooltip test = _tooltipInstance.GetComponent<HandleTooltip>();
        if (!test) Debug.LogError("Could not find HandleTooltip component");
        test.SetText(pos);
        _tooltipInstance.SetActive(false);
    }

    public void ChangeVisibility(bool isVisible)
    {
        if(!_tooltipInstance) Debug.LogError("Could not find DefectTooltipUI component");
        _tooltipInstance.SetActive(isVisible);
    }
    
}
