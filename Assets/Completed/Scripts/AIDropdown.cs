using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Completed;

public class AIDropdown : MonoBehaviour {

	public int team;

	private GameManager gm;
	private Dictionary<int, string> OptionsToAI;

	// Use this for initialization
	void Start ()
	{
		OptionsToAI = new Dictionary<int, string>();
        ListAI.initAI();
        var dropdown = this.GetComponentInParent<Dropdown>();
        dropdown.options.Clear();
		int index = 0;
        foreach (string s in ListAI.AIPrograms.Keys)
        {
            dropdown.options.Add(new Dropdown.OptionData(s));
			OptionsToAI.Add(index++, s);
        }

		dropdown.onValueChanged.AddListener(delegate {
				onDropdownChanged(dropdown);
		});
	}

	void onDropdownChanged(Dropdown dropdown)
	{
		if (team == 0)
			BoardManager.AIBlue = OptionsToAI[dropdown.value];
		else
			BoardManager.AIRed = OptionsToAI[dropdown.value];
	}

	// Update is called once per frame
	void Update () {
	
	}
}
