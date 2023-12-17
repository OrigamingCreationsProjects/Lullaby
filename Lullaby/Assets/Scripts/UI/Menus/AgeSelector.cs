using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lullaby.UI.Menus
{
    public class AgeSelector : MonoBehaviour
    {
        public int currentAge = 0;
        public Button plusButton;
        public Button minusButton;
        public TextMeshProUGUI ageText;
        private void Start()
        {
            plusButton.onClick.AddListener(PlusButton);
            minusButton.onClick.AddListener(MinusButton);
        }
        
        private void PlusButton()
        {
            currentAge++;
            if (currentAge > 99)
            {
                currentAge = 99;
            }
            GamePlayerData.instance.UpdateAge(currentAge);
            ageText.text = currentAge.ToString();
        }
        
        private void MinusButton()
        {
            currentAge--;
            if (currentAge < 0)
            {
                currentAge = 0;
            }
            GamePlayerData.instance.UpdateAge(currentAge);
            ageText.text = currentAge.ToString();
        }
    }
}