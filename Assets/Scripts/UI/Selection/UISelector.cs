using UnityEngine;

public class UISelector : MonoBehaviour
{
    UISelectable selected;
    public void Select(UISelectable selectable)
    {
        if (selected != null) selected.OnDeselect(this);
        selected = selectable;
        selected.OnSelect(this);
    }
    public void Deselect()
    {
        if(selected != null) selected.OnDeselect(this);
        selected = null;
    }
    private void Update()
    {
        if (selected != null) selected.OnSelectUpdate(this);
    }
}
