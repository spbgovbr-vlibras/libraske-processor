using UnityEngine;
using UnityEngine.UI;

public class SetMusicNameBtn : MonoBehaviour
{

    public string music_id;

    void Start()
    {

        GetComponent<Button>().onClick.AddListener(() =>
            {
                GameData.music = music_id;
            }
        );

    }

}
