
/****************************************************
	文件：EffectOfflineData.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:58   	
	Features：
*****************************************************/
using UnityEngine;
namespace EdgeFramework.Res
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