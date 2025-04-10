using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BilingualText : MonoBehaviour
{
    [SerializeField] LangText content;
    TMP_Text body;
    private void OnEnable()
    {
        if(body == null) body = GetComponent<TMP_Text>();
        body.text = content.text;
        Settings.onLanguageChange += OnLanguageChange;
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
    }
    void OnLanguageChange()
    {
        body.text = content.text;
    }
}