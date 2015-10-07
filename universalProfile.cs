﻿//Smarbot Universal Profile
//Contributors : Wbulot
//Decks supported : Paladin Secret - Zoo - Worgen OTK - FaceWarrior - Priest - Warrior Mech - Mage Mech -

using System.Linq;
using System.Collections.Generic;

namespace SmartBot.Plugins.API
{
    public class bProfile : RemoteProfile
    {
        //Init value for Card Draw
        private int FriendCardDrawValue = 2;
        private int EnemyCardDrawValue = 2;

        //Init value for heros
        private int HeroEnemyHealthValue = 2;
        private int HeroFriendHealthValue = 2;

        //Init value for minion
        private int MinionEnemyAttackValue = 2;
        private int MinionEnemyHealthValue = 2;
        private int MinionFriendAttackValue = 2;
        private int MinionFriendHealthValue = 2;

        //GlobalValueModifier
        private int GlobalValueModifier = 0;

        public override float GetBoardValue(Board board)
        {
            float value = 0;

            //Change base value according to the deck played and situation
            if (myDeck.isPaladinSecret(board))
            {
                FriendCardDrawValue = 6;
                EnemyCardDrawValue = 0;
                HeroEnemyHealthValue = 3;
                HeroFriendHealthValue = 1;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 3;
                MinionFriendHealthValue = 2;
                if (board.EnemyClass == Card.CClass.HUNTER)//Against hunter, save more health and little more control
                {
                    HeroFriendHealthValue = 4;
                    MinionEnemyAttackValue = 5;
                }
                else//Against other Hero than hunter
                {
                    if (board.TurnCount <= 4)//Little more control before turn 4
                    {
                        MinionEnemyAttackValue = 4;
                    }
                }
            }
            else if (myDeck.isMechWarrior(board) || myDeck.isMechMage(board))
            {
                FriendCardDrawValue = 5;
                EnemyCardDrawValue = 6;
                HeroEnemyHealthValue = 3;
                HeroFriendHealthValue = 1;
                MinionEnemyAttackValue = 3;
                MinionEnemyHealthValue = 2;
                MinionFriendAttackValue = 3;
                MinionFriendHealthValue = 2;
            }
            

            //Hero friend value
            value += (board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor) * HeroFriendHealthValue;

            //Hero enemy value
            value -= (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) * HeroEnemyHealthValue;

            //enemy board
            foreach (Card c in board.MinionEnemy)
            {
                value -= GetCardValue(board, c);
            }

            //friend board
            foreach (Card c in board.MinionFriend)
            {
                value += GetCardValue(board, c);
            }

            //Lethal and save my ass
            if (board.HeroEnemy.CurrentHealth <= 0)
                value += 100000;

            if (board.HeroFriend.CurrentHealth <= 0 && board.FriendCardDraw == 0)
                value -= 100000;

            value += GlobalValueModifier;

            value += board.FriendCardDraw * FriendCardDrawValue;
            value -= board.EnemyCardDraw * EnemyCardDrawValue;

            //Setup Lethal
            if ((board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) <= GetDmgInHand(board))
            {
                GlobalValueModifier += 50;
            }

            return value;
        }

        //Uncomment this only for Debug wierd play
        //public override void OnBoardReady(Board board)
        //{
        //    if (board.IsOwnTurn)
        //    {
        //        foreach (var item in board.ActionsStack)
        //        {
        //            Debug(item.ToString());
        //        }

        //        Debug("Board : " + board.GetValue());
        //    }
        //}

