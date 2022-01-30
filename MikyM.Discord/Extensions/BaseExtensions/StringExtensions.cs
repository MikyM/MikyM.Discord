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

using System.Text.RegularExpressions;

namespace MikyM.Discord.Extensions.BaseExtensions;

public static class StringExtensions
{
    public static bool TryParseRoleMention(this string value, out ulong roleId)
    {
        var res = Regex.Match(value, @"(?<=\\<@&)[0-9]{17,18}(?=\\>)");

        roleId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    public static bool TryParseUserMention(this string value, out ulong roleId)
    {
        var res = Regex.Match(value, @"(?<=\<@!|\<@)[0-9]{17,18}(?=\>)");

        roleId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    public static bool TryParseChannelMention(this string value, out ulong roleId)
    {
        var res = Regex.Match(value, @"(?<=\<#)[0-9]{17,18}(?=\>)");

        roleId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    public static bool TryParseDiscordMention(this string value, out ulong mentionId)
    {
        return TryParseUserMention(value, out mentionId) || TryParseChannelMention(value, out mentionId) ||
               TryParseRoleMention(value, out mentionId);
    }
}