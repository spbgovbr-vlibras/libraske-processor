using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ScenesManagement : MonoBehaviour
{


    public Scene this_scene;

    public enum SCENE
    {
        SPLASH_MENU,
        PRACTICE,
        PRA_VALER,
        LOADING,
        COUNTDOWN,
        GAMEPLAY,
        SCORE
    }

    public static Dictionary<SCENE, string> scene_map = new Dictionary<SCENE, string>()
    {
        { SCENE.SPLASH_MENU, "01_SplashMenu" },
        { SCENE.PRACTICE, "02_Praticar" },
        { SCENE.PRA_VALER, "02_PraValer" },
        { SCENE.LOADING, "03_Loading" },
        { SCENE.COUNTDOWN, "04_ContagemRegressiva" },
        { SCENE.GAMEPLAY, "05_Jogando" },
        { SCENE.SCORE, "06_Pontuacao" }
    };

    void Start()
    {
        this_scene = SceneManager.GetActiveScene();
        if(this_scene.name.Equals(scene_map[SCENE.PRA_VALER]))
        {
            StartCoroutine(TimedScene(3f, SCENE.LOADING));
        }
        if (this_scene.name.Equals(scene_map[SCENE.LOADING]))
        {
            StartCoroutine(TimedScene(4f, SCENE.COUNTDOWN));
        }
        else if(this_scene.name.Equals(scene_map[SCENE.COUNTDOWN]))
        {
            StartCoroutine(TimedScene(5f, SCENE.GAMEPLAY));
        }
    }

    public static void NextSceneByName(SCENE s)
    {
        SceneManager.LoadScene(scene_map[s]);
    }

    public static void NextSceneByName(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void LoadNextSceneByName(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(this_scene.buildIndex + 1);
    }

    public void PreviousScene()
    {
        SceneManager.LoadScene(this_scene.buildIndex - 1);
    }

    public static IEnumerator TimedScene(float time, SCENE s)
    {
        yield return new WaitForSeconds(time);
        NextSceneByName(s);
    }

}
