﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttributeComponent : MonoBehaviour {


    public int team;
    public int hp; //Lebenspunkte
    public int ap; //Ausgegebene AP
    public int maxMovRange; //Maximale Bewegungsreichweite
    public int actMovRange; //Aktuelle Bewegungsreichweite
    public int regPerAP; //Regeneration pro Aufruf von Move
    public float minAccuracy; //Mindest Trefferwahrscheinlichkeit
    public int attackRange = 100; //Dient der Wurfrange von Granaten etc.
    public bool canShoot; // Spieler kann nur 1 mal pro Runde schießen
    public bool highCover; // Spieler ist hinter hoher Deckung
    public bool lowCover; // Spieler ist niedriger hoher Deckung
    public bool armored; // Spieler hat Rüstung
    public GameObject armor;
    public GameObject weapon;
    public InventoryComponent items; //Inventory
    public static int maxMoveAP; //Maximale AP die für Movement ausgegeben werden können
    public static int maxShootAP; //Maximale AP die Schießen ausgegeben werden können
    public Enums.Prof prof; //Die Klasse der Einheit
    Cell cell; //Zelle auf der sich die Einheit befindet

    public List<Enums.Actions> skills;

    public Enums.Prof profession = 0;

    public Animator anim;
    public GameObject model;
    ArmoryComponent armory;
    private int animId_iStance;

	// Use this for initialization
	void Start ()
    {
      //  skills = new List<Enums.Actions>();
        canShoot = true;
        skills.Add(Enums.Actions.Move);
        
	}

    void Awake()
    {
        animId_iStance = Animator.StringToHash("Stance");
    }
	
	// Update is called once per frame
	void Update () {
        
        if (items.isPrimary && items.primary != null)
        {
            weapon = items.primary.gameObject;
        }
        else if(items.secondary != null)
        {
            weapon = items.secondary.gameObject;
        }
        
    }

    public void setCurrentCell(Cell cell)
    {
        this.cell = cell;
        if(this.cell.setOnFire == true)
        {
            hp -= 1;
        }
    }

    public Cell getCurrentCell()
    {
        return cell;
    }

    public void setCurrentCell(int x, int z)
    {
        
        this.cell = GameObject.Find(x.ToString() + "|" + z.ToString()).GetComponent<Cell>();
    }


    //Legt die Klasse fuer die Einheit fest
    public void setProf(int i)
    {
        prof = (Enums.Prof)i;
        armory = GameObject.Find("Armory").GetComponent<ArmoryComponent>();
        ManagerSystem managersys = (ManagerSystem) FindObjectOfType(typeof(ManagerSystem));
        items = this.GetComponent<InventoryComponent>();

        GameObject prefab = managersys.policePrefab;
        model = Instantiate(managersys.policePrefab);
        model.transform.SetParent(this.transform);
        model.transform.localPosition = prefab.transform.position;
        model.transform.localRotation = prefab.transform.rotation;
        model.transform.localScale = prefab.transform.localScale;

        anim = (Animator)model.GetComponent(typeof(Animator));


        Enums.Stance stance = Enums.Stance.Range1H;

        profession = (Enums.Prof)i;
        WeaponHolding weapons = (WeaponHolding)model.GetComponent(typeof(WeaponHolding));
        weapons.owner = this.gameObject;

        if (profession == Enums.Prof.Riot)
        {

            stance = Enums.Stance.MeleeRiot;
            weapons.initializeEquip(armory.p_Stick, armory.p_Riotshield, null, null , stance, Enums.Stance.None);
            GameObject tmp = weapons.leftHandObjectPrimary;
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.Pipe;
            items.amountTeargas = 4;
            hp += 20;
            skills.Add(Enums.Actions.Hit);
            skills.Add(Enums.Actions.Teargas);
            items.utility1 = Enums.Equipment.Teargas;
            
            

        }
        else if (profession == Enums.Prof.Soldier)
        {
            stance = Enums.Stance.Range2H;
            weapons.initializeEquip(armory.p_AssaultRifle, null, null, null, stance, Enums.Stance.None);
            GameObject tmp = weapons.leftHandObjectPrimary;
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.AssaultRifle;
            items.amountGrenades = 2;
            hp += 20;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            skills.Add(Enums.Actions.Grenade);

            items.utility1 = Enums.Equipment.Grenade;
            
        }
       
        else if (profession == Enums.Prof.Support)
        {
            stance = Enums.Stance.Range2H;
            weapons.initializeEquip(armory.p_AssaultRifle, null, armory.p_Pistol, null, stance, Enums.Stance.Range1H);
            GameObject tmp = weapons.leftHandObjectPrimary;

            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.AssaultRifle;

            tmp = weapons.leftHandObjectSecondary;
            items.secondary = tmp.GetComponent<WeaponComponent>();
            items.secondaryWeaponType = Enums.SecondaryWeapons.Pistol;
            items.amountMediKits = 2;
            hp += 20;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            skills.Add(Enums.Actions.ChangeWeapon);
            skills.Add(Enums.Actions.Heal);


            items.utility1 = Enums.Equipment.MediPack;
        }
            /*
        else if (profession == Enums.Prof.HeavyGunner)
        {
            GameObject tmp = Instantiate(armory.MG);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.MG;
            hp += 20;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            
            stance = Enums.Stance.Range2H;
        }
        if (profession == Enums.Prof.Sniper)
        {
            GameObject tmp = Instantiate(armory.Sniper);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.Sniper;

            tmp = Instantiate(armory.Pistol);
            tmp.transform.SetParent(transform);
            items.secondary = tmp.GetComponent<WeaponComponent>();
            items.secondaryWeaponType = Enums.SecondaryWeapons.Pistol;
            items.amountGrenades = 1;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            skills.Add(Enums.Actions.ChangeWeapon);

            stance = Enums.Stance.Range2H;
        }
             * */
        anim.SetInteger(animId_iStance, (int)stance);
    }

    //Rebels
    public void setEquip(Vector4 v)
    {
        armory = GameObject.Find("Armory").GetComponent<ArmoryComponent>();

        items = this.GetComponent<InventoryComponent>();

        ManagerSystem managersys = (ManagerSystem) FindObjectOfType(typeof(ManagerSystem));

        GameObject prefab = managersys.rebelPrefab;
        model = Instantiate(prefab);

        model.transform.SetParent(this.transform);
        model.transform.localPosition = prefab.transform.position;
        model.transform.localRotation = prefab.transform.rotation;
        model.transform.localScale = prefab.transform.localScale;
        anim = (Animator)model.GetComponent(typeof(Animator));

        Enums.Stance stance = Enums.Stance.Range1H;

        WeaponHolding weapons = (WeaponHolding)model.GetComponent(typeof(WeaponHolding));
        weapons.owner = this.gameObject;
        GameObject tmp;

        //primärwaffe
        items.primaryWeaponType = (Enums.PrimaryWeapons)v.x;
        if (items.primaryWeaponType == Enums.PrimaryWeapons.Pipe)
        {
            tmp = weapons.setLeftHandItem(armory.r_Pipe, true);
            stance = Enums.Stance.Melee1H;
            weapons.primaryStance = stance;

           items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.Pipe;
           skills.Add(Enums.Actions.Hit);
            
        }
        /*
        if (items.primaryWeaponType == Enums.PrimaryWeapons.ShieldnStick)
        {
            tmp = Instantiate(armory.ShieldnStick);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.ShieldnStick;
            hp += 20;
            skills.Add(Enums.Actions.Hit);
            stance = Enums.Stance.MeleeRiot;


        }
        else if (items.primaryWeaponType == Enums.PrimaryWeapons.Shotgun)
        {
            tmp = Instantiate(armory.Shotgun);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.Shotgun;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            stance = Enums.Stance.Range2H;

        }
        else if (items.primaryWeaponType == Enums.PrimaryWeapons.HuntingRifle)
        {
            tmp = Instantiate(armory.HuntingRifle);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.HuntingRifle;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            stance = Enums.Stance.Range2H;
        }
         */
        else if (items.primaryWeaponType == Enums.PrimaryWeapons.AssaultRifle)
        {
            tmp = weapons.setLeftHandItem(armory.r_AssaultRifle, true);
            stance = Enums.Stance.Range2H;
            weapons.primaryStance = stance;

            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.AssaultRifle;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            
        }
            /*
        else if (items.primaryWeaponType == Enums.PrimaryWeapons.MG)
        {
            tmp = Instantiate(armory.MG);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.MG;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            stance = Enums.Stance.Range2H;
        }
        else if (items.primaryWeaponType == Enums.PrimaryWeapons.Sniper)
        {
            tmp = Instantiate(armory.Sniper);
            tmp.transform.SetParent(transform);
            items.primary = tmp.GetComponent<WeaponComponent>();
            items.primaryWeaponType = Enums.PrimaryWeapons.Sniper;
            skills.Add(Enums.Actions.Shoot);
            skills.Add(Enums.Actions.Reload);
            stance = Enums.Stance.Range2H;
        }*/
        //if (items.primary != null)
            //items.primary.gameObject.transform.SetParent(this.transform);

        //sekundärwaffe
        items.secondaryWeaponType = (Enums.SecondaryWeapons)v.y;
        if (items.secondaryWeaponType == Enums.SecondaryWeapons.Pistol)
        {
            tmp = weapons.setLeftHandItem(armory.r_Pistol, false);
            items.secondary = tmp.GetComponent<WeaponComponent>();
            items.secondaryWeaponType = Enums.SecondaryWeapons.Pistol;

            if (items.primaryWeaponType != Enums.PrimaryWeapons.None)
            {
                if (items.primaryWeaponType == Enums.PrimaryWeapons.Pipe) //|| items.primaryWeaponType == Enums.PrimaryWeapons.ShieldnStick
                {
                    skills.Add(Enums.Actions.Shoot);
                    skills.Add(Enums.Actions.Reload);
                }
                skills.Add(Enums.Actions.ChangeWeapon);
            }
                // keine primary weapon
            else
            {
                items.isPrimary = false;
                skills.Add(Enums.Actions.Shoot);
                skills.Add(Enums.Actions.Reload);
                //Ist auch default schon auf Range1H
                stance = Enums.Stance.Range1H;
            }
        }


        anim.SetInteger(animId_iStance, (int)stance);


        /*    
        else if (items.secondaryWeaponType == Enums.SecondaryWeapons.Mortar)
        {

            items.secondary = Instantiate(armory.Mortar).GetComponent<WeaponComponent>();
        } 
        else if (items.secondaryWeaponType == Enums.SecondaryWeapons.RPG)
        {

            items.secondary = Instantiate(armory.RPG).GetComponent<WeaponComponent>();
        }


        */
        //utility1
        items.utility1 = (Enums.Equipment)v.z;
        if (items.utility1 == Enums.Equipment.Kevlar)
        {
            hp += 10;
        }
        else if (items.utility1 == Enums.Equipment.Helmet)
        {
            hp += 10;
        }
        /*
         else if (items.utility1 == Enums.Equipment.SuicideBelt)
        {

        }
        */
   
        else if (items.utility1 == Enums.Equipment.MediPack)
        {
            items.amountMediKits = 2; 
            skills.Add(Enums.Actions.Heal);
        }
        
        /*
         * else if (items.utility1 == Enums.Equipment.Mine)
        {
            items.amountMines = 2;
        }
        else if (items.utility1 == Enums.Equipment.Rocks)
        {

        }
         */
        else if (items.utility1 == Enums.Equipment.Mollotov)
        {
            items.amountMolotovs = 2;
            skills.Add(Enums.Actions.Molotov);
        }
        else if (items.utility1 == Enums.Equipment.Grenade)
        {
            items.amountGrenades = 1;
            skills.Add(Enums.Actions.Grenade);
        }
        else if (items.utility1 == Enums.Equipment.SmokeGreneade)
        {
            items.amountSmokes = 2;
            skills.Add(Enums.Actions.Smoke);
        }
        else if (items.utility1 == Enums.Equipment.Teargas)
        {
            items.amountTeargas = 2;
            skills.Add(Enums.Actions.Teargas);
        }




        //utility 2
        items.utility2 = (Enums.Equipment)v.w;
        if (items.utility2 == Enums.Equipment.Kevlar)
        {
            hp += 10;
        }
        else if (items.utility2 == Enums.Equipment.Helmet)
        {
            hp += 10;
        }

        else if (items.utility2 == Enums.Equipment.MediPack)
        {
            items.amountMediKits = 2;
            skills.Add(Enums.Actions.Heal);
        }
            /*
        else if (items.utility2 == Enums.Equipment.SuicideBelt)
        {

        }
             
        else if (items.utility2 == Enums.Equipment.Scope)
        {

        }

        else if (items.utility2 == Enums.Equipment.Mine)
        {
            items.amountMines = 2;
        }
        else if (items.utility2 == Enums.Equipment.Rocks)
        {

        }
             * */
        else if (items.utility2 == Enums.Equipment.Mollotov)
        {
            items.amountMolotovs = 2;
            skills.Add(Enums.Actions.Molotov);
        }
        else if (items.utility2 == Enums.Equipment.Grenade)
        {
            items.amountGrenades = 1;
            skills.Add(Enums.Actions.Grenade);
        }
        else if (items.utility2 == Enums.Equipment.SmokeGreneade)
        {
            items.amountSmokes = 2;
            skills.Add(Enums.Actions.Smoke);
        }
            
        else if (items.utility2 == Enums.Equipment.Teargas)
        {
            items.amountTeargas = 2;
            skills.Add(Enums.Actions.Teargas);
        }
        
    }

    public void regenerateMovepoints()
    {
        actMovRange = Mathf.Clamp(actMovRange + regPerAP, 0, maxMovRange);
    }

}
