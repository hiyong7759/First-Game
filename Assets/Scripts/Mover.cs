using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("왼쪽으로 이동하는 속도")]
    public float moveSpeed = 5f;

    private bool hasInitialized = false;

    private void Start()
    {
        hasInitialized = true;
    }

    private void Update()
    {
        if (!hasInitialized) return;

        // 왼쪽으로 이동
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
}
