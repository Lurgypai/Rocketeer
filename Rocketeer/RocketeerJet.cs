using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace EntityStates.RocketeerStates
{
    public class RocketeerJet : BaseSkillState
    {
        public float recoil = 3f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");

        private float duration = 1.7f;
        private bool hasFired;
        
        private float baseMaxFlySpeed = 2.0f;
        private float baseTurnSpeed = 0.6f;
        private float turnSpeed;

        private float maxFlySpeed;
        private float baseAccel = 0.05f;
        private float baseUpwardAccelFalling = 0.1f;
        private float baseUpwardsAccel = 0.08f;
        private float forwardAccel;
        private Vector3 forwardDirection;
        private float upwardsAccel;
        private float upwardsAccelFalling;
        private Vector3 steerDirection;

        private Vector3 currVel;
        public override void OnEnter()
        {
            base.OnEnter();
            //base.characterBody.SetAimTimer(2f);

            StartFlight();
            //base.PlayAnimation("Gesture, Override", "FireArrow", "FireArrow.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void StartFlight()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                if (base.isAuthority)
                {
                    if (base.inputBank && base.characterDirection)
                    {
                        this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                    }


                    base.characterMotor.Motor.ForceUnground();
                    this.forwardAccel = baseAccel * base.moveSpeedStat;
                    this.upwardsAccel = baseUpwardsAccel * base.moveSpeedStat;
                    this.upwardsAccelFalling = baseUpwardAccelFalling * base.moveSpeedStat;
                    this.maxFlySpeed = this.baseMaxFlySpeed * base.moveSpeedStat;
                    currVel = this.characterMotor.velocity;
                    this.steerDirection = this.forwardDirection;
                    float newTurnSpeed = this.baseTurnSpeed * base.moveSpeedStat;
                    turnSpeed = newTurnSpeed > 1.0f ? 1.0f : newTurnSpeed;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (base.fixedAge < this.duration / 2)
                {
                    if (currVel.y < 0) {
                        currVel.y += upwardsAccelFalling;
                    }
                    else {
                        if (currVel.y < maxFlySpeed)
                        {
                            currVel.y += upwardsAccel;
                        }
                    }

                } else
                {
                    currVel.y = this.characterMotor.velocity.y;
                }
                if (base.inputBank && base.characterDirection)
                {
                    this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                }

                Vector3 horizontalDir = new Vector3(currVel.x, 0, currVel.z);
                horizontalDir = horizontalDir.normalized;

                this.steerDirection = (this.forwardDirection * turnSpeed) + ((1.0f - turnSpeed) * horizontalDir);

                currVel += this.forwardAccel * this.steerDirection;

                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = currVel;

                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                }

                if (!base.inputBank.skill3.down)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}