using UnityEngine;
using UnityEngine.UI;

public class ScoreSetter : MonoBehaviour {
	
	void Start ()
	{
		gameObject.GetComponent<Text>().text = Mathf.Clamp(GameData.score / 0.08f, 0, 10f).ToString("0.0");
	}

}
