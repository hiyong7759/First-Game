using UnityEngine;

public class IntroUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartGame();
            gameObject.SetActive(false);
        }
    }
} 