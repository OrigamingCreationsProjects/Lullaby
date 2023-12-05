using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Moon Animation")]
    public class MoonAnimation : MonoBehaviour
    {
        private Animator _animator;
        private Transform _smallMoon;
        private Transform _bigMoon;

        public AnimationCurve punchCurve;
        [Header("Particles")] 
        public ParticleSystem glowParticle;
        public ParticleSystem chargeParticle;
        public ParticleSystem explodeParticle;
        public ParticleSystem smokeParticle;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _bigMoon = transform.GetChild(0);
            _smallMoon = transform.GetChild(1);
        }
        
        public Sequence Reset(float time)
        {
            _animator.enabled = false;
            Sequence s = DOTween.Sequence();
            s.Append(_bigMoon.DOLocalRotate(Vector3.zero, time).SetEase(Ease.InOutSine));
            s.Join(_smallMoon.DOLocalRotate(Vector3.zero, time).SetEase(Ease.InOutSine));
            return s;
        }

        public Sequence PullMoon(float pullTime)
        {
            glowParticle.Play();
            chargeParticle.Play();

            Sequence s = DOTween.Sequence();

            s.Append(_bigMoon.DOLocalRotate(new Vector3(0, 0, 360 * 2), pullTime, RotateMode.LocalAxisAdd))
                .SetEase(Ease.OutQuart);
            s.Join(_smallMoon.DOLocalRotate(new Vector3(0, 0, 360 * 2), pullTime, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuart));
            s.Join(_smallMoon.DOLocalMoveZ(-4.2f, pullTime));

            return s;
        }

        public Sequence PunchMoon(float punchTime)
        {
            CinemachineImpulseSource[] impulses = FindObjectsOfType<CinemachineImpulseSource>();

            _animator.enabled = false;

            Sequence s = DOTween.Sequence();
            
            s.AppendCallback(() => explodeParticle.Play());
            s.AppendCallback(() => smokeParticle.Play());
            s.AppendCallback(() => impulses[0].GenerateImpulse());
            s.Append(_smallMoon.DOLocalMove(Vector3.zero, .8f).SetEase(punchCurve));
            s.Join(_smallMoon.DOLocalRotate(new Vector3(0, 0, 360 * 2), .8f).SetEase(Ease.OutBack));
            s.AppendInterval(.8f);
            s.AppendCallback(() => _animator.enabled = true);

            return s;
        }
        
    }
}