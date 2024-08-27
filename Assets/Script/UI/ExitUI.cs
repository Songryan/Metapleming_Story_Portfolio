using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
    public void ExitGame()
    {
        SceneManager.LoadScene("Login");
        Destroy(GameObject.Find("Player (1)"));
        Destroy(GameObject.Find("Slime1"));
        Destroy(GameObject.Find("NetworkManager"));
        Destroy(GameObject.Find("PlayerManager"));
        Destroy(GameObject.Find("ItemManager"));
    }
}
