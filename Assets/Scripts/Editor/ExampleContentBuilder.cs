#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TowerArena.Abilities;
using TowerArena.Art;
using TowerArena.Cards;
using TowerArena.Decks;
using TowerArena.Enemies;
using TowerArena.Match;
using TowerArena.Spells;
using TowerArena.Support;
using TowerArena.Units;
using TowerArena.Upgrades;

namespace TowerArena.EditorTools
{
    public static class ExampleContentBuilder
    {
        private const string RootFolder = "Assets/ExampleContent";
        private const string PrefabFolder = RootFolder + "/Prefabs";
        private const string CardsFolder = RootFolder + "/Cards";
        private const string UpgradesFolder = RootFolder + "/Upgrades";
        private const string SpellsFolder = RootFolder + "/Spells";
        private const string SupportFolder = RootFolder + "/Support";
        private const string BalloonsFolder = RootFolder + "/Balloons";
        private const string AbilitiesFolder = RootFolder + "/Abilities";
        private const string DecksFolder = RootFolder + "/Decks";
        private const string WavesFolder = RootFolder + "/Waves";
        private const string SettingsFolder = RootFolder + "/Settings";

        [MenuItem("TowerArena/Build Example Content")]
        public static void BuildExampleContent()
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                EnsureFolder("Assets", "ExampleContent");
                EnsureFolder(RootFolder, "Prefabs");
                EnsureFolder(RootFolder, "Cards");
                EnsureFolder(RootFolder, "Upgrades");
                EnsureFolder(RootFolder, "Spells");
                EnsureFolder(RootFolder, "Support");
                EnsureFolder(RootFolder, "Balloons");
                EnsureFolder(RootFolder, "Abilities");
                EnsureFolder(RootFolder, "Decks");
                EnsureFolder(RootFolder, "Waves");
                EnsureFolder(RootFolder, "Settings");

                var projectilePrefab = CreateBasicProjectilePrefab();

                var dartMonkeyPrefab = CreateProjectileUnitPrefab(
                    "DartMonkey",
                    projectilePrefab,
                    new UnitStatProfile
                    {
                        Damage = 1.5f,
                        AttackRate = 1.1f,
                        Range = 4.5f,
                        Pierce = 2,
                        CanDetectCamo = false,
                        Targeting = TargetingPriority.First
                    });

                var engineerMonkeyPrefab = CreateProjectileUnitPrefab(
                    "EngineerMonkey",
                    projectilePrefab,
                    new UnitStatProfile
                    {
                        Damage = 1.2f,
                        AttackRate = 0.8f,
                        Range = 5.5f,
                        Pierce = 1,
                        CanDetectCamo = true,
                        Targeting = TargetingPriority.Strong
                    });

                var monkeyAcePrefab = CreateProjectileUnitPrefab(
                    "MonkeyAce",
                    projectilePrefab,
                    new UnitStatProfile
                    {
                        Damage = 2.0f,
                        AttackRate = 0.6f,
                        Range = 8.0f,
                        Pierce = 3,
                        CanDetectCamo = true,
                        Targeting = TargetingPriority.First
                    });

                var heroPrefab = CreateProjectileUnitPrefab(
                    "HeroSauda",
                    projectilePrefab,
                    new UnitStatProfile
                    {
                        Damage = 3.5f,
                        AttackRate = 0.9f,
                        Range = 5.0f,
                        Pierce = 2,
                        CanDetectCamo = true,
                        Targeting = TargetingPriority.Strong,
                        StartingAbilities = new[] { "ability_orbital_strike" }
                    },
                    bodyColor: new Color(0.3f, 0.1f, 0.6f));

                var basicBalloonPrefab = CreateBalloonPrefab("BasicBalloon", Color.red);
                var camoBalloonPrefab = CreateBalloonPrefab("CamoBalloon", new Color(0.2f, 0.6f, 0.2f));
                var leadBalloonPrefab = CreateBalloonPrefab("LeadBalloon", Color.gray);
                var bossBalloonPrefab = CreateBalloonPrefab("BossBalloon", new Color(0.5f, 0.1f, 0.1f));

