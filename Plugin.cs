using System;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Utilla;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using BepInEx.Configuration;
using BananaSpawner;
using BuildSafe;

namespace BananaSpawner
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private PhysicMaterial bouncyMat = new PhysicMaterial();

        public static List<GameObject> Clones = new List<GameObject>();
        public static GameObject BananaSpawner;
        public static GameObject BananaBoard;

        public static bool inModded = false;
        public static bool AntiRepeat = false;
        public static bool AAntiRepeat = false;
        public static bool ModOn = true;
        public static bool Spammy = true;
        public static bool AntiBananaRepeat = false;
        public static Plugin instance;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called */
            /* Put code in OnGameInitialized to avoid null references */
            //bouncyMat.bounciness = 0.7f;
            LoadAssets();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }


        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled */
            BananaSpawner.SetActive(true);
            BananaBoard.SetActive(true);
            inModded = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled */
            BananaSpawner.transform.position = new Vector3(0, 0, 0);
            BananaSpawner.SetActive(false);
            inModded = false;
            ClearClones();
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled */
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup) */
            HarmonyPatches.RemoveHarmonyPatches();
            ClearClones();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            instance = this;
            //spawner
            BananaSpawner = Instantiate(BananaSpawner);
            BananaSpawner.transform.position = new Vector3(0, 0, 0);
            BananaSpawner.SetActive(true);

            //boards
            BananaBoard = Instantiate(BananaBoard);
            BananaBoard.transform.position = new Vector3(-65.7817f,11.046f,-84.1963f);
            BananaBoard.transform.localScale = new Vector3(2f,2f,2f);
            BananaBoard.transform.rotation = Quaternion.Euler(358.5999f,138.7856f,0.3f);
            BananaBoard.SetActive(true);
            LoadButtons();
        }

        public static void LoadAssets()
        {
            AssetBundle bundle = LoadAssetBundle("BananaSpawner.bannna");
            BananaSpawner = bundle.LoadAsset<GameObject>("Bannnar");
            BananaBoard = bundle.LoadAsset<GameObject>("BananaBoard");
            bundle = (AssetBundle)null;
        }

        public static void LoadButtons()
        {
            GameObject NoCDButton = GameObject.Find("No Spawn");
            GameObject ModOnButton = GameObject.Find("Active Mod");
            GameObject DeleteBananasButton = GameObject.Find("DeleteBanans");
            NoCDButton.AddComponent<NoCD>();
            ModOnButton.AddComponent<ModOnAndOff>();
            DeleteBananasButton.AddComponent<DeleteAllBananas>();
            NoCDButton.layer = 18;
            ModOnButton.layer = 18;
            DeleteBananasButton.layer = 18;
        }

        public static AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        void L(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        void Update()
        {
            if (!PhotonNetwork.InRoom || inModded)
            {
                if (AAntiRepeat == false)
                {
                    BananaBoard.transform.position = new Vector3(-65.7817f, 11.046f, -84.1963f);
                    AntiRepeat = false;
                    AAntiRepeat = true;
                }
                if (ModOn)
                {
                    if (ControllerInputPoller.instance.leftControllerIndexFloat > .5f)
                    {
                        if (!AntiBananaRepeat)
                        {
                            if (Spammy == false)
                            { AntiBananaRepeat = true; }
                            else { AntiBananaRepeat = false; }
                            GameObject bananaClone = Instantiate(BananaSpawner);
                            bananaClone.layer = 8;
                            bananaClone.transform.localScale = new Vector3(2f, 2f, 2f);
                            bananaClone.AddComponent<Rigidbody>();
                            Clones.Add(bananaClone);
                            bananaClone.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.transform.position - new Vector3(0f, 0.5f, 0f); ;
                            bananaClone.GetComponent<Rigidbody>().AddForce(GorillaLocomotion.Player.Instance.leftControllerTransform.transform.forward * 25f);
                        }
                    }
                    else
                    {
                        AntiBananaRepeat = false;
                    }
                }
            }
            else if (!inModded && PhotonNetwork.InRoom)
            {
                if (AntiRepeat == false)
                {
                    ClearClones();
                    BananaBoard.transform.position = new Vector3(0f, 0f, 0f);               
                    AAntiRepeat = false;
                    AntiRepeat = true;
                }               
            }
        }

        public static void ClearClones()
        {
            foreach (GameObject clone in Clones)
            {
                Destroy(clone);
            }
            Clones.Clear();
        }

        public static void ModOFf()
        {
            GameObject ModOnTxt = GameObject.Find("AMT");
            ModOn = !ModOn;
            if (ModOn)
            {
                ModOnTxt.GetComponent<TextMesh>().text = "MOD\n ON";
            }
            else
            {
                ModOnTxt.GetComponent<TextMesh>().text = "MOD\n OFF";             
            }
        }
        public static void NoCD()
        {
            GameObject NoCDTxt = GameObject.Find("No Cooldown");
            Spammy = !Spammy;
            if (Spammy) 
            {
                 NoCDTxt.GetComponent<TextMesh>().text = "No Spawn\nCooldown\n\n     ON";              
            }
            else
            {
                 NoCDTxt.GetComponent<TextMesh>().text = "No Spawn\nCooldown\n\n     OFF";
            }
        }
    }
}
