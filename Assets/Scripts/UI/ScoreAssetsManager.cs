using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LAViD.LibrasKe;

public class ScoreAssetsManager : MonoBehaviour {

    public ScoreSystem score;
    public float interval = 5f;
    public Sprite[] messages;

    private Image image;
	private int last = -1;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        StartCoroutine(TimeWait());
    }

    void SetScoreAsset(int n)
    {
		if (n >= 0 && n < messages.Length && n != last)
        {
            image.enabled = true;
            image.sprite = messages[n];
			last = n;
        }
        else
		{
			image.enabled = false;
			last = -1;
		}
        
        image.SetNativeSize();
    }

    IEnumerator TimeWait()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            int n;
            float ratio = score.ScoreRatioOnLast(5);

			// "Fantástico"
			if (ratio >= 0.8) n = 0;

			// "Exelente"
			else if (ratio >= 0.7) n = 1;

			// "Ótimo"
			else if (ratio >= 0.6) n = 2;

			// "Bom"
			else if (ratio >= 0.5) n = 3;

			// "Continue"
			else if (ratio >= 0.4) n = 4;

			// "Precisa melhorar"
			else if (ratio >= 0.2) n = 5;

			// No message
			else n = -1;

            SetScoreAsset(n);
        }
    }

}
