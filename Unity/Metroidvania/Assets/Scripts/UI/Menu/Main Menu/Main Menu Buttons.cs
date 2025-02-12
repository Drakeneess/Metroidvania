using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MenuButtons{
    private bool WaitForSelect=true;
    protected override void Awake()
    {
        base.Awake();
        buttons[0].onClick.AddListener(ContinueGame);
        buttons[1].onClick.AddListener(NewGame);
        buttons[2].onClick.AddListener(Options);
        buttons[3].onClick.AddListener(QuitGame);
    }
    protected override void Start() {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if(WaitForSelect){
            currentSelection=SaveDataController.AreSavedData()?0:1;
            WaitForSelect=false;
        }
    }

    private void ContinueGame(){
        print("Continue game");
    }
    private void NewGame() {
        print("New game");
    }
    private void Options() {
        print("Options");
    }
    private void QuitGame() {
        print("Quit game");
        Application.Quit();
    }
    protected override void NavigateVertical(Vector2 direction)
    {
        if (direction.y > 0) // Arriba
        {
            currentSelection--;
            if(currentSelection==0 && !SaveDataController.AreSavedData()){
                currentSelection--;
            }
        }
        else if (direction.y < 0) // Abajo
        {
            currentSelection++;
        }

        // Envolver el índice para que sea cíclico
        if (currentSelection < 0)
        {
            currentSelection = buttons.Length - 1; // Ir al último botón
        }
        else if (currentSelection >= buttons.Length)
        {
            currentSelection = 0; // Volver al primer botón
            if(!SaveDataController.AreSavedData()){
                currentSelection++;
            }
        }

        UpdateButtonSelection();
    }
}
