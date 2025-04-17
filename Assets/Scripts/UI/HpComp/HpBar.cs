using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] HpComp target;

    [Header("Display")]
    [SerializeField] Transform healthScaler;
    private void OnEnable()
    {
        target.onHpChange += OnHpChange;
        OnHpChange();
    }
    private void OnDisable()
    {
        target.onHpChange -= OnHpChange;
    }
    void OnHpChange()
    {
        healthScaler.localScale = new Vector2(target.hp.Value / target.maxHp.Value, 1.0f);
    }
}