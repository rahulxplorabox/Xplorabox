using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorSelectionButton : MonoBehaviour {

	public ColoringController parent;

	private Color32 color;
	public void Init(ColoringController _parent, Color32 _color){
		parent = _parent;
		color = _color;
		GetComponent<Image> ().color = _color;
	}
	public void Click(){
		parent.SetColor (color);
        print("Set color in parent : "+parent+" and color is "+color);
	}
}