                var dartPaths = CreateUpgradePaths("dartMonkey", new[]
                {
                    new UpgradePathSpec
                    {
                        PathId = "dart_core",
                        DisplayName = "Sharp Shots",
                        Description = "Improve dart potency and pierce.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 120, "Sharper Darts", "Increase damage by 0.5 and pierce by 1.") { DamageMod = 0.5f, PierceMod = 1 },
                            new UpgradeTierSpec(2, 240, "Razor Darts", "Increase pierce and range.") { PierceMod = 1, RangeMod = 0.5f },
                            new UpgradeTierSpec(3, 500, "Crossbow", "Big boost to damage and attack rate.") { DamageMod = 1.0f, AttackRateMod = -0.2f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "dart_support",
                        DisplayName = "Quick Shots",
                        Description = "Fire faster to overwhelm balloons.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 100, "Quick Shots", "Improves attack speed.") { AttackRateMod = -0.15f },
                            new UpgradeTierSpec(2, 220, "Very Quick Shots", "Further attack speed boost.") { AttackRateMod = -0.2f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "dart_utility",
                        DisplayName = "Enhanced Vision",
                        Description = "Detect hidden balloon types.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 150, "Enhanced Eyesight", "Allows detection of camo balloons.") { EnableCamo = true },
                            new UpgradeTierSpec(2, 300, "Night Vision", "Further range increase.") { RangeMod = 0.75f }
                        }
                    }
                });

                var engineerPaths = CreateUpgradePaths("engineerMonkey", new[]
                {
                    new UpgradePathSpec
                    {
                        PathId = "eng_core",
                        DisplayName = "Sprocket Boost",
                        Description = "Enhance turret damage.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 150, "Calibration", "Increase damage by 0.6.") { DamageMod = 0.6f },
                            new UpgradeTierSpec(2, 300, "Overclock", "Increase attack speed.") { AttackRateMod = -0.25f },
                            new UpgradeTierSpec(3, 600, "Tri-Shot", "Adds extra pierce and damage.") { DamageMod = 1.0f, PierceMod = 1 }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "eng_support",
                        DisplayName = "Sentry Support",
                        Description = "Improves support capabilities.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 130, "Deconstruction", "Bonus damage versus fortified.") { DamageMod = 0.4f },
                            new UpgradeTierSpec(2, 260, "Cleansing Foam", "Improves range for debuff support.") { RangeMod = 0.5f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "eng_utility",
                        DisplayName = "Utility Tools",
                        Description = "Expose hidden balloons.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 140, "Scanner", "Reveal camo balloons.") { EnableCamo = true },
                            new UpgradeTierSpec(2, 320, "EMP", "Improves pierce.") { PierceMod = 1 }
                        }
                    }
                });

                var acePaths = CreateUpgradePaths("monkeyAce", new[]
                {
                    new UpgradePathSpec
                    {
                        PathId = "ace_core",
                        DisplayName = "Bombardment",
                        Description = "Increase damage dramatically.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 200, "Heavy Bombs", "Boosts damage by 1.5.") { DamageMod = 1.5f },
                            new UpgradeTierSpec(2, 400, "Cluster Payload", "Adds splash damage via pierce increase.") { PierceMod = 1 },
                            new UpgradeTierSpec(3, 800, "Sky Shredder", "Major attack rate boost.") { AttackRateMod = -0.25f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "ace_support",
                        DisplayName = "Ace Maneuvers",
                        Description = "Improves agility.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 180, "Faster Flights", "Raise attack speed.") { AttackRateMod = -0.15f },
                            new UpgradeTierSpec(2, 360, "Sharper Turns", "Increase range.") { RangeMod = 1.0f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "ace_utility",
                        DisplayName = "Spy Plane",
                        Description = "Detect camo balloons.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 160, "Spy Plane", "Grants camo detection.") { EnableCamo = true },
                            new UpgradeTierSpec(2, 340, "Recon Sensors", "Increases pierce.") { PierceMod = 1 }
                        }
                    }
                });

                var heroPaths = CreateUpgradePaths("heroSauda", new[]
                {
                    new UpgradePathSpec
                    {
                        PathId = "hero_core",
                        DisplayName = "Sword Mastery",
                        Description = "Empower hero strikes.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 250, "Blade Dance", "Increase damage significantly.") { DamageMod = 1.5f },
                            new UpgradeTierSpec(2, 500, "Twin Blades", "Increase attack rate.") { AttackRateMod = -0.25f },
                            new UpgradeTierSpec(3, 900, "Arc Slash", "Improve range and pierce.") { RangeMod = 0.8f, PierceMod = 1 }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "hero_support",
                        DisplayName = "War Cry",
                        Description = "Buff nearby allies.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 220, "Battle Shout", "Improves damage aura.") { DamageMod = 0.5f },
                            new UpgradeTierSpec(2, 320, "Rallying Call", "Adds range buff.") { RangeMod = 0.5f }
                        }
                    },
                    new UpgradePathSpec
                    {
                        PathId = "hero_ability",
                        DisplayName = "Ultimate", 
                        Description = "Enhances orbital strike ability.",
                        Tiers = new []
                        {
                            new UpgradeTierSpec(1, 280, "Orbital Calibration", "Reduce ability cooldown.") { AbilityId = "ability_orbital_strike" }
                        }
                    }
                });

                var dartCard = CreateUnitCard("card_dart_monkey", "Dart Monkey", "Reliable dart thrower with flexible upgrades.", 3, CardRarity.Common, dartMonkeyPrefab, dartPaths);
                var engineerCard = CreateUnitCard("card_engineer", "Engineer Monkey", "Builds gadgets and reveals tricky bloons.", 4, CardRarity.Rare, engineerMonkeyPrefab, engineerPaths);
                var aceCard = CreateUnitCard("card_monkey_ace", "Monkey Ace", "Sky coverage with wide range and splash potential.", 5, CardRarity.Epic, monkeyAcePrefab, acePaths);
                var heroCard = CreateUnitCard("card_hero_sauda", "Hero: Sauda", "Frontline hero with orbital strike ability.", 6, CardRarity.Legendary, heroPrefab, heroPaths);

                var revealCamoEffect = CreateOrUpdateAsset<RevealCamoSpell>(SpellsFolder + "/RevealCamoSpell.asset", so =>
                {
                    so.FindProperty("radius").floatValue = 5f;
                    so.FindProperty("revealDuration").floatValue = 6f;
                });

                var bombEffect = CreateOrUpdateAsset<AreaDamageSpell>(SpellsFolder + "/ClusterStrikeSpell.asset", so =>
                {
                    so.FindProperty("radius").floatValue = 2.5f;
                    so.FindProperty("damage").floatValue = 8f;
                    so.FindProperty("canBreakLead").boolValue = true;
                });

                var supportZoneEffect = CreateOrUpdateAsset<SupportZoneEffect>(SupportFolder + "/JungleDrumsEffect.asset", so =>
                {
                    so.FindProperty("radius").floatValue = 4f;
                    so.FindProperty("damageBuff").floatValue = 1f;
                    so.FindProperty("attackRateBuff").floatValue = -0.15f;
                    so.FindProperty("duration").floatValue = 12f;
                });

                var revealCard = CreateSpellCard("card_reveal_camo", "Detection Pulse", "Temporarily reveals camo balloons in an area.", 2, CardRarity.Rare, revealCamoEffect);
                var bombCard = CreateSpellCard("card_cluster_bomb", "Cluster Strike", "Call in an explosive strike dealing splash damage.", 5, CardRarity.Epic, bombEffect);
                var supportCard = CreateSupportCard("card_jungle_drums", "Jungle Drums", "Buff nearby units with faster attacks and damage.", 3, CardRarity.Rare, supportZoneEffect);

                var orbitalAbility = CreateOrUpdateAsset<OrbitalStrikeAbility>(AbilitiesFolder + "/OrbitalStrikeAbility.asset", so =>
                {
                    so.FindProperty("abilityId").stringValue = "ability_orbital_strike";
                    so.FindProperty("displayName").stringValue = "Orbital Strike";
                    so.FindProperty("description").stringValue = "Call down a powerful orbital blast that heavily damages balloons.";
                    so.FindProperty("cooldownSeconds").floatValue = 25f;
                    so.FindProperty("requiresTargetPosition").boolValue = true;
                    so.FindProperty("requiresTargetBalloon").boolValue = false;
                    so.FindProperty("radius").floatValue = 3.5f;
                    so.FindProperty("damage").floatValue = 25f;
                    so.FindProperty("canBreakLead").boolValue = true;
                });

                var abilityCatalog = CreateOrUpdateAsset<AbilityCatalog>(AbilitiesFolder + "/AbilityCatalog.asset", so =>
                {
                    var abilitiesProp = so.FindProperty("abilities");
                    abilitiesProp.arraySize = 1;
                    abilitiesProp.GetArrayElementAtIndex(0).objectReferenceValue = orbitalAbility;
                });

                var basicBalloon = CreateOrUpdateAsset<BalloonType>(BalloonsFolder + "/BasicBalloon.asset", so =>
                {
                    so.FindProperty("id").stringValue = "balloon_basic";
                    so.FindProperty("displayName").stringValue = "Red Balloon";
                    so.FindProperty("color").colorValue = Color.red;
                    so.FindProperty("maxHealth").floatValue = 5f;
                    so.FindProperty("movementSpeed").floatValue = 1.5f;
                    so.FindProperty("rewardCurrency").intValue = 5;
                    so.FindProperty("traits").intValue = (int)BalloonTraits.None;
                    so.FindProperty("balloonPrefab").objectReferenceValue = basicBalloonPrefab;
                });

                var camoBalloon = CreateOrUpdateAsset<BalloonType>(BalloonsFolder + "/CamoBalloon.asset", so =>
                {
                    so.FindProperty("id").stringValue = "balloon_camo";
                    so.FindProperty("displayName").stringValue = "Green Camo";
                    so.FindProperty("color").colorValue = new Color(0.2f, 0.6f, 0.2f);
                    so.FindProperty("maxHealth").floatValue = 8f;
                    so.FindProperty("movementSpeed").floatValue = 2.0f;
                    so.FindProperty("rewardCurrency").intValue = 8;
                    so.FindProperty("traits").intValue = (int)BalloonTraits.Camo;
                    so.FindProperty("balloonPrefab").objectReferenceValue = camoBalloonPrefab;
                });

                var leadBalloon = CreateOrUpdateAsset<BalloonType>(BalloonsFolder + "/LeadBalloon.asset", so =>
                {
                    so.FindProperty("id").stringValue = "balloon_lead";
                    so.FindProperty("displayName").stringValue = "Lead Balloon";
                    so.FindProperty("color").colorValue = Color.gray;
                    so.FindProperty("maxHealth").floatValue = 20f;
                    so.FindProperty("movementSpeed").floatValue = 1.2f;
                    so.FindProperty("rewardCurrency").intValue = 15;
                    so.FindProperty("traits").intValue = (int)(BalloonTraits.Lead | BalloonTraits.Fortified);
                    so.FindProperty("balloonPrefab").objectReferenceValue = leadBalloonPrefab;
                });

                var bossBalloon = CreateOrUpdateAsset<BalloonType>(BalloonsFolder + "/BossBalloon.asset", so =>
                {
                    so.FindProperty("id").stringValue = "balloon_boss";
                    so.FindProperty("displayName").stringValue = "MOAB-Class";
                    so.FindProperty("color").colorValue = new Color(0.5f, 0.1f, 0.1f);
                    so.FindProperty("maxHealth").floatValue = 250f;
                    so.FindProperty("movementSpeed").floatValue = 0.9f;
                    so.FindProperty("rewardCurrency").intValue = 150;
                    so.FindProperty("traits").intValue = (int)(BalloonTraits.Boss | BalloonTraits.Fortified);
                    so.FindProperty("balloonPrefab").objectReferenceValue = bossBalloonPrefab;
                });

                var starterDeck = CreateOrUpdateAsset<DeckDefinition>(DecksFolder + "/StarterDeck.asset", so =>
                {
                    so.FindProperty("deckId").stringValue = "deck_starter";
                    so.FindProperty("displayName").stringValue = "Starter Battle Deck";
                    var cardsProp = so.FindProperty("cards");
                    cardsProp.arraySize = 8;
                    cardsProp.GetArrayElementAtIndex(0).objectReferenceValue = dartCard;
                    cardsProp.GetArrayElementAtIndex(1).objectReferenceValue = engineerCard;
                    cardsProp.GetArrayElementAtIndex(2).objectReferenceValue = aceCard;
                    cardsProp.GetArrayElementAtIndex(3).objectReferenceValue = heroCard;
                    cardsProp.GetArrayElementAtIndex(4).objectReferenceValue = revealCard;
                    cardsProp.GetArrayElementAtIndex(5).objectReferenceValue = bombCard;
                    cardsProp.GetArrayElementAtIndex(6).objectReferenceValue = supportCard;
                    cardsProp.GetArrayElementAtIndex(7).objectReferenceValue = dartCard; // duplicate for cycling
                });

                var waveOne = CreateOrUpdateAsset<WaveDefinition>(WavesFolder + "/Wave1.asset", so =>
                {
                    so.FindProperty("id").stringValue = "wave_01";
                    so.FindProperty("startTime").floatValue = 5f;
                    ConfigureWaveEntries(so.FindProperty("entries"), new []
                    {
                        new WaveEntrySpec { Balloon = basicBalloon, Quantity = 12, SpawnInterval = 0.6f, LaneIndex = 0 },
                        new WaveEntrySpec { Balloon = basicBalloon, Quantity = 12, SpawnInterval = 0.6f, LaneIndex = 1 }
                    });
                });

                var waveTwo = CreateOrUpdateAsset<WaveDefinition>(WavesFolder + "/Wave2.asset", so =>
                {
                    so.FindProperty("id").stringValue = "wave_02";
                    so.FindProperty("startTime").floatValue = 25f;
                    ConfigureWaveEntries(so.FindProperty("entries"), new []
                    {
                        new WaveEntrySpec { Balloon = camoBalloon, Quantity = 15, SpawnInterval = 0.5f, LaneIndex = 0 },
                        new WaveEntrySpec { Balloon = leadBalloon, Quantity = 6, SpawnInterval = 1.0f, LaneIndex = 1 }
                    });
                });

                var waveThree = CreateOrUpdateAsset<WaveDefinition>(WavesFolder + "/Wave3.asset", so =>
                {
                    so.FindProperty("id").stringValue = "wave_03";
                    so.FindProperty("startTime").floatValue = 50f;
                    ConfigureWaveEntries(so.FindProperty("entries"), new []
                    {
                        new WaveEntrySpec { Balloon = bossBalloon, Quantity = 1, SpawnInterval = 0.1f, LaneIndex = 0 }
                    });
                });

                var matchSettings = CreateOrUpdateAsset<MatchSettings>(SettingsFolder + "/DefaultMatchSettings.asset", so =>
                {
                    so.FindProperty("startingElixir").intValue = 5;
                    so.FindProperty("maxElixir").intValue = 10;
                    so.FindProperty("elixirRegenRate").floatValue = 1.8f;
                    so.FindProperty("startingCurrency").intValue = 300;
                    so.FindProperty("currencyCap").intValue = 9999;
                    so.FindProperty("endpointHealth").intValue = 20;
                    so.FindProperty("timeBeforeFirstWave").floatValue = 5f;
                    so.FindProperty("timeBetweenWaves").floatValue = 20f;
                });

                Debug.Log("Example content generated under Assets/ExampleContent. Assign assets in scene components to test.");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        #region Helper Structures

        private struct UnitStatProfile
        {
            public float Damage;
            public float AttackRate;
            public float Range;
            public int Pierce;
            public bool CanDetectCamo;
            public TargetingPriority Targeting;
            public string[] StartingAbilities;
        }

        private struct UpgradeTierSpec
        {
            public int Tier;
            public int Cost;
            public string Name;
            public string Description;
            public float DamageMod;
            public float AttackRateMod;
            public float RangeMod;
            public int PierceMod;
            public bool EnableCamo;
            public string AbilityId;

            public UpgradeTierSpec(int tier, int cost, string name, string description) : this()
            {
                Tier = tier;
                Cost = cost;
                Name = name;
                Description = description;
            }
        }

        private struct UpgradePathSpec
        {
            public string PathId;
            public string DisplayName;
            public string Description;
            public UpgradeTierSpec[] Tiers;
        }

        private struct WaveEntrySpec
        {
            public BalloonType Balloon;
            public int Quantity;
            public float SpawnInterval;
            public int LaneIndex;
        }

        #endregion

        #region Creation Helpers

        private static void ApplyCartoonyMaterial(GameObject target, Color color)
        {
            var renderer = target.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                color = color
            };
            material.SetFloat("_Smoothness", 0.1f);
            material.SetFloat("_SpecColor", 0f);
            renderer.sharedMaterial = material;
        }

