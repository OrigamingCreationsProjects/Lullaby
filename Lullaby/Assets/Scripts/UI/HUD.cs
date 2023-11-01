using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities;
using MoreMountains.Tools;
using UnityEngine;

namespace Lullaby.UI
{
    [AddComponentMenu("Lullaby/UI/HUD")]
    public class HUD : MonoBehaviour
    {
        [Header("UI Elements")]
        public MMProgressBar healthBar;

        protected Player _player;

        protected float timerStep;
        protected static float timerRefreshRate = .1f; // 10 times per second

        protected virtual void UpdateHealth()
        {
            healthBar.UpdateBar01(NormalizeValue(_player.health.current, 0, _player.health.max));
            //healthBar.Minus10Percent();
        }

        public virtual void Refresh()
        {
            UpdateHealth();
        }

        private float NormalizeValue(float valueToNormalize, float minVal, float maxVal)
        {
            return ((valueToNormalize - minVal) / (maxVal - minVal));
        }
        
        protected virtual void Start()
        {
            _player = FindObjectOfType<Player>();
            _player.health.onChange.AddListener(UpdateHealth);
            //Refresh();
        }
    }
}