        public float GetCardValue(Board board, Card card)
        {
            float value = 0;

            if (card.IsFriend)
            {
                value += card.CurrentHealth * MinionFriendHealthValue + card.CurrentAtk * MinionFriendAttackValue;

                if (card.IsFrozen)
                    value -= 2 + card.CurrentAtk;

                if (card.IsDivineShield)
                    value += 2 + card.CurrentAtk;

                if (card.IsStealth)
                    value += 2 + card.CurrentAtk;

                if (card.IsTargetable == false)
                    value += 3;

                //Tweak some minions Friend value
                switch (card.Template.Id)
                {
                    case Card.Cards.FP1_007://Nerubian Egg
                        if (card.IsTaunt)
                        {
                            value += 5;
                        }
                        break;
                }
            }
            else
            {
                //Taunt value
                if (card.IsTaunt)
                    value += 2;

                //Windfury value
                if (card.IsWindfury)
                    value += 4;

                //Divine shield value
                if (card.IsDivineShield)
                    value += 2 + card.CurrentAtk;

                //Targetable Value
                if (card.IsTargetable == false)
                    value += 3;

                //Frozen Value
                if (card.IsFrozen)
                    value += 2 + card.CurrentAtk;

                value += card.CurrentHealth * MinionEnemyHealthValue + card.CurrentAtk * MinionEnemyAttackValue;

                //Tweak some minions enemy value
                switch (card.Template.Id)
                {
                    case Card.Cards.EX1_412://Raging Worgen
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_565://Flametongue Totem
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.BRM_002://Flamewaker
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_402://Armorsmith
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_006://Mechwarper
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.FP1_005://Shade of Naxxramas
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.FP1_013://Kel'Thuzad
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.BRM_031://Chromaggus
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_559://Archmage Antonidas
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_021://Mal'Ganis
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_608://Sorcerer's Apprentice
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_012://Mana Wyrm
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_595://Cult Master
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_170://Emperor Cobra
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.BRM_028://Emperor Thaurissan
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.GVG_100://Floating Watcher
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.tt_004://Flesheating Ghoul
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_604://Frothing Berserker
                        if (card.IsSilenced == false)
                        {
                            value += 20;
                        }
                        break;

                    case Card.Cards.BRM_019://Grim Patron
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_084://Warsong Commander
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_095://Gadgetzan Auctioneer
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_040://Hogger
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_104://Hobgoblin
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_614://Illidan Stormrage
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_027://Iron Sensei
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_094://Jeeves
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_019://Knife Juggler
                        if (card.IsSilenced == false)
                        {
                            value += 9;
                        }
                        break;

                    case Card.Cards.EX1_001://Lightwarden
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_563://Malygos
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_103://Micro Machine
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.EX1_044://Questing Adventurer
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.NEW1_020://Wild Pyromancer
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.GVG_013://Cogmaster
                        if (card.IsSilenced == false)
                        {
                            value += 5;
                        }
                        break;

                    case Card.Cards.CS2_237://Starving Buzzard
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;

                    case Card.Cards.EX1_080://Secretkeeper
                        if (card.IsSilenced == false)
                        {
                            value += 7;
                        }
                        break;

                    case Card.Cards.GVG_002://Snowchugger
                        if (card.IsSilenced == false)
                        {
                            value += 10;
                        }
                        break;
                }
            }

            return value;
        }

        public override void OnCastMinion(Board board, Card minion, Card target)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.AT_079://Mysterious Challenger
                    if (myDeck.isPaladinSecret(board))
                    {
                        GlobalValueModifier += 10;
                    }
                    break;

                case Card.Cards.CS2_188://Abusive Sergeant
                    GlobalValueModifier -= 9;
                    if (board.MinionEnemy.Count == 0) //Avoid play abusive if empty enemy board (use it for trade)
                    {
                        GlobalValueModifier -= 4;
                    }
                    break;

                case Card.Cards.FP1_002://Haunted Creeper
                    if (myDeck.isPaladinSecret(board))
                    {
                        if (board.Hand.Count(x => x.Template.Id == Card.Cards.NEW1_019) >= 1 && board.ManaAvailable <= 3)
                        {
                            GlobalValueModifier += 7;
                        }
                    }
                    break;

