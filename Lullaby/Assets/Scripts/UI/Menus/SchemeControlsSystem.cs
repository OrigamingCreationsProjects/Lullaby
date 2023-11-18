using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.UI.Menus
{
    public class SchemeControlsSystem : MonoBehaviour
    {
        public Scheme currentScheme = null;
        public Scheme defaultScheme = null;
       
        // Start is called before the first frame update
        void Start()
        {
            SetupSchemes();
        }
    
        private void SetupSchemes()
        {
            Scheme[] schemes = GetComponentsInChildren<Scheme>();
            foreach (Scheme scheme in schemes)
            {
                scheme.Setup(this);
            }
            currentScheme.Show();
        }
        
        public void SetCurrent(Scheme newScheme)
        {
            currentScheme.Hide();
            currentScheme = newScheme;
            currentScheme.Show();
        }

        public void SetDefaultPanel()
        {
            currentScheme.Hide();
            currentScheme = defaultScheme;
            currentScheme.Show();
        }
        
        
    }
}