using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    [SerializeField] private float minY = -2.15f;
    [SerializeField] private float maxY = -0.54f;
    [SerializeField] private PetCollectionUI sliderArea;
    private bool onDrag = false;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("Handle"))
            {
                onDrag = true;
            }
        }
        else if (onDrag && Input.GetMouseButton(0))
        {
            transform.position = new Vector3(transform.position.x, mousePosition.y, -1);
            float nextY = Mathf.Clamp(transform.localPosition.y, minY, maxY);
            transform.localPosition = new Vector3(transform.localPosition.x, nextY, -1);
            sliderArea.SetButtonViewPosition((nextY - minY) / (maxY - minY));
        }
        if (Input.GetMouseButtonUp(0))
        {
            onDrag = false;
        }
    }
}
