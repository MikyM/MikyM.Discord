﻿// This file is part of Lisbeth.Bot project
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

using MikyM.Discord.EmbedBuilders.Builders;

namespace MikyM.Discord.EmbedBuilders.Enums;

/// <summary>
/// Base types of <see cref="EnhancedDiscordEmbedBuilder"/>.
/// </summary>
public enum DiscordBotAction
{
    /// <summary>
    /// Undefined action.
    /// </summary>
    Undefined,
    /// <summary>
    /// Response action.
    /// </summary>
    Response,
    /// <summary>
    /// Log action.
    /// </summary>
    Log
}
