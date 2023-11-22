using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Panel : MonoBehaviour
{
    public Button defaultButton = null;
    
    private Canvas _canvas = null; // Usamos canvas pero podría ser un canvas group si queremos hacer fade in - out etc.
    private MenuSystem _menuManager = null;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        defaultButton = GetComponentInChildren<Button>();
    }

    //Utilizamos esta funcion para que el menuManager se encarge de setear todos los paneles o canvas que tiene y cada
    //panel sepa cual es su manager
    public void Setup(MenuSystem menuManager)
    {
        _menuManager = menuManager;
        //Al terminar de setearlo ocultamos el panel para que todos queden ocultos por default
        //y no dependa de estar tocando cosas en el inspector
        Hide();
    }
    
    //Las siguientes funciones se encargan de activar y desactivar el canvas para ocultarlo, pero se podria ejecutar
    //una funcion que hiciera un fade o una animacion de aparicion/desaparicion, fundamentalmente manejan
    //como se muestran y ocultan los paneles o su componente canvas
    public void Show()
    {
        _canvas.enabled = true;
        //defaultButton.Select();
        //gameObject.SetActive(true);
    }

    public void Hide()
    {
        _canvas.enabled = false;
        //gameObject.SetActive(false);
    }
}
