using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnyKeyChangeScene : MonoBehaviour
{
    private SceneChange sceneChange;
    private AudioSource audio;

    [SerializeField]
    private string nextScene = "Main";
    [SerializeField]
    private AudioClip selectSFX;

    private void Start()
    {
        this.sceneChange = FindObjectOfType<SceneChange>();
        this.audio = FindObjectOfType<AudioSource>();
    }

    void Update()
    {
        if(!Input.GetKey(KeyCode.Escape) && Input.anyKeyDown)
        {
            this.audio.PlayOneShot(this.selectSFX);
            this.sceneChange.FadeToLevel(this.nextScene);
        }
    }
}
