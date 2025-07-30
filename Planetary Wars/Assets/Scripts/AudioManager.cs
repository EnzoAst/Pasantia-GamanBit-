using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip buttonClick;

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClick);
    }
}
