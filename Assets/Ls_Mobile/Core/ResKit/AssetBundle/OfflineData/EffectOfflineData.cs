using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    public class EffectOfflineData :OfflineData
    {
        public ParticleSystem[] particle;
        public TrailRenderer[] trailRe;

        public override void ResetProp()
        {
            base.ResetProp();
            foreach (ParticleSystem particle in particle)
            {
                particle.Clear(true);
                particle.Play();
            }
            foreach (TrailRenderer trail in trailRe)
            {
                trail.Clear();
            }
        }
        public override void BindData()
        {
            base.BindData();
            particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            trailRe = gameObject.GetComponentsInChildren<TrailRenderer>(true);
        }
    }
}