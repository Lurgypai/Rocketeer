using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Rocketeer
{
    class RocketeerLaunchUlt : BaseSkillState
    {
        private float flightVelocity = 60.0f;
        private float duration = 4.0f;
        private float damageCoefficient = 7.0f;
        private float blastForce = 1.0f;
        private Vector3 bonusForce = new Vector3(0, 0, 0);
        private float blastRadius = 10;
        private float procCoefficient = 1.0f;

        private float startTime = 0.0f;

        private bool flying = false;
        private Transform targetTransform;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.inputBank.skill1.down)
            {
                startTime = base.fixedAge;
                if (base.isAuthority)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow80);
                }
                flying = true;
            }

            if (flying)
            {
                doFlight();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow80);
            }

            if (!this.targetTransform)
            {
                //this.targetTransform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ShieldTransferIndicator"), this.trackingTarget.transform.position, Quaternion.identity).transform;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            EffectData effectData = new EffectData
            {
                origin = base.transform.position,
                scale = this.blastRadius
            };

            if (NetworkServer.active)
            {
                GameObject explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/Impacteffects/ExplosionVFX");
                EffectManager.SpawnEffect(explosionEffect, effectData, true);
                new BlastAttack
                {
                    attacker = this.characterBody.gameObject,
                    baseDamage = this.damageCoefficient * this.damageStat,
                    baseForce = this.blastForce,
                    bonusForce = this.bonusForce,
                    attackerFiltering = AttackerFiltering.NeverHit,
                    crit = this.characterBody.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = RoR2.DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.SweetSpot,
                    inflictor = base.characterBody.gameObject,
                    position = base.characterBody.transform.position,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = this.procCoefficient,
                    radius = this.blastRadius,
                    teamIndex = this.characterBody.teamComponent.teamIndex
                }.Fire();
            }
        }

        private void doFlight()
        {
            Ray aim = this.GetAimRay();
            Vector3 targetDirection = aim.direction.normalized;
            Vector3 currDirection = base.characterMotor.velocity.normalized;

            float changeRate = 0.1f;
            Vector3 newAim = changeRate * targetDirection + (1 - changeRate) * currDirection;


            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = newAim.normalized * flightVelocity;

            if (base.inputBank.interact.down)
            {
                this.outer.SetNextStateToMain();
            }

            if (base.fixedAge - this.startTime >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
