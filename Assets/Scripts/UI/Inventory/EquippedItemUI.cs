using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedItemUI : MonoBehaviour, ILocalPlayerSpawnCallbackReceiver
{
    [Header("Skill1")]
    [SerializeField] GameObject skill1Display;
    [SerializeField] Image skill1Back;
    [SerializeField] Color skill1Available, skill1Unavailable;
    [SerializeField] Image skill1Icon, skill1CooldownBar;

    [Header("Skill2")]
    [SerializeField] GameObject skill2Display;
    [SerializeField] Image skill2Back;
    [SerializeField] Color skill2Available, skill2Unavailable;
    [SerializeField] Image skill2Icon, skill2CooldownBar;
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
        if(displaying is ISkill1User skill1User)
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
    }
    private void Update()
    {
        if(displaying is ISkill1User skill1User) UpdateSkill1(skill1User);
        if(displaying is ISkill2User skill2User) UpdateSkill2(skill2User);
    }
    void UpdateSkill1(ISkill1User skill1User)
    {
        skill1Back.color = skill1User.canUseSkill1 ? skill1Available : skill1Unavailable;
        skill1CooldownBar.fillAmount = skill1User.skill1CooldownBarFill;
    }
    void UpdateSkill2(ISkill2User skill2User)
    {
        skill2Back.color = skill2User.canUseSkill2 ? skill2Available : skill2Unavailable;
        skill2CooldownBar.fillAmount = skill2User.skill2CooldownBarFill;
    }
}