using System;
using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Huntress;

namespace Rocketeer
{
    class RocketeerChargeUlt : BaseSkillState
    {
        private float baseJumpCoefficient = 0.2f;
        private float duration = 6.0f;

        private float projectileVelocity = 70.0f;
        private float damageCoefficient = 15.0f;

        private float recoilBase = 20.0f;
        private float recoilPerFire = 2.0f;

        private float stockCoefficient = 0.50f;

        private bool hasJumped = false;

        private float minSpread = 0.0f;
        private float maxSpread = 4.0f;
        private float spreadPitchScale = 1.0f;
        private float spreadYawScale = 1.0f;
        private float force = 1.0f;

        private GameObject trackingIndicator = null;
        private HurtBox targetHurtbox;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                GenericSkill specialSkill = this.skillLocator.special;
                int stockCount = specialSkill.stock;

                Ray aimRay = this.GetAimRay();

                Vector3[] array = new Vector3[stockCount];
                Vector3 up = Vector3.up;
                Vector3 axis = Vector3.Cross(up, aimRay.direction);
                int num = 0;
                while ((long)num < (long)((ulong)stockCount))
                {
                    float x = UnityEngine.Random.Range(this.minSpread, this.maxSpread);
                    float z = UnityEngine.Random.Range(0f, 360f);
                    Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
                    float y = vector.y;
                    vector.y = 0f;
                    float angle = (Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f) * this.spreadYawScale;
                    float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f * this.spreadPitchScale;
                    array[num] = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * aimRay.direction);
                    num++;
                }

                bool crit = base.RollCrit();

                GameObject target = this.targetHurtbox != null ? this.targetHurtbox.gameObject : null;
                float damage = this.damageCoefficient * this.damageStat * (1.0f + (this.stockCoefficient * ((float)(stockCount) / (float)(specialSkill.maxStock))));

                for (int i = 0; i != stockCount; ++i) {
                    ProjectileManager.instance.FireProjectile(ContentPacks.specialProjectile,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(array[i]),
                        base.gameObject,
                        damage,
                        this.force,
                        crit,
                        DamageColorIndex.Default,
                        target,
                        this.projectileVelocity);
                }

                specialSkill.DeductStock(stockCount);
                base.characterBody.characterMotor.velocity = aimRay.direction.normalized * -(this.recoilBase + this.recoilPerFire * stockCount);

                if (this.trackingIndicator)
                    Destroy(this.trackingIndicator.gameObject);
                this.trackingIndicator = null;
            }
            base.OnExit();
        }

        

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                findTarget();
                if (base.inputBank.jump.down && !hasJumped)
                {
                    hasJumped = true;
                    base.characterBody.characterMotor.Motor.ForceUnground();
                    Vector3 currVelocity = base.characterMotor.velocity;
                    currVelocity.y = this.baseJumpCoefficient * base.characterBody.jumpPower * base.moveSpeedStat;
                    base.characterMotor.velocity = currVelocity;
                }
                if ((!base.inputBank.skill1.down && base.inputBank.skill1.wasDown) || base.fixedAge >= this.duration)
                {
                    base.outer.SetNextStateToMain();
                }
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        internal void findTarget()
        {
            float maxDistance = 200.0f;
            float maxAngle = 90;

            if (base.characterBody)
            {
                float num = 0;
                Ray ray = CameraRigController.ModifyAimRayIfApplicable(base.GetAimRay(), base.gameObject, out num);
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.searchOrigin = ray.origin;
                bullseyeSearch.searchDirection = ray.direction;
                bullseyeSearch.maxDistanceFilter = maxDistance + num;
                bullseyeSearch.maxAngleFilter = maxAngle;
                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                bullseyeSearch.RefreshCandidates();
                this.targetHurtbox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
            }
            if (this.targetHurtbox)
            {
                if (!this.trackingIndicator)
                {
                    this.trackingIndicator = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
                    this.trackingIndicator.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                }
                this.trackingIndicator.transform.position = this.targetHurtbox.transform.position;
            }
            else if (this.trackingIndicator)
            {
                EntityState.Destroy(this.trackingIndicator.gameObject);
                this.trackingIndicator = null;
            }
        }
    }
}