                case Card.Cards.CS2_203://Ironbeak Owl
                    GlobalValueModifier -= 15;
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.CS2_203) == 2)
                    {
                        GlobalValueModifier += 10;
                    }

                    if (board.EnemyClass == Card.CClass.HUNTER)//Inscrease value of owl against hunter
                    {
                        if (target != null && target.IsFriend == false)
                        {
                            GlobalValueModifier += 10;
                        }

                        if (target != null && target.Template.Id == Card.Cards.FP1_004) //Inscrease value of owl against mad scientitst
                        {
                            GlobalValueModifier += 3;
                        }
                    }

                    if (target != null && target.Template.Id == Card.Cards.NEW1_021 && target.IsSilenced == false) //Inscrease value of owl against doomsayer
                    {
                        GlobalValueModifier += 110;
                    }
                    break;

                case Card.Cards.AT_076://Murloc Knight
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.NEW1_019://Knife Juggler
                    if (CanSurvive(minion, board) == false)
                    {
                        GlobalValueModifier -= 2;
                    }
                    break;

                case Card.Cards.GVG_013://Cogmaster
                    if (board.Hand.Count(x => x.Template.Id == Card.Cards.GVG_051) >= 1)
                    {
                        GlobalValueModifier += 3;
                    }
                    break;

                case Card.Cards.GVG_102://Tinkertown Technician
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                    {
                        GlobalValueModifier -= 4;
                    }
                    break;

                case Card.Cards.GVG_055://Screwjank Clunker
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.GVG_085://Annoy-o-Tron
                    if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.GVG_013) >= 1)
                    {
                        GlobalValueModifier += 3;
                    }
                    break;

                case Card.Cards.GVG_004://Goblin Blastmage
                    if (board.MinionFriend.Count(x => x.Race == Card.CRace.MECH) == 0)
                    {
                        GlobalValueModifier -= 6;
                    }
                    break;

                case Card.Cards.EX1_116://Leeroy Jenkins
                    GlobalValueModifier -= 30;
                    break;
            }

            foreach (Card card in board.MinionEnemy)
            {
                if (card.Template.Id == Card.Cards.NEW1_021 && card.IsSilenced == false)//Tweak Doomsayer
                {
                    GlobalValueModifier -= 100;
                }
            }
        }

        public override void OnMinionDeath(Board board, Card minion)
        {
            switch (minion.Template.Id)
            {
                case Card.Cards.FP1_004://Mad Scientist
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 20;
                    }
                    break;

                case Card.Cards.FP1_022://Voidcaller
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 5;
                    }
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 5;
                        if (board.Hand.Count(x => x.Race == Card.CRace.DEMON) == 1 && board.Hand.Count(x => x.Template.Id == Card.Cards.EX1_310) == 1) //Some tweak here for higher chance to proc doomguard
                        {
                            GlobalValueModifier += 5;
                        }
                    }
                    break;

                case Card.Cards.EX1_029://Leper Gnome
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 6;
                    }
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.FP1_002://Haunted Creeper Friend
                    if (minion.IsFriend == true && minion.IsSilenced == false)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.GVG_096://Piloted Shredder
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier += 5;
                    }
                    break;

                case Card.Cards.FP1_007://Nerubian Egg
                    if (minion.IsFriend == true && minion.IsSilenced == false && minion.IsTaunt)
                    {
                        GlobalValueModifier -= 9;
                    }
                    break;

                case Card.Cards.EX1_556://Harvest Golem
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier += 5;
                    }
                    break;

                case Card.Cards.AT_123://Harvest Golem
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier += 13;
                    }
                    break;

                case Card.Cards.NEW1_021://Doomsayer
                    if (minion.IsFriend == false && minion.IsSilenced == false)
                    {
                        GlobalValueModifier += 1000;
                    }
                    break;


            }
        }

        public override void OnCastSpell(Board board, Card spell, Card target)
        {
            switch (spell.Template.Id)
            {
                case Card.Cards.EX1_349://Divine Favor
                    if ((board.Hand.Count > board.EnemyCardCount))
                    {
                        GlobalValueModifier -= 6;
                    }
                    break;

                case Card.Cards.AT_073://Competitive Spirit
                    if (board.MinionFriend.Count <= 1)
                    {
                        GlobalValueModifier -= 5;
                    }
                    else
                    {
                        GlobalValueModifier += 2;
                    }
                    break;

                case Card.Cards.CS2_093://Consecration
                    if (myDeck.isPaladinSecret(board))
                    {
                        GlobalValueModifier -= 11;
                    }
                    break;

                case Card.Cards.CS2_092://Blessing of Kings
                    if (target.IsDivineShield && target.IsFriend == true)
                    {
                        GlobalValueModifier += 1;
                    }
                    break;

                case Card.Cards.EX1_379://Repentance
                    if (board.TurnCount == 1)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.EX1_136://Redemption
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.EX1_130://Noble Sacrifice
                    if (board.MinionFriend.Count == 0)
                    {
                        GlobalValueModifier -= 3;
                    }
                    break;

                case Card.Cards.GVG_061://Muster for Battle
                    GlobalValueModifier -= 2;
                    if (board.WeaponEnemy != null && board.WeaponEnemy.CurrentDurability <= 1 && board.WeaponEnemy.Template.Id == Card.Cards.FP1_021)
                    {
                        GlobalValueModifier -= 8;
                    }
                    if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 && board.WeaponFriend.Template.Id == Card.Cards.CS2_097)
                    {
                        GlobalValueModifier -= 5;
                    }
                    if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability > 1 && board.WeaponFriend.Template.Id == Card.Cards.EX1_383t)
                    {
                        GlobalValueModifier -= 13;
                    }
                    break;

                case Card.Cards.CS2_105://Heroic Strike
                    GlobalValueModifier -= 13;
                    break;

                case Card.Cards.EX1_277://Arcane Missiles
                    GlobalValueModifier -= 3;
                    break;

                case Card.Cards.EX1_408://Mortal Strike
                    GlobalValueModifier -= 16;
                    if (target != board.HeroEnemy || target.IsTaunt == false)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.GVG_003://Unstable Portal
                    GlobalValueModifier += 5;
                    break;

                case Card.Cards.CS2_029://Fireball
                    GlobalValueModifier -= 10;
                    if (target.Type == Card.CType.HERO)
                    {
                        GlobalValueModifier -= 7;
                    }
                    break;

                case Card.Cards.CS2_024://Frostbolt
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.AT_005://Polymorph: Boar
                    if (target.IsFriend)
                    {
                        GlobalValueModifier -= 15;
                    }
                    else
                    {
                        GlobalValueModifier -= 12;
                    }
                    break;

                case Card.Cards.PART_001://Armor Plating
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.PART_002://Time Rewinder
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.PART_003://Rusty Horn
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.PART_004://Finicky Cloakfield
                    GlobalValueModifier -= 5;
                    break;

                case Card.Cards.PART_005://Emergency Coolant
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.PART_006://Reversing Switch
                    GlobalValueModifier -= 1;
                    if (target.CanAttack == false && target.IsFriend == true)
                    {
                        GlobalValueModifier -= 5;
                    }
                    break;

                case Card.Cards.PART_007://Whirling Blades
                    GlobalValueModifier -= 1;
                    break;

                case Card.Cards.GAME_005://The Coin
                    GlobalValueModifier -= 4;
                    break;
            }

            if (spell.Template.IsSecret)
            {
                GlobalValueModifier += 5;
            }
        }

        public override void OnCastWeapon(Board board, Card weapon, Card target)
        {
            if (board.WeaponFriend != null && board.WeaponFriend.CurrentDurability >= 1 && board.WeaponFriend.Template.Id != Card.Cards.CS2_091)
            {
                GlobalValueModifier -= 6;
            }
            if (myDeck.isMechWarrior(board) || myDeck.isMechMage(board))
            {
                GlobalValueModifier += 2;
            }
        }

        public override void OnAttack(Board board, Card attacker, Card target)
        {
            if (board.WeaponFriend != null && attacker.Type == Card.CType.WEAPON)
            {
                if (myDeck.isMechWarrior(board))
                {
                    if (target == board.HeroEnemy && board.MinionEnemy.Count > 0 && (board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor) >= 20) //If enemy health is high, attack minion instead
                    {
                        GlobalValueModifier -= 5;
                    }
                }

                if (attacker.Type == Card.CType.WEAPON && target.Type == Card.CType.HERO && board.Hand.FindAll(x => x.Type == Card.CType.WEAPON).Count == 0 && board.WeaponFriend.CurrentDurability == 1) //If we have a weapon with 1 durability and no other weapon in hand, avoid to attack the HERO
                {
                    GlobalValueModifier -= 18;
                }

                if (board.WeaponFriend.Template.Id == Card.Cards.FP1_021 && board.WeaponFriend.CurrentDurability == 1 && target.CurrentHealth == 1 && !target.IsTaunt) //If Death's Bite durability is 1, avoid to directly attack 1 health minion (unless if its taunt)
                {
                    GlobalValueModifier -= 46;
                }
            }
            

            if (target.Template.Id == Card.Cards.EX1_007 && target.IsFriend == false) //Try to one shot acolyte to avoid enemy draw
            {
                if (attacker.CurrentAtk >= target.CurrentHealth)
                {
                    GlobalValueModifier += 4;
                    if (board.EnemyClass == Card.CClass.WARRIOR)
                    {
                        GlobalValueModifier += 4;
                    }
                }
            }
        }

        public override void OnCastAbility(Board board, Card ability, Card target)
        {
            if (board.MinionFriend.Count(x => x.Template.Id == Card.Cards.AT_076) >= 1)
            {
                GlobalValueModifier += 5;
            }

            if (myDeck.isMechWarrior(board))
            {
                GlobalValueModifier += 1;
            }
        }

        public override void OnProcessAction(Action a, Board board)
        {
            float moveVal = board.TrapMgr.GetSecretModifier(a, board, true);
            GlobalValueModifier += (int)moveVal;
        }

        public override RemoteProfile DeepClone()
        {
            bProfile ret = new bProfile();
            ret.HeroEnemyHealthValue = HeroEnemyHealthValue;
            ret.HeroFriendHealthValue = HeroFriendHealthValue;
            ret.MinionEnemyAttackValue = MinionEnemyAttackValue;
            ret.MinionEnemyHealthValue = MinionEnemyHealthValue;
            ret.MinionFriendAttackValue = MinionFriendAttackValue;
            ret.MinionFriendHealthValue = MinionFriendHealthValue;

            ret.GlobalValueModifier = GlobalValueModifier;

            ret._logBestMove.AddRange(_logBestMove);
            ret._log = _log;

            return ret;
        }

        public bool CanSurvive(Card card, Board board)
        {
            if (card.MaxHealth > GetTotalEnemyDmg(board))
            {
                return true;
                //Debug("True");
            }
            else
            {
                return false;
                //Debug("False");
            }
        }

        public int GetTotalEnemyDmg(Board board)
        {
            int i = 0;

            foreach (Card card in board.MinionEnemy)
            {
                i = card.CurrentAtk + i;
            }

            i = board.HeroEnemy.CurrentAtk + i;

            return i;
        }

        public int GetDmgInHand(Board board)
        {
            int iw = 0;
            int i = 0;

            foreach (Card card in board.Hand)
            {
                if (card.Type == Card.CType.WEAPON)//Weapons
                {
                    if (card.CurrentAtk >= iw)
                    {
                        iw = card.CurrentAtk;
                    }
                }
                if (card.Template.Id == Card.Cards.NEW1_011)//Kor'kron Elite
                {
                    i += 4;
                }
                if (card.Template.Id == Card.Cards.CS2_105) //Heroic strike
                {
                    i += 4;
                }
                if (card.Template.Id == Card.Cards.EX1_408) //Mortal Strike
                {
                    if (board.HeroFriend.CurrentHealth < 12)
                    {
                        i += 6;
                    }
                    else
                    {
                        i += 4;
                    }
                }
                if (card.Template.Id == Card.Cards.CS2_029) //Fireball
                {
                    i += 6;
                }
                if (card.Template.Id == Card.Cards.CS2_024) //Frostbolt
                {
                    i += 3;
                }
            }
            if (board.WeaponFriend != null && board.HeroFriend.CanAttack == false)
            {
                i += board.HeroFriend.CurrentAtk;
            }
            else
            {
                i += iw; //We add weapon hand dmg
            }

            //Debug("We have " + i + "dmg in hand");
            return i;
        }

        public class myDeck
        {
            public static bool isPaladinSecret(Board board)
            {
                return false;
            }
            public static bool isZoo(Board board)
            {
                return false;
            }
            public static bool isMechWarrior(Board board)
            {
                return true;
            }
            public static bool isMechMage(Board board)
            {
                return false;
            }
            public static bool isWorgenOtk(Board board)
            {
                return false;
            }
            public static bool isFaceWarrior(Board board)
            {
                return false;
            }
            public static bool isPriestControl(Board board)
            {
                return false;
            }
        }

        public class opponentDeck
        {
            public static bool isHandLock(Board board)
            {
                //Check if opponent has played some handlock card (check graveyard)
                if (board.TurnCount == 3 && board.EnemyGraveyard.Count == 0 && board.HeroEnemy.Template.Id == Card.Cards.HERO_07)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool isFreezeMage(Board board)
            {
                if (board.EnemyGraveyard.Count(x => x == Card.Cards.NEW1_021) >= 1 && board.HeroEnemy.Template.Id == Card.Cards.HERO_08)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}