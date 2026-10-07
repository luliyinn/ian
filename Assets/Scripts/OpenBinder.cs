using UnityEditor.Build.Content;
using UnityEngine;

public class OpenBinder : MonoBehaviour
{
    [SerializeField] GameObject OpenedBinder;
    private void OnMouseDown()
    {
        OpenedBinder.SetActive(true);
        gameObject.SetActive(false);
    }
}
