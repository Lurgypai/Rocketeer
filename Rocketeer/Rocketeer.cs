using System;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using R2API;
using R2API.Utils;
using EntityStates;
using EntityStates.RocketeerStates;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using KinematicCharacterController;

namespace Rocketeer
{

    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin(MODUID, "Rocketeer", "0.0.2")] // put your own name and version here
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LanguageAPI), nameof(LoadoutAPI), nameof(ItemAPI))] // need these dependencies for the mod to work properly


    public class Rocketeer : BaseUnityPlugin
    {
        public const string MODUID = "com.lurgypai.Rocketeer"; // put your own names here

        private void Awake()
        {
            Hooks.hook();
            ContentPacks.LoadContent();
        }
    }
}