using Rocketeer;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RocketeerStates
{
    public class RocketeerFireRocket2 : BaseSkillState
    {
        public float baseDuration = 0.3f;
        public float recoil = 0f;

        private float duration;
        private bool hasFired;

        private float baseDamage = 3.5f;
        private float defaultVelocity = 200.0f;


        public override void OnEnter()
        {
            base.OnEnter();
            FireRocket();
            this.duration = this.baseDuration / this.attackSpeedStat;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireRocket()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                Ray aimRay = base.GetAimRay();

                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(ContentPacks.arrowProjectile,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject,
                        this.baseDamage * this.damageStat,
                        0f,
                        base.RollCrit(),
                        DamageColorIndex.Default,
                        null,
                        defaultVelocity);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

//list rocket/mine skill
//firerocket triggers rocket skill
//firemine triggers mine skill
//jet can trigger rocket and mine skill