using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        audioSource.Stop(); // ���� �ε�Ǹ� ������� ����ϴ�.
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // ������Ʈ�� �ı��� �� �̺�Ʈ ���� ����
    }
}
