using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HidePauseMenu();
    }

    public void HidePauseMenu()
	{
        Canvas canvas = GetComponent<Canvas>();
        canvas.gameObject.SetActive(false);
	}

    public void ShowPauseMenu()
	{
        Canvas canvas = GetComponent<Canvas>();
        canvas.gameObject.SetActive(true);
	}
}
