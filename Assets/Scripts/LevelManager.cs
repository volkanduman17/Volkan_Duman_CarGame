using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        Instantiate(Levels[DataManager.Level]);
    }

    public List<GameObject> Levels;

    public void NextLevel()
    {
        DataManager.Level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("level arttýrýldý");
        Instantiate(Levels[DataManager.Level]);
    }

}
