using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlimentListUIElement : MonoBehaviour
{
    Aliment target;
    [SerializeField] Image alimentIcon;
    [SerializeField] TMP_Text alimentText;
    [SerializeField] Transform timeLeftScaler;
    public AlimentListUIElement Set(Aliment target)
    {
        if (this.target != null) this.target.onAlimentDisplayChange -= OnAlimentDisplayChange;
        this.target = target;
        target.onAlimentDisplayChange += OnAlimentDisplayChange;
        alimentText.color = target.settings.alimentColor;
        alimentIcon.sprite = target.settings.alimentIcon;
        timeLeftScaler.localScale = new Vector3(target.duration / target.initialDuration, 1.0f, 1.0f);
        OnAlimentDisplayChange();
        return this;
    }
    void OnAlimentDisplayChange()
    {
        alimentText.text = target.alimentDisplay.text;
    }
    private void Update()
    {
        timeLeftScaler.localScale = new Vector3(target.duration / target.initialDuration, 1.0f, 1.0f);
    }
}