using UnityEngine;

[AddComponentMenu("")]
public class CharacterShadowHelper : MonoBehaviour
{
	void OnPreCull()
	{
		CharacterShadow shadow = (CharacterShadow)transform.parent.GetComponent(typeof(CharacterShadow));
		shadow.OnPreCull();
	}
	void OnPostRender()
	{
		CharacterShadow shadow = (CharacterShadow)transform.parent.GetComponent(typeof(CharacterShadow));
		shadow.OnPostRender();
	}
}
