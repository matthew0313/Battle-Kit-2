using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedStringBuilderText : MonoBehaviour
{
    [SerializeField] HookStringBuilderElement[] elements;

    [Header("Display")]
    [SerializeField] TMP_Text text;
    private void OnEnable()
    {
        bool languageDependant = false;
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.LangText) languageDependant = true;
            else if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.AssignCallback(OnValueChange);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.AssignCallback(OnValueChange);
            }
        }
        if (languageDependant) Settings.onLanguageChange += OnValueChange;
        OnValueChange();
    }
    private void OnDisable()
    {
        bool languageDependant = false;
        foreach (var i in elements)
        {
            if (i.type == HookStringBuilderElementType.LangText) languageDependant = true;
            else if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.RemoveCallback(OnValueChange);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.RemoveCallback(OnValueChange);
            }
        }
        if(languageDependant) Settings.onLanguageChange -= OnValueChange;
    }
    StringBuilder sb = new();
    void OnValueChange()
    {
        sb.Clear();
        foreach (var element in elements)
        {
            switch (element.type)
            {
                case HookStringBuilderElementType.Text:
                    sb.Append(element.text);
                    break;
                case HookStringBuilderElementType.LangText:
                    sb.Append(element.langText.text);
                    break;
                case HookStringBuilderElementType.IntegerHook:
                    sb.Append((element.integerHook.value * element.integerMultiplier).ToString());
                    break;
                case HookStringBuilderElementType.FloatHook:
                    sb.Append((Mathf.Round(element.floatHook.value * element.floatMultiplier * Mathf.Pow(10, element.floatRounding)) / Mathf.Pow(10, element.floatRounding)).ToString());
                    break;
                case HookStringBuilderElementType.StringHook:
                    sb.Append(element.stringHook.value);
                    break;
            }
        }
        text.text = sb.ToString();
    }
}