        private static GameObject CreateBasicProjectilePrefab()
        {
            var path = PrefabFolder + "/BasicProjectile.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            var temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.name = "BasicProjectile";
            UnityEngine.Object.DestroyImmediate(temp.GetComponent<Collider>());
            ApplyCartoonyMaterial(temp, CartoonyPalette.AccentYellow);
            var projectile = temp.AddComponent<Combat.ProjectileBase>();
            var so = new SerializedObject(projectile);
            so.FindProperty("speed").floatValue = 14f;
            so.FindProperty("lifetime").floatValue = 4f;
            so.FindProperty("splashRadius").floatValue = 0f;
            so.ApplyModifiedProperties();

            var prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            UnityEngine.Object.DestroyImmediate(temp);
            return prefab;
        }

        private static GameObject CreateProjectileUnitPrefab(string name, GameObject projectilePrefab, UnitStatProfile stats, Color? bodyColor = null)
        {
            var path = PrefabFolder + $"/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            var root = new GameObject(name);
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            if (bodyColor.HasValue)
            {
                ApplyCartoonyMaterial(visual, bodyColor.Value);
            }
            UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());

            var firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(root.transform, false);
            firePoint.transform.localPosition = new Vector3(0f, 1.2f, 0.5f);

            var unit = root.AddComponent<ProjectileUnit>();
            var so = new SerializedObject(unit);
            so.FindProperty("damage").floatValue = stats.Damage;
            so.FindProperty("attackRate").floatValue = stats.AttackRate;
            so.FindProperty("range").floatValue = stats.Range;
            so.FindProperty("pierce").intValue = stats.Pierce;
            so.FindProperty("canDetectCamo").boolValue = stats.CanDetectCamo;
            so.FindProperty("targetingPriority").enumValueIndex = (int)stats.Targeting;
            so.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab.GetComponent<Combat.ProjectileBase>();
            so.FindProperty("firePoint").objectReferenceValue = firePoint.transform;
            so.FindProperty("canBreakLead").boolValue = stats.CanDetectCamo;
            so.FindProperty("splashDamage").boolValue = false;
            so.FindProperty("splashRadius").floatValue = 0f;

