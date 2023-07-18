using UnityEngine;
using System.Collections;
using TMPro;
public class FPSCounter : MonoBehaviour
{
    private TextMeshPro fpsText;
    float deltaTime = 0.0f;
    private void Awake() {
        fpsText = GetComponent<TextMeshPro>();
    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = ((int)fps).ToString();
    }
}