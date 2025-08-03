using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedItemUI : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    [Header("Color")]
    [SerializeField] Color availableColor;
    [SerializeField] Color unavailableColor;

    [Header("LMBUser")]
    [SerializeField] GameObject lmbUseDisplay;
    [SerializeField] Image lmbUseBack;
    [SerializeField] Image lmbUseIcon, lmbUseCooldownBar;

    [Header("Skill1")]
    [SerializeField] GameObject skill1Display;
    [SerializeField] Image skill1Back;
    [SerializeField] Image skill1Icon, skill1CooldownBar;

    [Header("Skill2")]
    [SerializeField] GameObject skill2Display;
    [SerializeField] Image skill2Back;
    [SerializeField] Image skill2Icon, skill2CooldownBar;

    [Header("Reload")]
    [SerializeField] GameObject reloadDisplay;
    [SerializeField] Image reloadBack;
    [SerializeField] Image reloadCooldownBar;

    Player player;
    public void OnLocalPlayerSpawn(Player player)
    {
        this.player = player;
        player.inventory.onEquippedItemChange += OnEquippedItemChange;
        OnEquippedItemChange(player.inventory.equipped);
    }
    Item displaying;
    void OnEquippedItemChange(Item item)
    {
        displaying = item;
        if (displaying is ILMBUser lmbUser)
        {
            lmbUseDisplay.SetActive(true);
            lmbUseIcon.sprite = lmbUser.lmbSprite;
            UpdateLMB(lmbUser);
        }
        else
        {
            lmbUseDisplay.SetActive(false);
        }
        if (displaying is ISkill1User skill1User)
        {
            skill1Display.SetActive(true);
            skill1Icon.sprite = skill1User.skill1Sprite;
            UpdateSkill1(skill1User);
        }
        else
        {
            skill1Display.SetActive(false);
        }
        if(displaying is ISkill2User skill2User)
        {
            skill2Display.SetActive(true);
            skill2Icon.sprite = skill2User.skill2Sprite;
            UpdateSkill2(skill2User);
        }
        else
        {
            skill2Display.SetActive(false);
        }
        if(displaying is IReloadable reloadable)
        {
            reloadDisplay.SetActive(true);
            UpdateReload(reloadable);
        }
        else
        {
            reloadDisplay.SetActive(false);
        }
    }
    private void Update()
    {
        if (displaying is ILMBUser lmbUser) UpdateLMB(lmbUser);
        if (displaying is ISkill1User skill1User) UpdateSkill1(skill1User);
        if (displaying is ISkill2User skill2User) UpdateSkill2(skill2User);
        if (displaying is IReloadable reloadable) UpdateReload(reloadable);
    }
    void UpdateLMB(ILMBUser lmbUser)
    {
        lmbUseBack.color = lmbUser.canUseLmb ? availableColor : unavailableColor;
        lmbUseCooldownBar.fillAmount = lmbUser.lmbCooldownBarFill;
    }
    void UpdateSkill1(ISkill1User skill1User)
    {
        skill1Back.color = skill1User.canUseSkill1 ? availableColor : unavailableColor;
        skill1CooldownBar.fillAmount = skill1User.skill1CooldownBarFill;
    }
    void UpdateSkill2(ISkill2User skill2User)
    {
        skill2Back.color = skill2User.canUseSkill2 ? availableColor : unavailableColor;
        skill2CooldownBar.fillAmount = skill2User.skill2CooldownBarFill;
    }
    void UpdateReload(IReloadable reloadable)
    {
        reloadBack.color = reloadable.canReload ? availableColor : unavailableColor;
        reloadCooldownBar.fillAmount = reloadable.reloadBarFill;
    }
}