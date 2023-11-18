using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities.Weapons
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        protected Player _player;
        protected MeleeWeapon _meleeWeapon;

        // Start is called before the first frame update
        void Awake()
        {
            _player = GetComponentInParent<Player>();
            _meleeWeapon = _player.GetComponentInChildren<MeleeWeapon>();
            
            
        }

        public void MeleeAttackStart(int throwing = 0)
        {
            _meleeWeapon.BeginAttack(throwing != 0);
        }

        public void MeleeAttackFinish()
        {
            _meleeWeapon.EndAttack();
        }
    }
}
