﻿using System.Text.Json;

namespace Fyson;

public delegate void CommandHandler(string command, JsonElement args);