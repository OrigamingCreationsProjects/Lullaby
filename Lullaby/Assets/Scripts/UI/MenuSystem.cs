using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public Panel currentPanel = null;

    private List<Panel> _panelHistory = new List<Panel>();
    private List<Button> _buttonHistory = new List<Button>();
    // Start is called before the first frame update
    void Start()
    {
        SetupPanels();
    }

    private void SetupPanels()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();
        foreach (Panel panel in panels)
        {
            panel.Setup(this);
        }
        currentPanel.Show();
    }
    
    void OnBack(InputValue value) // Al pulsar el botón de ir hacia atrás pues retrocederemos en los menús
    {
        GoToPrevious();
    }

    public void GoToPrevious()
    {
        if (_panelHistory.Count == 0)
            //A parte del return podemos poner una funcion que pregunte al usuario si quiere cerrar el juego ya que
            //cuando el historial este vacio estaremos en el primer menu (en caso de menu principal, si no se puede hacer
            //lo mismo en un menu de pausa pero cerrando el menu de pausa o preguntando si quiere salir al menu principal)
            return;
        
        int lastIndex = _panelHistory.Count - 1;
        //Seteamos el panel actual al ultimo guardado en el historial para volver al que va anterior al que estamos
        SetCurrent(_panelHistory[lastIndex]);
        //Borramos el panel del historial ya que nos movemos a el
        _panelHistory.RemoveAt(lastIndex);
    }
    //Esta funcion sera llamada cuando cambiemos a un panel posterior
    public void SetCurrentWithHistory(Panel newPanel)
    {
        //Agregamos el panel actual al historial y el siguiente lo ponemos como el actual
        _panelHistory.Add(currentPanel);
        SetCurrent(newPanel);
    }

    private void SetCurrent(Panel newPanel)
    {
        currentPanel.Hide();
        currentPanel = newPanel;
        currentPanel.defaultButton.Select();
        currentPanel.Show();
    }
}