            var abilitiesProp = so.FindProperty("startingAbilities");
            if (stats.StartingAbilities != null && stats.StartingAbilities.Length > 0)
            {
                abilitiesProp.arraySize = stats.StartingAbilities.Length;
                for (int i = 0; i < stats.StartingAbilities.Length; i++)
                {
                    abilitiesProp.GetArrayElementAtIndex(i).stringValue = stats.StartingAbilities[i];
                }
            }
            else
            {
                abilitiesProp.arraySize = 0;
            }

            so.ApplyModifiedProperties();

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject CreateBalloonPrefab(string name, Color color)
        {
            var path = PrefabFolder + $"/{name}.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.localScale = Vector3.one * 0.6f;
            ApplyCartoonyMaterial(go, color);

            if (!go.TryGetComponent<BalloonInstance>(out _))
            {
                go.AddComponent<BalloonInstance>();
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        private static List<UpgradePathDefinition> CreateUpgradePaths(string unitPrefix, UpgradePathSpec[] specs)
        {
            var result = new List<UpgradePathDefinition>(specs.Length);
            for (int i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var tiers = new List<UpgradeTierDefinition>();
                foreach (var tierSpec in spec.Tiers)
                {
                    var tierAsset = CreateOrUpdateAsset<UpgradeTierDefinition>($"{UpgradesFolder}/{unitPrefix}_{spec.PathId}_T{tierSpec.Tier}.asset", so =>
                    {
                        so.FindProperty("tier").intValue = tierSpec.Tier;
                        so.FindProperty("cost").intValue = tierSpec.Cost;
                        so.FindProperty("upgradeName").stringValue = tierSpec.Name;
                        so.FindProperty("description").stringValue = tierSpec.Description;
                        so.FindProperty("damageModifier").floatValue = tierSpec.DamageMod;
                        so.FindProperty("attackRateModifier").floatValue = tierSpec.AttackRateMod;
                        so.FindProperty("rangeModifier").floatValue = tierSpec.RangeMod;
                        so.FindProperty("pierceModifier").intValue = tierSpec.PierceMod;
                        so.FindProperty("enableCamoDetection").boolValue = tierSpec.EnableCamo;
                        var abilityProp = so.FindProperty("grantAbility");
                        var abilityIdProp = so.FindProperty("abilityId");
                        var hasAbility = !string.IsNullOrEmpty(tierSpec.AbilityId);
                        abilityProp.boolValue = hasAbility;
                        abilityIdProp.stringValue = hasAbility ? tierSpec.AbilityId : string.Empty;
                    });
                    tiers.Add(tierAsset);
                }

                var pathAsset = CreateOrUpdateAsset<UpgradePathDefinition>($"{UpgradesFolder}/{unitPrefix}_{spec.PathId}.asset", so =>
                {
                    so.FindProperty("id").stringValue = spec.PathId;
                    so.FindProperty("displayName").stringValue = spec.DisplayName;
                    so.FindProperty("description").stringValue = spec.Description;
                    var tiersProp = so.FindProperty("tiers");
                    tiersProp.arraySize = tiers.Count;
                    for (int t = 0; t < tiers.Count; t++)
                    {
                        tiersProp.GetArrayElementAtIndex(t).objectReferenceValue = tiers[t];
                    }
                });

                result.Add(pathAsset);
            }

            return result;
        }

        private static UnitCard CreateUnitCard(string id, string name, string description, int elixir, CardRarity rarity, GameObject unitPrefab, List<UpgradePathDefinition> upgradePaths)
        {
            var card = CreateOrUpdateAsset<UnitCard>($"{CardsFolder}/{name.Replace(" ", string.Empty)}Card.asset", so =>
            {
                so.FindProperty("id").stringValue = id;
                so.FindProperty("displayName").stringValue = name;
                so.FindProperty("description").stringValue = description;
                so.FindProperty("cardType").enumValueIndex = (int)CardType.Unit;
                so.FindProperty("rarity").enumValueIndex = (int)rarity;
                so.FindProperty("elixirCost").intValue = elixir;
                so.FindProperty("unitPrefab").objectReferenceValue = unitPrefab.GetComponent<UnitBase>();
                so.FindProperty("maxParagonCount").intValue = 1;
                var pathsProp = so.FindProperty("upgradePaths");
                pathsProp.arraySize = upgradePaths.Count;
                for (int i = 0; i < upgradePaths.Count; i++)
                {
                    pathsProp.GetArrayElementAtIndex(i).objectReferenceValue = upgradePaths[i];
                }
            });
            return card;
        }

        private static SpellCard CreateSpellCard(string id, string name, string description, int elixir, CardRarity rarity, SpellEffectDefinition effect)
        {
            return CreateOrUpdateAsset<SpellCard>($"{CardsFolder}/{name.Replace(" ", string.Empty)}Card.asset", so =>
            {
                so.FindProperty("id").stringValue = id;
                so.FindProperty("displayName").stringValue = name;
                so.FindProperty("description").stringValue = description;
                so.FindProperty("cardType").enumValueIndex = (int)CardType.Spell;
                so.FindProperty("rarity").enumValueIndex = (int)rarity;
                so.FindProperty("elixirCost").intValue = elixir;
                so.FindProperty("spellEffect").objectReferenceValue = effect;
            });
        }

        private static SupportCard CreateSupportCard(string id, string name, string description, int elixir, CardRarity rarity, SupportEffectDefinition effect)
        {
            return CreateOrUpdateAsset<SupportCard>($"{CardsFolder}/{name.Replace(" ", string.Empty)}Card.asset", so =>
            {
                so.FindProperty("id").stringValue = id;
                so.FindProperty("displayName").stringValue = name;
                so.FindProperty("description").stringValue = description;
                so.FindProperty("cardType").enumValueIndex = (int)CardType.Support;
                so.FindProperty("rarity").enumValueIndex = (int)rarity;
                so.FindProperty("elixirCost").intValue = elixir;
                so.FindProperty("supportEffect").objectReferenceValue = effect;
            });
        }

        private static void ConfigureWaveEntries(SerializedProperty entriesProp, WaveEntrySpec[] specs)
        {
            entriesProp.arraySize = specs.Length;
            for (int i = 0; i < specs.Length; i++)
            {
                var entry = entriesProp.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("balloonType").objectReferenceValue = specs[i].Balloon;
                entry.FindPropertyRelative("quantity").intValue = specs[i].Quantity;
                entry.FindPropertyRelative("spawnInterval").floatValue = specs[i].SpawnInterval;
                entry.FindPropertyRelative("laneIndex").intValue = specs[i].LaneIndex;
            }
        }

        private static T CreateOrUpdateAsset<T>(string path, Action<SerializedObject> configure) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }

            var so = new SerializedObject(asset);
            configure?.Invoke(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            if (!AssetDatabase.IsValidFolder(System.IO.Path.Combine(parent, folderName)))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        #endregion
    }
}
#endif
