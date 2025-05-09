using DG.Tweening;
using UnityEngine;

public class HpBarUI : MonoBehaviour
{
    [SerializeField] HpComp target;

    [Header("Display")]
    [SerializeField] Transform healthScaler;
    [SerializeField] Transform damageScaler, healScaler;
    [SerializeField] float scalerDelay = 0.5f, scalerDuration = 0.5f;
    private void OnEnable()
    {
        target.onHpChange += OnHpChange;
        OnHpChange();
    }
    private void OnDisable()
    {
        target.onHpChange -= OnHpChange;
    }
    float currentScale = 1.0f;
    Tween tween;
    void OnHpChange()
    {
        float scale = target.hp.Value / target.maxHp.Value;
        tween?.Kill();
        if (scale < currentScale)
        {
            healScaler.gameObject.SetActive(false);
            damageScaler.gameObject.SetActive(true);
            healthScaler.localScale = new Vector2(scale, 1.0f);
            damageScaler.localScale = new Vector2(currentScale, 1.0f);
            tween = DOTween.To(() => currentScale, value => currentScale = value, scale, scalerDuration)
                .SetDelay(scalerDelay)
                .SetEase(Ease.OutCirc)
                .OnUpdate(() =>
                {
                    damageScaler.localScale = new Vector2(currentScale, 1.0f);
                });
        }
        else if(scale > currentScale)
        {
            healScaler.gameObject.SetActive(true);
            damageScaler.gameObject.SetActive(false);
            healthScaler.localScale = new Vector2(currentScale, 1.0f);
            healScaler.localScale = new Vector2(scale, 1.0f);
            tween = DOTween.To(() => currentScale, value => currentScale = value, scale, scalerDuration)
                .SetDelay(scalerDelay)
                .SetEase(Ease.OutCirc)
                .OnUpdate(() =>
                {
                    healthScaler.localScale = new Vector2(currentScale, 1.0f);
                });
        }
    }
}