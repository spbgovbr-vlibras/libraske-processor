using UnityEngine;
using UnityEngine.UI;

public class GetMusicName : MonoBehaviour {

	void Start () {
        gameObject.GetComponent<Text>().text = GameData.get_music_presentation_name();
	}
}
