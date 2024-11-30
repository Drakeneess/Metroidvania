using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableButtonUI : MonoBehaviour
{
    private Image buttonIcon;
    public void SetButtonIcon(Sprite icon){
        buttonIcon.sprite = icon;
    }
    public void Activate(){
        gameObject.SetActive(true);
    }
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}
