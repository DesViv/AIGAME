using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIDropdown : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ListAI.initAI();
        var dropdown = this.GetComponentInParent<Dropdown>();
        dropdown.options.Clear();
        foreach (string s in ListAI.AIPrograms.Keys)
        {
            dropdown.options.Add(new Dropdown.OptionData(s));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
