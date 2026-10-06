using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform headPos;

    private void Update()
    {
        transform.position = headPos.position;
    }
}
