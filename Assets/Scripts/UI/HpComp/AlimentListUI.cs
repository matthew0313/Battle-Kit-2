using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlimentListUI : MonoBehaviour
{
    [SerializeField] HpComp target;
    [SerializeField] Transform elementAnchor;
    [SerializeField] AlimentListUIElement elementPrefab;
    readonly List<AlimentListUIElement> elements = new();
    private void OnEnable()
    {
        target.onAlimentsUpdate += OnAlimentsUpdate;
        OnAlimentsUpdate();
    }
    private void OnDisable()
    {
        target.onAlimentsUpdate -= OnAlimentsUpdate;
    }
    void OnAlimentsUpdate()
    {
        int i;
        for(i = 0; i < target.aliments.Count; i++)
        {
            if (elements.Count <= i) elements.Add(Instantiate(elementPrefab, elementAnchor));
            elements[i].Set(target.aliments.ElementAt(i).Value);
            elements[i].gameObject.SetActive(true);
        }
        for(; i < elements.Count; i++)
        {
            elements[i].gameObject.SetActive(false);
        }
    }
}