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

using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;

namespace MikyM.Discord.Extensions;

/// <summary>
/// Discord member extensions.
/// </summary>
[PublicAPI]
public static class DiscordMemberExtensions
{
    /// <summary>
    /// Gets the full display name of the member (DisplayName + Discriminator).
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>The name.</returns>
    public static string GetFullDisplayName(this DiscordMember member)
    {
        return member.DisplayName + "#" + member.Discriminator;
    }

    /// <summary>
    /// Checks if the user is a moderator (has ban permission, all permissions or is an admin/owner).
    /// </summary>
    /// <param name="member">Member.</param>
    /// <returns>True if the member is a moderator, otherwise false.</returns>
    public static bool IsModerator(this DiscordMember member)
    {
        return member.Roles.Any(x =>
                   x.Permissions.HasPermission(DiscordPermissions.BanMembers) ||
                   x.Permissions.HasPermission(DiscordPermissions.Administrator) ||
                   x.Permissions.HasPermission(DiscordPermissions.All)) ||
               member.Permissions.HasPermission(DiscordPermissions.BanMembers) ||
               member.Permissions.HasPermission(DiscordPermissions.All) ||
               member.Permissions.HasPermission(DiscordPermissions.Administrator) ||
               member.IsOwner;
    }

    /// <summary>
    /// Checks if the user is an admin.
    /// </summary>
    /// <param name="member">Member.</param>
    /// <returns>True if the member is an admin, otherwise false.</returns>
    public static bool IsAdmin(this DiscordMember member)
    {
        return member.Permissions.HasPermission(DiscordPermissions.Administrator) || 
               member.Roles.Any(x =>
                   x.Permissions.HasPermission(DiscordPermissions.Administrator)) ||
               member.Permissions.HasPermission(DiscordPermissions.All) ||
               member.IsOwner;
    }

    /// <summary>
    /// Checks if the user is the owner of the bot.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="client">The client.</param>
    /// <returns>True if the member is the bot owner, otherwise false.</returns>
    public static bool IsBotOwner(this DiscordMember member, DiscordClient client)
    {
        return ((DiscordUser)member).IsBotOwner(client);
    }
}
