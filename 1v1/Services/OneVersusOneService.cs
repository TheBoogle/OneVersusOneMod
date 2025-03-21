﻿using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using EFT;
using EFT.InventoryLogic;
using EFT.HealthSystem;
using OneVersusOne.Patches;

namespace OneVersusOne.Services
{
    public class OneVersusOneService
    {
        public static void MovePlayerToLocation(Player player, Vector3 Position)
        {
            player.Transform.position = Position;
        }

        public static void TeleportCorrectly(Player player, string Location)
        {
            Vector3 HostSpawnPoint = new Vector3(0, 0, 0);
            Vector3 ClientSpawnPoint = new Vector3(0, 0, 0);

            OneVersusOnePlugin.LocationConfig MatchingLocation = OneVersusOnePlugin.ModConfig.Locations.FirstOrDefault(x => x.Name.ToLower() == Location.ToLower());

            if (MatchingLocation != null)
            {
                HostSpawnPoint = MatchingLocation.HostPosition;
                ClientSpawnPoint = MatchingLocation.ClientPosition;
            }

            if (Fika.Core.Coop.Utils.FikaBackendUtils.IsServer)
            {
                OneVersusOneService.MovePlayerToLocation(player, HostSpawnPoint);
            }
            else
            {
                OneVersusOneService.MovePlayerToLocation(player, ClientSpawnPoint);
            }
        }

        public static void RepairArmor(Player player)
        {
            foreach (Item item in player.Inventory.GetPlayerItems(EPlayerItems.Equipment))
            {
                if (item is ArmorPlateItemClass)
                {
                    ArmorPlateItemClass itemClass = (ArmorPlateItemClass)item;
                    itemClass.Armor.Repairable.Durability = itemClass.Armor.Repairable.MaxDurability;
                }
            }
        }

        public static void Heal(Player player)
        {
            ActiveHealthController HealthController = player.HealthController as ActiveHealthController;

            foreach (EBodyPart BodyPart in Enum.GetValues(typeof(EBodyPart)))
            {
                HealthController.method_18(BodyPart, (ignore) => true);
            }

            HealthController.RestoreFullHealth();

            player.MovementContext.RemoveStateSpeedLimit(Player.ESpeedLimit.HealthCondition);

            Task.Delay(1000).ContinueWith(t =>
            {
                RepairArmor(player);

                foreach (EBodyPart BodyPart in Enum.GetValues(typeof(EBodyPart)))
                {
                    HealthController.method_18(BodyPart, (ignore) => true);
                }

                HealthController.RestoreFullHealth();

                HealthController.DoPainKiller();

                player.MovementContext.RemoveStateSpeedLimit(Player.ESpeedLimit.HealthCondition);
            });
        }

        public static void HealAndTeleport(Player player)
        {
            Heal(player);
            TeleportCorrectly(player, player.Location.ToLower());
        }
    }
}
