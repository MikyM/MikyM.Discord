// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;

namespace MikyM.Discord.Extensions;

/// <summary>
/// Discord user extensions.
/// </summary>
[PublicAPI]
public static class DiscordUserExtensions
{
    /// <summary>
    /// Gets the full username of the user (Username + Discriminator).
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns></returns>
    public static string GetFullUsername(this DiscordUser user)
    {
        return user.Username + "#" + user.Discriminator;
    }

    /// <summary>
    /// Checks if the user is the owner of the bot.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="client"></param>
    /// <returns>True if the member is the bot owner, otherwise false.</returns>
    public static bool IsBotOwner(this DiscordUser user, DiscordClient client)
    {
        if (client.CurrentApplication.Owners == null)
        {
            throw new InvalidOperationException("Bot owners were null");
        }
        
        return client.CurrentApplication.Owners.Any(x => x.Id == user.Id);
    }
}
