using Lullaby.Entities.Weapons;
using UnityEngine;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Platformer Project/Player/Player Weapon")]
    public class PlayerWeapon : MonoBehaviour
    {
        protected MeleeWeapon _meleeWeapon;
        protected Player _player;
        
        protected virtual void EquipMeleeWeapon(bool equip)
        {
            _meleeWeapon.gameObject.SetActive(equip);
        }

        protected virtual void ActivateWeapon()
        {
            EquipMeleeWeapon(true);
            _meleeWeapon.ChangeSimulationSpaceFeatherParticle(ParticleSystemSimulationSpace.World);
            foreach (ParticleSystem p in  _meleeWeapon.appearParticles)
            {
                p.Play();
            }
           
        }
        
        protected virtual void DeactivateWeapon()
        {
            EquipMeleeWeapon(false);
        }
        
        protected virtual void InitializePlayer()
        {
            _player = GetComponent<Player>();
        }

        protected virtual void InitializeWeapon()
        {
            _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        }

        protected void Awake()
        {
            InitializeWeapon();
            _meleeWeapon.SetOwner(gameObject);
        }

        protected virtual void Start()
        {
            InitializePlayer();
            _player.playerEvents.OnAttackStarted?.AddListener(ActivateWeapon);
            _player.playerEvents.OnAttackFinished?.AddListener(DeactivateWeapon);
            _meleeWeapon.gameObject.SetActive(false);
        }
        
    }
    
}
