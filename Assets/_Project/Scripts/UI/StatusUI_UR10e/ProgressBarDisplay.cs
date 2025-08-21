using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarDisplay : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;
    private void Start()
    {
        if (!_slider) Debug.LogError("Slider Component not found!");
    }
    private void Update()
    {
        if (!_slider) return;
        _text.text = (_slider.value * 100).ToString("f0") + "%";

    }
}
