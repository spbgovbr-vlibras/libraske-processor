using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour {

    private Button button;
    private Color color;
    public Image test_image;

    private Color NewColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); ;
    }

    void Start ()
    {

        button = GetComponent<Button>();
        color = NewColor();
        GetComponent<Image>().color = color;

        button.onClick.AddListener(() =>
            {
                test_image.color = color;
                color = NewColor();
                GetComponent<Image>().color = color;
            }
        );

	}
	
}
