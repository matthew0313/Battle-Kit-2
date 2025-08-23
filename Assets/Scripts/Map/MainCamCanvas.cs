using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCamCanvas : MonoBehaviour
{
    Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }
}