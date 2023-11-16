using UnityEngine;

namespace Lullaby.UI.Menus
{
    [RequireComponent(typeof(Canvas))]
    public class Scheme : MonoBehaviour
    {
        private Canvas _canvas = null; // Usamos canvas pero podría ser un canvas group si queremos hacer fade in - out etc.
        private SchemeControlsSystem _schemeControlsSystem = null;
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        //Utilizamos esta funcion para que el menuManager se encarge de setear todos los paneles o canvas que tiene y cada
        //panel sepa cual es su manager
        public void Setup(SchemeControlsSystem schemeControlsSystem)
        {
            _schemeControlsSystem = schemeControlsSystem;
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
            //gameObject.SetActive(true);
        }

        public void Hide()
        {
            _canvas.enabled = false;
            //gameObject.SetActive(false);
        }
    }
}