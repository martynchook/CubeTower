using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour {

    public Sprite musicOn, musicOff;

    private void Start() {
        if (PlayerPrefs.GetString("music") == "No" && gameObject.name == "Music") {
            GetComponent<Image>().sprite = musicOff;
        }
    }

    public void RestartGame () {
        if (PlayerPrefs.GetString("music") != "No ") {
            GetComponent<AudioSource>().Play();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadGitHub () {
         if (PlayerPrefs.GetString("music") != "No ") {
            GetComponent<AudioSource>().Play();
        }
        Application.OpenURL("https://github.com/martynchook/Waytiti-Studio");
    }

    public void MusicWork() {
        // проверка включена ли музыка
        if (PlayerPrefs.GetString("music") == "No") {
            GetComponent<AudioSource>().Play();
            PlayerPrefs.SetString("music", "Yes");
            GetComponent<Image>().sprite = musicOn;
        } else {
            PlayerPrefs.SetString("music", "No");
            GetComponent<Image>().sprite = musicOff;
        }
    }

}
