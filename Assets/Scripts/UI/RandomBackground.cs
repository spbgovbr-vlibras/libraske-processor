using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour {

    public Sprite i, j, k;

    private void SetSprite(Sprite s)
    {
        gameObject.GetComponent<Image>().sprite = s;
    }

	void Start ()
    {
        switch(Random.Range(1, 4))
        {
            case 1: SetSprite(i);
                break;
            case 2: SetSprite(j);
                break;
            case 3: SetSprite(k);
                break;
            default: SetSprite(i);
                break;
        }
        
	}
	
	void Update () {
	
	}

}
