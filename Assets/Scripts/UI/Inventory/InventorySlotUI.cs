using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemCount;

    [Header("AmmoCountDisplayed")]
    [SerializeField] GameObject ammoCountDisplay;
    [SerializeField] Image ammoIcon;
    [SerializeField] TMP_Text ammoCountText;

    Item displaying;
    public InventorySlot origin { get; private set; }
    public void Set(InventorySlot slot)
    {
        if(slot != null)
        {
            slot.onItemChange -= OnItemChange;
            slot.onCountChange -= OnCountChange;
        }
        this.origin = slot;
        if (slot != null)
        {
            slot.onItemChange += OnItemChange;
            slot.onCountChange += OnCountChange;
        }
        OnItemChange();
        OnCountChange();
    }
    void OnItemChange()
    {
        if (displaying is IAmmoCountDisplayed)
        {
            (displaying as IAmmoCountDisplayed).onAmmoCountDisplayUpdate -= OnAmmoCountDisplayUpdate;
        }

        displaying = origin.item;

        if(displaying != null)
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = displaying.data.itemIcon;
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }
        if (displaying is IAmmoCountDisplayed)
        {
            ammoCountDisplay.gameObject.SetActive(true);
            (displaying as IAmmoCountDisplayed).onAmmoCountDisplayUpdate += OnAmmoCountDisplayUpdate;
            OnAmmoCountDisplayUpdate();
        }
        else
        {
            ammoCountDisplay.gameObject.SetActive(false);
        }
    }
    void OnCountChange()
    {
        if(origin.count <= 1) itemCount.gameObject.SetActive(false);
        else
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = $"x{origin.count}";
        }
    }
    void OnAmmoCountDisplayUpdate()
    {
        ammoCountText.text = (displaying as IAmmoCountDisplayed).ammoCountText;
        ammoIcon.sprite = (displaying as IAmmoCountDisplayed ).ammoIcon;
    }
}