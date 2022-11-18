using DiscordBot.DiscordData;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Services
{
    public class CashbackService
    {
        public const int MAX_PRIMOGEMS = 80;

        /// <summary>
        /// Adds random cashback value to the user if duplicate passed, returns cashback value (0 if not duplicate)
        /// </summary>
        /// <param name="user">User to check</param>
        /// <param name="character">Character to check for duplicate</param>
        /// <returns>Cashback value (basically, amount of money that will be returned)</returns>
        public static int CashbackIfNeeded(UserData user, Character character)
        {
            Random rnd = new Random();
            int cashbackValue = 0;

            if (user.characters.Find(chars => chars.characterName == character.characterName) != null)
            {
                cashbackValue = rnd.Next(0, MAX_PRIMOGEMS);
                user.wallet.primogems += cashbackValue;
            }

            return cashbackValue;
        }
    }
}
