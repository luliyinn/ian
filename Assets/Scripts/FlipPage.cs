using System.Collections.Generic;
using UnityEngine;

public class FlipPage : MonoBehaviour
{
    [Header("Binder Spreads")]
    [SerializeField] private List<GameObject> pageSpreads = new List<GameObject>();

    [Header("Binder Reference")]
    // Drag your binder object here in the Inspector
    [SerializeField] private Transform binderTransform;

    private int currentSpreadIndex = 0;

    private void Start()
    {
        if (binderTransform == null)
        {
            binderTransform = transform;
        }

        UpdatePageVisibility();
    }

    private void Update()
    {
        // 1 = Right Mouse Button
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Verify the click hit the binder or one of its child objects
                if (hit.transform.IsChildOf(binderTransform) || hit.transform == binderTransform)
                {
                    // Convert world click position to binder's local space
                    Vector3 localHit = binderTransform.InverseTransformPoint(hit.point);

                    // Clicked on the RIGHT side of the 3D model
                    if (localHit.x > 0)
                    {
                        NextPage();
                    }
                    // Clicked on the LEFT side of the 3D model
                    else
                    {
                        PreviousPage();
                    }
                }
            }
        }
    }

    public void NextPage()
    {
        if (currentSpreadIndex < pageSpreads.Count - 1)
        {
            currentSpreadIndex++;
            UpdatePageVisibility();
        }
    }

    public void PreviousPage()
    {
        if (currentSpreadIndex > 0)
        {
            currentSpreadIndex--;
            UpdatePageVisibility();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pageSpreads.Count; i++)
        {
            pageSpreads[i].SetActive(i == currentSpreadIndex);
        }
    }
}