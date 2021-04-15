using BepInEx;
using EntityStates;
using EntityStates.RocketeerStates;
using KinematicCharacterController;
using System;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Rocketeer
{
	public class ContentPacks
	{
		public static void LoadContent()
        {
            //register item so we can use it
            RegisterItems();
            CreatePrefab();
            SkillSetup();
            RegisterProjectiles();
            RegisterSurvivors();
            CreateDoppelganger();
            CreateContentPack();
            
        }



		public static void CreateContentPack()
		{
            ContentPacks.contentPack = new ContentPack();
            ContentPacks.contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
            ContentPacks.contentPack.buffDefs.Add(buffDefs.ToArray());
            ContentPacks.contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
            ContentPacks.contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());
            ContentPacks.contentPack.survivorDefs.Add(survivorDefs.ToArray());
            ContentPacks.contentPack.itemDefs.Add(itemDefs.ToArray());

            On.RoR2.ContentManagement.ContentManager.SetContentPacks += new On.RoR2.ContentManagement.ContentManager.hook_SetContentPacks(ContentPacks.AddContent);
		}
		private static void AddContent(On.RoR2.ContentManagement.ContentManager.orig_SetContentPacks orig, List<ReadOnlyContentPack> newContentPacks)
		{
			newContentPacks.Add(ContentPacks.contentPack);
			orig.Invoke(newContentPacks);
		}

		public static void CreatePrefab()
        {
            // first clone the commando prefab so we can turn that into our own survivor
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "RocketeerBody", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "CreatePrefab", 151);

            characterPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            // set up the character body here
            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            GameObject gameObject4 = new GameObject("AimOrigin");
            bodyComponent.name = "RocketeerBody";
            bodyComponent.baseNameToken = "ROCKETEER_NAME";
            bodyComponent.subtitleNameToken = "ROCKETEER_SUBTITLE";
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;
            bodyComponent.mainRootSpeed = 0f;
            bodyComponent.baseMaxHealth = 85f;
            bodyComponent.levelMaxHealth = 20f;
            bodyComponent.baseRegen = 1.0f;
            bodyComponent.levelRegen = 0.2f;
            bodyComponent.baseMaxShield = 0f;
            bodyComponent.levelMaxShield = 0f;
            bodyComponent.baseMoveSpeed = 7f;
            bodyComponent.levelMoveSpeed = 0f;
            bodyComponent.baseAcceleration = 80f;
            bodyComponent.baseJumpPower = 15f;
            bodyComponent.levelJumpPower = 0f;
            bodyComponent.baseDamage = 13f;
            bodyComponent.levelDamage = 2.7f;
            bodyComponent.baseAttackSpeed = 1f;
            bodyComponent.levelAttackSpeed = 0f;
            bodyComponent.baseCrit = 1f;
            bodyComponent.levelCrit = 0f;
            bodyComponent.baseArmor = 20f;
            bodyComponent.levelArmor = 0f;
            bodyComponent.baseJumpCount = 1;
            bodyComponent.sprintingSpeedMultiplier = 1.5f;
            bodyComponent.wasLucky = false;
            bodyComponent.hideCrosshair = false;
            bodyComponent.crosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            bodyComponent.aimOriginTransform = gameObject4.transform;
            bodyComponent.hullClassification = HullClassification.Human;
            bodyComponent.isChampion = false;
            bodyComponent.currentVehicle = null;
            bodyComponent.skinIndex = 0U;
            bodyComponent.bodyColor = characterColor;
            bodyComponent.tag = "Player";

            // the charactermotor controls the survivor's movement and stuff
            CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            characterMotor.walkSpeedPenaltyCoefficient = 1f;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 95f;
            characterMotor.airControl = 0.7f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;
            //characterMotor.useGravity = true;
            //characterMotor.isFlying = false;

            InputBankTest inputBankTest = characterPrefab.GetComponent<InputBankTest>();
            inputBankTest.moveVector = Vector3.zero;

            CameraTargetParams cameraTargetParams = characterPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = null;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;

            // this component is used to locate the character model(duh), important to set this up here
            ModelLocator modelLocator = characterPrefab.GetComponent<ModelLocator>();
            //modelLocator.modelTransform = transform;
            //modelLocator.modelBaseTransform = gameObject.transform;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.noCorpse = false;
            modelLocator.normalizeToFloor = false; // set true if you want your character to rotate on terrain like acrid does
            modelLocator.preserveModel = false;


            TeamComponent teamComponent = null;
            if (characterPrefab.GetComponent<TeamComponent>() != null) teamComponent = characterPrefab.GetComponent<TeamComponent>();
            else teamComponent = characterPrefab.GetComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = false;
            teamComponent.teamIndex = TeamIndex.None;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.health = 100f;
            healthComponent.shield = 0f;
            healthComponent.barrier = 0f;
            healthComponent.magnetiCharge = 0f;
            healthComponent.body = null;
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;

            characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
            characterPrefab.GetComponent<InteractionDriver>().highlightInteractor = true;

            // this disables ragdoll since the character's not set up for it, and instead plays a death animation
            CharacterDeathBehavior characterDeathBehavior = characterPrefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = characterPrefab.GetComponent<EntityStateMachine>();
            characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));

            // edit the sfxlocator if you want different sounds
            SfxLocator sfxLocator = characterPrefab.GetComponent<SfxLocator>();
            sfxLocator.deathSound = "Play_ui_player_death";
            sfxLocator.barkSound = "";
            sfxLocator.openSound = "";
            sfxLocator.landingSound = "Play_char_land";
            sfxLocator.fallDamageSound = "Play_char_land_fall_damage";
            sfxLocator.aliveLoopStart = "";
            sfxLocator.aliveLoopStop = "";

            Rigidbody rigidbody = characterPrefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.None;

            CapsuleCollider capsuleCollider = characterPrefab.GetComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = false;
            capsuleCollider.material = null;
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;

            KinematicCharacterMotor kinematicCharacterMotor = characterPrefab.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor.Rigidbody = rigidbody;

            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.center = new Vector3(0, 0, 0);
            capsuleCollider.material = null;

            kinematicCharacterMotor.DetectDiscreteCollisions = false;
            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.PreventSnappingOnLedges = false;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;
            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;
            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = Vector3.up;
            kinematicCharacterMotor.StepHandling = StepHandlingMethod.None;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;

            characterPrefab.AddComponent<RocketeerShinyJetpackController>();
        }

        public static void SkillSetup()
        {
            // get rid of the original skills first, otherwise we'll have commando's loadout and we don't want that
            foreach (GenericSkill obj in characterPrefab.GetComponentsInChildren<GenericSkill>())
            {
                BaseUnityPlugin.DestroyImmediate(obj);
            }

            ContentPacks.entityStates.Add(typeof(RocketeerFireRocket2));
            ContentPacks.entityStates.Add(typeof(RocketeerAimThrowableBase));
            ContentPacks.entityStates.Add(typeof(RocketeerAimDetpack));
            ContentPacks.entityStates.Add(typeof(RocketeerJet));
            ContentPacks.entityStates.Add(typeof(RocketeerChargeUlt));
            ContentPacks.entityStates.Add(typeof(RocketeerLaunchUlt));

            PassiveSetup();
            PrimarySetup();
            SecondarySetup();
            UtilitySetup();
            UltSetup();
        }

        internal static void PassiveSetup()
        {
            // set up the passive skill here if you want
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("ROCKETEER_PASSIVE_NAME", "Shiny Jetpack");
            LanguageAPI.Add("ROCKETEER_PASSIVE_DESC", "Oooooh, look at that!");

            component.passiveSkill.enabled = true;
            component.passiveSkill.skillNameToken = "ROCKETEER_PASSIVE_NAME";
            component.passiveSkill.skillDescriptionToken = "ROCKETEER_PASSIVE_DESC";
            component.passiveSkill.icon = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
        }

        internal static void PrimarySetup()
        {
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("ROCKETEER_PRIMARY_ROCKET_NAME", "Rocket");
            LanguageAPI.Add("ROCKETEER_PRIMARY_ROCKET_DESCRIPTION", "Fire a rocket that does <style=cIsDamage>350% damage</style>. Hold up to <style=cIsDamage>6</style>. All rockets are recharged on cooldown.");

            // set up your primary skill def here!

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(RocketeerFireRocket2));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 6;
            mySkillDef.baseRechargeInterval = 1.3f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.rechargeStock = 6;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            mySkillDef.skillDescriptionToken = "ROCKETEER_PRIMARY_ROCKET_DESCRIPTION";
            mySkillDef.skillName = "ROCKETEER_PRIMARY_ROCKET_NAME";
            mySkillDef.skillNameToken = "ROCKETEER_PRIMARY_ROCKET_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.primary = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.primary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };


            // add this code after defining a new skilldef if you're adding an alternate skill

            /*Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(newSkillDef.skillNameToken, false, null)
            };*/

            ContentPacks.skillDefs.Add(mySkillDef);
            ContentPacks.skillFamilies.Add(newFamily);

            LoadoutAPI.AddSkill(typeof(RocketeerFireRocket2));
        }

        internal static void SecondarySetup()
        {
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("ROCKETEER_SECONDARY_DETPACK_NAME", "Sticky Grenades");
            LanguageAPI.Add("ROCKETEER_SECONDARY_DETPACK_DESCRIPTION", "Stick to the wall. <style=cIsDamage>600% Damage.</style>");

            // set up your primary skill def here!

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(RocketeerAimDetpack));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 3;
            mySkillDef.baseRechargeInterval = 3f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            mySkillDef.skillDescriptionToken = "ROCKETEER_SECONDARY_DETPACK_DESCRIPTION";
            mySkillDef.skillName = "ROCKETEER_SECONDARY_DETPACK_NAME";
            mySkillDef.skillNameToken = "ROCKETEER_SECONDARY_DETPACK_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.secondary = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.secondary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.secondary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };


            // add this code after defining a new skilldef if you're adding an alternate skill

            /*Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(newSkillDef.skillNameToken, false, null)
            };*/


            ContentPacks.skillDefs.Add(mySkillDef);
            ContentPacks.skillFamilies.Add(newFamily);

            LoadoutAPI.AddSkill(typeof(RocketeerAimDetpack));
        }

        internal static void UtilityStateMachineSetup()
        {
            EntityStateMachine entityStateMachine = characterPrefab.AddComponent<EntityStateMachine>();
            entityStateMachine.customName = "ROCKETEER_Utility";
            entityStateMachine.initialStateType = new SerializableEntityStateType(typeof(BaseState));
            entityStateMachine.mainStateType = new SerializableEntityStateType(typeof(BaseState));
        }
        internal static void UtilitySetup()
        {
            UtilityStateMachineSetup();

            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("ROCKETEER_UTILITY_JET_NAME", "Jet");
            LanguageAPI.Add("ROCKETEER_UTILITY_JET_DESCRIPTION", "They fly now.");

            // set up your primary skill def here!

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(RocketeerJet));
            mySkillDef.activationStateMachineName = "ROCKETEER_Utility";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 6.0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            mySkillDef.skillDescriptionToken = "ROCKETEER_UTILITY_JET_DESCRIPTION";
            mySkillDef.skillName = "ROCKETEER_UTILITY_JET_NAME";
            mySkillDef.skillNameToken = "ROCKETEER_UTILITY_JET_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.utility = characterPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.utility.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.utility.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };


            // add this code after defining a new skilldef if you're adding an alternate skill

            /*Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(newSkillDef.skillNameToken, false, null)
            };*/


            ContentPacks.skillDefs.Add(mySkillDef);
            ContentPacks.skillFamilies.Add(newFamily);

            LoadoutAPI.AddSkill(typeof(RocketeerJet));
        }

        internal static void UltSetup()
        {
            {
                SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

                LanguageAPI.Add("ROCKETEER_ULT_NAME", "Big Booma Cannon");
                LanguageAPI.Add("ROCKETEER_ULT_DESCRIPTION", "Ready to go, Boss. " +
                    "Each rocket does <style=cIsDamage>1500% damage</style> and takes <style=cIsDamage>12 seconds</style> to recharge. " +
                    "Additionally, when launched each rocket launched increases the damage of all rockets by <style=cIsDamage>10%</style>. Hold up to <style=cIsDamage>5</style>.");

                // set up your primary skill def here!

                chargeUltDef = ScriptableObject.CreateInstance<SkillDef>();
                chargeUltDef.activationState = new SerializableEntityStateType(typeof(RocketeerChargeUlt));
                chargeUltDef.activationStateMachineName = "Weapon";
                chargeUltDef.baseMaxStock = 5;
                chargeUltDef.baseRechargeInterval = 12f;
                chargeUltDef.beginSkillCooldownOnSkillEnd = true;
                chargeUltDef.canceledFromSprinting = false;
                chargeUltDef.fullRestockOnAssign = true;
                chargeUltDef.interruptPriority = InterruptPriority.Skill;
                chargeUltDef.isCombatSkill = true;
                chargeUltDef.mustKeyPress = false;
                chargeUltDef.rechargeStock = 1;
                chargeUltDef.requiredStock = 1;
                chargeUltDef.stockToConsume = 0;
                chargeUltDef.icon = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
                chargeUltDef.skillDescriptionToken = "ROCKETEER_ULT_DESCRIPTION";
                chargeUltDef.skillName = "ROCKETEER_ULT_NAME";
                chargeUltDef.skillNameToken = "ROCKETEER_ULT_NAME";

                LoadoutAPI.AddSkillDef(chargeUltDef);

                component.special = characterPrefab.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                component.special.SetFieldValue("_skillFamily", newFamily);
                SkillFamily skillFamily = component.special.skillFamily;

                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = chargeUltDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(chargeUltDef.skillNameToken, false, null)
                };


                // add this code after defining a new skilldef if you're adding an alternate skill


                ContentPacks.skillDefs.Add(chargeUltDef);
                ContentPacks.skillFamilies.Add(newFamily);
            }
        }
        public static void RegisterProjectiles()
        {
            // clone rex's syringe projectile prefab here to use as our own projectile
            arrowProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SyringeProjectile"), "Prefabs/Projectiles/RocketeerRocketProjectile", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "RegisterCharacter", 155);

            // just setting the numbers to 1 as the entitystate will take care of those
            arrowProjectile.GetComponent<ProjectileController>().procCoefficient = 1.0f;
            arrowProjectile.GetComponent<ProjectileController>().ghostPrefab = Resources.Load<GameObject>("Prefabs/RocketGhost");
            arrowProjectile.GetComponent<ProjectileDamage>().damage = 0f;
            arrowProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;

            UnityEngine.Object.Destroy(arrowProjectile.GetComponent<ProjectileSingleTargetImpact>());

            arrowProjectile.AddComponent<ProjectileImpactExplosion>();
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = true;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnWorld = true;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().blastRadius = 4;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = 1;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().impactEffect = Resources.Load<GameObject>("Prefabs/Effects/Impacteffects/ExplosionVFX");
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().offsetForLifetimeExpiredSound = 0;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().timerAfterImpact = false;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().lifetime = 1.0f;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().lifetimeAfterImpact = 0;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().blastAttackerFiltering = AttackerFiltering.Default;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.Linear;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().bonusBlastForce = new Vector3(0.0f, 0.0f, 0.0f);
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().fireChildren = false;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().childrenCount = 0;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().childrenDamageCoefficient = 0;
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().minAngleOffset = new Vector3(0f, 0f, 0f);
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().maxAngleOffset = new Vector3(0f, 0f, 0f);
            arrowProjectile.GetComponent<ProjectileImpactExplosion>().transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            arrowProjectile.GetComponent<SphereCollider>().radius = 0.5f;

            specialProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SyringeProjectile"), "Prefabs/Projectiles/RocketeerRocketProjectile", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "RegisterCharacter", 155);

            specialProjectile.GetComponent<ProjectileController>().procCoefficient = 1.0f;
            specialProjectile.GetComponent<ProjectileController>().ghostPrefab = Resources.Load<GameObject>("Prefabs/RocketGhost");
            specialProjectile.GetComponent<ProjectileDamage>().damage = 0f;
            specialProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            specialProjectile.GetComponent<ProjectileSimple>().updateAfterFiring = true;

            UnityEngine.Object.Destroy(specialProjectile.GetComponent<ProjectileSingleTargetImpact>());

            specialProjectile.AddComponent<ProjectileImpactExplosion>();
            specialProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = true;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().destroyOnWorld = true;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().blastRadius = 2;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = 1;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().impactEffect = Resources.Load<GameObject>("Prefabs/Effects/Impacteffects/ExplosionVFX");
            specialProjectile.GetComponent<ProjectileImpactExplosion>().offsetForLifetimeExpiredSound = 0;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().timerAfterImpact = false;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().lifetime = 10;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().lifetimeAfterImpact = 0;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().blastAttackerFiltering = AttackerFiltering.Default;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().bonusBlastForce = new Vector3(0.0f, 0.0f, 0.0f);
            specialProjectile.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.SweetSpot;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().fireChildren = false;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().childrenCount = 0;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().childrenDamageCoefficient = 0;
            specialProjectile.GetComponent<ProjectileImpactExplosion>().minAngleOffset = new Vector3(0f, 0f, 0f);
            specialProjectile.GetComponent<ProjectileImpactExplosion>().maxAngleOffset = new Vector3(0f, 0f, 0f);
            specialProjectile.GetComponent<ProjectileImpactExplosion>().transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            specialProjectile.AddComponent<ProjectileTargetComponent>();

            specialProjectile.AddComponent<ProjectileSteerTowardTarget>();
            specialProjectile.GetComponent<ProjectileSteerTowardTarget>().rotationSpeed = 700;
            specialProjectile.GetComponent<ProjectileSteerTowardTarget>().yAxisOnly = false;
            //specialProjectile.GetComponent<SphereCollider>().radius = 0.5f;

            detPack = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/StickyBomb"), "Prefabs/Projectiles/RocketeerDetpack", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "RegisterCharacter", 155);
            detPack.GetComponent<ProjectileController>().procCoefficient = 1.0f;
            detPack.GetComponent<SphereCollider>().radius = 0.5f;

            // register it for networking
            if (arrowProjectile) PrefabAPI.RegisterNetworkPrefab(arrowProjectile);
            if (specialProjectile) PrefabAPI.RegisterNetworkPrefab(specialProjectile);
            if (detPack) PrefabAPI.RegisterNetworkPrefab(detPack);

            ContentPacks.projectilePrefabs.Add(arrowProjectile);
            ContentPacks.projectilePrefabs.Add(specialProjectile);
            ContentPacks.projectilePrefabs.Add(detPack);

        }

        public static void RegisterSurvivors()
		{

            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.displayNameToken = "ROCKETEER_NAME";
			survivorDef.unlockableDef = null;
			survivorDef.descriptionToken = "ROCKETEER_DESCRIPTION";
			survivorDef.primaryColor = characterColor;
			survivorDef.bodyPrefab = characterPrefab;
			survivorDef.displayPrefab = characterDisplay;
			survivorDef.outroFlavorToken = "MINER_OUTRO_FLAVOR";
			survivorDef.hidden = false;
			survivorDef.desiredSortPosition = 17f;

			bodyPrefabs.Add(characterPrefab);
			survivorDefs.Add(survivorDef);

            // now that the body prefab's set up, clone it here to make the display prefab
            characterDisplay = PrefabAPI.InstantiateClone(characterPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "RocketeerDisplay", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "RegisterCharacter", 153);
            characterDisplay.AddComponent<NetworkIdentity>();

            // write a clean survivor description here!
            string desc = "Rocketeer.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Shoot." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Shoot." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bang." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Boom.</color>" + Environment.NewLine + Environment.NewLine;

            // add the language tokens
            LanguageAPI.Add("ROCKETEER_NAME", "Rocketeer");
            LanguageAPI.Add("ROCKETEER_DESCRIPTION", desc);
            LanguageAPI.Add("ROCKETEER_SUBTITLE", "Flight of the boomble-bee.");
        }

        public static void RegisterItems()
        {
            shinyJetpackDef = ScriptableObject.CreateInstance<ItemDef>();
            shinyJetpackDef.name = "ROCKETEER_JETPACKITEM_NAME";
            shinyJetpackDef.nameToken = "ROCKETEER_JETPACKITEM_NAME";
            shinyJetpackDef.pickupToken = "ROCKETEER_JETPACKITEM_PICKUP";
            shinyJetpackDef.descriptionToken = "ROCKETEER_JETPACKITEM_DESC";
            shinyJetpackDef.loreToken = "ROCKETEER_JETPACKITEM_LORE";
            shinyJetpackDef.tier = ItemTier.Tier3;
            shinyJetpackDef.pickupIconSprite = Resources.Load<Sprite>("Textures/MiscIcons/texMysteryIcon");
            shinyJetpackDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
            shinyJetpackDef.canRemove = true;
            shinyJetpackDef.hidden = false;

            LanguageAPI.Add("ROCKETEER_JETPACKITEM_NAME", "Shiny Jetpack");
            LanguageAPI.Add("ROCKETEER_JETPACKITEM_PICKUP", "I'm a little rusty, but this jetpack isn't!");
            LanguageAPI.Add("ROCKETEER_JETPACKITEM_DESC", "Triples move speed and doubles jump height after other effects are added.");
            LanguageAPI.Add("ROCKETEER_JETPACK_LORE", "Its been cleaned up, but its still not that great.");

            ItemAPI.Add(new CustomItem(shinyJetpackDef, new ItemDisplayRuleDict(null)));
            //itemDefs.Add(shinyJetpackDef);

        }
        public static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "RocketeerMonsterMaster", true, "C:\\Users\\Tinde\\Desktop\\Lurgypai\\ROR2\\mods\\Projects\\files\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\ExampleSurvivor\\Rocketeer.cs", "CreateDoppelganger", 159);

            CharacterMaster component = doppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = characterPrefab;

            ContentPacks.masterPrefabs.Add(doppelganger);
        }

        public static List<GameObject> bodyPrefabs = new List<GameObject>();
		public static List<BuffDef> buffDefs = new List<BuffDef>();
		public static List<EffectDef> effectDefs = new List<EffectDef>();
		public static List<Type> entityStates = new List<Type>();
		public static List<GameObject> masterPrefabs = new List<GameObject>();
		public static List<GameObject> projectilePrefabs = new List<GameObject>();
		public static List<SkillDef> skillDefs = new List<SkillDef>();
		public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
		public static List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
        public static List<ItemDef> itemDefs = new List<ItemDef>();

        public static GameObject characterPrefab;
        public static GameObject characterDisplay;

        public static GameObject arrowProjectile;
        public static GameObject specialProjectile;
        public static GameObject detPack;
        public static GameObject doppelganger;

        public static SkillDef chargeUltDef;
        public static SkillDef launchUltDef;

        public static ItemDef shinyJetpackDef;

        internal static ContentPack contentPack;
        public static readonly Color characterColor = new Color(0.55f, 0.55f, 0.55f); // color used for the survivor
    }
}
