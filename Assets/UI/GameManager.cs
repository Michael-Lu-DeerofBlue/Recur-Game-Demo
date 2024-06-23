using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject creditsPanel;
    public Slider volumeSlider;
    public Text creditsText;
    public AudioSource audioSource;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(false);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
}
      void TogglePause()
    {
        isPaused = !isPaused;
        mainPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void ShowCredits()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
       StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        float duration = 10f; // Duration to scroll through credits
        float elapsed = 0f;

        Vector3 startPos = creditsText.transform.localPosition;
        Vector3 endPos = new Vector3(startPos.x, -creditsText.rectTransform.rect.height, startPos.z);

        while (elapsed < duration)
        {
            creditsText.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        creditsText.transform.localPosition = startPos;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
