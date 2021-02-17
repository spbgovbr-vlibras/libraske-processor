using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovieReproducer : MonoBehaviour
{

    public MovieTexture os_gonzagas_amor, seu_pereira_carimbo, beto_brito_xote;
    public Material mt_os_gonzagas_amor, mt_seu_pereira_carimbo, mt_beto_brito_xote;
    public AudioSource audio_source;
    public Slider slider;

    private float elapsedTime = 0f;
    private float startTime = 0f;
    private float scene_duration = 0f;


    private void PlayAudio(MovieTexture mov)
    {
        audio_source.clip = mov.audioClip;
        audio_source.Play();
    }

    private void PlayMovieSetDurationAndMaterial(MovieTexture mov, Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
        scene_duration = mov.duration;
        mov.Play();
        PlayAudio(mov);
    }

    void Start()
    {

        audio_source = GameObject.Find("AudioSystem").GetComponent<AudioSource>();

        switch (GameData.music)
        {
                
            case "os_gonzagas_amor":
                PlayMovieSetDurationAndMaterial(os_gonzagas_amor, mt_os_gonzagas_amor);
                break;
            case "seu_pereira_carimbo":
                PlayMovieSetDurationAndMaterial(seu_pereira_carimbo, mt_seu_pereira_carimbo);
                break;
            case "beto_brito_xote":
                PlayMovieSetDurationAndMaterial(beto_brito_xote, mt_beto_brito_xote);
                break;
            default:
                Debug.Log("Default case: os_gonzagas");
                PlayMovieSetDurationAndMaterial(os_gonzagas_amor, mt_os_gonzagas_amor);
                break;

        }

        startTime = Time.time;
        
        if (SceneManager.GetActiveScene().name.Equals("02_Praticar"))
        {
            scene_duration = 30f;
            StartCoroutine(ScenesManagement.TimedScene(scene_duration, ScenesManagement.SCENE.LOADING));
        }
        else
        {
            StartCoroutine(ScenesManagement.TimedScene(scene_duration, ScenesManagement.SCENE.SCORE));
        }

    }

    void Update()
    {
        elapsedTime = Time.time - startTime;
        slider.value = elapsedTime / scene_duration;
    }

}
