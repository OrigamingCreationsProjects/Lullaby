using Lullaby.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Lullaby
{
    public class SignGraphic : Sign
    {
        public Sprite tutorialSprite;
        public Image tutorialImage;
        private ContextIndicator _contextIndicator;
        protected override void Awake()
        {
            _contextIndicator = GetComponentInChildren<ContextIndicator>();
            finalPosition = canvas.transform.position;
            canvas.transform.localPosition = backPosition;
            initialScale = canvas.transform.localScale;
            canvas.transform.localScale = Vector3.zero;
            //Quiza haya que inicializar la escala
            canvas.gameObject.SetActive(true);
            collider = GetComponent<Collider>();
            camera = Camera.main;
            tutorialImage.sprite = tutorialSprite;
        }

        public override void Show()
        {
            base.Show();
            _contextIndicator.ShowContextIndicator();
        }
        
        public override void Hide()
        {
            base.Hide();
            _contextIndicator.HideContextIndicator();
        }
    }
}